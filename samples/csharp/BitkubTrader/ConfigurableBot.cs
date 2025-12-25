using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace BitkubTrader
{
    /// <summary>
    /// 🎛️ Configurable Trading Bot
    /// บอทที่ปรับแต่งได้ทุกอย่าง + LINE OA + Chart Planning
    /// </summary>
    public class ConfigurableBot
    {
        private readonly BitkubClient _client;
        private readonly DatabaseManager _db;
        private readonly TradingConfig _config;
        private readonly LineMessenger _lineMessenger;

        private List<decimal> _priceHistory = new();
        private List<decimal> _volumeHistory = new();
        private List<Position> _openPositions = new();
        private bool _isRunning;
        private decimal _initialBalance;
        private decimal _currentBalance;
        private int _todayTrades = 0;
        private decimal _todayProfitLoss = 0;

        public ConfigurableBot(
            BitkubClient client,
            DatabaseManager db,
            TradingConfig config,
            decimal initialBalance)
        {
            _client = client;
            _db = db;
            _config = config;
            _initialBalance = initialBalance;
            _currentBalance = initialBalance;

            _lineMessenger = new LineMessenger(
                _config.Notifications.LineChannelAccessToken,
                _config.Notifications.LineUserIds,
                _config.Notifications.EnableLineOA
            );
        }

        /// <summary>
        /// เริ่มบอท
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _isRunning = true;

            ShowIntro();

            // ทดสอบ LINE
            if (_config.Notifications.EnableLineOA)
            {
                await _lineMessenger.TestConnectionAsync();
                await Task.Delay(2000);
            }

            while (_isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await RunTradingCycleAsync();
                    await Task.Delay(_config.General.UpdateInterval * 1000, cancellationToken);
                }
                catch (Exception ex)
                {
                    ConsoleUI.ShowError($"Error: {ex.Message}");
                    await _lineMessenger.NotifyErrorAsync(ex.Message, ex.StackTrace);
                    await Task.Delay(10000, cancellationToken);
                }
            }
        }

        private void ShowIntro()
        {
            AnsiConsole.Clear();

            var title = new FigletText("CONFIGURABLE BOT")
            {
                Color = Color.Gold1
            };

            AnsiConsole.Write(title);

            var panel = new Panel(
                Align.Center(
                    new Markup(
                        $"[bold gold1]🎛️ Configurable Trading Bot 🎛️[/]\n\n" +
                        $"[cyan]Symbol:[/] [yellow]{_config.General.Symbol}[/]\n" +
                        $"[cyan]Mode:[/] [green]{_config.General.TradingMode}[/]\n" +
                        $"[cyan]Balance:[/] [green]{_initialBalance:N0}[/] THB\n\n" +
                        $"[cyan]Auto Trading:[/] {(_config.General.EnableAutoTrading ? "[green]ON[/]" : "[red]OFF[/]")}\n" +
                        $"[cyan]LINE OA:[/] {(_config.Notifications.EnableLineOA ? "[green]ON[/]" : "[red]OFF[/]")}\n\n" +
                        $"[dim]ปรับแต่งได้ทุกอย่างใน trading_config.json[/]"
                    ),
                    VerticalAlignment.Middle
                ))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Gold1),
                Padding = new Padding(2, 1)
            };

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }

        /// <summary>
        /// รอบการเทรด
        /// </summary>
        private async Task RunTradingCycleAsync()
        {
            // 1. ดึงข้อมูลตลาด
            var ticker = await _client.GetTickerAsync(_config.General.Symbol);
            if (!ticker.TryGetValue(_config.General.Symbol, out var tickerInfo))
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
                ConsoleUI.ShowInfo($"📊 กำลังรวบรวมข้อมูล... ({_priceHistory.Count}/50)");
                return;
            }

            // 2. วิเคราะห์แบบครบถ้วน
            var analysis = await PerformAnalysisAsync(tickerInfo);

            // 3. แสดงผล
            DisplayAnalysis(analysis, currentPrice);

            // 4. ตรวจสอบ Positions ที่เปิดอยู่
            await ManageOpenPositionsAsync(currentPrice, analysis);

            // 5. ตรวจสอบสัญญาณเข้า (ถ้ายังไม่เต็ม positions)
            if (_openPositions.Count < _config.General.MaxOpenPositions)
            {
                await CheckEntrySignalAsync(currentPrice, analysis);
            }

            // 6. ตรวจสอบ Custom Events
            await CheckCustomEventsAsync(currentPrice, analysis);
        }

        /// <summary>
        /// วิเคราะห์ตลาด
        /// </summary>
        private async Task<UltimateAnalysis> PerformAnalysisAsync(TickerInfo ticker)
        {
            var analysis = new UltimateAnalysis();

            // Technical
            analysis.TechnicalSignal = TechnicalAnalysis.GenerateSignal(_priceHistory);

            // Elliott Wave
            analysis.ElliottWave = AdvancedAnalysis.DetectElliottWave(_priceHistory);

            // Fibonacci
            analysis.FibonacciLevels = AdvancedAnalysis.FindFibonacciSupportResistance(_priceHistory);

            // Volume Profile
            analysis.VolumeProfile = AdvancedAnalysis.CalculateVolumeProfile(_priceHistory, _volumeHistory);

            // Market Structure
            analysis.MarketStructure = AdvancedAnalysis.AnalyzeMarketStructure(_priceHistory);

            // Order Flow
            var depth = await _client.GetDepthAsync(_config.General.Symbol, 20);
            analysis.OrderFlow = AdvancedAnalysis.AnalyzeOrderFlow(depth.Bids, depth.Asks);

            // ML
            analysis.MLPredictions = MachineLearning.PredictNextPrices(_priceHistory, 5);
            analysis.LinearRegression = MachineLearning.LinearRegression(_priceHistory);
            analysis.RecognizedPatterns = MachineLearning.RecognizePatterns(_priceHistory);

            // News
            try
            {
                var newsAnalyzer = new NewsAnalyzer();
                analysis.NewsSentiment = await newsAnalyzer.GetMarketSentimentAsync();
            }
            catch { }

            // Master Score
            analysis.MasterScore = CalculateMasterScore(analysis);

            return analysis;
        }

        /// <summary>
        /// คำนวณ Master Score ตาม Config
        /// </summary>
        private int CalculateMasterScore(UltimateAnalysis analysis)
        {
            var score = 50;

            // Technical (20 points)
            if (analysis.TechnicalSignal.Action.Contains("STRONG_BUY")) score += 20;
            else if (analysis.TechnicalSignal.Action.Contains("BUY")) score += 10;
            else if (analysis.TechnicalSignal.Action.Contains("STRONG_SELL")) score -= 20;
            else if (analysis.TechnicalSignal.Action.Contains("SELL")) score -= 10;

            // Elliott Wave (15 points)
            if (_config.Entry.ElliottWaveConditions.PreferredWaves
                .Contains(analysis.ElliottWave.CurrentWave))
                score += 15;
            else if (analysis.ElliottWave.Pattern == "CORRECTIVE_WAVE")
                score -= 10;

            // Fibonacci (10 points)
            var nearestFib = analysis.FibonacciLevels.FirstOrDefault();
            if (nearestFib != null)
            {
                if (nearestFib.Type == "SUPPORT" &&
                    Math.Abs(nearestFib.Distance) < _config.Entry.FibonacciConditions.MaxDistanceFromSupport)
                    score += 10;
                else if (nearestFib.Type == "RESISTANCE" && Math.Abs(nearestFib.Distance) < 1)
                    score -= 10;
            }

            // Volume Profile (10 points)
            var currentPrice = _priceHistory.Last();
            if (_config.Entry.VolumeProfileConditions.RequireAbovePOC)
            {
                if (currentPrice > analysis.VolumeProfile.POC) score += 5;
                else score -= 5;
            }

            // Market Structure (15 points)
            if (analysis.MarketStructure.Trend == _config.Entry.MarketStructureConditions.RequiredTrend)
                score += 15;
            else if (analysis.MarketStructure.Trend != "RANGING")
                score -= 15;

            // Order Flow (10 points)
            if (analysis.OrderFlow.Ratio >= _config.Entry.OrderFlowConditions.MinBidAskRatio)
                score += 10;
            else if (analysis.OrderFlow.Ratio < 1.0m / _config.Entry.OrderFlowConditions.MinBidAskRatio)
                score -= 10;

            // ML (10 points)
            if (analysis.LinearRegression.Trend == "UPTREND") score += 10;
            else if (analysis.LinearRegression.Trend == "DOWNTREND") score -= 10;

            // Patterns (5 points)
            var pattern = analysis.RecognizedPatterns.MostLikelyPattern;
            if (pattern.Contains("DOUBLE_BOTTOM") || pattern.Contains("CUP") ||
                pattern.Contains("ASCENDING"))
                score += 5;
            else if (pattern.Contains("DOUBLE_TOP") || pattern.Contains("DESCENDING") ||
                     pattern.Contains("HEAD"))
                score -= 5;

            // News (5 points)
            if (analysis.NewsSentiment?.OverallSentiment.Contains("POSITIVE") == true)
                score += 5;
            else if (analysis.NewsSentiment?.OverallSentiment.Contains("NEGATIVE") == true)
                score -= 5;

            return Math.Clamp(score, 0, 100);
        }

        /// <summary>
        /// ตรวจสอบสัญญาณเข้า
        /// </summary>
        private async Task CheckEntrySignalAsync(decimal currentPrice, UltimateAnalysis analysis)
        {
            // ตรวจสอบเงื่อนไข
            if (analysis.MasterScore < _config.Entry.MinMasterScore)
                return;

            // ตรวจสอบ Technical Conditions
            if (!CheckTechnicalConditions(analysis))
                return;

            // ตรวจสอบ Elliott Wave
            if (!CheckElliottWaveConditions(analysis))
                return;

            // ตรวจสอบ Market Structure
            if (!CheckMarketStructureConditions(analysis))
                return;

            // ตรวจสอบ Order Flow
            if (!CheckOrderFlowConditions(analysis))
                return;

            // ตรวจสอบ Risk Management
            if (_todayTrades >= _config.Risk.MaxDailyTrades)
            {
                ConsoleUI.ShowWarning("⚠️ ถึงจำนวน trades สูงสุดต่อวันแล้ว");
                return;
            }

            if (Math.Abs(_todayProfitLoss) >= _currentBalance * (_config.Risk.MaxDailyLossPercent / 100))
            {
                ConsoleUI.ShowWarning("⚠️ ถึง loss limit ต่อวันแล้ว");
                return;
            }

            // สร้างแผนการเทรด
            var plan = TradingPlanBuilder.CreatePlan(currentPrice, _config, analysis, _currentBalance);

            // แสดงกราฟวางแผน
            ChartPlanner.ShowEntryPlan(currentPrice, plan, analysis);

            // แจ้งเตือน LINE
            if (_config.Notifications.NotifyOnSignal)
            {
                var reasoning = BuildReasoning(analysis);
                await _lineMessenger.NotifySignalAsync(
                    _config.General.Symbol,
                    "BUY",
                    analysis.MasterScore,
                    reasoning
                );
            }

            // ถ้าเปิด Auto Trading
            if (_config.General.EnableAutoTrading)
            {
                await ExecuteEntryAsync(plan, analysis);
            }
            else
            {
                ConsoleUI.ShowInfo("\n💡 Auto Trading ปิดอยู่ - กด Enter เพื่อข้ามสัญญาณนี้");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// ทำการเข้า Position
        /// </summary>
        private async Task ExecuteEntryAsync(TradingPlan plan, UltimateAnalysis analysis)
        {
            var entry = plan.EntryPoints.First();
            var position = new Position
            {
                Symbol = plan.Symbol,
                EntryPrice = entry.Price,
                Amount = entry.Amount,
                StopLoss = plan.StopLoss,
                TakeProfitLevels = plan.TakeProfitLevels,
                EntryTime = DateTime.Now,
                Strategy = plan.Strategy
            };

            _openPositions.Add(position);
            _todayTrades++;

            ConsoleUI.ShowSuccess($"✅ เข้า Position: {entry.Amount:N8} @ {entry.Price:N2}");

            // แจ้งเตือน LINE
            if (_config.Notifications.NotifyOnEntry)
            {
                await _lineMessenger.NotifyEntryAsync(
                    plan.Symbol,
                    entry.Price,
                    entry.Amount,
                    plan.StopLoss,
                    plan.TakeProfitLevels.First().Price
                );
            }

            // บันทึก Database
            await SavePositionToDBAsync(position, analysis);
        }

        /// <summary>
        /// จัดการ Positions ที่เปิดอยู่
        /// </summary>
        private async Task ManageOpenPositionsAsync(decimal currentPrice, UltimateAnalysis analysis)
        {
            var positionsToClose = new List<Position>();

            foreach (var position in _openPositions)
            {
                // ตรวจสอบ Stop Loss
                if (currentPrice <= position.StopLoss)
                {
                    ConsoleUI.ShowError($"🛑 Stop Loss ถูกกระตุ้น! {currentPrice:N2} <= {position.StopLoss:N2}");

                    var loss = (currentPrice - position.EntryPrice) * position.Amount;
                    _todayProfitLoss += loss;
                    _currentBalance += loss;

                    await _lineMessenger.NotifyStopLossAsync(
                        position.Symbol,
                        position.EntryPrice,
                        currentPrice,
                        loss
                    );

                    await _lineMessenger.NotifyExitAsync(
                        position.Symbol,
                        position.EntryPrice,
                        currentPrice,
                        position.Amount,
                        "Stop Loss"
                    );

                    positionsToClose.Add(position);
                    continue;
                }

                // ตรวจสอบ Take Profit
                foreach (var tp in position.TakeProfitLevels.Where(t => !t.Reached))
                {
                    if (currentPrice >= tp.Price)
                    {
                        var tpAmount = position.Amount * (tp.Percentage / 100m);
                        var profit = (currentPrice - position.EntryPrice) * tpAmount;

                        ConsoleUI.ShowSuccess($"🎯 Take Profit {tp.Percentage}% ถึงเป้า! กำไร: {profit:N2}");

                        _todayProfitLoss += profit;
                        _currentBalance += profit;

                        await _lineMessenger.NotifyTakeProfitAsync(
                            position.Symbol,
                            position.EntryPrice,
                            currentPrice,
                            profit
                        );

                        tp.Reached = true;
                        position.Amount -= tpAmount;

                        if (position.Amount <= 0)
                        {
                            positionsToClose.Add(position);
                        }
                    }
                }

                // Trailing Stop
                if (_config.Exit.EnableTrailingStop && position.HighestPrice > 0)
                {
                    if (currentPrice > position.HighestPrice)
                    {
                        position.HighestPrice = currentPrice;
                        position.TrailingStop = currentPrice * (1 - _config.Exit.TrailingStopPercent / 100);
                        ConsoleUI.ShowInfo($"📈 Trailing Stop อัพเดท: {position.TrailingStop:N2}");
                    }

                    if (currentPrice <= position.TrailingStop)
                    {
                        ConsoleUI.ShowWarning($"⚠️ Trailing Stop ถูกกระตุ้น!");
                        positionsToClose.Add(position);
                    }
                }
                else if (_config.Exit.EnableTrailingStop)
                {
                    position.HighestPrice = currentPrice;
                    position.TrailingStop = currentPrice * (1 - _config.Exit.TrailingStopPercent / 100);
                }

                // Exit on Master Score
                if (_config.Exit.ExitOnMasterScoreBelow > 0 &&
                    analysis.MasterScore < _config.Exit.ExitOnMasterScoreBelow)
                {
                    ConsoleUI.ShowWarning($"⚠️ Master Score ต่ำเกินไป ({analysis.MasterScore})");
                    positionsToClose.Add(position);
                }

                // Max Holding Time
                var holdingHours = (DateTime.Now - position.EntryTime).TotalHours;
                if (holdingHours >= _config.Exit.MaxHoldingHours)
                {
                    ConsoleUI.ShowWarning($"⏰ ถือครองเกิน {_config.Exit.MaxHoldingHours} ชั่วโมง");
                    positionsToClose.Add(position);
                }
            }

            // ปิด Positions
            foreach (var position in positionsToClose)
            {
                await ClosePositionAsync(position, currentPrice, "Auto Exit");
                _openPositions.Remove(position);
            }
        }

        private async Task ClosePositionAsync(Position position, decimal exitPrice, string reason)
        {
            var profitLoss = (exitPrice - position.EntryPrice) * position.Amount;
            _todayProfitLoss += profitLoss;
            _currentBalance += profitLoss;

            await _lineMessenger.NotifyExitAsync(
                position.Symbol,
                position.EntryPrice,
                exitPrice,
                position.Amount,
                reason
            );
        }

        /// <summary>
        /// ตรวจสอบ Custom Events
        /// </summary>
        private async Task CheckCustomEventsAsync(decimal currentPrice, UltimateAnalysis analysis)
        {
            foreach (var customEvent in _config.Notifications.CustomEvents)
            {
                var shouldNotify = EvaluateCondition(
                    customEvent.Condition,
                    currentPrice,
                    analysis
                );

                if (shouldNotify)
                {
                    var message = customEvent.Message
                        .Replace("{score}", analysis.MasterScore.ToString())
                        .Replace("{price}", currentPrice.ToString("N2"));

                    await _lineMessenger.NotifyCustomEventAsync(customEvent.Name, message);
                }
            }
        }

        private bool EvaluateCondition(string condition, decimal price, UltimateAnalysis analysis)
        {
            // Simple condition parser
            try
            {
                if (condition.Contains("MasterScore"))
                {
                    if (condition.Contains(">="))
                    {
                        var threshold = int.Parse(condition.Split(">=")[1].Trim());
                        return analysis.MasterScore >= threshold;
                    }
                }

                if (condition.Contains("Price"))
                {
                    if (condition.Contains(">"))
                    {
                        var threshold = decimal.Parse(condition.Split(">")[1].Trim());
                        return price > threshold;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        // === Condition Checkers ===

        private bool CheckTechnicalConditions(UltimateAnalysis analysis)
        {
            var conditions = _config.Entry.TechnicalConditions;

            // RSI check (ถ้ามีข้อมูล)
            var rsi = TechnicalAnalysis.CalculateRSI(_priceHistory);
            if (rsi > 0 && (rsi < conditions.RSI_Min || rsi > conditions.RSI_Max))
                return false;

            return true;
        }

        private bool CheckElliottWaveConditions(UltimateAnalysis analysis)
        {
            var conditions = _config.Entry.ElliottWaveConditions;

            if (conditions.PreferredWaves.Any())
            {
                if (!conditions.PreferredWaves.Contains(analysis.ElliottWave.CurrentWave))
                    return false;
            }

            if (analysis.ElliottWave.Confidence < conditions.MinConfidence)
                return false;

            return true;
        }

        private bool CheckMarketStructureConditions(UltimateAnalysis analysis)
        {
            var conditions = _config.Entry.MarketStructureConditions;

            if (!string.IsNullOrEmpty(conditions.RequiredTrend))
            {
                if (analysis.MarketStructure.Trend != conditions.RequiredTrend)
                    return false;
            }

            return true;
        }

        private bool CheckOrderFlowConditions(UltimateAnalysis analysis)
        {
            var conditions = _config.Entry.OrderFlowConditions;

            if (analysis.OrderFlow.Ratio < conditions.MinBidAskRatio)
                return false;

            if (conditions.RequireBuyingPressure &&
                analysis.OrderFlow.Pressure != "BUYING_PRESSURE")
                return false;

            return true;
        }

        private void DisplayAnalysis(UltimateAnalysis analysis, decimal currentPrice)
        {
            AnsiConsole.Clear();

            // Master Score
            var scoreColor = analysis.MasterScore >= 70 ? "green" :
                            analysis.MasterScore <= 30 ? "red" : "yellow";

            var scorePanel = new Panel(
                Align.Center(
                    new Markup($"[bold {scoreColor}]{analysis.MasterScore}[/]/100")
                ))
            {
                Header = new PanelHeader("🎯 MASTER SCORE", Justify.Center),
                Border = BoxBorder.Heavy
            };

            AnsiConsole.Write(scorePanel);

            // Status
            var statusTable = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("Info")
                .AddColumn("Value");

            statusTable.AddRow("💰 Balance", $"{_currentBalance:N2} THB");
            statusTable.AddRow("📊 Today P/L", $"{_todayProfitLoss:N2} THB");
            statusTable.AddRow("🔢 Today Trades", $"{_todayTrades}");
            statusTable.AddRow("📈 Open Positions", $"{_openPositions.Count}");

            AnsiConsole.Write(statusTable);

            ConsoleUI.ShowInfo($"\n💡 กด Ctrl+C เพื่อหยุด | อัพเดทใน {_config.General.UpdateInterval} วินาที");
        }

        private string BuildReasoning(UltimateAnalysis analysis)
        {
            var reasons = new List<string>
            {
                $"Technical: {analysis.TechnicalSignal.Action}",
                $"Elliott: {analysis.ElliottWave.CurrentWave}",
                $"Structure: {analysis.MarketStructure.Trend}",
                $"ML: {analysis.LinearRegression.Trend}"
            };

            return string.Join(" | ", reasons);
        }

        private async Task SavePositionToDBAsync(Position position, UltimateAnalysis analysis)
        {
            await _db.SavePriceAsync(
                position.Symbol,
                position.EntryPrice,
                _priceHistory.Max(),
                _priceHistory.Min(),
                _volumeHistory.Last(),
                0
            );

            await _db.SaveSignalAsync(
                position.Symbol,
                analysis.TechnicalSignal,
                position.EntryPrice
            );
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }

    /// <summary>
    /// Position ที่เปิดอยู่
    /// </summary>
    public class Position
    {
        public string Symbol { get; set; } = "";
        public decimal EntryPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal StopLoss { get; set; }
        public List<TakeProfitLevel> TakeProfitLevels { get; set; } = new();
        public DateTime EntryTime { get; set; }
        public string Strategy { get; set; } = "";

        // Trailing Stop
        public decimal HighestPrice { get; set; }
        public decimal TrailingStop { get; set; }
    }
}
