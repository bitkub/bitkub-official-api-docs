using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitkubTrader
{
    public class BitkubClient : IDisposable
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.bitkub.com";

        public BitkubClient(string apiKey, string apiSecret)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        #region Helper Methods

        private string JsonEncode(object data)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None
            });
        }

        private string Sign(string jsonPayload)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_apiSecret);
            var messageBytes = Encoding.UTF8.GetBytes(jsonPayload);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(messageBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private async Task<long> GetServerTimeAsync()
        {
            var response = await _httpClient.GetAsync("/api/servertime");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return long.Parse(content);
        }

        private async Task<T> PostSecureAsync<T>(string endpoint, Dictionary<string, object> parameters)
        {
            var timestamp = await GetServerTimeAsync();
            parameters["ts"] = timestamp;

            // Sort keys for consistent signature
            var sortedParams = new SortedDictionary<string, object>(parameters);
            var jsonPayload = JsonConvert.SerializeObject(sortedParams, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None
            });

            var signature = Sign(jsonPayload);
            sortedParams["sig"] = signature;

            var finalPayload = JsonConvert.SerializeObject(sortedParams, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None
            });

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(finalPayload, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("X-BTK-APIKEY", _apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(responseContent);

            if (result == null)
            {
                throw new Exception("Failed to deserialize response");
            }

            return result;
        }

        private async Task<T> GetAsync<T>(string endpoint, Dictionary<string, string>? queryParams = null)
        {
            var url = endpoint;
            if (queryParams != null && queryParams.Count > 0)
            {
                var queryString = string.Join("&", queryParams.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
                url += "?" + queryString;
            }

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(content);

            if (result == null)
            {
                throw new Exception("Failed to deserialize response");
            }

            return result;
        }

        #endregion

        #region Market Data (Public Endpoints)

        /// <summary>
        /// Get server timestamp
        /// </summary>
        public async Task<long> GetServerTimestampAsync()
        {
            return await GetServerTimeAsync();
        }

        /// <summary>
        /// Get all available symbols
        /// </summary>
        public async Task<SymbolsResponse> GetSymbolsAsync()
        {
            return await GetAsync<SymbolsResponse>("/api/market/symbols");
        }

        /// <summary>
        /// Get ticker information for a specific symbol or all symbols
        /// </summary>
        public async Task<Dictionary<string, TickerInfo>> GetTickerAsync(string? symbol = null)
        {
            var queryParams = symbol != null
                ? new Dictionary<string, string> { { "sym", symbol } }
                : null;

            return await GetAsync<Dictionary<string, TickerInfo>>("/api/market/ticker", queryParams);
        }

        /// <summary>
        /// Get market depth (orderbook)
        /// </summary>
        public async Task<DepthResponse> GetDepthAsync(string symbol, int limit = 10)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "sym", symbol },
                { "lmt", limit.ToString() }
            };

            return await GetAsync<DepthResponse>("/api/market/depth", queryParams);
        }

        #endregion

        #region Account (Secure Endpoints)

        /// <summary>
        /// Get available balances
        /// </summary>
        public async Task<WalletResponse> GetWalletAsync()
        {
            return await PostSecureAsync<WalletResponse>("/api/market/wallet", new Dictionary<string, object>());
        }

        /// <summary>
        /// Get balances with available and reserved amounts
        /// </summary>
        public async Task<BalancesResponse> GetBalancesAsync()
        {
            return await PostSecureAsync<BalancesResponse>("/api/market/balances", new Dictionary<string, object>());
        }

        #endregion

        #region Trading (Secure Endpoints)

        /// <summary>
        /// Place a buy order (bid)
        /// </summary>
        /// <param name="symbol">Trading symbol (e.g., THB_BTC)</param>
        /// <param name="amount">Amount in quote currency (THB) you want to spend</param>
        /// <param name="rate">Price rate (0 for market order)</param>
        /// <param name="type">Order type: "limit" or "market"</param>
        public async Task<PlaceOrderResponse> PlaceBidAsync(string symbol, decimal amount, decimal rate, string type = "limit")
        {
            var parameters = new Dictionary<string, object>
            {
                { "sym", symbol },
                { "amt", amount },
                { "rat", rate },
                { "typ", type }
            };

            return await PostSecureAsync<PlaceOrderResponse>("/api/market/place-bid", parameters);
        }

        /// <summary>
        /// Test place a buy order (no balance deducted)
        /// </summary>
        public async Task<PlaceOrderResponse> PlaceBidTestAsync(string symbol, decimal amount, decimal rate, string type = "limit")
        {
            var parameters = new Dictionary<string, object>
            {
                { "sym", symbol },
                { "amt", amount },
                { "rat", rate },
                { "typ", type }
            };

            return await PostSecureAsync<PlaceOrderResponse>("/api/market/place-bid/test", parameters);
        }

        /// <summary>
        /// Place a sell order (ask)
        /// </summary>
        /// <param name="symbol">Trading symbol (e.g., THB_BTC)</param>
        /// <param name="amount">Amount of cryptocurrency you want to sell</param>
        /// <param name="rate">Price rate (0 for market order)</param>
        /// <param name="type">Order type: "limit" or "market"</param>
        public async Task<PlaceOrderResponse> PlaceAskAsync(string symbol, decimal amount, decimal rate, string type = "limit")
        {
            var parameters = new Dictionary<string, object>
            {
                { "sym", symbol },
                { "amt", amount },
                { "rat", rate },
                { "typ", type }
            };

            return await PostSecureAsync<PlaceOrderResponse>("/api/market/place-ask", parameters);
        }

        /// <summary>
        /// Test place a sell order (no balance deducted)
        /// </summary>
        public async Task<PlaceOrderResponse> PlaceAskTestAsync(string symbol, decimal amount, decimal rate, string type = "limit")
        {
            var parameters = new Dictionary<string, object>
            {
                { "sym", symbol },
                { "amt", amount },
                { "rat", rate },
                { "typ", type }
            };

            return await PostSecureAsync<PlaceOrderResponse>("/api/market/place-ask/test", parameters);
        }

        /// <summary>
        /// Cancel an open order
        /// </summary>
        /// <param name="symbol">Trading symbol</param>
        /// <param name="orderId">Order ID</param>
        /// <param name="side">Order side: "buy" or "sell"</param>
        public async Task<CancelOrderResponse> CancelOrderAsync(string symbol, long orderId, string side)
        {
            var parameters = new Dictionary<string, object>
            {
                { "sym", symbol },
                { "id", orderId },
                { "sd", side }
            };

            return await PostSecureAsync<CancelOrderResponse>("/api/market/cancel-order", parameters);
        }

        /// <summary>
        /// Cancel an order by hash
        /// </summary>
        public async Task<CancelOrderResponse> CancelOrderByHashAsync(string orderHash)
        {
            var parameters = new Dictionary<string, object>
            {
                { "hash", orderHash }
            };

            return await PostSecureAsync<CancelOrderResponse>("/api/market/cancel-order", parameters);
        }

        /// <summary>
        /// Get all open orders for a symbol
        /// </summary>
        public async Task<OpenOrdersResponse> GetOpenOrdersAsync(string symbol)
        {
            var parameters = new Dictionary<string, object>
            {
                { "sym", symbol }
            };

            return await PostSecureAsync<OpenOrdersResponse>("/api/market/my-open-orders", parameters);
        }

        /// <summary>
        /// Get order history (matched orders)
        /// </summary>
        /// <param name="symbol">Trading symbol</param>
        /// <param name="page">Page number</param>
        /// <param name="limit">Results per page</param>
        /// <param name="start">Start timestamp (optional)</param>
        /// <param name="end">End timestamp (optional)</param>
        public async Task<OrderHistoryResponse> GetOrderHistoryAsync(
            string symbol,
            int? page = null,
            int? limit = null,
            long? start = null,
            long? end = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "sym", symbol }
            };

            if (page.HasValue) parameters["p"] = page.Value;
            if (limit.HasValue) parameters["lmt"] = limit.Value;
            if (start.HasValue) parameters["start"] = start.Value;
            if (end.HasValue) parameters["end"] = end.Value;

            return await PostSecureAsync<OrderHistoryResponse>("/api/market/my-order-history", parameters);
        }

        #endregion

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
