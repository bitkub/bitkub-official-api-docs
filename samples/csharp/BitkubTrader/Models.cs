using Newtonsoft.Json;
using System.Collections.Generic;

namespace BitkubTrader
{
    // Market Data Models
    public class SymbolInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonProperty("info")]
        public string Info { get; set; } = string.Empty;
    }

    public class SymbolsResponse
    {
        [JsonProperty("error")]
        public int Error { get; set; }

        [JsonProperty("result")]
        public List<SymbolInfo> Result { get; set; } = new();
    }

    public class TickerInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("last")]
        public decimal Last { get; set; }

        [JsonProperty("lowestAsk")]
        public decimal LowestAsk { get; set; }

        [JsonProperty("highestBid")]
        public decimal HighestBid { get; set; }

        [JsonProperty("percentChange")]
        public decimal PercentChange { get; set; }

        [JsonProperty("baseVolume")]
        public decimal BaseVolume { get; set; }

        [JsonProperty("quoteVolume")]
        public decimal QuoteVolume { get; set; }

        [JsonProperty("isFrozen")]
        public int IsFrozen { get; set; }

        [JsonProperty("high24hr")]
        public decimal High24Hr { get; set; }

        [JsonProperty("low24hr")]
        public decimal Low24Hr { get; set; }
    }

    public class BalanceInfo
    {
        [JsonProperty("available")]
        public decimal Available { get; set; }

        [JsonProperty("reserved")]
        public decimal Reserved { get; set; }
    }

    public class BalancesResponse
    {
        [JsonProperty("error")]
        public int Error { get; set; }

        [JsonProperty("result")]
        public Dictionary<string, BalanceInfo> Result { get; set; } = new();
    }

    public class WalletResponse
    {
        [JsonProperty("error")]
        public int Error { get; set; }

        [JsonProperty("result")]
        public Dictionary<string, decimal> Result { get; set; } = new();
    }

    // Order Models
    public class OrderResult
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; } = string.Empty;

        [JsonProperty("typ")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("amt")]
        public decimal Amount { get; set; }

        [JsonProperty("rat")]
        public decimal Rate { get; set; }

        [JsonProperty("fee")]
        public decimal Fee { get; set; }

        [JsonProperty("cre")]
        public decimal Credit { get; set; }

        [JsonProperty("rec")]
        public decimal Receive { get; set; }

        [JsonProperty("ts")]
        public long Timestamp { get; set; }
    }

    public class PlaceOrderResponse
    {
        [JsonProperty("error")]
        public int Error { get; set; }

        [JsonProperty("result")]
        public OrderResult? Result { get; set; }
    }

    public class OpenOrder
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; } = string.Empty;

        [JsonProperty("side")]
        public string Side { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("fee")]
        public decimal Fee { get; set; }

        [JsonProperty("credit")]
        public decimal Credit { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("receive")]
        public decimal Receive { get; set; }

        [JsonProperty("parent_id")]
        public long ParentId { get; set; }

        [JsonProperty("super_id")]
        public long SuperId { get; set; }

        [JsonProperty("ts")]
        public long Timestamp { get; set; }
    }

    public class OpenOrdersResponse
    {
        [JsonProperty("error")]
        public int Error { get; set; }

        [JsonProperty("result")]
        public List<OpenOrder> Result { get; set; } = new();
    }

    public class OrderHistory
    {
        [JsonProperty("txn_id")]
        public string TxnId { get; set; } = string.Empty;

        [JsonProperty("order_id")]
        public long OrderId { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; } = string.Empty;

        [JsonProperty("parent_order_id")]
        public long ParentOrderId { get; set; }

        [JsonProperty("super_order_id")]
        public long SuperOrderId { get; set; }

        [JsonProperty("taken_by_me")]
        public bool TakenByMe { get; set; }

        [JsonProperty("is_maker")]
        public bool IsMaker { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("fee")]
        public decimal Fee { get; set; }

        [JsonProperty("credit")]
        public decimal Credit { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("ts")]
        public long Timestamp { get; set; }
    }

    public class Pagination
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("last")]
        public int Last { get; set; }

        [JsonProperty("next")]
        public int Next { get; set; }

        [JsonProperty("prev")]
        public int Prev { get; set; }
    }

    public class OrderHistoryResponse
    {
        [JsonProperty("error")]
        public int Error { get; set; }

        [JsonProperty("result")]
        public List<OrderHistory> Result { get; set; } = new();

        [JsonProperty("pagination")]
        public Pagination? Pagination { get; set; }
    }

    public class CancelOrderResponse
    {
        [JsonProperty("error")]
        public int Error { get; set; }
    }

    public class OrderBook
    {
        [JsonProperty("bids")]
        public List<List<decimal>> Bids { get; set; } = new();

        [JsonProperty("asks")]
        public List<List<decimal>> Asks { get; set; } = new();
    }

    public class DepthResponse
    {
        [JsonProperty("bids")]
        public List<List<decimal>> Bids { get; set; } = new();

        [JsonProperty("asks")]
        public List<List<decimal>> Asks { get; set; } = new();
    }
}
