using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace BitkubTrader
{
    /// <summary>
    /// Advanced Live Trading Dashboard with Real-time Updates
    /// </summary>
    public class LiveDashboard
    {
        private readonly BitkubClient _client;
        private readonly string _symbol;
        private List<decimal> _priceHistory = new();
        private DateTime _startTime = DateTime.Now;

        public LiveDashboard(BitkubClient client, string symbol)
        {
            _client = client;
            _symbol = symbol;
        }

        /// <summary>
        /// Run the live dashboard
        /// </summary>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            AnsiConsole.Clear();

            await AnsiConsole.Live(CreateLayout(null, null, null))
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .Cropping(VerticalOverflowCropping.Top)
                .StartAsync(async ctx =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            // Fetch data in parallel
                            var tickerTask = _client.GetTickerAsync(_symbol);
                            var depthTask = _client.GetDepthAsync(_symbol, 10);
                            var balancesTask = _client.GetBalancesAsync();
                            var openOrdersTask = _client.GetOpenOrdersAsync(_symbol);

                            await Task.WhenAll(tickerTask, depthTask, balancesTask, openOrdersTask);

                            var tickers = await tickerTask;
                            var depth = await depthTask;
                            var balances = await balancesTask;
                            var openOrders = await openOrdersTask;

                            TickerInfo? ticker = tickers.ContainsKey(_symbol) ? tickers[_symbol] : null;

                            if (ticker != null)
                            {
                                _priceHistory.Add(ticker.Last);
                                if (_priceHistory.Count > 50)
                                    _priceHistory.RemoveAt(0);
                            }

                            var layout = CreateLayout(
                                ticker,
                                depth,
                                balances.Error == 0 ? balances.Result : null,
                                openOrders.Error == 0 ? openOrders.Result : null
                            );

                            ctx.UpdateTarget(layout);
                            await Task.Delay(2000, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            ctx.UpdateTarget(new Panel($"[red]Error: {ex.Message}[/]"));
                            await Task.Delay(5000, cancellationToken);
                        }
                    }
                });
        }

        private Layout CreateLayout(
            TickerInfo? ticker,
            DepthResponse? depth,
            Dictionary<string, BalanceInfo>? balances,
            List<OpenOrder>? openOrders)
        {
            var layout = new Layout("Root")
                .SplitRows(
                    new Layout("Header").Size(3),
                    new Layout("Body"),
                    new Layout("Footer").Size(3)
                );

            // Header
            layout["Header"].Update(CreateHeader());

            // Body - Split into left and right
            layout["Body"].SplitColumns(
                new Layout("Left"),
                new Layout("Right")
            );

            // Left side - Price and Chart
            layout["Body"]["Left"].SplitRows(
                new Layout("Price").Size(12),
                new Layout("Chart").Size(15),
                new Layout("OrderBook")
            );

            layout["Body"]["Left"]["Price"].Update(CreatePricePanel(ticker));
            layout["Body"]["Left"]["Chart"].Update(CreatePriceChart());
            layout["Body"]["Left"]["OrderBook"].Update(CreateOrderBookPanel(depth));

            // Right side - Balance and Orders
            layout["Body"]["Right"].SplitRows(
                new Layout("Balance"),
                new Layout("Orders")
            );

            layout["Body"]["Right"]["Balance"].Update(CreateBalancePanel(balances, ticker?.Last ?? 0));
            layout["Body"]["Right"]["Orders"].Update(CreateOpenOrdersPanel(openOrders));

            // Footer
            layout["Footer"].Update(CreateFooter());

            return layout;
        }

        private Panel CreateHeader()
        {
            var uptime = DateTime.Now - _startTime;
            var header = new Markup(
                $"[bold cyan]⚡ BITKUB LIVE TRADING DASHBOARD ⚡[/]  " +
                $"[dim]│[/]  " +
                $"[yellow]{_symbol}[/]  " +
                $"[dim]│[/]  " +
                $"[green]Uptime: {uptime:hh\\:mm\\:ss}[/]  " +
                $"[dim]│[/]  " +
                $"[cyan]{DateTime.Now:yyyy-MM-dd HH:mm:ss}[/]"
            );

            return new Panel(Align.Center(header, VerticalAlignment.Middle))
            {
                Border = BoxBorder.Heavy,
                BorderStyle = new Style(Color.Cyan1)
            };
        }

        private Panel CreatePricePanel(TickerInfo? ticker)
        {
            if (ticker == null)
            {
                return new Panel("[dim]Loading...[/]");
            }

            var changeColor = ticker.PercentChange >= 0 ? "green" : "red";
            var changeIcon = ticker.PercentChange >= 0 ? "▲" : "▼";
            var trend = ticker.PercentChange >= 0 ? "🚀" : "📉";

            var grid = new Grid()
                .AddColumn(new GridColumn().Width(30))
                .AddColumn(new GridColumn().Width(30));

            // Big price display
            var priceMarkup = new Panel(
                Align.Center(
                    new Markup($"[bold yellow]{ticker.Last:N2}[/]\n[dim]THB[/]"),
                    VerticalAlignment.Middle))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Yellow)
            };

            // 24h change
            var changeMarkup = new Panel(
                Align.Center(
                    new Markup($"[bold {changeColor}]{changeIcon} {ticker.PercentChange:N2}%[/]\n[dim]24h Change[/]"),
                    VerticalAlignment.Middle))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(changeColor == "green" ? Color.Green : Color.Red)
            };

            grid.AddRow(priceMarkup, changeMarkup);

            // Stats row
            var statsGrid = new Grid()
                .AddColumn()
                .AddColumn()
                .AddColumn();

            statsGrid.AddRow(
                $"[bold]High:[/] [green]{ticker.High24Hr:N2}[/]",
                $"[bold]Low:[/] [red]{ticker.Low24Hr:N2}[/]",
                $"[bold]Volume:[/] [cyan]{ticker.BaseVolume:N4}[/]"
            );

            statsGrid.AddRow(
                $"[bold]Bid:[/] [green]{ticker.HighestBid:N2}[/]",
                $"[bold]Ask:[/] [red]{ticker.LowestAsk:N2}[/]",
                $"[bold]Quote:[/] [yellow]{ticker.QuoteVolume:N0}[/]"
            );

            var content = new Rows(grid, new Text(""), statsGrid);

            return new Panel(content)
            {
                Header = new PanelHeader($"{trend} [bold cyan]PRICE INFO[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Cyan1)
            };
        }

        private Panel CreatePriceChart()
        {
            if (_priceHistory.Count < 2)
            {
                return new Panel("[dim]Collecting price data...[/]");
            }

            var chart = new BarChart()
                .Width(60)
                .Label("[bold yellow]Price History (Last 50 updates)[/]");

            // Show last 20 prices in the chart
            var recentPrices = _priceHistory.TakeLast(20).ToList();
            for (int i = 0; i < recentPrices.Count; i++)
            {
                var price = recentPrices[i];
                var color = Color.Yellow;

                if (i > 0)
                {
                    var prevPrice = recentPrices[i - 1];
                    color = price > prevPrice ? Color.Green : price < prevPrice ? Color.Red : Color.Yellow;
                }

                chart.AddItem($"{i + 1}", (double)price / 1000, color);
            }

            return new Panel(chart)
            {
                Header = new PanelHeader("📊 [bold]PRICE TREND[/]", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Yellow)
            };
        }

        private Panel CreateOrderBookPanel(DepthResponse? depth)
        {
            if (depth == null || depth.Bids.Count == 0 || depth.Asks.Count == 0)
            {
                return new Panel("[dim]Loading order book...[/]");
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Cyan1)
                .AddColumn(new TableColumn("[green bold]BID[/]").RightAligned())
                .AddColumn(new TableColumn("[yellow bold]AMOUNT[/]").RightAligned())
                .AddColumn(new TableColumn("[dim]│[/]").Centered())
                .AddColumn(new TableColumn("[yellow bold]AMOUNT[/]").RightAligned())
                .AddColumn(new TableColumn("[red bold]ASK[/]").RightAligned());

            for (int i = 0; i < Math.Min(10, Math.Min(depth.Bids.Count, depth.Asks.Count)); i++)
            {
                var bid = depth.Bids[i];
                var ask = depth.Asks[i];

                table.AddRow(
                    $"[green]{bid[0]:N0}[/]",
                    $"[dim]{bid[1]:N6}[/]",
                    "[dim]│[/]",
                    $"[dim]{ask[1]:N6}[/]",
                    $"[red]{ask[0]:N0}[/]"
                );
            }

            var highestBid = depth.Bids[0][0];
            var lowestAsk = depth.Asks[0][0];
            var spread = lowestAsk - highestBid;
            var spreadPercent = (spread / lowestAsk) * 100;

            var spreadInfo = new Markup(
                $"\n[dim]Spread:[/] [yellow]{spread:N2}[/] THB ([cyan]{spreadPercent:N4}%[/])"
            );

            var content = new Rows(table, spreadInfo);

            return new Panel(content)
            {
                Header = new PanelHeader("📚 [bold]ORDER BOOK[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Cyan1)
            };
        }

        private Panel CreateBalancePanel(Dictionary<string, BalanceInfo>? balances, decimal btcPrice)
        {
            if (balances == null || balances.Count == 0)
            {
                return new Panel("[dim]Loading balances...[/]");
            }

            var table = new Table()
                .Border(TableBorder.None)
                .HideHeaders()
                .AddColumn("")
                .AddColumn("")
                .AddColumn("");

            decimal totalValue = 0;
            foreach (var balance in balances.Take(5))
            {
                if (balance.Value.Available > 0 || balance.Value.Reserved > 0)
                {
                    var icon = balance.Key switch
                    {
                        "BTC" => "₿",
                        "ETH" => "Ξ",
                        "THB" => "฿",
                        _ => "●"
                    };

                    decimal total = balance.Value.Available + balance.Value.Reserved;
                    decimal price = balance.Key == "BTC" ? btcPrice : 1;
                    decimal value = total * price;
                    totalValue += value;

                    table.AddRow(
                        $"[bold]{icon} {balance.Key}[/]",
                        $"[cyan]{total:N4}[/]",
                        $"[yellow]{value:N2}[/]"
                    );
                }
            }

            table.AddEmptyRow();
            table.AddRow("[bold yellow]TOTAL[/]", "", $"[bold green]{totalValue:N2} ฿[/]");

            return new Panel(table)
            {
                Header = new PanelHeader("💰 [bold]BALANCE[/]", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Yellow)
            };
        }

        private Panel CreateOpenOrdersPanel(List<OpenOrder>? orders)
        {
            if (orders == null)
            {
                return new Panel("[dim]Loading orders...[/]");
            }

            if (orders.Count == 0)
            {
                return new Panel(Align.Center(
                    new Markup("[dim]No open orders[/]"),
                    VerticalAlignment.Middle))
                {
                    Header = new PanelHeader("📋 [bold]OPEN ORDERS[/]", Justify.Center),
                    Border = BoxBorder.Rounded,
                    BorderStyle = new Style(Color.Magenta)
                };
            }

            var table = new Table()
                .Border(TableBorder.None)
                .HideHeaders()
                .AddColumn("")
                .AddColumn("")
                .AddColumn("")
                .AddColumn("");

            foreach (var order in orders.Take(5))
            {
                var sideColor = order.Side == "BUY" ? "green" : "red";
                var icon = order.Side == "BUY" ? "📈" : "📉";

                table.AddRow(
                    $"[{sideColor}]{icon}[/]",
                    $"[bold]{order.Side}[/]",
                    $"[yellow]{order.Rate:N0}[/]",
                    $"[cyan]{order.Amount:N6}[/]"
                );
            }

            if (orders.Count > 5)
            {
                table.AddRow("[dim]...[/]", $"[dim]+{orders.Count - 5} more[/]", "", "");
            }

            return new Panel(table)
            {
                Header = new PanelHeader($"📋 [bold]OPEN ORDERS ({orders.Count})[/]", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Magenta)
            };
        }

        private Panel CreateFooter()
        {
            var footer = new Markup(
                "[dim]Press [bold cyan]Ctrl+C[/] to exit  │  " +
                "Updates every 2 seconds  │  " +
                "[yellow]⚠ Trading involves risk[/][/]"
            );

            return new Panel(Align.Center(footer, VerticalAlignment.Middle))
            {
                Border = BoxBorder.Heavy,
                BorderStyle = new Style(Color.Grey)
            };
        }
    }
}
