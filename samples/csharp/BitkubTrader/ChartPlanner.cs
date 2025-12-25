using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

namespace BitkubTrader
{
    /// <summary>
    /// 📊 กราฟวางแผนการเทรด - แสดงจุดเข้า, Stop Loss, Take Profit
    /// พร้อมแสดงระดับ Support/Resistance, Fibonacci, Volume Profile
    /// </summary>
    public class ChartPlanner
    {
        /// <summary>
        /// แสดงกราฟวางแผนการเข้า Position
        /// </summary>
        public static void ShowEntryPlan(
            decimal currentPrice,
            TradingPlan plan,
            UltimateAnalysis analysis)
        {
            AnsiConsole.Clear();

            // Header
            var title = new FigletText("TRADING PLAN")
            {
                Color = Color.Cyan1
            };
            AnsiConsole.Write(title);
            AnsiConsole.WriteLine();

            // Current Market Info
            ShowCurrentMarket(currentPrice, plan.Symbol);

            AnsiConsole.WriteLine();

            // Entry Points
            ShowEntryPoints(currentPrice, plan);

            AnsiConsole.WriteLine();

            // Exit Strategy
            ShowExitStrategy(currentPrice, plan);

            AnsiConsole.WriteLine();

            // Key Levels
            ShowKeyLevels(currentPrice, analysis);

            AnsiConsole.WriteLine();

            // Visual Price Chart
            ShowVisualChart(currentPrice, plan, analysis);

            AnsiConsole.WriteLine();

            // Risk/Reward Analysis
            ShowRiskReward(plan);
        }

        /// <summary>
        /// แสดงข้อมูลตลาดปัจจุบัน
        /// </summary>
        private static void ShowCurrentMarket(decimal currentPrice, string symbol)
        {
            var panel = new Panel(
                new Markup(
                    $"[bold cyan]Symbol:[/] [yellow]{symbol}[/]\n" +
                    $"[bold cyan]ราคาปัจจุบัน:[/] [green]{currentPrice:N2}[/] THB\n" +
                    $"[bold cyan]เวลา:[/] {DateTime.Now:dd/MM/yyyy HH:mm:ss}"
                ))
            {
                Header = new PanelHeader("📊 [bold]ตลาดปัจจุบัน[/]", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Cyan1)
            };

            AnsiConsole.Write(panel);
        }

        /// <summary>
        /// แสดงจุดเข้า (Entry Points)
        /// </summary>
        private static void ShowEntryPoints(decimal currentPrice, TradingPlan plan)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Green)
                .AddColumn("[green]จุดเข้า[/]")
                .AddColumn("[yellow]ราคา[/]")
                .AddColumn("[cyan]ระยะห่าง[/]")
                .AddColumn("[magenta]จำนวน[/]")
                .AddColumn("[blue]มูลค่า[/]");

            foreach (var entry in plan.EntryPoints)
            {
                var distance = ((entry.Price - currentPrice) / currentPrice) * 100;
                var distanceColor = Math.Abs(distance) < 1 ? "green" :
                                   Math.Abs(distance) < 3 ? "yellow" : "red";

                var marker = entry.Price == plan.EntryPoints[0].Price ? "👉 " : "   ";

                table.AddRow(
                    $"{marker}{entry.Type}",
                    $"{entry.Price:N2}",
                    $"[{distanceColor}]{distance:+0.00;-0.00}%[/]",
                    $"{entry.Amount:N8}",
                    $"{(entry.Price * entry.Amount):N2} THB"
                );
            }

            var panel = new Panel(table)
            {
                Header = new PanelHeader("🎯 [bold]จุดเข้า (Entry Points)[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Green)
            };

            AnsiConsole.Write(panel);
        }

