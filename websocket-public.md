# WebSocket API — Public (2023-04-19)

## Announcement

* **⚠️ 2026-05-18:** `market.trade.<symbol>` stream will be permanently closed. Please migrate to Private WebSocket.

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

---

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
  "bid": "2048451",
  "sid": "2924729",
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
| bid    | string | Buy order ID (string since 2023-04-19)  |
| sid    | string | Sell order ID (string since 2023-04-19) |
| ts     | int    | Trade timestamp (Unix seconds)          |

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

### Live Order Book Stream

#### Name:
orderbook.\<symbol-id\>

#### Description:
Real-time order book data using numeric symbol ID. Emits 5 event types: `bidschanged`, `askschanged`, `tradeschanged`, `ticker`, and `global.ticker`.

| Event         | Trigger                                                              | Max Depth |
| ------------- | -------------------------------------------------------------------- | --------- |
| bidschanged   | Any buy order opened/closed/cancelled on this symbol                 | 30 orders |
| askschanged   | Any sell order opened/closed/cancelled on this symbol                | 30 orders |
| tradeschanged | Orders matched on this symbol (also sent as initial data on connect) | 30 each   |
| ticker        | Any of bidschanged/askschanged/tradeschanged fires on this symbol    | —         |
| global.ticker | Any of bidschanged/askschanged/tradeschanged fires on any symbol     | —         |

#### Response:

**Event: bidschanged / askschanged**
```json
{
  "data": [
    [121.82, 112510.1, 0.00108283, 0, false, false]
  ],
  "event": "bidschanged",
  "pairing_id": 1
}
```

**Event: tradeschanged**
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

**Event: ticker / global.ticker**
```json
{
  "data": {
    "baseVolume": 106302.39237032,
    "change": 0.16,
    "close": 15.9,
    "high24hr": 16.72,
    "highestBid": 15.81,
    "highestBidSize": 5640.39911448,
    "id": 139,
    "isFrozen": 0,
    "last": 15.9,
    "low24hr": 15.7,
    "lowestAsk": 16.22,
    "lowestAskSize": 1582,
    "open": 15.74,
    "percentChange": 1.02,
    "quoteVolume": 1715566.77,
    "stream": "market.ticker.thb_1inch"
  },
  "event": "ticker",
  "pairing_id": 1
}
```

#### Field Descriptions:

**bidschanged / askschanged — array items:**

| Index | Type    | Description                |
| ----- | ------- | -------------------------- |
| 0     | float   | Volume                     |
| 1     | float   | Rate (price)               |
| 2     | float   | Amount                     |
| 3     | int     | Reserved (always 0)        |
| 4     | boolean | Is new order               |
| 5     | boolean | User is owner (deprecated) |

**tradeschanged — array[0] latest trades:**

| Index | Type    | Description                                   |
| ----- | ------- | --------------------------------------------- |
| 0     | int     | Timestamp                                     |
| 1     | float   | Rate (price)                                  |
| 2     | float   | Amount                                        |
| 3     | string  | Side: BUY or SELL                             |
| 4     | int     | Reserved (always 0)                           |
| 5     | int     | Reserved (always 0)                           |
| 6     | boolean | Is new order                                  |
| 7     | boolean | User is buyer (available when authenticated)  |
| 8     | boolean | User is seller (available when authenticated) |

**tradeschanged — array[1] buy orders / array[2] sell orders:**

| Index | Type    | Description                                  |
| ----- | ------- | -------------------------------------------- |
| 0     | float   | Volume                                       |
| 1     | float   | Rate (price)                                 |
| 2     | float   | Amount                                       |
| 3     | int     | Reserved (always 0)                          |
| 4     | boolean | Is new order                                 |
| 5     | boolean | User is owner (available when authenticated) |

**ticker / global.ticker — data object:**

| Field          | Type   | Description                         |
| -------------- | ------ | ----------------------------------- |
| baseVolume     | float  | Amount of crypto                    |
| change         | float  | Price difference compared to latest |
| close          | float  | Close price                         |
| high24hr       | float  | Highest price in last 24 hours      |
| highestBid     | float  | Highest bidding price               |
| highestBidSize | float  | Amount of the highest bidding order |
| id             | int    | Symbol ID                           |
| isFrozen       | int    | Symbol trade status                 |
| last           | float  | Latest price                        |
| low24hr        | float  | Lowest price in last 24 hours       |
| lowestAsk      | float  | Lowest asking price                 |
| lowestAskSize  | float  | Amount of the lowest asking order   |
| open           | float  | Open price                          |
| percentChange  | float  | Price difference in percent         |
| quoteVolume    | float  | Amount of fiat                      |
| stream         | string | Stream name                         |

---

## Reference

### Stream Demo

The demo page is available at https://api.bitkub.com/websocket-api?streams= for testing stream subscriptions.

### Order Status Values

N/A — Public streams do not carry order status data.

### Error Codes

N/A — Public streams do not return structured error codes. Connection failures result in WebSocket disconnect.

---

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
