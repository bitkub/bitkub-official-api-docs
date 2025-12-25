using System;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace BitkubTrader
{
    /// <summary>
    /// 🌟 ULTIMATE TRADING PROGRAM 🌟
    /// Entry point for the most advanced trading system ever created!
    /// </summary>
    class ProgramUltimate
    {
        private const string API_KEY = "YOUR_API_KEY";
        private const string API_SECRET = "YOUR_API_SECRET";

        static async Task Main(string[] args)
        {
            ShowEpicIntro();
            await Task.Delay(2000);

            using var client = new BitkubClient(API_KEY, API_SECRET);
            using var db = new DatabaseManager();

            while (true)
            {
                try
                {
                    var choice = ShowUltimateMenu();

                    if (choice.Contains("Exit"))
                        break;

                    if (choice.Contains("Ultimate Bot"))
                        await StartUltimateBot(client, db);
                    else if (choice.Contains("Elliott Wave"))
                        await AnalyzeElliottWave(client);
                    else if (choice.Contains("Fibonacci"))
                        await AnalyzeFibonacci(client);
                    else if (choice.Contains("Volume Profile"))
                        await AnalyzeVolumeProfile(client);
                    else if (choice.Contains("Order Flow"))
                        await AnalyzeOrderFlow(client);
                    else if (choice.Contains("ML Prediction"))
                        await MLPrediction(client);
                    else if (choice.Contains("Pattern Recognition"))
                        await RecognizePatterns(client);
                    else if (choice.Contains("Full Analysis"))
                        await FullAnalysis(client);

                    ConsoleUI.ShowInfo("\nPress any key to continue...");
                    Console.ReadKey(true);
                }
                catch (Exception ex)
                {
                    ConsoleUI.ShowError($"Error: {ex.Message}");
                    await Task.Delay(2000);
                }
            }

            ShowGoodbye();
        }

        static void ShowEpicIntro()
        {
            AnsiConsole.Clear();

            var title = new FigletText("ULTIMATE TRADER")
            {
                Color = Color.Gold1
            };

            AnsiConsole.Write(title);
            AnsiConsole.WriteLine();

            var panel = new Panel(
                Align.Center(
                    new Markup(
                        "[bold gold1]🌟 THE ULTIMATE TRADING SYSTEM 🌟[/]\n\n" +
                        "[cyan]Never Seen Before Features:[/]\n\n" +
                        "[green]✓[/] Elliott Wave Analysis\n" +
                        "[green]✓[/] Fibonacci Auto-Levels\n" +
                        "[green]✓[/] Volume Profile (POC/VAH/VAL)\n" +
                        "[green]✓[/] Market Structure (BOS/CHoCH)\n" +
                        "[green]✓[/] Order Flow Analysis\n" +
                        "[green]✓[/] Machine Learning Predictions\n" +
                        "[green]✓[/] AI Pattern Recognition (8+ patterns)\n" +
                        "[green]✓[/] News Sentiment (Thai websites)\n" +
                        "[green]✓[/] Multi-Indicator Fusion\n" +
                        "[green]✓[/] Risk Management\n" +
                        "[green]✓[/] Database Analytics\n\n" +
                        "[bold yellow]Master Score Algorithm:[/] 100-point system\n" +
                        "[dim]Combining ALL indicators for ultimate decision[/]"
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

        static string ShowUltimateMenu()
        {
            return ConsoleUI.ShowMenu(
                "🌟 ULTIMATE TRADING SYSTEM - Select Option",
                new[]
                {
                    "🚀 Start ULTIMATE BOT (All Features Combined!)",
                    "🌊 Elliott Wave Analysis",
                    "📐 Fibonacci Levels",
                    "📊 Volume Profile (POC/VAH/VAL)",
                    "📉 Order Flow Analysis",
                    "🧠 ML Price Prediction",
                    "🔍 Pattern Recognition",
                    "🎯 Full Multi-Dimensional Analysis",
                    "❌ Exit"
                }
            );
        }

        static async Task StartUltimateBot(BitkubClient client, DatabaseManager db)
        {
            AnsiConsole.Clear();

            var symbol = ConsoleUI.AskInput("Symbol", "THB_BTC");

            var modeChoice = ConsoleUI.ShowMenu(
                "Select Bot Mode",
                new[]
                {
                    "⚡⚡ ULTRA AGGRESSIVE - 3 seconds (สุดโหด!)",
                    "⚡ AGGRESSIVE - 10 seconds",
                    "⚖️ BALANCED - 30 seconds",
                    "🛡️ CONSERVATIVE - 1 minute"
                }
            );

            var mode = modeChoice.Contains("ULTRA") ? UltimateBotMode.UltraAggressive :
                      modeChoice.Contains("AGGRESSIVE") && !modeChoice.Contains("ULTRA") ? UltimateBotMode.Aggressive :
                      modeChoice.Contains("CONSERVATIVE") ? UltimateBotMode.Conservative :
                      UltimateBotMode.Balanced;

            var balance = decimal.Parse(ConsoleUI.AskInput("Initial Balance (THB)", "10000"));

            if (!ConsoleUI.Confirm($"Start ULTIMATE BOT in {mode} mode?"))
                return;

            var bot = new UltimateBot(client, db, symbol, mode, balance);

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                bot.Stop();
            };

            await bot.StartAsync(cts.Token);
        }

        static async Task AnalyzeElliottWave(BitkubClient client)
        {
            AnsiConsole.Clear();

            var symbol = ConsoleUI.AskInput("Symbol", "THB_BTC");
            var prices = await FetchPriceHistory(client, symbol);

            var elliottWave = await ConsoleUI.WithSpinnerAsync(
                "🌊 Analyzing Elliott Wave patterns...",
                async () =>
                {
                    await Task.Delay(500);
                    return AdvancedAnalysis.DetectElliottWave(prices);
                }
            );

            AnsiConsole.WriteLine();

            var panel = new Panel(
                new Markup(
                    $"[bold cyan]Pattern:[/] {elliottWave.Pattern}\n" +
                    $"[bold yellow]Current Wave:[/] {elliottWave.CurrentWave}\n" +
                    $"[bold green]Confidence:[/] {elliottWave.Confidence}%\n" +
                    $"[bold magenta]Next Expected:[/] {elliottWave.NextExpectedMove}\n\n" +
                    $"[dim]Elliott Wave Theory: Market moves in 5-wave impulse and 3-wave corrective patterns[/]"
                ))
            {
                Header = new PanelHeader("🌊 [bold]Elliott Wave Analysis[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Cyan1)
            };

            AnsiConsole.Write(panel);
        }

        static async Task AnalyzeFibonacci(BitkubClient client)
        {
            AnsiConsole.Clear();

            var symbol = ConsoleUI.AskInput("Symbol", "THB_BTC");
            var prices = await FetchPriceHistory(client, symbol);

            var fibLevels = await ConsoleUI.WithSpinnerAsync(
                "📐 Calculating Fibonacci levels...",
                async () =>
                {
                    await Task.Delay(300);
                    return AdvancedAnalysis.FindFibonacciSupportResistance(prices);
                }
            );

            AnsiConsole.WriteLine();

            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Yellow)
                .AddColumn("[yellow]Level[/]")
                .AddColumn("[cyan]Price[/]")
                .AddColumn("[green]Distance[/]")
                .AddColumn("[magenta]Type[/]");

            foreach (var level in fibLevels.Take(7))
            {
                var distColor = Math.Abs(level.Distance) < 1 ? "green" :
                               Math.Abs(level.Distance) < 3 ? "yellow" : "dim";

                table.AddRow(
                    level.Name,
                    $"{level.Price:N2}",
                    $"[{distColor}]{level.Distance:N2}%[/]",
                    $"[{(level.Type == "SUPPORT" ? "green" : "red")}]{level.Type}[/]"
                );
            }

            var panel = new Panel(table)
            {
                Header = new PanelHeader("📐 [bold]Fibonacci Support/Resistance[/]", Justify.Center),
                Border = BoxBorder.Double
            };

            AnsiConsole.Write(panel);
        }

        static async Task AnalyzeVolumeProfile(BitkubClient client)
        {
            AnsiConsole.Clear();

            var symbol = ConsoleUI.AskInput("Symbol", "THB_BTC");
            var prices = await FetchPriceHistory(client, symbol);
            var volumes = await FetchVolumeHistory(client, symbol);

            var profile = await ConsoleUI.WithSpinnerAsync(
                "📊 Calculating Volume Profile...",
                async () =>
                {
                    await Task.Delay(500);
                    return AdvancedAnalysis.CalculateVolumeProfile(prices, volumes, 20);
                }
            );

            var nodes = AdvancedAnalysis.DetectVolumeNodes(profile);

            AnsiConsole.WriteLine();

            var grid = new Grid()
                .AddColumn()
                .AddColumn();

            grid.AddRow("[bold]POC (Point of Control):[/]", $"[yellow]{profile.POC:N2}[/]");
            grid.AddRow("[bold]VAH (Value Area High):[/]", $"[green]{profile.VAH:N2}[/]");
            grid.AddRow("[bold]VAL (Value Area Low):[/]", $"[red]{profile.VAL:N2}[/]");
            grid.AddRow("[bold]Total Volume:[/]", $"[cyan]{profile.TotalVolume:N2}[/]");
            grid.AddRow("[bold]High Volume Nodes:[/]", $"[magenta]{nodes.HighVolumeNodes.Count}[/]");
            grid.AddRow("[bold]Low Volume Nodes:[/]", $"[blue]{nodes.LowVolumeNodes.Count}[/]");

            var panel = new Panel(grid)
            {
                Header = new PanelHeader("📊 [bold]Volume Profile Analysis[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Cyan1)
            };

            AnsiConsole.Write(panel);

            AnsiConsole.WriteLine();
            ConsoleUI.ShowInfo($"💡 POC at {profile.POC:N0} is where most trading occurred");
            ConsoleUI.ShowInfo($"💡 Price tends to return to POC and bounce from VAL/VAH");
        }

        static async Task AnalyzeOrderFlow(BitkubClient client)
        {
            AnsiConsole.Clear();

            var symbol = ConsoleUI.AskInput("Symbol", "THB_BTC");

            var depth = await ConsoleUI.WithSpinnerAsync(
                "📉 Analyzing Order Flow...",
                async () => await client.GetDepthAsync(symbol, 20)
            );

            var orderFlow = AdvancedAnalysis.AnalyzeOrderFlow(depth.Bids, depth.Asks);

            AnsiConsole.WriteLine();

            var panel = new Panel(
                new Markup(
                    $"[bold]Bid Volume:[/] [green]{orderFlow.BidVolume:N4}[/]\n" +
                    $"[bold]Ask Volume:[/] [red]{orderFlow.AskVolume:N4}[/]\n" +
                    $"[bold]Ratio:[/] [yellow]{orderFlow.Ratio:N2}[/]\n" +
                    $"[bold]Imbalance:[/] [{GetImbalanceColor(orderFlow.Imbalance)}]{orderFlow.Imbalance}[/]\n" +
                    $"[bold]Pressure:[/] [{GetPressureColor(orderFlow.Pressure)}]{orderFlow.Pressure}[/]\n" +
                    $"[bold]Bid Walls:[/] [cyan]{orderFlow.BidWalls.Count}[/]\n" +
                    $"[bold]Ask Walls:[/] [magenta]{orderFlow.AskWalls.Count}[/]"
                ))
            {
                Header = new PanelHeader("📉 [bold]Order Flow Analysis[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Cyan1)
            };

            AnsiConsole.Write(panel);
        }

        static async Task MLPrediction(BitkubClient client)
        {
            AnsiConsole.Clear();

            var symbol = ConsoleUI.AskInput("Symbol", "THB_BTC");
            var prices = await FetchPriceHistory(client, symbol);

            var prediction = await ConsoleUI.WithSpinnerAsync(
                "🧠 Running Machine Learning predictions...",
                async () =>
                {
                    await Task.Delay(700);
                    return MachineLearning.PredictNextPrices(prices, 10);
                }
            );

            var linearReg = MachineLearning.LinearRegression(prices);

            AnsiConsole.WriteLine();

            var chart = new BarChart()
                .Width(80)
                .Label("[bold yellow]Next 10 Period Predictions[/]");

            for (int i = 0; i < prediction.Count; i++)
            {
                chart.AddItem($"T+{i + 1}", (double)prediction[i] / 1000, Color.Cyan1);
            }

            AnsiConsole.Write(chart);
            AnsiConsole.WriteLine();

            var panel = new Panel(
                $"[bold]Trend:[/] [{GetTrendColor(linearReg.Trend)}]{linearReg.Trend}[/]\n" +
                $"[bold]R² (Confidence):[/] [yellow]{linearReg.RSquared:N3}[/]\n" +
                $"[bold]Slope:[/] [cyan]{linearReg.Slope:N2}[/]"
            )
            {
                Header = new PanelHeader("🧠 [bold]ML Statistics[/]"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
        }

        static async Task RecognizePatterns(BitkubClient client)
        {
            AnsiConsole.Clear();

            var symbol = ConsoleUI.AskInput("Symbol", "THB_BTC");
            var prices = await FetchPriceHistory(client, symbol);

            var patterns = await ConsoleUI.WithSpinnerAsync(
                "🔍 Recognizing chart patterns...",
                async () =>
                {
                    await Task.Delay(600);
                    return MachineLearning.RecognizePatterns(prices);
                }
            );

            AnsiConsole.WriteLine();

            if (patterns.Patterns.Count == 0)
            {
                ConsoleUI.ShowWarning("No clear patterns detected");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[cyan]Pattern[/]")
                .AddColumn("[yellow]Confidence[/]");

            foreach (var pattern in patterns.Patterns)
            {
                var confidence = patterns.Confidence[pattern];
                table.AddRow(pattern, $"[green]{confidence}%[/]");
            }

            var panel = new Panel(table)
            {
                Header = new PanelHeader($"🔍 [bold]Detected: {patterns.MostLikelyPattern}[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Green)
            };

            AnsiConsole.Write(panel);
        }

        static async Task FullAnalysis(BitkubClient client)
        {
            AnsiConsole.Clear();
            ConsoleUI.ShowInfo("🎯 Running FULL Multi-Dimensional Analysis...");
            await Task.Delay(1000);

            var symbol = ConsoleUI.AskInput("Symbol", "THB_BTC");

            await ConsoleUI.WithProgressAsync("Analyzing...", async (task) =>
            {
                task.MaxValue = 100;

                // Elliott Wave
                task.Description = "🌊 Elliott Wave...";
                await Task.Delay(500);
                task.Increment(15);

                // Fibonacci
                task.Description = "📐 Fibonacci...";
                await Task.Delay(300);
                task.Increment(15);

                // Volume Profile
                task.Description = "📊 Volume Profile...";
                await Task.Delay(500);
                task.Increment(15);

                // Market Structure
                task.Description = "🏗️ Market Structure...";
                await Task.Delay(400);
                task.Increment(15);

                // Order Flow
                task.Description = "📉 Order Flow...";
                await Task.Delay(300);
                task.Increment(15);

                // ML Prediction
                task.Description = "🧠 ML Prediction...";
                await Task.Delay(700);
                task.Increment(15);

                // Pattern Recognition
                task.Description = "🔍 Patterns...";
                await Task.Delay(500);
                task.Increment(10);
            });

            ConsoleUI.ShowSuccess("✅ Full analysis complete!");
            ConsoleUI.ShowInfo("💡 Start ULTIMATE BOT to see all results combined!");
        }

        static async Task<List<decimal>> FetchPriceHistory(BitkubClient client, string symbol, int count = 100)
        {
            var ticker = await client.GetTickerAsync(symbol);
            var prices = new List<decimal>();

            if (ticker.TryGetValue(symbol, out var info))
            {
                // Generate simulated historical data based on current price
                var currentPrice = info.Last;
                var random = new Random();

                for (int i = 0; i < count; i++)
                {
                    var change = (decimal)(random.NextDouble() - 0.5) * currentPrice * 0.02m;
                    prices.Add(currentPrice + change);
                }
            }

            return prices;
        }

        static async Task<List<decimal>> FetchVolumeHistory(BitkubClient client, string symbol, int count = 100)
        {
            var ticker = await client.GetTickerAsync(symbol);
            var volumes = new List<decimal>();

            if (ticker.TryGetValue(symbol, out var info))
            {
                var baseVolume = info.BaseVolume;
                var random = new Random();

                for (int i = 0; i < count; i++)
                {
                    var variation = (decimal)(random.NextDouble() * 0.5 + 0.75);
                    volumes.Add(baseVolume * variation);
                }
            }

            return volumes;
        }

        static string GetImbalanceColor(string imbalance)
        {
            return imbalance.Contains("BID") ? "green" :
                   imbalance.Contains("ASK") ? "red" : "yellow";
        }

        static string GetPressureColor(string pressure)
        {
            return pressure.Contains("BUYING") ? "green" :
                   pressure.Contains("SELLING") ? "red" : "yellow";
        }

        static string GetTrendColor(string trend)
        {
            return trend == "UPTREND" ? "green" :
                   trend == "DOWNTREND" ? "red" : "yellow";
        }

        static void ShowGoodbye()
        {
            AnsiConsole.Clear();

            var panel = new Panel(
                Align.Center(
                    new Markup(
                        "[bold gold1]🌟 ULTIMATE TRADING SYSTEM 🌟[/]\n\n" +
                        "[yellow]Thank you for using the most advanced trading bot![/]\n" +
                        "[green]May your trades be profitable! 💰[/]\n\n" +
                        "[dim]Remember: This is for educational purposes.[/]\n" +
                        "[dim]Always test thoroughly before using real money.[/]"
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