        /// <summary>
        /// แสดงกลยุทธ์ออก (Exit Strategy)
        /// </summary>
        private static void ShowExitStrategy(decimal currentPrice, TradingPlan plan)
        {
            var grid = new Grid()
                .AddColumn()
                .AddColumn()
                .AddColumn();

            // Stop Loss
            var slDistance = ((plan.StopLoss - currentPrice) / currentPrice) * 100;
            grid.AddRow(
                new Markup("[bold red]🛑 Stop Loss:[/]"),
                new Markup($"[red]{plan.StopLoss:N2}[/] THB"),
                new Markup($"[red]({slDistance:N2}%)[/]")
            );

            grid.AddEmptyRow();

            // Take Profit Levels
            for (int i = 0; i < plan.TakeProfitLevels.Count; i++)
            {
                var tp = plan.TakeProfitLevels[i];
                var tpDistance = ((tp.Price - currentPrice) / currentPrice) * 100;

                grid.AddRow(
                    new Markup($"[bold green]🎯 TP {i + 1} ({tp.Percentage}%):[/]"),
                    new Markup($"[green]{tp.Price:N2}[/] THB"),
                    new Markup($"[green](+{tpDistance:N2}%)[/]")
                );
            }

            var panel = new Panel(grid)
            {
                Header = new PanelHeader("🚪 [bold]กลยุทธ์ออก (Exit Strategy)[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Yellow)
            };

            AnsiConsole.Write(panel);
        }

        /// <summary>
        /// แสดงระดับสำคัญ (Key Levels)
        /// </summary>
        private static void ShowKeyLevels(decimal currentPrice, UltimateAnalysis analysis)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[cyan]ประเภท[/]")
                .AddColumn("[yellow]ระดับ[/]")
                .AddColumn("[green]ราคา[/]")
                .AddColumn("[magenta]ระยะห่าง[/]");

            // Fibonacci Levels
            if (analysis.FibonacciLevels?.Any() == true)
            {
                foreach (var fib in analysis.FibonacciLevels.Take(3))
                {
                    var color = fib.Type == "SUPPORT" ? "green" : "red";
                    table.AddRow(
                        "📐 Fibonacci",
                        fib.Name,
                        $"[{color}]{fib.Price:N2}[/]",
                        $"{fib.Distance:N2}%"
                    );
                }
            }

            // Volume Profile
            if (analysis.VolumeProfile != null)
            {
                table.AddRow(
                    "📊 Volume",
                    "POC",
                    $"[yellow]{analysis.VolumeProfile.POC:N2}[/]",
                    $"{((analysis.VolumeProfile.POC - currentPrice) / currentPrice * 100):N2}%"
                );

                table.AddRow(
                    "📊 Volume",
                    "VAH",
                    $"[green]{analysis.VolumeProfile.VAH:N2}[/]",
                    $"{((analysis.VolumeProfile.VAH - currentPrice) / currentPrice * 100):N2}%"
                );

                table.AddRow(
                    "📊 Volume",
                    "VAL",
                    $"[red]{analysis.VolumeProfile.VAL:N2}[/]",
                    $"{((analysis.VolumeProfile.VAL - currentPrice) / currentPrice * 100):N2}%"
                );
            }

            // Market Structure
            if (analysis.MarketStructure != null)
            {
                table.AddRow(
                    "🏗️ Structure",
                    "Trend",
                    $"[cyan]{analysis.MarketStructure.Trend}[/]",
                    "-"
                );
            }

            var panel = new Panel(table)
            {
                Header = new PanelHeader("📍 [bold]ระดับสำคัญ (Key Levels)[/]", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Cyan1)
            };

            AnsiConsole.Write(panel);
        }

