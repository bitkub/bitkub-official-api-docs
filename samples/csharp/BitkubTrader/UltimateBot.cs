using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace BitkubTrader
{
    /// <summary>
    /// 🌟 ULTIMATE TRADING BOT 🌟
    /// บอทเทรดที่ล้ำสมัยที่สุดในจักรวาล!
    ///
    /// Features:
    /// - Elliott Wave Analysis
    /// - Fibonacci Auto-levels
    /// - Volume Profile (POC, VAH, VAL)
    /// - Market Structure (BOS, CHoCH)
    /// - Order Flow Analysis
    /// - Machine Learning Predictions
    /// - Pattern Recognition (8+ patterns)
    /// - Multi-indicator Fusion
    /// - Risk Management
    /// - News Sentiment
    /// - Database Tracking
    /// </summary>
    public class UltimateBot
    {
        private readonly BitkubClient _client;
        private readonly DatabaseManager _db;
        private readonly string _symbol;
        private readonly UltimateBotMode _mode;

        private List<decimal> _priceHistory = new();
        private List<decimal> _volumeHistory = new();
        private decimal _initialBalance;
        private int _totalTrades;
        private bool _isRunning;

        public UltimateBot(
            BitkubClient client,
            DatabaseManager db,
            string symbol,
            UltimateBotMode mode,
            decimal initialBalance)
        {
            _client = client;
            _db = db;
            _symbol = symbol;
            _mode = mode;
            _initialBalance = initialBalance;
        }

        /// <summary>
        /// Start Ultimate Bot
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _isRunning = true;
            ShowUltimateIntro();

            await Task.Delay(3000);

            while (_isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await RunUltimateAnalysisAsync();
                    await Task.Delay(GetUpdateInterval(), cancellationToken);
                }
                catch (Exception ex)
                {
                    ConsoleUI.ShowError($"Error: {ex.Message}");
                    await Task.Delay(10000, cancellationToken);
                }
            }
        }

        private void ShowUltimateIntro()
        {
            AnsiConsole.Clear();

            var title = new FigletText("ULTIMATE BOT")
            {
                Color = Color.Gold1
            };

            AnsiConsole.Write(title);

            var features = new Panel(
                Align.Center(
                    new Markup(
                        "[bold gold1]🌟 THE MOST ADVANCED TRADING BOT EVER CREATED 🌟[/]\n\n" +
                        "[cyan]Elliott Wave[/] • [yellow]Fibonacci[/] • [green]Volume Profile[/]\n" +
                        "[magenta]ML Predictions[/] • [blue]Order Flow[/] • [red]Pattern Recognition[/]\n\n" +
                        $"[bold white]Symbol:[/] [yellow]{_symbol}[/]\n" +
                        $"[bold white]Mode:[/] [cyan]{_mode}[/]\n" +
                        $"[bold white]Balance:[/] [green]{_initialBalance:N0}[/] THB"
                    ),
                    VerticalAlignment.Middle
                ))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Gold1),
                Padding = new Padding(2, 1)
            };

            AnsiConsole.Write(features);
            AnsiConsole.WriteLine();
        }

        /// <summary>
        /// Run comprehensive analysis using ALL tools
        /// </summary>
        private async Task RunUltimateAnalysisAsync()
        {
            // 1. Fetch market data
            var ticker = await _client.GetTickerAsync(_symbol);
            if (!ticker.TryGetValue(_symbol, out var tickerInfo))
                return;

            var currentPrice = tickerInfo.Last;
            _priceHistory.Add(currentPrice);
            _volumeHistory.Add(tickerInfo.BaseVolume);

            if (_priceHistory.Count > 200)
            {
                _priceHistory.RemoveAt(0);
                _volumeHistory.RemoveAt(0);
            }

            if (_priceHistory.Count < 50)
            {
                ConsoleUI.ShowInfo($"Collecting data... ({_priceHistory.Count}/50)");
                return;
            }

            // 2. Multi-layer Analysis
            var analysis = await PerformUltimateAnalysisAsync(tickerInfo);

            // 3. Display beautiful results
            DisplayUltimateAnalysis(analysis);

            // 4. Make trading decision
            var decision = MakeUltimateDecision(analysis);

            // 5. Execute if confidence is high enough
            if (decision.ShouldTrade)
            {
                await ExecuteUltimateTradeAsync(decision, currentPrice);
            }

            // 6. Save to database
            await SaveAnalysisToDBAsync(analysis, currentPrice);
        }

        /// <summary>
        /// Perform comprehensive multi-dimensional analysis
        /// </summary>
        private async Task<UltimateAnalysis> PerformUltimateAnalysisAsync(TickerInfo ticker)
        {
            var analysis = new UltimateAnalysis();

            // === TECHNICAL ANALYSIS ===
            var technicalSignal = TechnicalAnalysis.GenerateSignal(_priceHistory);
            analysis.TechnicalSignal = technicalSignal;

            // === ELLIOTT WAVE ===
            var elliottWave = AdvancedAnalysis.DetectElliottWave(_priceHistory);
            analysis.ElliottWave = elliottWave;

            // === FIBONACCI ===
            var high = _priceHistory.Max();
            var low = _priceHistory.Min();
            var fibLevels = AdvancedAnalysis.FindFibonacciSupportResistance(_priceHistory);
            analysis.FibonacciLevels = fibLevels;

            // === VOLUME PROFILE ===
            var volumeProfile = AdvancedAnalysis.CalculateVolumeProfile(_priceHistory, _volumeHistory);
            analysis.VolumeProfile = volumeProfile;

            // === MARKET STRUCTURE ===
            var structure = AdvancedAnalysis.AnalyzeMarketStructure(_priceHistory);
            analysis.MarketStructure = structure;

            // === ORDER FLOW ===
            var depth = await _client.GetDepthAsync(_symbol, 20);
            var orderFlow = AdvancedAnalysis.AnalyzeOrderFlow(depth.Bids, depth.Asks);
            analysis.OrderFlow = orderFlow;

            // === MACHINE LEARNING ===
            var mlPrediction = MachineLearning.PredictNextPrices(_priceHistory, 5);
            var linearReg = MachineLearning.LinearRegression(_priceHistory);
            var patterns = MachineLearning.RecognizePatterns(_priceHistory);
            analysis.MLPredictions = mlPrediction;
            analysis.LinearRegression = linearReg;
            analysis.RecognizedPatterns = patterns;

            // === NEWS SENTIMENT ===
            try
            {
                var newsAnalyzer = new NewsAnalyzer();
                var sentiment = await newsAnalyzer.GetMarketSentimentAsync();
                analysis.NewsSentiment = sentiment;
            }
            catch
            {
                // News might fail, continue anyway
            }

            // === CALCULATE MASTER SCORE ===
            analysis.MasterScore = CalculateMasterScore(analysis);

            return analysis;
        }

        /// <summary>
        /// Calculate Master Score from all indicators (0-100)
        /// </summary>
        private int CalculateMasterScore(UltimateAnalysis analysis)
        {
            var score = 50; // Start neutral

            // Technical Analysis (20 points)
            if (analysis.TechnicalSignal.Action.Contains("STRONG_BUY"))
                score += 20;
            else if (analysis.TechnicalSignal.Action.Contains("BUY"))
                score += 10;
            else if (analysis.TechnicalSignal.Action.Contains("STRONG_SELL"))
                score -= 20;
            else if (analysis.TechnicalSignal.Action.Contains("SELL"))
                score -= 10;

            // Elliott Wave (15 points)
            if (analysis.ElliottWave.CurrentWave == "WAVE_3")
                score += 15; // Wave 3 is strongest
            else if (analysis.ElliottWave.CurrentWave == "WAVE_5")
                score += 10;
            else if (analysis.ElliottWave.Pattern == "CORRECTIVE_WAVE")
                score -= 10;

            // Fibonacci (10 points)
            var nearestFib = analysis.FibonacciLevels.FirstOrDefault();
            if (nearestFib != null)
            {
                if (nearestFib.Type == "SUPPORT" && Math.Abs(nearestFib.Distance) < 1)
                    score += 10; // Near support
                else if (nearestFib.Type == "RESISTANCE" && Math.Abs(nearestFib.Distance) < 1)
                    score -= 10; // Near resistance
            }

            // Volume Profile (10 points)
            var currentPrice = _priceHistory.Last();
            if (currentPrice > analysis.VolumeProfile.POC)
                score += 5; // Above POC
            else
                score -= 5;

            if (currentPrice > analysis.VolumeProfile.VAH)
                score -= 5; // Too high
            else if (currentPrice < analysis.VolumeProfile.VAL)
                score += 5; // Low, might bounce

            // Market Structure (15 points)
            if (analysis.MarketStructure.Trend == "UPTREND")
                score += 15;
            else if (analysis.MarketStructure.Trend == "DOWNTREND")
                score -= 15;

            if (analysis.MarketStructure.LastBreak.Contains("BULLISH"))
                score += 10;
            else if (analysis.MarketStructure.LastBreak.Contains("BEARISH"))
                score -= 10;

            // Order Flow (10 points)
            if (analysis.OrderFlow.Pressure == "BUYING_PRESSURE")
                score += 10;
            else if (analysis.OrderFlow.Pressure == "SELLING_PRESSURE")
                score -= 10;

            // ML Predictions (10 points)
            if (analysis.LinearRegression.Trend == "UPTREND")
                score += 10;
            else if (analysis.LinearRegression.Trend == "DOWNTREND")
                score -= 10;

            // Pattern Recognition (5 points)
            var pattern = analysis.RecognizedPatterns.MostLikelyPattern;
            if (pattern.Contains("DOUBLE_BOTTOM") || pattern.Contains("CUP") || pattern.Contains("ASCENDING"))
                score += 5;
            else if (pattern.Contains("DOUBLE_TOP") || pattern.Contains("DESCENDING") || pattern.Contains("HEAD"))
                score -= 5;

            // News Sentiment (5 points)
            if (analysis.NewsSentiment?.OverallSentiment.Contains("POSITIVE") == true)
                score += 5;
            else if (analysis.NewsSentiment?.OverallSentiment.Contains("NEGATIVE") == true)
                score -= 5;

            return Math.Clamp(score, 0, 100);
        }

        /// <summary>
        /// Make ultimate trading decision
        /// </summary>
        private UltimateDecision MakeUltimateDecision(UltimateAnalysis analysis)
        {
            var decision = new UltimateDecision();
            var score = analysis.MasterScore;

            // Confidence thresholds based on mode
            var minBuyScore = _mode switch
            {
                UltimateBotMode.UltraAggressive => 60,
                UltimateBotMode.Aggressive => 70,
                UltimateBotMode.Balanced => 75,
                UltimateBotMode.Conservative => 80,
                _ => 75
            };

            var minSellScore = 100 - minBuyScore;

            if (score >= minBuyScore)
            {
                decision.Action = score >= 85 ? "STRONG_BUY" : "BUY";
                decision.ShouldTrade = true;
                decision.Confidence = score;
            }
            else if (score <= minSellScore)
            {
                decision.Action = score <= 15 ? "STRONG_SELL" : "SELL";
                decision.ShouldTrade = true;
                decision.Confidence = 100 - score;
            }
            else
            {
                decision.Action = "HOLD";
                decision.ShouldTrade = false;
                decision.Confidence = 50;
            }

            // Build reasoning
            decision.Reasoning = BuildReasoning(analysis);

            return decision;
        }

        private string BuildReasoning(UltimateAnalysis analysis)
        {
            var reasons = new List<string>();

            reasons.Add($"Technical: {analysis.TechnicalSignal.Action}");
            reasons.Add($"Elliott: {analysis.ElliottWave.CurrentWave}");
            reasons.Add($"Structure: {analysis.MarketStructure.Trend}");
            reasons.Add($"Order Flow: {analysis.OrderFlow.Pressure}");
            reasons.Add($"ML: {analysis.LinearRegression.Trend}");
            reasons.Add($"Pattern: {analysis.RecognizedPatterns.MostLikelyPattern}");

            return string.Join(" | ", reasons);
        }

        private async Task ExecuteUltimateTradeAsync(UltimateDecision decision, decimal currentPrice)
        {
            ConsoleUI.ShowSuccess($"🎯 ULTIMATE SIGNAL: {decision.Action}");
            ConsoleUI.ShowInfo($"   Confidence: {decision.Confidence}%");
            ConsoleUI.ShowInfo($"   Reasoning: {decision.Reasoning}");

            // Execute trade logic here (currently disabled for safety)
            // var result = await _client.PlaceBidAsync(...);

            _totalTrades++;
        }

        private void DisplayUltimateAnalysis(UltimateAnalysis analysis)
        {
            AnsiConsole.Clear();

            // Master Score Panel
            var scoreColor = analysis.MasterScore >= 70 ? "green" :
                            analysis.MasterScore <= 30 ? "red" : "yellow";

            var scorePanel = new Panel(
                Align.Center(
                    new Markup($"[bold {scoreColor}]{analysis.MasterScore}[/]/100"),
                    VerticalAlignment.Middle
                ))
            {
                Header = new PanelHeader("🎯 [bold]MASTER SCORE[/]", Justify.Center),
                Border = BoxBorder.Heavy,
                BorderStyle = new Style(Color.FromInt32(scoreColor == "green" ? 2 : scoreColor == "red" ? 1 : 3))
            };

            AnsiConsole.Write(scorePanel);
            AnsiConsole.WriteLine();

            // Analysis Grid
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[cyan]Analysis[/]")
                .AddColumn("[yellow]Result[/]")
                .AddColumn("[green]Score Impact[/]");

            table.AddRow("📊 Technical", analysis.TechnicalSignal.Action, GetScoreImpact(analysis.TechnicalSignal.Score));
            table.AddRow("🌊 Elliott Wave", $"{analysis.ElliottWave.Pattern} ({analysis.ElliottWave.CurrentWave})", analysis.ElliottWave.Confidence + "%");
            table.AddRow("📐 Fibonacci", analysis.FibonacciLevels.FirstOrDefault()?.Name ?? "N/A", "");
            table.AddRow("📊 Volume POC", $"{analysis.VolumeProfile.POC:N0}", "");
            table.AddRow("🏗️ Structure", analysis.MarketStructure.Trend, "");
            table.AddRow("📉 Order Flow", analysis.OrderFlow.Pressure, "");
            table.AddRow("🧠 ML Trend", analysis.LinearRegression.Trend, $"R²: {analysis.LinearRegression.RSquared:N2}");
            table.AddRow("🔍 Pattern", analysis.RecognizedPatterns.MostLikelyPattern, "");
            table.AddRow("📰 News", analysis.NewsSentiment?.OverallSentiment ?? "N/A", "");

            AnsiConsole.Write(table);
        }

        private string GetScoreImpact(int score)
        {
            return score switch
            {
                > 4 => "[green]+++++[/]",
                > 2 => "[green]+++[/]",
                > 0 => "[green]+[/]",
                < -4 => "[red]-----[/]",
                < -2 => "[red]---[/]",
                < 0 => "[red]-[/]",
                _ => "[yellow]○[/]"
            };
        }

        private async Task SaveAnalysisToDBAsync(UltimateAnalysis analysis, decimal price)
        {
            await _db.SavePriceAsync(_symbol, price, _priceHistory.Max(), _priceHistory.Min(),
                _volumeHistory.Last(), 0);

            await _db.SaveSignalAsync(_symbol, analysis.TechnicalSignal, price);
        }

        private int GetUpdateInterval()
        {
            return _mode switch
            {
                UltimateBotMode.UltraAggressive => 3000,   // 3 seconds
                UltimateBotMode.Aggressive => 10000,       // 10 seconds
                UltimateBotMode.Balanced => 30000,          // 30 seconds
                UltimateBotMode.Conservative => 60000,      // 1 minute
                _ => 30000
            };
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }

    public enum UltimateBotMode
    {
        UltraAggressive,  // ⚡⚡ สุดโหด! 3 วินาที
        Aggressive,       // ⚡ รวดเร็ว 10 วินาที
        Balanced,         // ⚖️ สมดุล 30 วินาที
        Conservative      // 🛡️ ระมัดระวัง 1 นาที
    }

    public class UltimateAnalysis
    {
        public TradingSignal TechnicalSignal { get; set; } = new();
        public ElliottWavePattern ElliottWave { get; set; } = new();
        public List<FibLevel> FibonacciLevels { get; set; } = new();
        public VolumeProfile VolumeProfile { get; set; } = new();
        public MarketStructure MarketStructure { get; set; } = new();
        public OrderFlowAnalysis OrderFlow { get; set; } = new();
        public List<decimal> MLPredictions { get; set; } = new();
        public LinearRegressionResult LinearRegression { get; set; } = new();
        public PatternRecognition RecognizedPatterns { get; set; } = new();
        public MarketSentiment? NewsSentiment { get; set; }
        public int MasterScore { get; set; }
    }

    public class UltimateDecision
    {
        public string Action { get; set; } = "HOLD";
        public bool ShouldTrade { get; set; }
        public int Confidence { get; set; }
        public string Reasoning { get; set; } = "";
    }
}
