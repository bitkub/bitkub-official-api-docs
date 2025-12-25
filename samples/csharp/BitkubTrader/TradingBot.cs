using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BitkubTrader
{
    /// <summary>
    /// ตัวอย่าง Trading Bot แบบง่ายพร้อม Strategy
    /// </summary>
    public class TradingBot
    {
        private readonly BitkubClient _client;
        private readonly string _symbol;
        private readonly decimal _orderAmount;
        private readonly decimal _profitPercent;
        private readonly decimal _stopLossPercent;
        private bool _isRunning;

        public TradingBot(
            BitkubClient client,
            string symbol = "THB_BTC",
            decimal orderAmount = 1000,
            decimal profitPercent = 1.5m,
            decimal stopLossPercent = 2.0m)
        {
            _client = client;
            _symbol = symbol;
            _orderAmount = orderAmount;
            _profitPercent = profitPercent;
            _stopLossPercent = stopLossPercent;
        }

        /// <summary>
        /// เริ่มต้น Trading Bot
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _isRunning = true;
            Console.WriteLine($"Starting Trading Bot for {_symbol}...");
            Console.WriteLine($"Order Amount: {_orderAmount} THB");
            Console.WriteLine($"Profit Target: {_profitPercent}%");
            Console.WriteLine($"Stop Loss: {_stopLossPercent}%");
            Console.WriteLine();

            while (_isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await RunStrategyAsync();
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                }
            }

            Console.WriteLine("Trading Bot stopped.");
        }

        /// <summary>
        /// หยุด Trading Bot
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
        }

        /// <summary>
        /// กลยุทธ์การเทรดหลัก
        /// </summary>
        private async Task RunStrategyAsync()
        {
            // 1. ดึงข้อมูลราคาปัจจุบัน
            var ticker = await _client.GetTickerAsync(_symbol);
            if (!ticker.TryGetValue(_symbol, out var tickerInfo))
            {
                Console.WriteLine("Failed to get ticker information");
                return;
            }

            decimal currentPrice = tickerInfo.Last;
            decimal highestBid = tickerInfo.HighestBid;
            decimal lowestAsk = tickerInfo.LowestAsk;

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Current: {currentPrice:N2} | Bid: {highestBid:N2} | Ask: {lowestAsk:N2} | Change: {tickerInfo.PercentChange:N2}%");

            // 2. ตรวจสอบคำสั่งที่เปิดอยู่
            var openOrders = await _client.GetOpenOrdersAsync(_symbol);
            if (openOrders.Error != 0)
            {
                Console.WriteLine($"Error getting open orders: {openOrders.Error}");
                return;
            }

            // 3. จัดการคำสั่งที่เปิดอยู่
            foreach (var order in openOrders.Result)
            {
                await ManageOpenOrderAsync(order, currentPrice);
            }

            // 4. ถ้าไม่มีคำสั่งเปิดอยู่ ให้สร้างคำสั่งใหม่
            if (openOrders.Result.Count == 0)
            {
                await CreateNewOrderAsync(currentPrice, highestBid, lowestAsk);
            }
        }

        /// <summary>
        /// จัดการคำสั่งที่เปิดอยู่
        /// </summary>
        private async Task ManageOpenOrderAsync(OpenOrder order, decimal currentPrice)
        {
            if (order.Side == "BUY")
            {
                // ถ้าเป็นคำสั่งซื้อที่รอดำเนินการ
                // ตรวจสอบว่าราคาเปลี่ยนแปลงมากเกินไปหรือไม่ ควรยกเลิกและสร้างใหม่
                decimal priceDiff = Math.Abs((currentPrice - order.Rate) / order.Rate * 100);
                if (priceDiff > 2) // ถ้าราคาต่างกันเกิน 2%
                {
                    Console.WriteLine($"Cancelling outdated BUY order #{order.Id} (price moved {priceDiff:N2}%)");
                    await _client.CancelOrderAsync(_symbol, order.Id, "buy");
                }
            }
            else if (order.Side == "SELL")
            {
                // ถ้าเป็นคำสั่งขายที่รอดำเนินการ
                // ตรวจสอบ Stop Loss
                decimal lossPct = (order.Rate - currentPrice) / order.Rate * 100;
                if (lossPct > _stopLossPercent)
                {
                    Console.WriteLine($"⚠️ Stop Loss triggered! Cancelling SELL order #{order.Id}");
                    await _client.CancelOrderAsync(_symbol, order.Id, "sell");
                }
            }
        }

        /// <summary>
        /// สร้างคำสั่งใหม่ตาม Strategy
        /// </summary>
        private async Task CreateNewOrderAsync(decimal currentPrice, decimal highestBid, decimal lowestAsk)
        {
            // Strategy: Simple Market Making
            // วาง buy order ต่ำกว่าราคาตลาดเล็กน้อย
            decimal buyPrice = Math.Floor(highestBid - 100); // ต่ำกว่า highest bid 100 บาท

            Console.WriteLine($"📊 Creating new BUY order at {buyPrice:N2} THB");

            // ทดสอบคำสั่งก่อน
            var testResult = await _client.PlaceBidTestAsync(_symbol, _orderAmount, buyPrice, "limit");
            if (testResult.Error != 0)
            {
                Console.WriteLine($"❌ Test order failed with error: {testResult.Error}");
                return;
            }

            // ถ้าทดสอบผ่าน ให้สั่งจริง
            var result = await _client.PlaceBidAsync(_symbol, _orderAmount, buyPrice, "limit");
            if (result.Error == 0 && result.Result != null)
            {
                Console.WriteLine($"✅ BUY order placed: #{result.Result.Id}");
                Console.WriteLine($"   Amount: {_orderAmount} THB");
                Console.WriteLine($"   Rate: {buyPrice:N2} THB");
                Console.WriteLine($"   Will receive: {result.Result.Receive:N8} BTC");

                // คำนวณราคาขายที่ต้องการกำไร
                decimal sellPrice = Math.Ceiling(buyPrice * (1 + _profitPercent / 100));
                Console.WriteLine($"   Target sell price: {sellPrice:N2} THB (profit: {_profitPercent}%)");
            }
            else
            {
                Console.WriteLine($"❌ Failed to place order. Error: {result.Error}");
            }
        }

        /// <summary>
        /// Strategy: Scalping - เทรดระยะสั้นเพื่อหากำไรเล็กน้อยบ่อยๆ
        /// </summary>
        public async Task ScalpingStrategyAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting Scalping Strategy...");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var ticker = await _client.GetTickerAsync(_symbol);
                    if (!ticker.TryGetValue(_symbol, out var info))
                        continue;

                    // ถ้า spread (ส่วนต่างระหว่าง ask และ bid) กว้างพอ
                    decimal spread = info.LowestAsk - info.HighestBid;
                    decimal spreadPercent = (spread / info.Last) * 100;

                    Console.WriteLine($"Spread: {spread:N2} ({spreadPercent:N4}%)");

                    if (spreadPercent > 0.5m) // ถ้า spread มากกว่า 0.5%
                    {
                        // วาง buy order ที่ highest bid + 1
                        decimal buyPrice = info.HighestBid + 1;
                        // วาง sell order ที่ lowest ask - 1
                        decimal sellPrice = info.LowestAsk - 1;

                        Console.WriteLine($"Placing scalping orders: Buy @ {buyPrice:N2}, Sell @ {sellPrice:N2}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Scalping error: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                }
            }
        }

        /// <summary>
        /// Strategy: DCA (Dollar Cost Averaging) - ซื้อเป็นงวดๆ ตามกำหนด
        /// </summary>
        public async Task DCAStrategyAsync(decimal totalAmount, int intervals, TimeSpan interval, CancellationToken cancellationToken)
        {
            decimal amountPerInterval = totalAmount / intervals;
            Console.WriteLine($"Starting DCA Strategy: {totalAmount} THB in {intervals} intervals of {interval.TotalMinutes} minutes");
            Console.WriteLine($"Amount per interval: {amountPerInterval:N2} THB");

            for (int i = 0; i < intervals && !cancellationToken.IsCancellationRequested; i++)
            {
                try
                {
                    Console.WriteLine($"\n[Interval {i + 1}/{intervals}]");

                    // ซื้อที่ราคาตลาด
                    var result = await _client.PlaceBidAsync(_symbol, amountPerInterval, 0, "market");
                    if (result.Error == 0 && result.Result != null)
                    {
                        Console.WriteLine($"✅ Bought {result.Result.Receive:N8} BTC for {amountPerInterval:N2} THB");
                    }
                    else
                    {
                        Console.WriteLine($"❌ Purchase failed: Error {result.Error}");
                    }

                    if (i < intervals - 1) // ไม่รอในรอบสุดท้าย
                    {
                        Console.WriteLine($"Waiting {interval.TotalMinutes} minutes for next interval...");
                        await Task.Delay(interval, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DCA error: {ex.Message}");
                }
            }

            Console.WriteLine("\nDCA Strategy completed!");
        }

        /// <summary>
        /// แสดงสรุปผลการเทรด
        /// </summary>
        public async Task ShowTradingSummaryAsync(DateTime since)
        {
            var history = await _client.GetOrderHistoryAsync(
                _symbol,
                start: new DateTimeOffset(since).ToUnixTimeSeconds(),
                end: DateTimeOffset.Now.ToUnixTimeSeconds()
            );

            if (history.Error != 0)
            {
                Console.WriteLine($"Error getting history: {history.Error}");
                return;
            }

            var buyOrders = history.Result.Where(o => o.Side == "buy").ToList();
            var sellOrders = history.Result.Where(o => o.Side == "sell").ToList();

            decimal totalBuyAmount = buyOrders.Sum(o => o.Amount * o.Rate);
            decimal totalSellAmount = sellOrders.Sum(o => o.Amount * o.Rate);
            decimal totalFees = history.Result.Sum(o => o.Fee);

            Console.WriteLine("\n=== Trading Summary ===");
            Console.WriteLine($"Period: {since:yyyy-MM-dd HH:mm} to {DateTime.Now:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"Total Orders: {history.Result.Count}");
            Console.WriteLine($"Buy Orders: {buyOrders.Count} (Total: {totalBuyAmount:N2} THB)");
            Console.WriteLine($"Sell Orders: {sellOrders.Count} (Total: {totalSellAmount:N2} THB)");
            Console.WriteLine($"Total Fees: {totalFees:N2} THB");
            Console.WriteLine($"Net P/L: {(totalSellAmount - totalBuyAmount - totalFees):N2} THB");
            Console.WriteLine("====================\n");
        }
    }
}