        /// <summary>
        /// แสดงกราฟภาพ (Visual Chart)
        /// </summary>
        private static void ShowVisualChart(decimal currentPrice, TradingPlan plan,
            UltimateAnalysis analysis)
        {
            // รวบรวมราคาทั้งหมด
            var allPrices = new List<(decimal Price, string Label, string Color)>
            {
                (currentPrice, ">>> ราคาปัจจุบัน", "yellow")
            };

            // Entry points
            foreach (var entry in plan.EntryPoints)
            {
                allPrices.Add((entry.Price, $"🎯 {entry.Type}", "green"));
            }

            // Stop Loss
            allPrices.Add((plan.StopLoss, "🛑 Stop Loss", "red"));

            // Take Profit
            foreach (var tp in plan.TakeProfitLevels)
            {
                allPrices.Add((tp.Price, $"💰 TP {tp.Percentage}%", "green"));
            }

            // Fibonacci
            if (analysis.FibonacciLevels?.Any() == true)
            {
                foreach (var fib in analysis.FibonacciLevels.Take(3))
                {
                    var color = fib.Type == "SUPPORT" ? "blue" : "magenta";
                    allPrices.Add((fib.Price, $"📐 {fib.Name}", color));
                }
            }

            // Volume Profile
            if (analysis.VolumeProfile != null)
            {
                allPrices.Add((analysis.VolumeProfile.POC, "📊 POC", "cyan"));
                allPrices.Add((analysis.VolumeProfile.VAH, "📊 VAH", "green"));
                allPrices.Add((analysis.VolumeProfile.VAL, "📊 VAL", "red"));
            }

            // เรียงลำดับ
            var sortedPrices = allPrices.OrderByDescending(x => x.Price).ToList();

            // สร้างกราฟ
            var chart = new Table()
                .Border(TableBorder.Heavy)
                .BorderColor(Color.Grey)
                .AddColumn(new TableColumn("ราคา").RightAligned())
                .AddColumn(new TableColumn("").Width(50))
                .AddColumn(new TableColumn("รายละเอียด").LeftAligned());

            var maxPrice = sortedPrices.Max(x => x.Price);
            var minPrice = sortedPrices.Min(x => x.Price);
            var priceRange = maxPrice - minPrice;

            foreach (var item in sortedPrices)
            {
                var percentage = priceRange > 0 ?
                    (item.Price - minPrice) / priceRange : 0.5m;
                var barLength = (int)(percentage * 40);

                var bar = new string('█', Math.Max(1, barLength));
                var spaces = new string(' ', Math.Max(0, 40 - barLength));

                var isCurrentPrice = item.Label.Contains("ราคาปัจจุบัน");
                var marker = isCurrentPrice ? ">>>" : "   ";

                chart.AddRow(
                    $"[{item.Color}]{item.Price:N0}[/]",
                    $"{marker}[{item.Color}]{bar}[/]{spaces}",
                    $"[{item.Color}]{item.Label}[/]"
                );
            }

            var panel = new Panel(chart)
            {
                Header = new PanelHeader("📈 [bold]กราฟราคา (Price Chart)[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Grey)
            };

            AnsiConsole.Write(panel);
        }

        /// <summary>
        /// แสดงการวิเคราะห์ Risk/Reward
        /// </summary>
        private static void ShowRiskReward(TradingPlan plan)
        {
            var entryPrice = plan.EntryPoints.First().Price;
            var risk = entryPrice - plan.StopLoss;
            var riskPercent = (risk / entryPrice) * 100;

            var rewards = new List<(string Label, decimal Reward, decimal Ratio)>();

            foreach (var tp in plan.TakeProfitLevels)
            {
                var reward = tp.Price - entryPrice;
                var ratio = risk > 0 ? reward / risk : 0;
                rewards.Add(($"TP {tp.Percentage}%", reward, ratio));
            }

            var grid = new Grid()
                .AddColumn()
                .AddColumn()
                .AddColumn();

            grid.AddRow(
                new Markup("[bold red]💸 Risk:[/]"),
                new Markup($"[red]{risk:N2}[/] THB"),
                new Markup($"[red]({riskPercent:N2}%)[/]")
            );

            grid.AddEmptyRow();

            foreach (var (label, reward, ratio) in rewards)
            {
                var rewardPercent = (reward / entryPrice) * 100;
                var ratioColor = ratio >= 3 ? "green" :
                                ratio >= 2 ? "yellow" : "red";

                grid.AddRow(
                    new Markup($"[bold green]💰 Reward ({label}):[/]"),
                    new Markup($"[green]{reward:N2}[/] THB"),
                    new Markup($"[green](+{rewardPercent:N2}%)[/]")
                );

                grid.AddRow(
                    new Markup($"[bold {ratioColor}]⚖️ Risk/Reward:[/]"),
                    new Markup($"[{ratioColor}]1 : {ratio:N2}[/]"),
                    new Markup($"[dim]{(ratio >= 2 ? "✅ ดี" : "⚠️ ควรปรับ")}[/]")
                );

                grid.AddEmptyRow();
            }

            var panel = new Panel(grid)
            {
                Header = new PanelHeader("⚖️ [bold]Risk/Reward Analysis[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Magenta1)
            };

            AnsiConsole.Write(panel);

            // คำแนะนำ
            var bestRatio = rewards.Max(x => x.Ratio);
            if (bestRatio >= 3)
            {
                AnsiConsole.MarkupLine("[green]✅ Risk/Reward Ratio ดีมาก! เหมาะสำหรับการเข้า[/]");
            }
            else if (bestRatio >= 2)
            {
                AnsiConsole.MarkupLine("[yellow]⚠️ Risk/Reward Ratio พอใช้ได้[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]❌ Risk/Reward Ratio ไม่ดี ควรปรับ Stop Loss หรือ Take Profit[/]");
            }
        }
    }

    /// <summary>
    /// แผนการเทรด
    /// </summary>
    public class TradingPlan
    {
        public string Symbol { get; set; } = "";
        public List<EntryPoint> EntryPoints { get; set; } = new();
        public decimal StopLoss { get; set; }
        public List<TakeProfitLevel> TakeProfitLevels { get; set; } = new();
        public decimal TotalRisk { get; set; }
        public string Strategy { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class EntryPoint
    {
        public string Type { get; set; } = ""; // "LIMIT", "MARKET", "STOP"
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public string Condition { get; set; } = ""; // เงื่อนไขการเข้า
    }

    public class TakeProfitLevel
    {
        public int Percentage { get; set; } // เปอร์เซ็นต์ของ position ที่จะขาย
        public decimal Price { get; set; }
        public string Reason { get; set; } = ""; // เหตุผลเลือกระดับนี้
    }

    /// <summary>
    /// Helper สำหรับสร้าง Trading Plan
    /// </summary>
    public class TradingPlanBuilder
    {
        /// <summary>
        /// สร้างแผนการเทรดจาก Config และ Analysis
        /// </summary>
        public static TradingPlan CreatePlan(
            decimal currentPrice,
            TradingConfig config,
            UltimateAnalysis analysis,
            decimal balance)
        {
            var plan = new TradingPlan
            {
                Symbol = config.General.Symbol
            };

            // คำนวณ Entry Point
            var entryPrice = currentPrice;

            // ถ้ามี Fibonacci Support ใกล้ๆ อาจรอเข้าที่ระดับนั้น
            var nearestSupport = analysis.FibonacciLevels?
                .Where(f => f.Type == "SUPPORT" && f.Price < currentPrice &&
                            Math.Abs(f.Distance) < 2)
                .OrderBy(f => Math.Abs(f.Distance))
                .FirstOrDefault();

            if (nearestSupport != null && config.Entry.FibonacciConditions.PreferredLevels.Any())
            {
                // เพิ่ม Entry ที่ Fibonacci
                plan.EntryPoints.Add(new EntryPoint
                {
                    Type = "LIMIT",
                    Price = nearestSupport.Price,
                    Amount = 0,
                    Condition = $"Fibonacci {nearestSupport.Name} Support"
                });
            }

            // Entry หลัก
            plan.EntryPoints.Add(new EntryPoint
            {
                Type = "MARKET",
                Price = currentPrice,
                Amount = 0,
                Condition = "Market Entry"
            });

            // คำนวณ Stop Loss
            plan.StopLoss = CalculateStopLoss(currentPrice, config, analysis);

            // คำนวณ Take Profit
            plan.TakeProfitLevels = CalculateTakeProfitLevels(currentPrice, config, analysis);

            // คำนวณ Position Size
            var risk = currentPrice - plan.StopLoss;
            var riskAmount = balance * (config.Risk.MaxRiskPerTradePercent / 100);
            var positionSize = risk > 0 ? riskAmount / risk : 0;

            // แบ่ง Position ตาม Entry Points
            foreach (var entry in plan.EntryPoints)
            {
                entry.Amount = positionSize / plan.EntryPoints.Count;
            }

            plan.TotalRisk = riskAmount;
            plan.Strategy = $"Master Score: {analysis.MasterScore}, Trend: {analysis.MarketStructure?.Trend}";

            return plan;
        }

        private static decimal CalculateStopLoss(
            decimal currentPrice,
            TradingConfig config,
            UltimateAnalysis analysis)
        {
            switch (config.Exit.StopLossType)
            {
                case "PERCENTAGE":
                    return currentPrice * (1 - config.Exit.StopLossPercent / 100);

                case "FIBONACCI":
                    // ใช้ Fibonacci Support ที่ต่ำกว่าเป็น SL
                    var fibSL = analysis.FibonacciLevels?
                        .Where(f => f.Type == "SUPPORT" && f.Price < currentPrice)
                        .OrderByDescending(f => f.Price)
                        .FirstOrDefault();

                    if (fibSL != null)
                        return fibSL.Price * 0.995m; // ต่ำกว่าอีกนิดเพื่อความปลอดภัย

                    goto case "PERCENTAGE"; // ถ้าไม่มี Fib ใช้ % แทน

                case "ATR":
                    // ใช้ ATR (ต้องคำนวณเพิ่ม)
                    goto case "PERCENTAGE";

                default:
                    return currentPrice * (1 - config.Exit.StopLossPercent / 100);
            }
        }

        private static List<TakeProfitLevel> CalculateTakeProfitLevels(
            decimal currentPrice,
            TradingConfig config,
            UltimateAnalysis analysis)
        {
            var levels = new List<TakeProfitLevel>();

            switch (config.Exit.TakeProfitType)
            {
                case "PERCENTAGE":
                    // TP แบบง่าย
                    levels.Add(new TakeProfitLevel
                    {
                        Percentage = 50,
                        Price = currentPrice * (1 + config.Exit.TakeProfitPercent / 100),
                        Reason = $"Take Profit {config.Exit.TakeProfitPercent}%"
                    });

                    levels.Add(new TakeProfitLevel
                    {
                        Percentage = 50,
                        Price = currentPrice * (1 + config.Exit.TakeProfitPercent / 100 * 1.5m),
                        Reason = $"Take Profit {config.Exit.TakeProfitPercent * 1.5m}%"
                    });
                    break;

                case "FIBONACCI":
                    // ใช้ Fibonacci Resistance เป็น TP
                    var fibResistances = analysis.FibonacciLevels?
                        .Where(f => f.Type == "RESISTANCE" && f.Price > currentPrice)
                        .OrderBy(f => f.Price)
                        .Take(3)
                        .ToList();

                    if (fibResistances?.Any() == true)
                    {
                        var percentage = 100 / fibResistances.Count;
                        foreach (var fib in fibResistances)
                        {
                            levels.Add(new TakeProfitLevel
                            {
                                Percentage = percentage,
                                Price = fib.Price,
                                Reason = $"Fibonacci {fib.Name} Resistance"
                            });
                        }
                    }
                    else
                    {
                        goto case "PERCENTAGE";
                    }
                    break;

                case "RESISTANCE":
                    // ใช้ resistance จาก Market Structure
                    goto case "PERCENTAGE";

                default:
                    goto case "PERCENTAGE";
            }

            return levels;
        }
    }
}
