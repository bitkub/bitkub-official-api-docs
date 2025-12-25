using System;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace BitkubTrader
{
    /// <summary>
    /// 🎛️ CONFIGURABLE TRADING SYSTEM
    /// โปรแกรมหลักสำหรับระบบที่ปรับแต่งได้ทุกอย่าง
    /// </summary>
    class ProgramConfigurable
    {
        private const string API_KEY = "YOUR_API_KEY";
        private const string API_SECRET = "YOUR_API_SECRET";
        private const string CONFIG_FILE = "trading_config.json";

        static async Task Main(string[] args)
        {
            ShowSuperIntro();
            await Task.Delay(2000);

            using var client = new BitkubClient(API_KEY, API_SECRET);
            using var db = new DatabaseManager();

            while (true)
            {
                try
                {
                    var choice = ShowMainMenu();

                    if (choice.Contains("Exit"))
                        break;

                    if (choice.Contains("Start Bot"))
                        await StartConfigurableBot(client, db);
                    else if (choice.Contains("Edit Config"))
                        await EditConfiguration();
                    else if (choice.Contains("View Config"))
                        ViewConfiguration();
                    else if (choice.Contains("Test LINE"))
                        await TestLineNotification();
                    else if (choice.Contains("Create Plan"))
                        await CreateTradingPlan(client, db);
                    else if (choice.Contains("View Positions"))
                        ViewOpenPositions();
                    else if (choice.Contains("Performance"))
                        await ViewPerformance(db);

                    if (!choice.Contains("Start Bot"))
                    {
                        ConsoleUI.ShowInfo("\nกด Enter เพื่อกลับเมนู...");
                        Console.ReadLine();
                    }
                }
                catch (Exception ex)
                {
                    ConsoleUI.ShowError($"Error: {ex.Message}");
                    await Task.Delay(2000);
                }
            }

            ShowGoodbye();
        }

        static void ShowSuperIntro()
        {
            AnsiConsole.Clear();

            var title = new FigletText("CONFIGURABLE")
            {
                Color = Color.Gold1
            };

            AnsiConsole.Write(title);

            var subtitle = new FigletText("TRADING SYSTEM")
            {
                Color = Color.Cyan1
            };

            AnsiConsole.Write(subtitle);
            AnsiConsole.WriteLine();

            var panel = new Panel(
                Align.Center(
                    new Markup(
                        "[bold gold1]🎛️ ระบบเทรดที่ปรับแต่งได้ทุกอย่าง! 🎛️[/]\n\n" +
                        "[green]✓[/] ตั้งค่ากลยุทธ์ได้ละเอียดทุกอย่าง\n" +
                        "[green]✓[/] แสดงกราฟวางแผนพร้อมจุดเข้า/ออก\n" +
                        "[green]✓[/] ส่งแจ้งเตือนไป LINE ตามเหตุการณ์\n" +
                        "[green]✓[/] Master Score 100 คะแนน\n" +
                        "[green]✓[/] Risk Management อัตโนมัติ\n" +
                        "[green]✓[/] Trailing Stop & Multiple TP\n\n" +
                        "[bold yellow]ใช้งานง่าย เข้าใจง่าย ปรับแต่งได้ครบ![/]\n" +
                        "[dim]แก้ไข Config ใน trading_config.json[/]"
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

        static string ShowMainMenu()
        {
            return ConsoleUI.ShowMenu(
                "🎛️ CONFIGURABLE TRADING SYSTEM - เลือกเมนู",
                new[]
                {
                    "🚀 Start Configurable Bot (เริ่มเทรด!)",
                    "⚙️ Edit Configuration (แก้ไข Config)",
                    "👀 View Current Configuration",
                    "📱 Test LINE Notification",
                    "📊 Create Trading Plan (วางแผนการเทรด)",
                    "💼 View Open Positions",
                    "📈 View Performance Analytics",
                    "❌ Exit"
                }
            );
        }

        static async Task StartConfigurableBot(BitkubClient client, DatabaseManager db)
        {
            AnsiConsole.Clear();

            // โหลด Config
            var config = TradingConfig.Load(CONFIG_FILE);

            // Validate
            var (isValid, errors) = config.Validate();
            if (!isValid)
            {
                ConsoleUI.ShowError("❌ Config ไม่ถูกต้อง:");
                foreach (var error in errors)
                {
                    ConsoleUI.ShowError($"  - {error}");
                }
                return;
            }

            // แสดง Config Summary
            ShowConfigSummary(config);

            var balance = decimal.Parse(ConsoleUI.AskInput("Initial Balance (THB)", "10000"));

            if (!ConsoleUI.Confirm($"เริ่ม Configurable Bot (Auto Trading: {config.General.EnableAutoTrading})?"))
                return;

            var bot = new ConfigurableBot(client, db, config, balance);

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                bot.Stop();
            };

            await bot.StartAsync(cts.Token);
        }

        static void ShowConfigSummary(TradingConfig config)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Cyan1)
                .AddColumn("[cyan]Setting[/]")
                .AddColumn("[yellow]Value[/]");

            table.AddRow("Symbol", config.General.Symbol);
            table.AddRow("Mode", config.General.TradingMode);
            table.AddRow("Auto Trading", config.General.EnableAutoTrading ? "ON" : "OFF");
            table.AddRow("Update Interval", $"{config.General.UpdateInterval}s");
            table.AddEmptyRow();

            table.AddRow("[bold]Entry Logic[/]", "");
            table.AddRow("  Min Master Score", config.Entry.MinMasterScore.ToString());
            table.AddRow("  Min Bullish Indicators", config.Entry.MinBullishIndicators.ToString());
            table.AddEmptyRow();

            table.AddRow("[bold]Exit Logic[/]", "");
            table.AddRow("  Stop Loss", $"{config.Exit.StopLossPercent}%");
            table.AddRow("  Take Profit", $"{config.Exit.TakeProfitPercent}%");
            table.AddRow("  Trailing Stop", config.Exit.EnableTrailingStop ? $"{config.Exit.TrailingStopPercent}%" : "OFF");
            table.AddEmptyRow();

            table.AddRow("[bold]Risk Management[/]", "");
            table.AddRow("  Max Risk/Trade", $"{config.Risk.MaxRiskPerTradePercent}%");
            table.AddRow("  Max Daily Loss", $"{config.Risk.MaxDailyLossPercent}%");
            table.AddRow("  Max Daily Trades", config.Risk.MaxDailyTrades.ToString());
            table.AddEmptyRow();

            table.AddRow("[bold]Notifications[/]", "");
            table.AddRow("  LINE Notify", config.Notifications.EnableLineNotify ? "ON" : "OFF");
            table.AddRow("  Notify on Signal", config.Notifications.NotifyOnSignal ? "ON" : "OFF");
            table.AddRow("  Notify on Entry", config.Notifications.NotifyOnEntry ? "ON" : "OFF");

            var panel = new Panel(table)
            {
                Header = new PanelHeader("⚙️ [bold]Configuration Summary[/]", Justify.Center),
                Border = BoxBorder.Double
            };

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }

        static async Task EditConfiguration()
        {
            AnsiConsole.Clear();

            var config = TradingConfig.Load(CONFIG_FILE);

            AnsiConsole.MarkupLine("[yellow]📝 แก้ไข Configuration[/]\n");

            var section = ConsoleUI.ShowMenu(
                "เลือกส่วนที่ต้องการแก้ไข",
                new[]
                {
                    "⚙️ General Settings",
                    "🎯 Entry Logic",
                    "🚪 Exit Logic",
                    "💰 Risk Management",
                    "📊 Indicators",
                    "📱 Notifications",
                    "💾 Save & Exit"
                }
            );

            if (section.Contains("General"))
            {
                config.General.TradingMode = ConsoleUI.AskInput("Trading Mode", config.General.TradingMode);
                config.General.Symbol = ConsoleUI.AskInput("Symbol", config.General.Symbol);
                config.General.UpdateInterval = int.Parse(ConsoleUI.AskInput("Update Interval (s)", config.General.UpdateInterval.ToString()));
                config.General.EnableAutoTrading = ConsoleUI.Confirm("Enable Auto Trading?");
            }
            else if (section.Contains("Entry"))
            {
                config.Entry.MinMasterScore = int.Parse(ConsoleUI.AskInput("Min Master Score", config.Entry.MinMasterScore.ToString()));
                config.Entry.MinBullishIndicators = int.Parse(ConsoleUI.AskInput("Min Bullish Indicators", config.Entry.MinBullishIndicators.ToString()));
            }
            else if (section.Contains("Exit"))
            {
                config.Exit.StopLossPercent = decimal.Parse(ConsoleUI.AskInput("Stop Loss %", config.Exit.StopLossPercent.ToString()));
                config.Exit.TakeProfitPercent = decimal.Parse(ConsoleUI.AskInput("Take Profit %", config.Exit.TakeProfitPercent.ToString()));
                config.Exit.EnableTrailingStop = ConsoleUI.Confirm("Enable Trailing Stop?");
                if (config.Exit.EnableTrailingStop)
                {
                    config.Exit.TrailingStopPercent = decimal.Parse(ConsoleUI.AskInput("Trailing Stop %", config.Exit.TrailingStopPercent.ToString()));
                }
            }
            else if (section.Contains("Risk"))
            {
                config.Risk.MaxRiskPerTradePercent = decimal.Parse(ConsoleUI.AskInput("Max Risk per Trade %", config.Risk.MaxRiskPerTradePercent.ToString()));
                config.Risk.MaxDailyLossPercent = decimal.Parse(ConsoleUI.AskInput("Max Daily Loss %", config.Risk.MaxDailyLossPercent.ToString()));
                config.Risk.MaxDailyTrades = int.Parse(ConsoleUI.AskInput("Max Daily Trades", config.Risk.MaxDailyTrades.ToString()));
            }
            else if (section.Contains("Notifications"))
            {
                config.Notifications.EnableLineNotify = ConsoleUI.Confirm("Enable LINE Notify?");
                if (config.Notifications.EnableLineNotify)
                {
                    config.Notifications.LineAccessToken = ConsoleUI.AskInput("LINE Access Token", config.Notifications.LineAccessToken);
                    config.Notifications.NotifyOnSignal = ConsoleUI.Confirm("Notify on Signal?");
                    config.Notifications.NotifyOnEntry = ConsoleUI.Confirm("Notify on Entry?");
                    config.Notifications.NotifyOnExit = ConsoleUI.Confirm("Notify on Exit?");
                }
            }

            config.Save(CONFIG_FILE);
            ConsoleUI.ShowSuccess($"✅ บันทึก Config แล้ว: {CONFIG_FILE}");
        }

        static void ViewConfiguration()
        {
            AnsiConsole.Clear();

            var config = TradingConfig.Load(CONFIG_FILE);

            AnsiConsole.MarkupLine($"[cyan]📄 Configuration File: {CONFIG_FILE}[/]\n");

            ShowConfigSummary(config);

            // แสดง Indicator Settings
            var indicatorTable = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("Indicator")
                .AddColumn("Settings");

            indicatorTable.AddRow("RSI", $"Period: {config.Indicators.RSI_Period}, OB: {config.Indicators.RSI_Overbought}, OS: {config.Indicators.RSI_Oversold}");
            indicatorTable.AddRow("MACD", $"Fast: {config.Indicators.MACD_Fast}, Slow: {config.Indicators.MACD_Slow}, Signal: {config.Indicators.MACD_Signal}");
            indicatorTable.AddRow("Bollinger Bands", $"Period: {config.Indicators.BollingerBands_Period}, StdDev: {config.Indicators.BollingerBands_StdDev}");
            indicatorTable.AddRow("ATR", $"Period: {config.Indicators.ATR_Period}, Multiplier: {config.Indicators.ATR_Multiplier}");

            var indicatorPanel = new Panel(indicatorTable)
            {
                Header = new PanelHeader("📊 Indicator Settings"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(indicatorPanel);

            // Validate
            var (isValid, errors) = config.Validate();
            if (isValid)
            {
                ConsoleUI.ShowSuccess("\n✅ Config ถูกต้อง พร้อมใช้งาน!");
            }
            else
            {
                ConsoleUI.ShowError("\n❌ Config มีปัญหา:");
                foreach (var error in errors)
                {
                    ConsoleUI.ShowError($"  - {error}");
                }
            }
        }

        static async Task TestLineNotification()
        {
            AnsiConsole.Clear();

            var config = TradingConfig.Load(CONFIG_FILE);

            if (!config.Notifications.EnableLineNotify)
            {
                ConsoleUI.ShowWarning("⚠️ LINE Notify ยังไม่ได้เปิดใช้งานใน Config");
                return;
            }

            var lineNotifier = new LineNotifier(
                config.Notifications.LineAccessToken,
                config.Notifications.EnableLineNotify
            );

            await lineNotifier.TestConnectionAsync();
        }

        static async Task CreateTradingPlan(BitkubClient client, DatabaseManager db)
        {
            AnsiConsole.Clear();

            var config = TradingConfig.Load(CONFIG_FILE);

            ConsoleUI.ShowInfo("📊 สร้างแผนการเทรด...\n");

            // Get current price
            var ticker = await client.GetTickerAsync(config.General.Symbol);
            if (!ticker.TryGetValue(config.General.Symbol, out var tickerInfo))
            {
                ConsoleUI.ShowError("❌ ไม่สามารถดึงราคาได้");
                return;
            }

            var currentPrice = tickerInfo.Last;

            // Simulate analysis (ในการใช้งานจริงควรดึงข้อมูลจริง)
            var analysis = new UltimateAnalysis
            {
                MasterScore = 75,
                TechnicalSignal = new TradingSignal { Action = "BUY", Score = 3 },
                ElliottWave = new ElliottWavePattern { Pattern = "IMPULSE_WAVE", CurrentWave = "WAVE_3", Confidence = 80 },
                FibonacciLevels = new List<FibLevel>
                {
                    new FibLevel { Name = "61.8%", Price = currentPrice * 0.95m, Type = "SUPPORT", Distance = -5 },
                    new FibLevel { Name = "50%", Price = currentPrice * 0.97m, Type = "SUPPORT", Distance = -3 }
                },
                VolumeProfile = new VolumeProfile { POC = currentPrice * 0.98m, VAH = currentPrice * 1.05m, VAL = currentPrice * 0.92m },
                MarketStructure = new MarketStructure { Trend = "UPTREND", LastBreak = "BULLISH_BOS" },
                OrderFlow = new OrderFlowAnalysis { Ratio = 1.5m, Pressure = "BUYING_PRESSURE" },
                LinearRegression = new LinearRegressionResult { Trend = "UPTREND", RSquared = 0.85 }
            };

            var balance = decimal.Parse(ConsoleUI.AskInput("Balance (THB)", "10000"));

            // สร้างแผน
            var plan = TradingPlanBuilder.CreatePlan(currentPrice, config, analysis, balance);

            // แสดงกราฟ
            ChartPlanner.ShowEntryPlan(currentPrice, plan, analysis);
        }

        static void ViewOpenPositions()
        {
            AnsiConsole.Clear();
            ConsoleUI.ShowInfo("💼 Open Positions\n");
            ConsoleUI.ShowWarning("ฟีเจอร์นี้จะแสดงข้อมูลจาก Database ในเวอร์ชันเต็ม");
        }

        static async Task ViewPerformance(DatabaseManager db)
        {
            AnsiConsole.Clear();

            var config = TradingConfig.Load(CONFIG_FILE);

            ConsoleUI.ShowInfo("📈 กำลังโหลดข้อมูล...\n");

            var performance = await db.GetPerformanceAsync(config.General.Symbol);

            if (performance == null)
            {
                ConsoleUI.ShowWarning("⚠️ ยังไม่มีข้อมูล Performance");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Green)
                .AddColumn("[cyan]Metric[/]")
                .AddColumn("[yellow]Value[/]");

            table.AddRow("Win Rate", $"{performance.WinRate:N2}%");
            table.AddRow("Net Profit", $"{performance.NetProfit:N2} THB");
            table.AddRow("Profit Factor", $"{performance.ProfitFactor:N2}");
            table.AddRow("Avg Win", $"{performance.AvgWin:N2} THB");
            table.AddRow("Avg Loss", $"{performance.AvgLoss:N2} THB");

            var panel = new Panel(table)
            {
                Header = new PanelHeader("📊 Performance Analytics"),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Green)
            };

            AnsiConsole.Write(panel);
        }

        static void ShowGoodbye()
        {
            AnsiConsole.Clear();

            var panel = new Panel(
                Align.Center(
                    new Markup(
                        "[bold gold1]🎛️ CONFIGURABLE TRADING SYSTEM 🎛️[/]\n\n" +
                        "[yellow]ขอบคุณที่ใช้งาน![/]\n" +
                        "[green]Happy Trading! 💰[/]\n\n" +
                        "[dim]Config ของคุณถูกบันทึกไว้ที่ trading_config.json[/]\n" +
                        "[dim]สามารถแก้ไขได้ตลอดเวลา[/]"
                    ),
                    VerticalAlignment.Middle
                ))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Gold1)
            };

            AnsiConsole.Write(panel);
        }
    }
}
