using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

namespace BitkubTrader
{
    /// <summary>
    /// Beautiful Console UI Helper with Spectre.Console
    /// </summary>
    public static class ConsoleUI
    {
        private static readonly FigletFont TitleFont = FigletFont.Load("standard.flf");

        /// <summary>
        /// แสดง ASCII Art Logo
        /// </summary>
        public static void ShowLogo()
        {
            var gradient = new Color[] { Color.Blue, Color.Aqua, Color.Cyan1, Color.Yellow, Color.Orange1 };

            AnsiConsole.Clear();

            var figlet = new FigletText("BITKUB TRADER")
                .Color(Color.Cyan1);

            var rule = new Rule("[yellow]Cryptocurrency Trading Bot[/]")
            {
                Style = Style.Parse("cyan dim")
            };

            var panel = new Panel(
                Align.Center(
                    new Markup("[bold cyan]⚡ Powered by C# & .NET 6.0 ⚡[/]\n[dim]Professional Trading Made Simple[/]"),
                    VerticalAlignment.Middle))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Cyan1)
            };

            AnsiConsole.Write(figlet);
            AnsiConsole.Write(rule);
            AnsiConsole.WriteLine();
            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }

        /// <summary>
        /// แสดง Loading Animation
        /// </summary>
        public static T WithSpinner<T>(string message, Func<T> action)
        {
            return AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("cyan bold"))
                .Start(message, ctx =>
                {
                    return action();
                });
        }

        /// <summary>
        /// แสดง Loading Animation (Async)
        /// </summary>
        public static async Task<T> WithSpinnerAsync<T>(string message, Func<Task<T>> action)
        {
            return await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("cyan bold"))
                .StartAsync(message, async ctx =>
                {
                    return await action();
                });
        }

        /// <summary>
        /// แสดง Progress Bar
        /// </summary>
        public static async Task WithProgressAsync(string description, Func<ProgressTask, Task> action)
        {
            await AnsiConsole.Progress()
                .Columns(
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new SpinnerColumn())
                .StartAsync(async ctx =>
                {
                    var task = ctx.AddTask(description, maxValue: 100);
                    await action(task);
                });
        }

        /// <summary>
        /// แสดงตารางราคา Ticker
        /// </summary>
        public static void ShowTickerTable(Dictionary<string, TickerInfo> tickers)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Cyan1)
                .AddColumn(new TableColumn("[cyan bold]Symbol[/]").Centered())
                .AddColumn(new TableColumn("[yellow bold]Last Price[/]").RightAligned())
                .AddColumn(new TableColumn("[green bold]24h High[/]").RightAligned())
                .AddColumn(new TableColumn("[red bold]24h Low[/]").RightAligned())
                .AddColumn(new TableColumn("[blue bold]Change %[/]").RightAligned())
                .AddColumn(new TableColumn("[magenta bold]Volume[/]").RightAligned());

            foreach (var ticker in tickers.Take(10))
            {
                var changeColor = ticker.Value.PercentChange >= 0 ? "green" : "red";
                var changeIcon = ticker.Value.PercentChange >= 0 ? "▲" : "▼";

                table.AddRow(
                    $"[bold]{ticker.Key}[/]",
                    $"[yellow]{ticker.Value.Last:N2}[/]",
                    $"[green]{ticker.Value.High24Hr:N2}[/]",
                    $"[red]{ticker.Value.Low24Hr:N2}[/]",
                    $"[{changeColor}]{changeIcon} {ticker.Value.PercentChange:N2}%[/]",
                    $"[magenta]{ticker.Value.BaseVolume:N4}[/]"
                );
            }

            var panel = new Panel(table)
            {
                Header = new PanelHeader("📊 [bold cyan]Market Overview[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Cyan1)
            };

            AnsiConsole.Write(panel);
        }

        /// <summary>
        /// แสดงตาราง Order Book
        /// </summary>
        public static void ShowOrderBook(List<List<decimal>> bids, List<List<decimal>> asks, string symbol)
        {
            var layout = new Layout("Root")
                .SplitColumns(
                    new Layout("Bids"),
                    new Layout("Asks"));

            // Bids Table (Buy Orders)
            var bidsTable = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Green)
                .AddColumn(new TableColumn("[green bold]Price (THB)[/]").RightAligned())
                .AddColumn(new TableColumn("[green bold]Amount[/]").RightAligned())
                .AddColumn(new TableColumn("[green bold]Total[/]").RightAligned());

            foreach (var bid in bids.Take(10))
            {
                bidsTable.AddRow(
                    $"[green]{bid[0]:N2}[/]",
                    $"{bid[1]:N8}",
                    $"{(bid[0] * bid[1]):N2}"
                );
            }

            var bidsPanel = new Panel(bidsTable)
            {
                Header = new PanelHeader("📈 [bold green]BUY ORDERS[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Green)
            };

            // Asks Table (Sell Orders)
            var asksTable = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Red)
                .AddColumn(new TableColumn("[red bold]Price (THB)[/]").RightAligned())
                .AddColumn(new TableColumn("[red bold]Amount[/]").RightAligned())
                .AddColumn(new TableColumn("[red bold]Total[/]").RightAligned());

            foreach (var ask in asks.Take(10))
            {
                asksTable.AddRow(
                    $"[red]{ask[0]:N2}[/]",
                    $"{ask[1]:N8}",
                    $"{(ask[0] * ask[1]):N2}"
                );
            }

            var asksPanel = new Panel(asksTable)
            {
                Header = new PanelHeader("📉 [bold red]SELL ORDERS[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Red)
            };

            layout["Bids"].Update(bidsPanel);
            layout["Asks"].Update(asksPanel);

            var mainPanel = new Panel(layout)
            {
                Header = new PanelHeader($"📊 [bold yellow]{symbol} Order Book[/]", Justify.Center),
                Border = BoxBorder.Heavy,
                BorderStyle = new Style(Color.Yellow)
            };

            AnsiConsole.Write(mainPanel);
        }

        /// <summary>
        /// แสดงยอดเงินในบัญชี
        /// </summary>
        public static void ShowBalances(Dictionary<string, BalanceInfo> balances, Dictionary<string, decimal> prices)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Yellow)
                .AddColumn(new TableColumn("[yellow bold]Currency[/]").Centered())
                .AddColumn(new TableColumn("[green bold]Available[/]").RightAligned())
                .AddColumn(new TableColumn("[orange1 bold]Reserved[/]").RightAligned())
                .AddColumn(new TableColumn("[cyan bold]Total[/]").RightAligned())
                .AddColumn(new TableColumn("[magenta bold]Value (THB)[/]").RightAligned());

            decimal totalValue = 0;

            foreach (var balance in balances.OrderByDescending(b => b.Value.Available + b.Value.Reserved))
            {
                if (balance.Value.Available > 0 || balance.Value.Reserved > 0)
                {
                    decimal total = balance.Value.Available + balance.Value.Reserved;
                    decimal price = prices.ContainsKey(balance.Key) ? prices[balance.Key] : 1;
                    decimal value = total * price;
                    totalValue += value;

                    var icon = balance.Key switch
                    {
                        "BTC" => "₿",
                        "ETH" => "Ξ",
                        "THB" => "฿",
                        _ => "●"
                    };

                    table.AddRow(
                        $"[bold]{icon} {balance.Key}[/]",
                        $"[green]{balance.Value.Available:N8}[/]",
                        $"[orange1]{balance.Value.Reserved:N8}[/]",
                        $"[cyan]{total:N8}[/]",
                        $"[magenta]{value:N2}[/]"
                    );
                }
            }

            table.AddEmptyRow();
            table.AddRow(
                "[bold yellow]TOTAL VALUE[/]",
                "", "", "",
                $"[bold magenta]{totalValue:N2} THB[/]"
            );

            var panel = new Panel(table)
            {
                Header = new PanelHeader("💰 [bold yellow]Portfolio Balance[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Yellow)
            };

            AnsiConsole.Write(panel);
        }

        /// <summary>
        /// แสดงข้อมูลคำสั่งซื้อขาย
        /// </summary>
        public static void ShowOrderResult(OrderResult order, string side)
        {
            var color = side.ToLower() == "buy" ? "green" : "red";
            var icon = side.ToLower() == "buy" ? "📈" : "📉";
            var title = side.ToLower() == "buy" ? "BUY ORDER" : "SELL ORDER";

            var grid = new Grid()
                .AddColumn()
                .AddColumn();

            grid.AddRow($"[bold]Order ID:[/]", $"[{color}]{order.Id}[/]");
            grid.AddRow($"[bold]Hash:[/]", $"[dim]{order.Hash}[/]");
            grid.AddRow($"[bold]Type:[/]", $"[{color}]{order.Type.ToUpper()}[/]");
            grid.AddRow($"[bold]Amount:[/]", $"[yellow]{order.Amount:N8}[/]");
            grid.AddRow($"[bold]Rate:[/]", $"[cyan]{order.Rate:N2} THB[/]");
            grid.AddRow($"[bold]Fee:[/]", $"[red]{order.Fee:N2} THB[/]");
            grid.AddRow($"[bold]Will Receive:[/]", $"[bold green]{order.Receive:N8}[/]");

            var panel = new Panel(grid)
            {
                Header = new PanelHeader($"{icon} [bold {color}]{title} PLACED[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.FromInt32(color == "green" ? 2 : 1))
            };

            AnsiConsole.Write(panel);
        }

        /// <summary>
        /// แสดงข้อความสำเร็จ
        /// </summary>
        public static void ShowSuccess(string message)
        {
            AnsiConsole.MarkupLine($"[bold green]✓[/] {message}");
        }

        /// <summary>
        /// แสดงข้อความเตือน
        /// </summary>
        public static void ShowWarning(string message)
        {
            AnsiConsole.MarkupLine($"[bold yellow]⚠[/] {message}");
        }

        /// <summary>
        /// แสดงข้อความผิดพลาด
        /// </summary>
        public static void ShowError(string message)
        {
            AnsiConsole.MarkupLine($"[bold red]✗[/] {message}");
        }

        /// <summary>
        /// แสดงข้อมูล
        /// </summary>
        public static void ShowInfo(string message)
        {
            AnsiConsole.MarkupLine($"[bold cyan]ℹ[/] {message}");
        }

        /// <summary>
        /// แสดง Live Trading Dashboard
        /// </summary>
        public static async Task ShowLiveDashboard(BitkubClient client, string symbol, CancellationToken cancellationToken)
        {
            await AnsiConsole.Live(CreateDashboard(symbol, null, null, null))
                .StartAsync(async ctx =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var ticker = await client.GetTickerAsync(symbol);
                            var depth = await client.GetDepthAsync(symbol, 5);
                            var balances = await client.GetBalancesAsync();

                            var dashboard = CreateDashboard(
                                symbol,
                                ticker.ContainsKey(symbol) ? ticker[symbol] : null,
                                depth,
                                balances.Error == 0 ? balances.Result : null
                            );

                            ctx.UpdateTarget(dashboard);
                            await Task.Delay(2000, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            ctx.UpdateTarget(new Markup($"[red]Error: {ex.Message}[/]"));
                        }
                    }
                });
        }

        private static Panel CreateDashboard(string symbol, TickerInfo? ticker, DepthResponse? depth, Dictionary<string, BalanceInfo>? balances)
        {
            var grid = new Grid()
                .AddColumn(new GridColumn().Width(60))
                .AddColumn(new GridColumn().Width(60));

            // Price Info Panel
            var pricePanel = CreatePricePanel(symbol, ticker);

            // Order Book Mini Panel
            var orderBookPanel = CreateMiniOrderBook(depth);

            grid.AddRow(pricePanel, orderBookPanel);

            var mainPanel = new Panel(grid)
            {
                Header = new PanelHeader($"⚡ [bold cyan]LIVE TRADING DASHBOARD[/] ⚡", Justify.Center),
                Border = BoxBorder.Heavy,
                BorderStyle = new Style(Color.Cyan1)
            };

            return mainPanel;
        }

        private static Panel CreatePricePanel(string symbol, TickerInfo? ticker)
        {
            if (ticker == null)
            {
                return new Panel("[dim]Loading...[/]");
            }

            var changeColor = ticker.PercentChange >= 0 ? "green" : "red";
            var changeIcon = ticker.PercentChange >= 0 ? "▲" : "▼";

            var markup = new Markup(
                $"[bold yellow]{symbol}[/]\n\n" +
                $"[bold white]Last:[/] [yellow]{ticker.Last:N2}[/] THB\n" +
                $"[bold white]24h:[/] [{changeColor}]{changeIcon} {ticker.PercentChange:N2}%[/]\n" +
                $"[bold white]High:[/] [green]{ticker.High24Hr:N2}[/]\n" +
                $"[bold white]Low:[/] [red]{ticker.Low24Hr:N2}[/]\n" +
                $"[bold white]Volume:[/] [cyan]{ticker.BaseVolume:N4}[/]"
            );

            return new Panel(markup)
            {
                Header = new PanelHeader("💹 [bold]Price Info[/]"),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Yellow)
            };
        }

        private static Panel CreateMiniOrderBook(DepthResponse? depth)
        {
            if (depth == null)
            {
                return new Panel("[dim]Loading...[/]");
            }

            var table = new Table()
                .HideHeaders()
                .Border(TableBorder.None)
                .AddColumn("")
                .AddColumn("")
                .AddColumn("");

            table.AddRow("[bold green]BUY[/]", "[bold]Price[/]", "[bold red]SELL[/]");

            for (int i = 0; i < 5; i++)
            {
                var bid = depth.Bids.Count > i ? depth.Bids[i][0].ToString("N0") : "-";
                var ask = depth.Asks.Count > i ? depth.Asks[i][0].ToString("N0") : "-";
                table.AddRow($"[green]{bid}[/]", "", $"[red]{ask}[/]");
            }

            return new Panel(table)
            {
                Header = new PanelHeader("📊 [bold]Order Book[/]"),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Cyan1)
            };
        }

        /// <summary>
        /// สร้างเมนูเลือก
        /// </summary>
        public static string ShowMenu(string title, string[] choices)
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[bold cyan]{title}[/]")
                    .PageSize(10)
                    .MoreChoicesText("[dim](Move up and down to reveal more options)[/]")
                    .AddChoices(choices)
                    .HighlightStyle(new Style(Color.Cyan1, decoration: Decoration.Bold))
            );
        }

        /// <summary>
        /// ขอ Input จากผู้ใช้
        /// </summary>
        public static string AskInput(string prompt, string? defaultValue = null)
        {
            var textPrompt = new TextPrompt<string>($"[cyan]{prompt}:[/]");

            if (defaultValue != null)
            {
                textPrompt.DefaultValue(defaultValue);
            }

            return AnsiConsole.Prompt(textPrompt);
        }

        /// <summary>
        /// ยืนยันการทำงาน
        /// </summary>
        public static bool Confirm(string question)
        {
            return AnsiConsole.Confirm($"[yellow]{question}[/]");
        }
    }
}
