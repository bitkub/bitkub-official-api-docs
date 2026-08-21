# WebSocket API — Public (2023-04-19)

## Change Log

* 2026-05-18 `market.trade.<symbol>` stream will be permanently closed on 2026-05-18
* 2023-04-19 market.trade fields `bid` and `sid` changed type from Integer to String
* 2023-01-16 Live Order Book updated with new event info
* 2022-08-31 Deprecated authentication requirement for Live Order Book

## Overview

The Public WebSocket API provides real-time market data streams for all users without authentication.

## Endpoint

| Environment | WebSocket URL |
| ----------- | ------------- |
| Production  | `wss://api.bitkub.com/websocket-api/<streamName>` |

## Getting Started

### Connection Requirements

- No authentication required
- Stream name format: `<serviceName>.<serviceType>.<symbol>` (case-insensitive)
- Multiple streams: combine with comma `,` e.g. `market.trade.thb_btc,market.ticker.thb_btc`
- Symbol IDs for orderbook: use numeric ID from GET /api/v3/market/symbols

### Authentication Flow

N/A — Public streams do not require authentication.

### Subscription Management

N/A — Connect directly to the stream URL. No subscribe/unsubscribe events needed.

### Keep-Alive

No explicit ping required. Reconnect on disconnect.

<br>

---

<br>

## Data Streams

### Trade Stream (DEPRECATED - closes 2026-05-18)

**⚠️ This stream will be PERMANENTLY CLOSED on 2026-05-18. Migrate to Private WebSocket.**

#### Name:
market.trade.\<symbol\>

#### Description:
Real-time matched order data. Each trade contains buy order id and sell order id. As of 2023-04-19, fields `bid` and `sid` changed type from Integer to String.

#### Response:
```json
{
  "stream": "market.trade.thb_eth",
  "sym": "THB_ETH",
  "txn": "ETHSELL0000074282",
  "rat": "5977.00",
  "amt": 1.556539,
  "ts": 1542268567
}
```

#### Field Descriptions:

| Field  | Type   | Description                             |
| ------ | ------ | --------------------------------------- |
| stream | string | Stream name                             |
| sym    | string | Symbol                                  |
| txn    | string | Transaction ID                          |
| rat    | string | Rate matched (price)                    |
| amt    | float  | Amount matched                          |
| ts     | int    | Trade timestamp (Unix seconds)          |

<br>

---

<br>

### Ticker Stream

#### Name:
market.ticker.\<symbol\>

#### Description:
Real-time ticker data. Re-calculated on every order creation, cancellation, and fulfillment.

#### Response:
```json
{
  "stream": "market.ticker.thb_btc",
  "id": 1,
  "last": 2883194.85,
  "lowestAsk": 2883194.9,
  "lowestAskSize": 0.0070947,
  "highestBid": 2881000.31,
  "highestBidSize": 0.00470253,
  "change": 60622.33,
  "percentChange": 2.15,
  "baseVolume": 89.25334259,
  "quoteVolume": 256768588.16,
  "isFrozen": 0,
  "high24hr": 2916959.99,
  "low24hr": 2819009.05,
  "open": 2822572.52,
  "close": 2883194.85
}
```

#### Field Descriptions:

| Field          | Type   | Description                              |
| -------------- | ------ | ---------------------------------------- |
| stream         | string | Stream name                              |
| id             | int    | Symbol ID                                |
| last           | float  | Latest price                             |
| lowestAsk      | float  | Lowest asking price                      |
| lowestAskSize  | float  | Amount of the lowest asking order        |
| highestBid     | float  | Highest bidding price                    |
| highestBidSize | float  | Amount of the highest bidding order      |
| change         | float  | Price change compared to open            |
| percentChange  | float  | Price change in percent                  |
| baseVolume     | float  | Amount of crypto traded in 24h           |
| quoteVolume    | float  | Amount of fiat traded in 24h             |
| isFrozen       | int    | Symbol trade status (0 = active)         |
| high24hr       | float  | Highest price in last 24 hours           |
| low24hr        | float  | Lowest price in last 24 hours            |
| open           | float  | Open price                               |
| close          | float  | Close price                              |

<br>

---

<br>

### Live Order Book Stream

#### Name:
orderbook.\<symbol-id\>

#### Description:
Real-time order book data using numeric symbol ID. Emits 5 event types: `bidschanged`, `askschanged`, `tradeschanged`, `depthchanged`, and `global.ticker`. bidschanged/askschanged fire when a buy/sell order is opened, closed, or cancelled (max 30 orders each); tradeschanged fires on matched trades and is also sent as initial data on connect (max 30 each); depthchanged fires when order book depth changes; ticker/global.ticker aggregate the above per-symbol or across all symbols respectively.

