using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace BitkubTrader
{
    class ProgramBeautiful
    {
        // Replace these with your actual API credentials
        private const string API_KEY = "YOUR_API_KEY";
        private const string API_SECRET = "YOUR_API_SECRET";

        static async Task Main(string[] args)
        {
            // แสดง Logo สวยงาม
            ConsoleUI.ShowLogo();

            await Task.Delay(1000); // Animation delay

            using var client = new BitkubClient(API_KEY, API_SECRET);

            while (true)
            {
                try
                {
                    var choice = ConsoleUI.ShowMenu(
                        "🚀 BITKUB TRADER - Select an option",
                        new[]
                        {
                            "1️⃣  📊 View Market Data",
                            "2️⃣  💰 Check Balance",
                            "3️⃣  📈 View Order Book",
                            "4️⃣  🎯 Place Test Order",
                            "5️⃣  📜 View Order History",
                            "6️⃣  ⚡ Live Trading Dashboard",
                            "7️⃣  🤖 Start Trading Bot",
                            "8️⃣  📊 All Market Tickers",
                            "9️⃣  ❌ Exit"
                        }
                    );

                    if (choice.Contains("Exit"))
                        break;

                    if (choice.Contains("Market Data"))
                        await ShowMarketData(client);
                    else if (choice.Contains("Balance"))
                        await ShowBalance(client);
                    else if (choice.Contains("Order Book"))
                        await ShowOrderBook(client);
                    else if (choice.Contains("Test Order"))
                        await PlaceTestOrder(client);
                    else if (choice.Contains("Order History"))
                        await ShowOrderHistory(client);
                    else if (choice.Contains("Live Trading Dashboard"))
                        await ShowLiveDashboard(client);
                    else if (choice.Contains("Trading Bot"))
                        await StartTradingBot(client);
                    else if (choice.Contains("All Market"))
                        await ShowAllTickers(client);

                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
                    Console.ReadKey(true);
                    AnsiConsole.Clear();
                    ConsoleUI.ShowLogo();
                }
                catch (Exception ex)
                {
                    ConsoleUI.ShowError($"Error: {ex.Message}");
                    await Task.Delay(2000);
                }
            }

            // Goodbye animation
            AnsiConsole.Clear();
            var goodbyePanel = new Panel(
                Align.Center(
                    new Markup("[bold yellow]Thank you for using Bitkub Trader! 🚀[/]\n[dim]Happy Trading! 💰[/]"),
                    VerticalAlignment.Middle))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Yellow)
            };
            AnsiConsole.Write(goodbyePanel);
        }

        static async Task ShowMarketData(BitkubClient client)
        {
            AnsiConsole.Clear();

            var symbol = ConsoleUI.AskInput("Enter symbol (e.g., THB_BTC)", "THB_BTC");

            var ticker = await ConsoleUI.WithSpinnerAsync(
                $"📡 Fetching market data for {symbol}...",
                async () => await client.GetTickerAsync(symbol)
            );

            if (ticker.TryGetValue(symbol, out var info))
            {
                AnsiConsole.WriteLine();

                // Create a fancy price display
                var priceGrid = new Grid()
                    .AddColumn()
                    .AddColumn()
                    .AddRow(
                        new Panel(
                            Align.Center(
                                new Markup($"[bold yellow]{info.Last:N2}[/]\n[dim]THB[/]"),
                                VerticalAlignment.Middle))
                        {
                            Header = new PanelHeader("💰 [bold]Current Price[/]"),
                            Border = BoxBorder.Rounded,
                            BorderStyle = new Style(Color.Yellow)
                        },
                        new Panel(
                            Align.Center(
                                new Markup($"[bold {(info.PercentChange >= 0 ? "green" : "red")}]{(info.PercentChange >= 0 ? "▲" : "▼")} {info.PercentChange:N2}%[/]"),
                                VerticalAlignment.Middle))
                        {
                            Header = new PanelHeader("📊 [bold]24h Change[/]"),
                            Border = BoxBorder.Rounded,
                            BorderStyle = new Style(info.PercentChange >= 0 ? Color.Green : Color.Red)
                        }
                    );

                AnsiConsole.Write(priceGrid);
                AnsiConsole.WriteLine();

                // Statistics table
                var statsTable = new Table()
                    .Border(TableBorder.Rounded)
                    .BorderColor(Color.Cyan1)
                    .AddColumn(new TableColumn("[cyan bold]Metric[/]"))
                    .AddColumn(new TableColumn("[yellow bold]Value[/]").RightAligned());

                statsTable.AddRow("[bold]24h High[/]", $"[green]{info.High24Hr:N2} THB[/]");
                statsTable.AddRow("[bold]24h Low[/]", $"[red]{info.Low24Hr:N2} THB[/]");
                statsTable.AddRow("[bold]Highest Bid[/]", $"[green]{info.HighestBid:N2} THB[/]");
                statsTable.AddRow("[bold]Lowest Ask[/]", $"[red]{info.LowestAsk:N2} THB[/]");
                statsTable.AddRow("[bold]Base Volume[/]", $"[cyan]{info.BaseVolume:N8}[/]");
                statsTable.AddRow("[bold]Quote Volume[/]", $"[magenta]{info.QuoteVolume:N2} THB[/]");

                var statsPanel = new Panel(statsTable)
                {
                    Header = new PanelHeader($"📈 [bold]{symbol} Statistics[/]"),
                    Border = BoxBorder.Double,
                    BorderStyle = new Style(Color.Cyan1)
                };

                AnsiConsole.Write(statsPanel);

                // Create a simple bar chart for high/low
                var chart = new BarChart()
                    .Width(60)
                    .Label($"[bold yellow]{symbol} 24h Range[/]")
                    .CenterLabel()
                    .AddItem("High", (double)info.High24Hr / 1000, Color.Green)
                    .AddItem("Current", (double)info.Last / 1000, Color.Yellow)
                    .AddItem("Low", (double)info.Low24Hr / 1000, Color.Red);

                AnsiConsole.WriteLine();
                AnsiConsole.Write(chart);
            }
            else
            {
                ConsoleUI.ShowError($"Symbol {symbol} not found!");
            }
        }

        static async Task ShowBalance(BitkubClient client)
        {
            AnsiConsole.Clear();

            var balances = await ConsoleUI.WithSpinnerAsync(
                "💰 Loading your portfolio...",
                async () => await client.GetBalancesAsync()
            );

            if (balances.Error == 0)
            {
                AnsiConsole.WriteLine();

                // Get prices for calculation
                var prices = new Dictionary<string, decimal>();
                var tickers = await client.GetTickerAsync();

                foreach (var ticker in tickers)
                {
                    var parts = ticker.Key.Split('_');
                    if (parts.Length == 2 && parts[0] == "THB")
                    {
                        prices[parts[1]] = ticker.Value.Last;
                    }
                }
                prices["THB"] = 1;

                ConsoleUI.ShowBalances(balances.Result, prices);

                // Create a pie chart of portfolio distribution
                decimal totalValue = 0;
                var distribution = new Dictionary<string, decimal>();

                foreach (var balance in balances.Result)
                {
                    if (balance.Value.Available > 0 || balance.Value.Reserved > 0)
                    {
                        decimal total = balance.Value.Available + balance.Value.Reserved;
                        decimal price = prices.ContainsKey(balance.Key) ? prices[balance.Key] : 1;
                        decimal value = total * price;
                        totalValue += value;
                        distribution[balance.Key] = value;
                    }
                }

                if (totalValue > 0)
                {
                    AnsiConsole.WriteLine();
                    var breakdownChart = new BreakdownChart()
                        .Width(80)
                        .ShowPercentage();

                    foreach (var item in distribution.OrderByDescending(x => x.Value))
                    {
                        breakdownChart.AddItem(
                            $"{item.Key} ({item.Value:N2} THB)",
                            (double)item.Value,
                            Color.FromInt32(new Random().Next(1, 255)));
                    }

                    var chartPanel = new Panel(breakdownChart)
                    {
                        Header = new PanelHeader("📊 [bold]Portfolio Distribution[/]"),
                        Border = BoxBorder.Rounded,
                        BorderStyle = new Style(Color.Cyan1)
                    };

                    AnsiConsole.Write(chartPanel);
                }
            }
            else
            {
                ConsoleUI.ShowError($"Failed to load balance. Error code: {balances.Error}");
            }
        }

        static async Task ShowOrderBook(BitkubClient client)
        {
            AnsiConsole.Clear();

            var symbol = ConsoleUI.AskInput("Enter symbol (e.g., THB_BTC)", "THB_BTC");
            var limitStr = ConsoleUI.AskInput("Number of orders to show", "10");
            var limit = int.Parse(limitStr);

            var depth = await ConsoleUI.WithSpinnerAsync(
                $"📚 Loading order book for {symbol}...",
                async () => await client.GetDepthAsync(symbol, limit)
            );

            AnsiConsole.WriteLine();
            ConsoleUI.ShowOrderBook(depth.Bids, depth.Asks, symbol);

            // Show spread info
            if (depth.Bids.Count > 0 && depth.Asks.Count > 0)
            {
                var highestBid = depth.Bids[0][0];
                var lowestAsk = depth.Asks[0][0];
                var spread = lowestAsk - highestBid;
                var spreadPercent = (spread / lowestAsk) * 100;

                AnsiConsole.WriteLine();
                var spreadPanel = new Panel(
                    $"[bold]Highest Bid:[/] [green]{highestBid:N2}[/] THB\n" +
                    $"[bold]Lowest Ask:[/] [red]{lowestAsk:N2}[/] THB\n" +
                    $"[bold]Spread:[/] [yellow]{spread:N2}[/] THB ([cyan]{spreadPercent:N4}%[/])")
                {
                    Header = new PanelHeader("📐 [bold]Spread Analysis[/]"),
                    Border = BoxBorder.Rounded,
                    BorderStyle = new Style(Color.Yellow)
                };

                AnsiConsole.Write(spreadPanel);
            }
        }

        static async Task PlaceTestOrder(BitkubClient client)
        {
            AnsiConsole.Clear();

            var orderType = ConsoleUI.ShowMenu(
                "Select order type",
                new[] { "📈 BUY (Bid)", "📉 SELL (Ask)", "← Back" }
            );

            if (orderType.Contains("Back"))
                return;

            var symbol = ConsoleUI.AskInput("Symbol (e.g., THB_BTC)", "THB_BTC");
            var amount = decimal.Parse(ConsoleUI.AskInput("Amount", "1000"));
            var rate = decimal.Parse(ConsoleUI.AskInput("Rate (0 for market)", "0"));
            var type = rate == 0 ? "market" : "limit";

            var confirmMessage = orderType.Contains("BUY")
                ? $"Test BUY {amount:N2} THB worth at {(type == "market" ? "market price" : rate.ToString("N2") + " THB")}"
                : $"Test SELL {amount:N8} at {(type == "market" ? "market price" : rate.ToString("N2") + " THB")}";

            if (!ConsoleUI.Confirm($"{confirmMessage}?"))
                return;

            AnsiConsole.WriteLine();

            if (orderType.Contains("BUY"))
            {
                var result = await ConsoleUI.WithSpinnerAsync(
                    "🎯 Placing test BUY order...",
                    async () => await client.PlaceBidTestAsync(symbol, amount, rate, type)
                );

                if (result.Error == 0 && result.Result != null)
                {
                    AnsiConsole.WriteLine();
                    ConsoleUI.ShowOrderResult(result.Result, "buy");
                    ConsoleUI.ShowSuccess("Test order successful! (No real transaction)");
                }
                else
                {
                    ConsoleUI.ShowError($"Order failed with error code: {result.Error}");
                }
            }
            else
            {
                var result = await ConsoleUI.WithSpinnerAsync(
                    "🎯 Placing test SELL order...",
                    async () => await client.PlaceAskTestAsync(symbol, amount, rate, type)
                );

                if (result.Error == 0 && result.Result != null)
                {
                    AnsiConsole.WriteLine();
                    ConsoleUI.ShowOrderResult(result.Result, "sell");
                    ConsoleUI.ShowSuccess("Test order successful! (No real transaction)");
                }
                else
                {
                    ConsoleUI.ShowError($"Order failed with error code: {result.Error}");
                }
            }
        }

        static async Task ShowOrderHistory(BitkubClient client)
        {
            AnsiConsole.Clear();

            var symbol = ConsoleUI.AskInput("Symbol (e.g., THB_BTC)", "THB_BTC");
            var limitStr = ConsoleUI.AskInput("Number of records", "20");
            var limit = int.Parse(limitStr);

            var history = await ConsoleUI.WithSpinnerAsync(
                "📜 Loading order history...",
                async () => await client.GetOrderHistoryAsync(symbol, limit: limit)
            );

            if (history.Error == 0)
            {
                AnsiConsole.WriteLine();

                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .BorderColor(Color.Cyan1)
                    .AddColumn(new TableColumn("[cyan]TXN ID[/]"))
                    .AddColumn(new TableColumn("[yellow]Side[/]"))
                    .AddColumn(new TableColumn("[green]Amount[/]").RightAligned())
                    .AddColumn(new TableColumn("[magenta]Rate[/]").RightAligned())
                    .AddColumn(new TableColumn("[red]Fee[/]").RightAligned())
                    .AddColumn(new TableColumn("[blue]Time[/]"));

                foreach (var order in history.Result)
                {
                    var sideColor = order.Side == "buy" ? "green" : "red";
                    var sideIcon = order.Side == "buy" ? "📈" : "📉";
                    var dt = DateTimeOffset.FromUnixTimeSeconds(order.Timestamp).DateTime;

                    table.AddRow(
                        $"[dim]{order.TxnId}[/]",
                        $"[{sideColor}]{sideIcon} {order.Side.ToUpper()}[/]",
                        $"{order.Amount:N8}",
                        $"{order.Rate:N2}",
                        $"{order.Fee:N2}",
                        $"[dim]{dt:yyyy-MM-dd HH:mm}[/]"
                    );
                }

                var panel = new Panel(table)
                {
                    Header = new PanelHeader($"📜 [bold]{symbol} Order History[/]"),
                    Border = BoxBorder.Double,
                    BorderStyle = new Style(Color.Cyan1)
                };

                AnsiConsole.Write(panel);

                // Summary
                var buyOrders = history.Result.Count(o => o.Side == "buy");
                var sellOrders = history.Result.Count(o => o.Side == "sell");
                var totalFees = history.Result.Sum(o => o.Fee);

                AnsiConsole.WriteLine();
                var summaryTable = new Table()
                    .Border(TableBorder.None)
                    .HideHeaders()
                    .AddColumn("")
                    .AddColumn("");

                summaryTable.AddRow("[bold]Total Orders:[/]", $"[cyan]{history.Result.Count}[/]");
                summaryTable.AddRow("[bold]Buy Orders:[/]", $"[green]{buyOrders}[/]");
                summaryTable.AddRow("[bold]Sell Orders:[/]", $"[red]{sellOrders}[/]");
                summaryTable.AddRow("[bold]Total Fees:[/]", $"[yellow]{totalFees:N2} THB[/]");

                AnsiConsole.Write(summaryTable);
            }
            else
            {
                ConsoleUI.ShowError($"Failed to load history. Error code: {history.Error}");
            }
        }

        static async Task ShowLiveDashboard(BitkubClient client)
        {
            AnsiConsole.Clear();

            var symbol = ConsoleUI.AskInput("Symbol to monitor (e.g., THB_BTC)", "THB_BTC");

            ConsoleUI.ShowInfo($"Starting live dashboard for {symbol}. Press Ctrl+C to stop.");
            await Task.Delay(1000);

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            await ConsoleUI.ShowLiveDashboard(client, symbol, cts.Token);
        }

        static async Task StartTradingBot(BitkubClient client)
        {
            AnsiConsole.Clear();

            var strategy = ConsoleUI.ShowMenu(
                "🤖 Select Trading Strategy",
                new[]
                {
                    "📊 Simple Market Making",
                    "💰 DCA (Dollar Cost Averaging)",
                    "⚡ Scalping",
                    "← Back"
                }
            );

            if (strategy.Contains("Back"))
                return;

            var symbol = ConsoleUI.AskInput("Symbol (e.g., THB_BTC)", "THB_BTC");

            if (strategy.Contains("Simple Market Making"))
            {
                var amount = decimal.Parse(ConsoleUI.AskInput("Order amount (THB)", "1000"));
                var profit = decimal.Parse(ConsoleUI.AskInput("Profit target (%)", "1.5"));
                var stopLoss = decimal.Parse(ConsoleUI.AskInput("Stop loss (%)", "2.0"));

                var bot = new TradingBot(client, symbol, amount, profit, stopLoss);

                ConsoleUI.ShowInfo("Starting trading bot... Press Ctrl+C to stop.");
                await Task.Delay(1000);

                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (s, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                await bot.StartAsync(cts.Token);
            }
            else if (strategy.Contains("DCA"))
            {
                var totalAmount = decimal.Parse(ConsoleUI.AskInput("Total amount to invest (THB)", "10000"));
                var intervals = int.Parse(ConsoleUI.AskInput("Number of intervals", "10"));
                var minutes = int.Parse(ConsoleUI.AskInput("Minutes between intervals", "60"));

                var bot = new TradingBot(client, symbol);

                ConsoleUI.ShowInfo($"Starting DCA strategy: {totalAmount} THB in {intervals} intervals...");

                var cts = new CancellationTokenSource();
                await bot.DCAStrategyAsync(totalAmount, intervals, TimeSpan.FromMinutes(minutes), cts.Token);
            }
        }

        static async Task ShowAllTickers(BitkubClient client)
        {
            AnsiConsole.Clear();

            var tickers = await ConsoleUI.WithSpinnerAsync(
                "📡 Loading all market tickers...",
                async () => await client.GetTickerAsync()
            );

            AnsiConsole.WriteLine();
            ConsoleUI.ShowTickerTable(tickers);
        }
    }
}
