using System;
using System.Threading;
using System.Threading.Tasks;

namespace BitkubTrader
{
    /// <summary>
    /// ตัวอย่างการใช้งาน Trading Bot ขั้นสูง
    /// </summary>
    public class AdvancedExample
    {
        private const string API_KEY = "YOUR_API_KEY";
        private const string API_SECRET = "YOUR_API_SECRET";

        /// <summary>
        /// ตัวอย่างที่ 1: Simple Trading Bot พร้อม Strategy
        /// </summary>
        public static async Task RunSimpleTradingBotAsync()
        {
            using var client = new BitkubClient(API_KEY, API_SECRET);
            var bot = new TradingBot(
                client,
                symbol: "THB_BTC",
                orderAmount: 1000,        // ใช้เงิน 1000 บาทต่อคำสั่ง
                profitPercent: 1.5m,      // เป้ากำไร 1.5%
                stopLossPercent: 2.0m     // หยุดขาดทุนที่ 2%
            );

            var cts = new CancellationTokenSource();

            // กด Ctrl+C เพื่อหยุด bot
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("\nStopping bot...");
                e.Cancel = true;
                cts.Cancel();
            };

            await bot.StartAsync(cts.Token);
        }

        /// <summary>
        /// ตัวอย่างที่ 2: DCA Strategy - ซื้อทีละน้อยในช่วงเวลาที่กำหนด
        /// </summary>
        public static async Task RunDCAStrategyAsync()
        {
            using var client = new BitkubClient(API_KEY, API_SECRET);
            var bot = new TradingBot(client, symbol: "THB_BTC");

            // ซื้อ BTC มูลค่า 10,000 บาท แบ่งออกเป็น 10 งวด ห่างกัน 1 ชั่วโมง
            await bot.DCAStrategyAsync(
                totalAmount: 10000,
                intervals: 10,
                interval: TimeSpan.FromHours(1),
                cancellationToken: CancellationToken.None
            );

            // แสดงสรุปผลการเทรด
            await bot.ShowTradingSummaryAsync(DateTime.Now.AddDays(-1));
        }

        /// <summary>
        /// ตัวอย่างที่ 3: Scalping Strategy - เทรดระยะสั้นหาส่วนต่างราคา
        /// </summary>
        public static async Task RunScalpingStrategyAsync()
        {
            using var client = new BitkubClient(API_KEY, API_SECRET);
            var bot = new TradingBot(
                client,
                symbol: "THB_BTC",
                orderAmount: 500,
                profitPercent: 0.3m,  // กำไรเป้าหมายเพียง 0.3%
                stopLossPercent: 0.5m // Stop loss ที่ 0.5%
            );

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            await bot.ScalpingStrategyAsync(cts.Token);
        }