#### Response (bidschanged / askschanged):
```json
{
  "data": [
    [121.82, 112510.1, 0.00108283, 0, false, false]
  ],
  "event": "bidschanged",
  "pairing_id": 1
}
```

#### Response (tradeschanged):
```json
{
  "data": [
    [[1550320593, 113587, 0.12817027, "BUY", 0, 0, true, false, false]],
    [[121.82, 112510.1, 0.00108283, 0, false, false]],
    [[51247.13, 113699, 0.45072632, 0, false, false]]
  ],
  "event": "tradeschanged",
  "pairing_id": 1
}
```

#### Response (depthchanged):
```json
{
  "data": {
    "bids": [
      { "price": 2466650.35, "base_volume": 0.0002027, "quote_volume": 500 },
      { "price": 2466650.33, "base_volume": 0.00299999, "quote_volume": 7399.93 }
    ],
    "asks": [
      { "price": 2467772.05, "base_volume": 0.003, "quote_volume": 7403.32 },
      { "price": 2468266.95, "base_volume": 0.03140643, "quote_volume": 77519.46 }
    ]
  },
  "event": "depthchanged",
  "pairing_id": 1
}
```

#### Response (global.ticker):
```json
{
  "data": {
    "id": 1,
    "last": "1500000.00",
    "percentChange": "2.45",
    "baseVolume": "123.456",
    "quoteVolume": "185184000.00",
    "high24hr": "1520000.00",
    "low24hr": "1460000.00",
    "highestBid": "1499900.00",
    "lowestAsk": "1500100.00"
  },
  "event": "global.ticker"
}
```

#### Field Descriptions (bidschanged / askschanged):

| Field       | Type    | Description                |
| ----------- | ------- | -------------------------- |
| data[0][0]  | float   | Volume                     |
| data[0][1]  | float   | Rate (price)               |
| data[0][2]  | float   | Amount                     |
| data[0][3]  | int     | Reserved (always 0)        |
| data[0][4]  | boolean | Is new order                |
| data[0][5]  | boolean | User is owner (deprecated) |

#### Field Descriptions (tradeschanged):

| Field                    | Type    | Description                                   |
| ------------------------ | ------- | ---------------------------------------------- |
| data[0][0]               | int     | Timestamp                                      |
| data[0][1]               | float   | Rate (price)                                   |
| data[0][2]               | float   | Amount                                         |
| data[0][3]               | string  | Side: BUY or SELL                              |
| data[0][4]               | int     | Reserved (always 0)                            |
| data[0][5]               | int     | Reserved (always 0)                            |
| data[0][6]               | boolean | Is new order                                   |
| data[0][7]               | boolean | User is buyer (available when authenticated)   |
| data[0][8]               | boolean | User is seller (available when authenticated)  |
| data[1][0] / data[2][0]  | float   | Volume                                         |
| data[1][1] / data[2][1]  | float   | Rate (price)                                   |
| data[1][2] / data[2][2]  | float   | Amount                                         |
| data[1][3] / data[2][3]  | int     | Reserved (always 0)                            |
| data[1][4] / data[2][4]  | boolean | Is new order                                   |
| data[1][5] / data[2][5]  | boolean | User is owner (available when authenticated)   |

#### Field Descriptions (depthchanged):

| Field        | Type  | Description                    |
| ------------ | ----- | ------------------------------- |
| price        | float | Price                            |
| base_volume  | float | Amount in base currency          |
| quote_volume | float | Amount in quote currency         |

#### Field Descriptions (global.ticker):

| Field          | Type   | Description                         |
| -------------- | ------ | ----------------------------------- |
| id             | int    | Symbol ID                           |
| last           | string | Latest price                        |
| percentChange  | string | Price difference in percent         |
| baseVolume     | string | Amount of crypto                    |
| quoteVolume    | string | Amount of fiat                      |
| high24hr       | string | Highest price in last 24 hours      |
| low24hr        | string | Lowest price in last 24 hours       |
| highestBid     | string | Highest bidding price               |
| lowestAsk      | string | Lowest asking price                 |

<br>

---

<br>

## Reference

### Stream Demo

The demo page is available at https://api.bitkub.com/websocket-api?streams= for testing stream subscriptions.

### Order Status Values

N/A — Public streams do not carry order status data.

### Error Codes

N/A — Public streams do not return structured error codes. Connection failures result in WebSocket disconnect.

<br>

---

<br>

## Complete Example

### JavaScript Implementation

```javascript
const ws = new WebSocket('wss://api.bitkub.com/websocket-api/market.ticker.thb_btc,market.trade.thb_btc');

ws.onopen = () => console.log('Connected');
ws.onmessage = (event) => console.log(JSON.parse(event.data));
ws.onclose = () => setTimeout(() => location.reload(), 5000);
```

## Security Best Practices

N/A — Public streams do not require authentication or API credentials.
