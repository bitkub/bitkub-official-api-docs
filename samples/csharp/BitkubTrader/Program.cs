using System;
using System.Linq;
using System.Threading.Tasks;

namespace BitkubTrader
{
    class Program
    {
        // Replace these with your actual API credentials
        private const string API_KEY = "YOUR_API_KEY";
        private const string API_SECRET = "YOUR_API_SECRET";

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Bitkub Trading Bot Demo ===\n");

            using var client = new BitkubClient(API_KEY, API_SECRET);

            try
            {
                // 1. Get Server Time
                Console.WriteLine("1. Getting server time...");
                var serverTime = await client.GetServerTimestampAsync();
                var dateTime = DateTimeOffset.FromUnixTimeSeconds(serverTime).DateTime;
                Console.WriteLine($"   Server Time: {serverTime} ({dateTime})\n");

                // 2. Get Available Symbols
                Console.WriteLine("2. Getting available symbols...");
                var symbols = await client.GetSymbolsAsync();
                Console.WriteLine($"   Found {symbols.Result.Count} symbols:");
                foreach (var symbol in symbols.Result.Take(5))
                {
                    Console.WriteLine($"   - {symbol.Symbol}: {symbol.Info}");
                }
                Console.WriteLine();

                // 3. Get Ticker Information
                Console.WriteLine("3. Getting ticker for THB_BTC...");
                var ticker = await client.GetTickerAsync("THB_BTC");
                if (ticker.TryGetValue("THB_BTC", out var btcTicker))
                {
                    Console.WriteLine($"   Last Price: {btcTicker.Last:N2} THB");
                    Console.WriteLine($"   24h High: {btcTicker.High24Hr:N2} THB");
                    Console.WriteLine($"   24h Low: {btcTicker.Low24Hr:N2} THB");
                    Console.WriteLine($"   24h Volume: {btcTicker.BaseVolume:N8} BTC");
                    Console.WriteLine($"   24h Change: {btcTicker.PercentChange:N2}%");
                    Console.WriteLine($"   Highest Bid: {btcTicker.HighestBid:N2} THB");
                    Console.WriteLine($"   Lowest Ask: {btcTicker.LowestAsk:N2} THB");
                }
                Console.WriteLine();

                // 4. Get Market Depth (Order Book)
                Console.WriteLine("4. Getting market depth for THB_BTC...");
                var depth = await client.GetDepthAsync("THB_BTC", 5);
                Console.WriteLine("   Top 5 Bids (Buy Orders):");
                foreach (var bid in depth.Bids.Take(5))
                {
                    Console.WriteLine($"   - Price: {bid[0]:N2} THB, Amount: {bid[1]:N8} BTC");
                }
                Console.WriteLine("\n   Top 5 Asks (Sell Orders):");
                foreach (var ask in depth.Asks.Take(5))
                {
                    Console.WriteLine($"   - Price: {ask[0]:N2} THB, Amount: {ask[1]:N8} BTC");
                }
                Console.WriteLine();

                // 5. Get Account Balances
                Console.WriteLine("5. Getting account balances...");
                var balances = await client.GetBalancesAsync();
                if (balances.Error == 0)
                {
                    Console.WriteLine("   Your Balances:");
                    foreach (var balance in balances.Result)
                    {
                        if (balance.Value.Available > 0 || balance.Value.Reserved > 0)
                        {
                            Console.WriteLine($"   - {balance.Key}:");
                            Console.WriteLine($"     Available: {balance.Value.Available:N8}");
                            Console.WriteLine($"     Reserved: {balance.Value.Reserved:N8}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"   Error getting balances: {balances.Error}");
                }
                Console.WriteLine();

                // 6. Get Open Orders
                Console.WriteLine("6. Getting open orders for THB_BTC...");
                var openOrders = await client.GetOpenOrdersAsync("THB_BTC");
                if (openOrders.Error == 0)
                {
                    Console.WriteLine($"   You have {openOrders.Result.Count} open orders");
                    foreach (var order in openOrders.Result)
                    {
                        Console.WriteLine($"   - Order #{order.Id} ({order.Side}):");
                        Console.WriteLine($"     Type: {order.Type}");
                        Console.WriteLine($"     Rate: {order.Rate:N2} THB");
                        Console.WriteLine($"     Amount: {order.Amount:N8}");
                        Console.WriteLine($"     Hash: {order.Hash}");
                    }
                }
                else
                {
                    Console.WriteLine($"   Error getting open orders: {openOrders.Error}");
                }
                Console.WriteLine();

                // 7. Get Order History
                Console.WriteLine("7. Getting order history for THB_BTC...");
                var history = await client.GetOrderHistoryAsync("THB_BTC", page: 1, limit: 5);
                if (history.Error == 0)
                {
                    Console.WriteLine($"   Found {history.Result.Count} recent orders:");
                    foreach (var order in history.Result)
                    {
                        var dt = DateTimeOffset.FromUnixTimeSeconds(order.Timestamp).DateTime;
                        Console.WriteLine($"   - {order.TxnId} ({order.Side}):");
                        Console.WriteLine($"     Rate: {order.Rate:N2} THB");
                        Console.WriteLine($"     Amount: {order.Amount:N8}");
                        Console.WriteLine($"     Fee: {order.Fee:N2}");
                        Console.WriteLine($"     Time: {dt}");
                    }
                }
                else
                {
                    Console.WriteLine($"   Error getting order history: {history.Error}");
                }
                Console.WriteLine();

                // 8. Test Place Buy Order (TEST MODE - no real transaction)
                Console.WriteLine("8. Testing place BUY order (no real transaction)...");
                Console.WriteLine("   This is a TEST order - no balance will be deducted");
                var testBid = await client.PlaceBidTestAsync(
                    symbol: "THB_BTC",
                    amount: 100,      // 100 THB
                    rate: 1000000,    // at 1,000,000 THB per BTC
                    type: "limit"
                );
                if (testBid.Error == 0 && testBid.Result != null)
                {
                    Console.WriteLine($"   TEST Order Result:");
                    Console.WriteLine($"   - Order ID: {testBid.Result.Id}");
                    Console.WriteLine($"   - Hash: {testBid.Result.Hash}");
                    Console.WriteLine($"   - Type: {testBid.Result.Type}");
                    Console.WriteLine($"   - Spending: {testBid.Result.Amount:N2} THB");
                    Console.WriteLine($"   - Rate: {testBid.Result.Rate:N2} THB");
                    Console.WriteLine($"   - Will Receive: {testBid.Result.Receive:N8} BTC");
                    Console.WriteLine($"   - Fee: {testBid.Result.Fee:N2} THB");
                }
                else
                {
                    Console.WriteLine($"   Error: {testBid.Error}");
                }
                Console.WriteLine();

                // 9. Test Place Sell Order (TEST MODE - no real transaction)
                Console.WriteLine("9. Testing place SELL order (no real transaction)...");
                Console.WriteLine("   This is a TEST order - no balance will be deducted");
                var testAsk = await client.PlaceAskTestAsync(
                    symbol: "THB_BTC",
                    amount: 0.001m,   // 0.001 BTC
                    rate: 1000000,    // at 1,000,000 THB per BTC
                    type: "limit"
                );
                if (testAsk.Error == 0 && testAsk.Result != null)
                {
                    Console.WriteLine($"   TEST Order Result:");
                    Console.WriteLine($"   - Order ID: {testAsk.Result.Id}");
                    Console.WriteLine($"   - Hash: {testAsk.Result.Hash}");
                    Console.WriteLine($"   - Type: {testAsk.Result.Type}");
                    Console.WriteLine($"   - Selling: {testAsk.Result.Amount:N8} BTC");
                    Console.WriteLine($"   - Rate: {testAsk.Result.Rate:N2} THB");
                    Console.WriteLine($"   - Will Receive: {testAsk.Result.Receive:N2} THB");
                    Console.WriteLine($"   - Fee: {testAsk.Result.Fee:N2} THB");
                }
                else
                {
                    Console.WriteLine($"   Error: {testAsk.Error}");
                }
                Console.WriteLine();

                Console.WriteLine("=== Demo Complete ===");
                Console.WriteLine("\nNote: To place REAL orders, use PlaceBidAsync() and PlaceAskAsync() instead of test methods.");
                Console.WriteLine("Warning: Real orders will deduct your balance. Please be careful!");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        // Example: Simple Trading Strategy
        static async Task SimpleScalpingStrategy(BitkubClient client, string symbol)
        {
            Console.WriteLine($"Starting scalping strategy for {symbol}...");

            // Get current ticker
            var ticker = await client.GetTickerAsync(symbol);
            if (!ticker.TryGetValue(symbol, out var tickerInfo))
            {
                Console.WriteLine("Failed to get ticker info");
                return;
            }

            // Get balances
            var balances = await client.GetBalancesAsync();
            if (balances.Error != 0)
            {
                Console.WriteLine("Failed to get balances");
                return;
            }

            // Simple strategy: Buy when price drops 1%, sell when it rises 1%
            decimal buyPrice = tickerInfo.Last * 0.99m;  // 1% below current price
            decimal sellPrice = tickerInfo.Last * 1.01m; // 1% above current price

            Console.WriteLine($"Current Price: {tickerInfo.Last:N2}");
            Console.WriteLine($"Buy Target: {buyPrice:N2}");
            Console.WriteLine($"Sell Target: {sellPrice:N2}");

            // Place buy order (limit order)
            var buyOrder = await client.PlaceBidTestAsync(symbol, 1000, buyPrice, "limit");
            if (buyOrder.Error == 0 && buyOrder.Result != null)
            {
                Console.WriteLine($"Buy order placed: {buyOrder.Result.Hash}");
            }

            // Monitor and execute strategy...
            // (In a real bot, you would continuously monitor prices and manage orders)
        }
    }
}