        /// <summary>
        /// ตัวอย่างที่ 4: Monitor และแจ้งเตือนราคา
        /// </summary>
        public static async Task RunPriceMonitorAsync()
        {
            using var client = new BitkubClient(API_KEY, API_SECRET);
            decimal alertPriceHigh = 1500000; // แจ้งเตือนเมื่อราคาสูงกว่า 1.5 ล้าน
            decimal alertPriceLow = 1000000;  // แจ้งเตือนเมื่อราคาต่ำกว่า 1 ล้าน

            Console.WriteLine($"Monitoring BTC price...");
            Console.WriteLine($"High Alert: {alertPriceHigh:N0} THB");
            Console.WriteLine($"Low Alert: {alertPriceLow:N0} THB");
            Console.WriteLine();

            while (true)
            {
                try
                {
                    var ticker = await client.GetTickerAsync("THB_BTC");
                    if (ticker.TryGetValue("THB_BTC", out var info))
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] BTC: {info.Last:N2} THB ({info.PercentChange:N2}%)");

                        if (info.Last >= alertPriceHigh)
                        {
                            Console.WriteLine($"🚨 HIGH PRICE ALERT! BTC reached {info.Last:N2} THB");
                            // ส่งการแจ้งเตือน (Email, Line Notify, etc.)
                        }
                        else if (info.Last <= alertPriceLow)
                        {
                            Console.WriteLine($"🚨 LOW PRICE ALERT! BTC dropped to {info.Last:N2} THB");
                            // ส่งการแจ้งเตือน
                        }
                    }

                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
            }
        }

        /// <summary>
        /// ตัวอย่างที่ 5: Grid Trading Strategy
        /// </summary>
        public static async Task RunGridTradingAsync()
        {
            using var client = new BitkubClient(API_KEY, API_SECRET);

            decimal basePrice = 1200000;     // ราคาฐาน
            decimal gridSize = 10000;        // ช่องละ 10,000 บาท
            int numberOfGrids = 10;          // จำนวน 10 ช่อง
            decimal amountPerGrid = 500;     // ใช้เงิน 500 บาทต่อช่อง

            Console.WriteLine("=== Grid Trading Strategy ===");
            Console.WriteLine($"Base Price: {basePrice:N0} THB");
            Console.WriteLine($"Grid Size: {gridSize:N0} THB");
            Console.WriteLine($"Number of Grids: {numberOfGrids}");
            Console.WriteLine($"Amount per Grid: {amountPerGrid:N0} THB");
            Console.WriteLine();

            // วางคำสั่งซื้อหลายชั้น
            for (int i = 1; i <= numberOfGrids; i++)
            {
                decimal buyPrice = basePrice - (gridSize * i);
                decimal sellPrice = buyPrice + gridSize;

                Console.WriteLine($"Grid {i}: Buy @ {buyPrice:N0}, Sell @ {sellPrice:N0}");

                // ทดสอบคำสั่งก่อน
                var testBuy = await client.PlaceBidTestAsync("THB_BTC", amountPerGrid, buyPrice, "limit");
                if (testBuy.Error == 0)
                {
                    // สั่งซื้อจริง
                    // var realBuy = await client.PlaceBidAsync("THB_BTC", amountPerGrid, buyPrice, "limit");
                    Console.WriteLine($"  ✓ Test buy order successful");
                }

                await Task.Delay(1000); // รอ 1 วินาที ระหว่างคำสั่ง
            }

            Console.WriteLine("\nGrid orders placed successfully!");
        }

        /// <summary>
        /// ตัวอย่างที่ 6: Portfolio Rebalancing
        /// </summary>
        public static async Task RunPortfolioRebalancingAsync()
        {
            using var client = new BitkubClient(API_KEY, API_SECRET);

            // กำหนดสัดส่วนที่ต้องการ
            decimal targetBTCPercent = 50;  // 50% BTC
            decimal targetETHPercent = 30;  // 30% ETH
            decimal targetTHBPercent = 20;  // 20% THB (cash)

            Console.WriteLine("=== Portfolio Rebalancing ===");
            Console.WriteLine($"Target: {targetBTCPercent}% BTC, {targetETHPercent}% ETH, {targetTHBPercent}% THB");
            Console.WriteLine();

            // ดึงยอดเงินปัจจุบัน
            var balances = await client.GetBalancesAsync();
            var btcTicker = await client.GetTickerAsync("THB_BTC");
            var ethTicker = await client.GetTickerAsync("THB_ETH");

            decimal btcBalance = balances.Result.ContainsKey("BTC") ? balances.Result["BTC"].Available : 0;
            decimal ethBalance = balances.Result.ContainsKey("ETH") ? balances.Result["ETH"].Available : 0;
            decimal thbBalance = balances.Result.ContainsKey("THB") ? balances.Result["THB"].Available : 0;

            decimal btcPrice = btcTicker["THB_BTC"].Last;
            decimal ethPrice = ethTicker["THB_ETH"].Last;

            decimal btcValue = btcBalance * btcPrice;
            decimal ethValue = ethBalance * ethPrice;
            decimal totalValue = btcValue + ethValue + thbBalance;

            Console.WriteLine("Current Portfolio:");
            Console.WriteLine($"BTC: {btcBalance:N8} ({btcValue:N2} THB) = {(btcValue / totalValue * 100):N2}%");
            Console.WriteLine($"ETH: {ethBalance:N8} ({ethValue:N2} THB) = {(ethValue / totalValue * 100):N2}%");
            Console.WriteLine($"THB: {thbBalance:N2} = {(thbBalance / totalValue * 100):N2}%");
            Console.WriteLine($"Total: {totalValue:N2} THB");
            Console.WriteLine();

            // คำนวณว่าต้อง rebalance อย่างไร
            decimal targetBTCValue = totalValue * targetBTCPercent / 100;
            decimal targetETHValue = totalValue * targetETHPercent / 100;

            decimal btcDiff = targetBTCValue - btcValue;
            decimal ethDiff = targetETHValue - ethValue;

            Console.WriteLine("Rebalancing needed:");
            if (btcDiff > 0)
                Console.WriteLine($"Buy {Math.Abs(btcDiff):N2} THB worth of BTC");
            else if (btcDiff < 0)
                Console.WriteLine($"Sell {Math.Abs(btcDiff):N2} THB worth of BTC");

            if (ethDiff > 0)
                Console.WriteLine($"Buy {Math.Abs(ethDiff):N2} THB worth of ETH");
            else if (ethDiff < 0)
                Console.WriteLine($"Sell {Math.Abs(ethDiff):N2} THB worth of ETH");
        }

        /// <summary>
        /// Main entry point for advanced examples
        /// </summary>
        public static async Task Main(string[] args)
        {
            Console.WriteLine("=== Bitkub Advanced Trading Examples ===\n");
            Console.WriteLine("Select an example to run:");
            Console.WriteLine("1. Simple Trading Bot");
            Console.WriteLine("2. DCA Strategy");
            Console.WriteLine("3. Scalping Strategy");
            Console.WriteLine("4. Price Monitor");
            Console.WriteLine("5. Grid Trading");
            Console.WriteLine("6. Portfolio Rebalancing");
            Console.Write("\nYour choice (1-6): ");

            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await RunSimpleTradingBotAsync();
                        break;
                    case "2":
                        await RunDCAStrategyAsync();
                        break;
                    case "3":
                        await RunScalpingStrategyAsync();
                        break;
                    case "4":
                        await RunPriceMonitorAsync();
                        break;
                    case "5":
                        await RunGridTradingAsync();
                        break;
                    case "6":
                        await RunPortfolioRebalancingAsync();
                        break;
                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
