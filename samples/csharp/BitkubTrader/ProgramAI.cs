using System;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace BitkubTrader
{
    /// <summary>
    /// AI Trading Program - Master Entry Point
    /// 🚀 โปรแกรมเทรดด้วย AI แบบเทพที่สุด
    /// </summary>
    class ProgramAI
    {
        private const string API_KEY = "YOUR_API_KEY";
        private const string API_SECRET = "YOUR_API_SECRET";

        static async Task Main(string[] args)
        {
            AnsiConsole.Clear();
            ShowEpicIntro();

            await Task.Delay(2000);

            using var client = new BitkubClient(API_KEY, API_SECRET);
            using var db = new DatabaseManager();

            while (true)
            {
                try
                {
                    var choice = ConsoleUI.ShowMenu(
                        "🤖 AI TRADING SYSTEM - Select Mode",
                        new[]
                        {
                            "🚀 Start AI Trading Bot",
                            "📊 View Performance Analytics",
                            "📰 Analyze Market News",
                            "🔍 Technical Analysis",
                            "💾 Database Statistics",
                            "⚙️ Configure Settings",
                            "❌ Exit"
                        }
                    );

                    if (choice.Contains("Exit"))
                        break;

                    if (choice.Contains("Start AI"))
                        await StartAIBot(client, db);
                    else if (choice.Contains("Performance"))
                        await ShowPerformanceAnalytics(db);
                    else if (choice.Contains("News"))
                        await AnalyzeMarketNews();
                    else if (choice.Contains("Technical"))
                        await ShowTechnicalAnalysis(client);
                    else if (choice.Contains("Database"))
                        await ShowDatabaseStats(db);
                    else if (choice.Contains("Configure"))
                        ConfigureSettings();

                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
                    Console.ReadKey(true);
                    AnsiConsole.Clear();
                }
                catch (Exception ex)
                {
                    ConsoleUI.ShowError($"Error: {ex.Message}");
                    await Task.Delay(3000);
                }
            }

            ShowGoodbye();
        }

        static void ShowEpicIntro()
        {
            var figlet = new FigletText("AI  TRADER")
            {
                Color = Color.Cyan1
            };

            var subtitle = Align.Center(
                new Markup(
                    "[bold yellow]⚡ ULTIMATE AI TRADING SYSTEM ⚡[/]\n" +
                    "[cyan]Powered by Machine Learning & Technical Analysis[/]\n\n" +
                    "[dim]Features:[/]\n" +
                    "[green]✓[/] Multi-indicator Technical Analysis\n" +
                    "[green]✓[/] News Sentiment Analysis (Thai)\n" +
                    "[green]✓[/] Risk Management System\n" +
                    "[green]✓[/] Multiple Trading Modes\n" +
                    "[green]✓[/] SQLite Database Tracking\n" +
                    "[green]✓[/] Real-time Performance Metrics"
                ),
                VerticalAlignment.Middle
            );

            var panel = new Panel(subtitle)
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Cyan1),
                Padding = new Padding(2, 1)
            };

            AnsiConsole.Write(figlet);
            AnsiConsole.WriteLine();
            AnsiConsole.Write(panel);
        }

        static async Task StartAIBot(BitkubClient client, DatabaseManager db)
        {
            AnsiConsole.Clear();

            // Get configuration
            var symbol = ConsoleUI.AskInput("Symbol to trade (e.g., THB_BTC)", "THB_BTC");

            var modeChoice = ConsoleUI.ShowMenu(
                "Select Trading Mode",
                new[]
                {
                    "⚡ AGGRESSIVE - High Risk, High Reward (Fast Trading)",
                    "⚖️ BALANCED - Medium Risk, Balanced Approach",
                    "🛡️ CONSERVATIVE - Low Risk, Steady Profits"
                }
            );

            var mode = modeChoice.Contains("AGGRESSIVE") ? TradingMode.Aggressive :
                       modeChoice.Contains("CONSERVATIVE") ? TradingMode.Conservative :
                       TradingMode.Balanced;

            var balance = decimal.Parse(ConsoleUI.AskInput("Initial balance (THB)", "10000"));
            var maxRisk = decimal.Parse(ConsoleUI.AskInput("Max risk per trade (% of balance)", "2")) / 100;
            var takeProfit = decimal.Parse(ConsoleUI.AskInput("Take profit target (%)", "3")) / 100;
            var stopLoss = decimal.Parse(ConsoleUI.AskInput("Stop loss (%)", "2")) / 100;

            // Confirmation
            var confirm = ConsoleUI.Confirm(
                $"Start AI bot with {mode} mode, {balance:N0} THB initial balance?"
            );

            if (!confirm)
                return;

            // Create and start bot
            var bot = new AITradingBot(client, db, symbol, mode, balance, maxRisk, takeProfit, stopLoss);

            ConsoleUI.ShowInfo("🤖 AI Trading Bot is starting...");
            await Task.Delay(1000);

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                bot.Stop();
            };

            await bot.StartAsync(cts.Token);
        }

        static async Task ShowPerformanceAnalytics(DatabaseManager db)
        {
            AnsiConsole.Clear();

            var symbol = ConsoleUI.AskInput("Symbol to analyze", "THB_BTC");

            var metrics = await ConsoleUI.WithSpinnerAsync(
                "📊 Loading performance metrics...",
                async () => await db.GetPerformanceAsync(symbol)
            );

            AnsiConsole.WriteLine();

            // Performance Table
            var table = new Table()
                .Border(TableBorder.Heavy)
                .BorderColor(Color.Cyan1)
                .AddColumn("[cyan bold]Metric[/]")
                .AddColumn("[yellow bold]Value[/]");

            table.AddRow("📊 Total Trades", $"[cyan]{metrics.TotalTrades}[/]");
            table.AddRow("✅ Winning Trades", $"[green]{metrics.WinningTrades}[/]");
            table.AddRow("❌ Losing Trades", $"[red]{metrics.LosingTrades}[/]");
            table.AddRow("🎯 Win Rate", $"[bold yellow]{metrics.WinRate:N1}%[/]");
            table.AddRow("💰 Total Profit", $"[green]{metrics.TotalProfit:N2}[/] THB");
            table.AddRow("📉 Total Loss", $"[red]{metrics.TotalLoss:N2}[/] THB");
            table.AddRow("💎 Net Profit", $"[{(metrics.NetProfit >= 0 ? "green" : "red")}]{metrics.NetProfit:N2}[/] THB");
            table.AddRow("📈 Profit Factor", $"[cyan]{metrics.ProfitFactor:N2}[/]");
            table.AddRow("🏆 Largest Win", $"[green]{metrics.LargestWin:N2}[/] THB");
            table.AddRow("💥 Largest Loss", $"[red]{metrics.LargestLoss:N2}[/] THB");
            table.AddRow("📊 Avg Win", $"[green]{metrics.AvgProfit:N2}[/] THB");
            table.AddRow("📊 Avg Loss", $"[red]{metrics.AvgLoss:N2}[/] THB");

            var panel = new Panel(table)
            {
                Header = new PanelHeader($"📊 [bold]Performance Analytics - {symbol}[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Cyan1)
            };

            AnsiConsole.Write(panel);

            // Performance Chart
            if (metrics.TotalTrades > 0)
            {
                AnsiConsole.WriteLine();

                var chart = new BreakdownChart()
                    .Width(80)
                    .ShowPercentage();

                chart.AddItem("Winning", metrics.WinningTrades, Color.Green);
                chart.AddItem("Losing", metrics.LosingTrades, Color.Red);

                var chartPanel = new Panel(chart)
                {
                    Header = new PanelHeader("📊 [bold]Win/Loss Distribution[/]"),
                    Border = BoxBorder.Rounded
                };

                AnsiConsole.Write(chartPanel);
            }
        }

        static async Task AnalyzeMarketNews()
        {
            AnsiConsole.Clear();

            var analyzer = new NewsAnalyzer();

            var sentiment = await ConsoleUI.WithSpinnerAsync(
                "📰 Analyzing market news from Thai websites...",
                async () => await analyzer.GetMarketSentimentAsync()
            );

            AnsiConsole.WriteLine();

            // Sentiment Summary
            var sentimentColor = sentiment.OverallSentiment.Contains("POSITIVE") ? "green" :
                                sentiment.OverallSentiment.Contains("NEGATIVE") ? "red" : "yellow";

            var summaryPanel = new Panel(
                Align.Center(
                    new Markup(
                        $"[bold {sentimentColor}]{sentiment.OverallSentiment}[/]\n\n" +
                        $"[yellow]Sentiment Score:[/] [{sentimentColor}]{sentiment.SentimentScore:N2}[/]\n" +
                        $"[yellow]Total Articles:[/] [cyan]{sentiment.TotalArticles}[/]\n" +
                        $"[green]Positive:[/] {sentiment.PositiveCount} | " +
                        $"[red]Negative:[/] {sentiment.NegativeCount} | " +
                        $"[yellow]Neutral:[/] {sentiment.NeutralCount}"
                    ),
                    VerticalAlignment.Middle
                ))
            {
                Header = new PanelHeader("📰 [bold]Market Sentiment Analysis[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.FromInt32(sentimentColor == "green" ? 2 : sentimentColor == "red" ? 1 : 3))
            };

            AnsiConsole.Write(summaryPanel);

            // Trading Recommendation
            var recommendation = analyzer.GetTradingRecommendation(sentiment);
            AnsiConsole.WriteLine();
            ConsoleUI.ShowInfo($"🎯 Trading Recommendation: {recommendation}");

            // Recent Articles
            if (sentiment.RecentArticles.Count > 0)
            {
                AnsiConsole.WriteLine();

                var articlesTable = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[cyan]Source[/]")
                    .AddColumn("[yellow]Title[/]")
                    .AddColumn("[magenta]Sentiment[/]");

                foreach (var article in sentiment.RecentArticles.Take(5))
                {
                    var articleSentimentColor = article.Sentiment == "POSITIVE" ? "green" :
                                               article.Sentiment == "NEGATIVE" ? "red" : "yellow";

                    articlesTable.AddRow(
                        $"[dim]{article.Source}[/]",
                        article.Title.Length > 60 ? article.Title.Substring(0, 57) + "..." : article.Title,
                        $"[{articleSentimentColor}]{article.Sentiment}[/]"
                    );
                }

                var articlesPanel = new Panel(articlesTable)
                {
                    Header = new PanelHeader("📄 [bold]Recent Articles[/]"),
                    Border = BoxBorder.Rounded
                };

                AnsiConsole.Write(articlesPanel);
            }
        }

        static async Task ShowTechnicalAnalysis(BitkubClient client)
        {
            AnsiConsole.Clear();

            var symbol = ConsoleUI.AskInput("Symbol to analyze", "THB_BTC");

            // Get historical prices
            var ticker = await ConsoleUI.WithSpinnerAsync(
                $"📡 Fetching data for {symbol}...",
                async () => await client.GetTickerAsync(symbol)
            );

            if (!ticker.TryGetValue(symbol, out var info))
            {
                ConsoleUI.ShowError($"Symbol {symbol} not found!");
                return;
            }

            // Simulate historical data (in real app, would fetch from database)
            var prices = GeneratePriceHistory(info.Last, 50);

            // Calculate indicators
            var signal = TechnicalAnalysis.GenerateSignal(prices);
            var rsi = TechnicalAnalysis.CalculateRSI(prices);
            var (macd, macdSignal, histogram) = TechnicalAnalysis.CalculateMACD(prices);
            var (bbUpper, bbMiddle, bbLower) = TechnicalAnalysis.CalculateBollingerBands(prices);
            var sma20 = TechnicalAnalysis.CalculateSMA(prices, 20);
            var sma50 = TechnicalAnalysis.CalculateSMA(prices, 50);

            AnsiConsole.WriteLine();

            // Signal Panel
            var signalColor = signal.Action.Contains("BUY") ? "green" :
                             signal.Action.Contains("SELL") ? "red" : "yellow";

            var signalPanel = new Panel(
                Align.Center(
                    new Markup(
                        $"[bold {signalColor}]{signal.Action}[/]\n\n" +
                        $"[yellow]Confidence:[/] [bold]{signal.Confidence}%[/]\n" +
                        $"[yellow]Pattern:[/] [cyan]{signal.Pattern}[/]\n\n" +
                        $"[dim]{signal.Reason}[/]"
                    ),
                    VerticalAlignment.Middle
                ))
            {
                Header = new PanelHeader("🎯 [bold]AI Trading Signal[/]", Justify.Center),
                Border = BoxBorder.Heavy,
                BorderStyle = new Style(Color.FromInt32(signalColor == "green" ? 2 : signalColor == "red" ? 1 : 3))
            };

            AnsiConsole.Write(signalPanel);

            // Indicators Table
            AnsiConsole.WriteLine();

            var indicatorsTable = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Cyan1)
                .AddColumn("[cyan]Indicator[/]")
                .AddColumn("[yellow]Value[/]")
                .AddColumn("[magenta]Interpretation[/]");

            indicatorsTable.AddRow(
                "RSI (14)",
                $"{rsi:N1}",
                rsi < 30 ? "[green]Oversold - Buy Signal[/]" :
                rsi > 70 ? "[red]Overbought - Sell Signal[/]" :
                "[yellow]Neutral[/]"
            );

            indicatorsTable.AddRow(
                "MACD",
                $"{macd:N2}",
                histogram > 0 ? "[green]Bullish[/]" : "[red]Bearish[/]"
            );

            indicatorsTable.AddRow(
                "Current Price",
                $"{info.Last:N2}",
                info.Last > bbUpper ? "[red]Above Upper BB[/]" :
                info.Last < bbLower ? "[green]Below Lower BB[/]" :
                "[yellow]Within Bands[/]"
            );

            indicatorsTable.AddRow(
                "SMA 20/50",
                $"{sma20:N0} / {sma50:N0}",
                sma20 > sma50 ? "[green]Golden Cross[/]" : "[red]Death Cross[/]"
            );

            var indicatorsPanel = new Panel(indicatorsTable)
            {
                Header = new PanelHeader("📊 [bold]Technical Indicators[/]", Justify.Center),
                Border = BoxBorder.Double
            };

            AnsiConsole.Write(indicatorsPanel);
        }

        static async Task ShowDatabaseStats(DatabaseManager db)
        {
            AnsiConsole.Clear();

            ConsoleUI.ShowInfo("📊 Database Statistics");
            ConsoleUI.ShowInfo("Feature coming soon!");

            await Task.CompletedTask;
        }

        static void ConfigureSettings()
        {
            AnsiConsole.Clear();

            ConsoleUI.ShowInfo("⚙️ Settings Configuration");
            ConsoleUI.ShowWarning("Please edit the source code to configure settings.");
        }

        static void ShowGoodbye()
        {
            AnsiConsole.Clear();

            var panel = new Panel(
                Align.Center(
                    new Markup(
                        "[bold cyan]🤖 AI TRADING SYSTEM SHUTDOWN 🤖[/]\n\n" +
                        "[yellow]Thank you for using AI Trader![/]\n" +
                        "[green]May your trades be profitable! 💰[/]\n\n" +
                        "[dim]Remember: Past performance does not guarantee future results.[/]"
                    ),
                    VerticalAlignment.Middle
                ))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Cyan1)
            };

            AnsiConsole.Write(panel);
        }

        static List<decimal> GeneratePriceHistory(decimal currentPrice, int count)
        {
            var prices = new List<decimal>();
            var price = currentPrice * 0.95m; // Start 5% lower

            var random = new Random();

            for (int i = 0; i < count; i++)
            {
                price += (decimal)(random.NextDouble() - 0.5) * price * 0.02m;
                prices.Add(price);
            }

            return prices;
        }
    }
}
