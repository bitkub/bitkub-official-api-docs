using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace BitkubTrader
{
    /// <summary>
    /// AI Trading Bot with Multiple Modes and Risk Management
    /// 🤖 โรบอทเทรดอัจฉริยะพร้อมระบบจัดการความเสี่ยง
    /// </summary>
    public class AITradingBot
    {
        private readonly BitkubClient _client;
        private readonly DatabaseManager _db;
        private readonly NewsAnalyzer _newsAnalyzer;
        private readonly string _symbol;
        private readonly TradingMode _mode;
        private readonly RiskManager _riskManager;

        private List<decimal> _priceHistory = new();
        private decimal _initialBalance;
        private decimal _currentBalance;
        private int _totalTrades;
        private decimal _totalProfit;
        private DateTime _startTime;
        private string _sessionId;
        private bool _isRunning;

        public AITradingBot(
            BitkubClient client,
            DatabaseManager db,
            string symbol,
            TradingMode mode,
            decimal initialBalance,
            decimal maxRiskPerTrade = 0.02m,
            decimal takeProfitPercent = 0.03m,
            decimal stopLossPercent = 0.02m)
        {
            _client = client;
            _db = db;
            _newsAnalyzer = new NewsAnalyzer();
            _symbol = symbol;
            _mode = mode;
            _initialBalance = initialBalance;
            _currentBalance = initialBalance;
            _sessionId = Guid.NewGuid().ToString("N");

            _riskManager = new RiskManager(
                maxRiskPerTrade,
                takeProfitPercent,
                stopLossPercent,
                mode
            );
        }

        /// <summary>
        /// Start AI Trading Bot
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _isRunning = true;
            _startTime = DateTime.Now;

            AnsiConsole.Clear();
            ShowBotIntro();

            await Task.Delay(2000);

            while (_isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await RunTradingCycleAsync();
                    await Task.Delay(GetUpdateInterval(), cancellationToken);
                }
                catch (Exception ex)
                {
                    ConsoleUI.ShowError($"Error: {ex.Message}");
                    await Task.Delay(30000, cancellationToken);
                }
            }

            await ShowFinalReport();
        }

        private void ShowBotIntro()
        {
            var panel = new Panel(
                Align.Center(
                    new Markup(
                        $"[bold cyan]🤖 AI TRADING BOT ACTIVATED 🤖[/]\n\n" +
                        $"[yellow]Symbol:[/] [bold]{_symbol}[/]\n" +
                        $"[yellow]Mode:[/] [bold]{GetModeDescription()}[/]\n" +
                        $"[yellow]Initial Balance:[/] [bold green]{_initialBalance:N2}[/] THB\n" +
                        $"[yellow]Risk per Trade:[/] [bold]{_riskManager.MaxRiskPerTrade * 100:N1}%[/]\n" +
                        $"[yellow]Take Profit:[/] [bold green]{_riskManager.TakeProfitPercent * 100:N1}%[/]\n" +
                        $"[yellow]Stop Loss:[/] [bold red]{_riskManager.StopLossPercent * 100:N1}%[/]\n\n" +
                        $"[dim]Session ID: {_sessionId}[/]"
                    ),
                    VerticalAlignment.Middle
                ))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Cyan1)
            };

            AnsiConsole.Write(panel);
        }

        /// <summary>
        /// Main trading cycle
        /// </summary>
        private async Task RunTradingCycleAsync()
        {
            // 1. Fetch current market data
            var ticker = await _client.GetTickerAsync(_symbol);
            if (!ticker.TryGetValue(_symbol, out var tickerInfo))
                return;

            var currentPrice = tickerInfo.Last;
            _priceHistory.Add(currentPrice);
            if (_priceHistory.Count > 100)
                _priceHistory.RemoveAt(0);

            // Save to database
            await _db.SavePriceAsync(_symbol, currentPrice, tickerInfo.High24Hr,
                tickerInfo.Low24Hr, tickerInfo.BaseVolume, tickerInfo.PercentChange);

            // 2. Technical Analysis
            if (_priceHistory.Count < 30)
            {
                ConsoleUI.ShowInfo($"Collecting data... ({_priceHistory.Count}/30)");
                return;
            }

            var signal = TechnicalAnalysis.GenerateSignal(_priceHistory);
            await _db.SaveSignalAsync(_symbol, signal, currentPrice);

            // 3. News Sentiment Analysis
            var sentiment = await _newsAnalyzer.GetMarketSentimentAsync();
            var newsRecommendation = _newsAnalyzer.GetTradingRecommendation(sentiment);

            // 4. Combine signals with AI weighting
            var finalDecision = MakeAIDecision(signal, sentiment, tickerInfo);

            // 5. Display current status
            DisplayTradingStatus(tickerInfo, signal, sentiment, finalDecision);

            // 6. Execute trade if signal is strong enough
            await ExecuteTradeAsync(finalDecision, currentPrice);

            // 7. Check and manage open positions
            await ManageOpenPositionsAsync(currentPrice);
        }

        /// <summary>
        /// AI Decision Making - Combine multiple signals
        /// </summary>
        private AIDecision MakeAIDecision(TradingSignal technicalSignal, MarketSentiment newsSentiment, TickerInfo ticker)
        {
            var decision = new AIDecision();
            var score = 0;
            var reasons = new List<string>();

            // Technical Analysis Weight (60%)
            if (technicalSignal.Action == "STRONG_BUY")
            {
                score += 6;
                reasons.Add($"Technical: STRONG_BUY ({technicalSignal.Confidence}%)");
            }
            else if (technicalSignal.Action == "BUY")
            {
                score += 3;
                reasons.Add($"Technical: BUY ({technicalSignal.Confidence}%)");
            }
            else if (technicalSignal.Action == "STRONG_SELL")
            {
                score -= 6;
                reasons.Add($"Technical: STRONG_SELL ({technicalSignal.Confidence}%)");
            }
            else if (technicalSignal.Action == "SELL")
            {
                score -= 3;
                reasons.Add($"Technical: SELL ({technicalSignal.Confidence}%)");
            }

            // News Sentiment Weight (25%)
            if (newsSentiment.OverallSentiment.Contains("POSITIVE"))
            {
                score += newsSentiment.OverallSentiment == "VERY_POSITIVE" ? 3 : 2;
                reasons.Add($"News: {newsSentiment.OverallSentiment}");
            }
            else if (newsSentiment.OverallSentiment.Contains("NEGATIVE"))
            {
                score -= newsSentiment.OverallSentiment == "VERY_NEGATIVE" ? 3 : 2;
                reasons.Add($"News: {newsSentiment.OverallSentiment}");
            }

            // Volume Analysis (15%)
            var avgVolume = ticker.BaseVolume;
            if (avgVolume > 100) // High volume
            {
                score += ticker.PercentChange > 0 ? 1 : -1;
                reasons.Add($"High volume ({avgVolume:N2})");
            }

            // Apply trading mode modifier
            score = _mode switch
            {
                TradingMode.Aggressive => (int)(score * 0.8), // Lower threshold
                TradingMode.Conservative => (int)(score * 1.2), // Higher threshold
                _ => score
            };

            // Determine action
            if (score >= 7)
            {
                decision.Action = "STRONG_BUY";
                decision.Confidence = Math.Min(95, 70 + score);
            }
            else if (score >= 4)
            {
                decision.Action = "BUY";
                decision.Confidence = 60 + score * 2;
            }
            else if (score <= -7)
            {
                decision.Action = "STRONG_SELL";
                decision.Confidence = Math.Min(95, 70 + Math.Abs(score));
            }
            else if (score <= -4)
            {
                decision.Action = "SELL";
                decision.Confidence = 60 + Math.Abs(score) * 2;
            }
            else
            {
                decision.Action = "HOLD";
                decision.Confidence = 50;
            }

            decision.Reasoning = string.Join(" | ", reasons);
            decision.Score = score;

            return decision;
        }

        /// <summary>
        /// Execute trade based on AI decision
        /// </summary>
        private async Task ExecuteTradeAsync(AIDecision decision, decimal currentPrice)
        {
            // Check if we should trade based on mode and confidence
            var minConfidence = _mode switch
            {
                TradingMode.Aggressive => 60,
                TradingMode.Conservative => 80,
                _ => 70
            };

            if (decision.Confidence < minConfidence)
                return;

            // Calculate position size based on risk management
            var positionSize = _riskManager.CalculatePositionSize(_currentBalance, currentPrice);

            if (decision.Action.Contains("BUY") && positionSize > 0)
            {
                await ExecuteBuyOrderAsync(currentPrice, positionSize, decision);
            }
            else if (decision.Action.Contains("SELL") && HasOpenPosition())
            {
                await ExecuteSellOrderAsync(currentPrice, decision);
            }
        }

        private async Task ExecuteBuyOrderAsync(decimal price, decimal amount, AIDecision decision)
        {
            try
            {
                // Test order first
                var testResult = await _client.PlaceBidTestAsync(_symbol, amount, price, "limit");

                if (testResult.Error == 0)
                {
                    // Real order (commented for safety - uncomment when ready for real trading)
                    // var result = await _client.PlaceBidAsync(_symbol, amount, price, "limit");

                    ConsoleUI.ShowSuccess($"🎯 BUY ORDER: {amount:N2} THB @ {price:N2}");
                    ConsoleUI.ShowInfo($"   Confidence: {decision.Confidence}%");
                    ConsoleUI.ShowInfo($"   Reason: {decision.Reasoning}");

                    _totalTrades++;

                    // Save to database
                    await _db.SaveOrderAsync(testResult.Result?.Id ?? 0, _symbol, "BUY", "limit",
                        amount, price, testResult.Result?.Fee ?? 0, "FILLED");
                }
            }
            catch (Exception ex)
            {
                ConsoleUI.ShowError($"Buy order failed: {ex.Message}");
            }
        }

        private async Task ExecuteSellOrderAsync(decimal price, AIDecision decision)
        {
            ConsoleUI.ShowWarning($"💰 SELL SIGNAL: @ {price:N2}");
            ConsoleUI.ShowInfo($"   Confidence: {decision.Confidence}%");
            ConsoleUI.ShowInfo($"   Reason: {decision.Reasoning}");

            // Implement sell logic here
            _totalTrades++;
        }

        /// <summary>
        /// Manage open positions (Take Profit / Stop Loss)
        /// </summary>
        private async Task ManageOpenPositionsAsync(decimal currentPrice)
        {
            var openOrders = await _client.GetOpenOrdersAsync(_symbol);

            if (openOrders.Error == 0 && openOrders.Result.Count > 0)
            {
                foreach (var order in openOrders.Result)
                {
                    // Check take profit
                    if (order.Side == "BUY")
                    {
                        var profitPercent = (currentPrice - order.Rate) / order.Rate;

                        if (profitPercent >= _riskManager.TakeProfitPercent)
                        {
                            ConsoleUI.ShowSuccess($"✅ TAKE PROFIT HIT: +{profitPercent * 100:N2}%");
                            // Cancel buy and place sell order
                            await _client.CancelOrderAsync(_symbol, order.Id, "buy");
                        }
                        else if (profitPercent <= -_riskManager.StopLossPercent)
                        {
                            ConsoleUI.ShowWarning($"🛑 STOP LOSS HIT: {profitPercent * 100:N2}%");
                            // Cancel buy order
                            await _client.CancelOrderAsync(_symbol, order.Id, "buy");
                        }
                    }
                }
            }
        }

        private void DisplayTradingStatus(TickerInfo ticker, TradingSignal signal, MarketSentiment sentiment, AIDecision decision)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Cyan1)
                .AddColumn("[cyan]Metric[/]")
                .AddColumn("[yellow]Value[/]");

            table.AddRow("💰 Current Price", $"[yellow]{ticker.Last:N2}[/] THB");
            table.AddRow("📊 24h Change", $"[{(ticker.PercentChange >= 0 ? "green" : "red")}]{(ticker.PercentChange >= 0 ? "▲" : "▼")} {ticker.PercentChange:N2}%[/]");
            table.AddRow("📈 RSI", $"[cyan]{signal.RSI:N1}[/]");
            table.AddRow("📰 News Sentiment", $"[magenta]{sentiment.OverallSentiment}[/]");
            table.AddRow("🤖 AI Decision", $"[bold {GetDecisionColor(decision.Action)}]{decision.Action}[/]");
            table.AddRow("💯 Confidence", $"[bold]{decision.Confidence}%[/]");
            table.AddRow("💼 Current Balance", $"[green]{_currentBalance:N2}[/] THB");
            table.AddRow("📊 Total Trades", $"[cyan]{_totalTrades}[/]");
            table.AddRow("💎 Total P/L", $"[{(_totalProfit >= 0 ? "green" : "red")}]{_totalProfit:N2}[/] THB");
            table.AddRow("⏱️ Runtime", $"[dim]{(DateTime.Now - _startTime).ToString(@"hh\:mm\:ss")}[/]");

            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[bold cyan]🤖 AI BOT - {_symbol}[/]") { Style = Style.Parse("cyan") });
            AnsiConsole.WriteLine();
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[dim]{decision.Reasoning}[/]");
        }

        private string GetDecisionColor(string action)
        {
            return action switch
            {
                "STRONG_BUY" => "green",
                "BUY" => "green",
                "STRONG_SELL" => "red",
                "SELL" => "red",
                _ => "yellow"
            };
        }

        private async Task ShowFinalReport()
        {
            var metrics = await _db.GetPerformanceAsync(_symbol);

            var report = new Panel(
                new Markup(
                    $"[bold cyan]📊 FINAL TRADING REPORT 📊[/]\n\n" +
                    $"[yellow]Total Trades:[/] {_totalTrades}\n" +
                    $"[yellow]Win Rate:[/] {metrics.WinRate:N1}%\n" +
                    $"[yellow]Total P/L:[/] [{(_totalProfit >= 0 ? "green" : "red")}]{_totalProfit:N2}[/] THB\n" +
                    $"[yellow]ROI:[/] [{(_totalProfit >= 0 ? "green" : "red")}]{(_totalProfit / _initialBalance * 100):N2}%[/]\n" +
                    $"[yellow]Runtime:[/] {(DateTime.Now - _startTime).ToString(@"hh\:mm\:ss")}"
                ))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(_totalProfit >= 0 ? Color.Green : Color.Red)
            };

            AnsiConsole.Write(report);
        }

        private int GetUpdateInterval()
        {
            return _mode switch
            {
                TradingMode.Aggressive => 5000,   // 5 seconds
                TradingMode.Conservative => 60000,  // 1 minute
                _ => 30000  // 30 seconds
            };
        }

        private string GetModeDescription()
        {
            return _mode switch
            {
                TradingMode.Aggressive => "⚡ AGGRESSIVE (High Risk, High Reward)",
                TradingMode.Conservative => "🛡️ CONSERVATIVE (Low Risk, Steady)",
                _ => "⚖️ BALANCED (Medium Risk)"
            };
        }

        private bool HasOpenPosition()
        {
            // Simplified - should check actual positions
            return false;
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }

    public enum TradingMode
    {
        Aggressive,    // เทรดบ่อย, ยอมรับความเสี่ยงสูง
        Balanced,      // สมดุลระหว่างความเสี่ยงและผลตอบแทน
        Conservative   // เทรดระมัดระวัง, ความเสี่ยงต่ำ
    }

    public class AIDecision
    {
        public string Action { get; set; } = "HOLD";
        public int Confidence { get; set; }
        public string Reasoning { get; set; } = "";
        public int Score { get; set; }
    }

    public class RiskManager
    {
        public decimal MaxRiskPerTrade { get; set; }
        public decimal TakeProfitPercent { get; set; }
        public decimal StopLossPercent { get; set; }
        public TradingMode Mode { get; set; }

        public RiskManager(decimal maxRisk, decimal takeProfit, decimal stopLoss, TradingMode mode)
        {
            MaxRiskPerTrade = maxRisk;
            TakeProfitPercent = takeProfit;
            StopLossPercent = stopLoss;
            Mode = mode;

            // Adjust based on mode
            if (mode == TradingMode.Aggressive)
            {
                MaxRiskPerTrade *= 1.5m;
                TakeProfitPercent *= 0.8m;
            }
            else if (mode == TradingMode.Conservative)
            {
                MaxRiskPerTrade *= 0.5m;
                TakeProfitPercent *= 1.2m;
            }
        }

        public decimal CalculatePositionSize(decimal balance, decimal price)
        {
            var riskAmount = balance * MaxRiskPerTrade;
            return Math.Floor(riskAmount / price * 100) / 100;
        }

        public bool ShouldTakeProfit(decimal entryPrice, decimal currentPrice, string side)
        {
            var profitPercent = side == "BUY" ?
                (currentPrice - entryPrice) / entryPrice :
                (entryPrice - currentPrice) / entryPrice;

            return profitPercent >= TakeProfitPercent;
        }

        public bool ShouldStopLoss(decimal entryPrice, decimal currentPrice, string side)
        {
            var lossPercent = side == "BUY" ?
                (entryPrice - currentPrice) / entryPrice :
                (currentPrice - entryPrice) / entryPrice;

            return lossPercent >= StopLossPercent;
        }
    }
}
