# REST API V3

## Announcement

* **⚠️ 2026-06-09:** Fiat v3 endpoints will be deprecated. Please migrate to Fiat v4 endpoints (rest-v4.md).
* **⚠️ 2025-12-09:** The following market endpoints will be deprecated. Please use v3 endpoints as replacement: GET /api/market/symbols, GET /api/market/ticker, GET /api/market/trades, GET /api/market/bids, GET /api/market/asks, GET /api/market/books, GET /api/market/depth
* Page-based pagination will be deprecated on 8 Sep 2025 for my-order-history.
* Order history older than 90 days is archived for my-order-history.
* Deprecation of Order Hash for my-open-orders, my-order-history, my-order-info, place-bid, place-ask, cancel-order on 28/02/2025 onwards.

## Change Log

* 2026-04-07 Announce Fiat v4 API and deprecation of Fiat v3 endpoints on 09 June 2026
* 2025-09-08 Update API my-order-history spec
* 2025-01-07 Update FIAT Withdraw error code
* 2024-12-20 Introducing Enhanced Market Data Endpoints: Ticker, Depth, Bids, Asks, Trades
* 2024-07-25 Deprecated Secure Endpoint V1/V2
* 2024-07-05 Update rate-limits of place-bid, place-ask, cancel-order, my-open-orders
* 2024-05-16 Post-Only Functionality added to place-bid and place-ask
* 2024-06-11 Added new error code 58 - Transaction Not Found

## Overview

V3 is the current stable REST API for market data and trading operations. It introduces header-based HMAC authentication replacing the older query-string approach.

## Base URL

`https://api.bitkub.com`

## Authentication

All secure endpoints require the following headers:

| Header | Description |
| ------ | ----------- |
| `X-BTK-APIKEY` | Your API key |
| `X-BTK-TIMESTAMP` | Timestamp in milliseconds (from GET /api/v3/servertime) |
| `X-BTK-SIGN` | HMAC SHA-256 signature in hex format |

**Signature format:** `{timestamp}{METHOD}{/api/path}{?query or body}`

```javascript
// GET example
1699381086593GET/api/v3/market/my-order-history?sym=BTC_THB

// POST example
1699376552354POST/api/v3/market/place-bid{"sym":"thb_btc","amt":1000,"rat":10,"typ":"limit"}
```

## Endpoints

## Non-Secure Endpoints

### Server Information

### GET /api/status

#### Description:
Get endpoint status. When status is not `ok`, it is highly recommended to wait until the status changes back to `ok`.


#### Required Permission:
N/A

#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/status'
```

#### Response:
```json
[
  {
    "name": "Non-secure endpoints",
    "status": "ok",
    "message": ""
  },
  {
    "name": "Secure endpoints",
    "status": "ok",
    "message": ""
  }
]
```

#### Field Descriptions:
N/A
### GET /api/servertime

#### Description:
Get server timestamp.

**⚠️ DEPRECATED:** Returns seconds. Use GET /api/v3/servertime instead (returns milliseconds).


#### Required Permission:
N/A

#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/servertime'
```

#### Response:
```json
1707220534359
```

#### Field Descriptions:
N/A
### GET /api/v3/servertime

#### Description:
Get server timestamp in milliseconds. Use this for generating request signatures.


#### Required Permission:
N/A

#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/servertime'
```

#### Response:
```json
1701251212273
```

#### Field Descriptions:
N/A
### Market Data (Read-Only)

### GET /api/v3/market/symbols

#### Description:
List all available trading symbols.


#### Required Permission:
N/A

#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/market/symbols'
```

#### Response:
```json
{
  "error": 0,
  "result": [
    {
      "base_asset": "BTC",
      "base_asset_scale": 8,
      "buy_price_gap_as_percent": 20,
      "created_at": "2017-10-30T22:16:10+07:00",
      "description": "Thai Baht to Bitcoin",
      "freeze_buy": false,
      "freeze_cancel": false,
      "freeze_sell": false,
      "market_segment": "SPOT",
      "min_quote_size": 10,
      "modified_at": "2025-05-20T16:48:04.599+07:00",
      "name": "Bitcoin",
      "pairing_id": 1,
      "price_scale": 2,
      "price_step": "0.01",
      "quantity_scale": 0,
      "quantity_step": "1",
      "quote_asset": "THB",
      "quote_asset_scale": 2,
      "sell_price_gap_as_percent": 20,
      "status": "active",
      "symbol": "BTC_THB",
      "source": "exchange"
    }
  ]
}
```

#### Field Descriptions:
N/A
### GET /api/v3/market/ticker

#### Description:
Get ticker information. If `sym` is omitted, returns all tickers.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| sym | string | false | The symbol (e.g. btc_thb) |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/market/ticker?sym=btc_thb'
```

#### Response:
```json
[
  {
    "symbol": "BTC_THB",
    "base_volume": "1875227.0489781",
    "high_24_hr": "3400000",
    "highest_bid": "3380000",
    "last": "3385000",
    "low_24_hr": "3300000",
    "lowest_ask": "3390000",
    "percent_change": "2.69",
    "quote_volume": "69080877.73"
  }
]
```

#### Field Descriptions:
N/A
### GET /api/v3/market/bids

#### Description:
List open buy orders.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| sym | string | true | The symbol (e.g. btc_thb) |
| lmt | int | false | Limit number of results |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/market/bids?sym=btc_thb&lmt=5'
```

#### Response:
```json
{
  "error": 0,
  "result": [
    {
      "order_id": "365357265",
      "price": "3330100.43",
      "side": "buy",
      "size": "0.87901418",
      "timestamp": 1734714699000,
      "volume": "2927205.5"
    }
  ]
}
```

#### Field Descriptions:
N/A
### GET /api/v3/market/asks

#### Description:
List open sell orders.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| sym | string | true | The symbol (e.g. btc_thb) |
| lmt | int | false | Limit number of results |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/market/asks?sym=btc_thb&lmt=5'
```

#### Response:
```json
{
  "error": 0,
  "result": [
    {
      "order_id": "303536416",
      "price": "3334889",
      "side": "sell",
      "size": "0.01289714",
      "timestamp": 1734689550000,
      "volume": "42903"
    }
  ]
}
```

#### Field Descriptions:
N/A
### GET /api/v3/market/depth

#### Description:
Get depth information (order book snapshot).


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| sym | string | true | The symbol (e.g. btc_thb) |
| lmt | int | false | Depth size |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/market/depth?sym=btc_thb&lmt=5'
```

#### Response:
```json
{
  "error": 0,
  "result": {
    "asks": [
      [3338932.98, 0.00619979],
      [3341006.36, 0.00134854]
    ],
    "bids": [
      [3334907.27, 0.00471255],
      [3334907.26, 0.36895805]
    ]
  }
}
```

#### Field Descriptions:
N/A
### GET /api/v3/market/trades

#### Description:
List recent trades.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| sym | string | true | The symbol (e.g. btc_thb) |
| lmt | int | false | Limit number of results |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/market/trades?sym=btc_thb&lmt=5'
```

#### Response:
```json
{
  "error": 0,
  "result": [
    [1734661894000, 3367353.98, 0.00148484, "BUY"],
    [1734661893000, 3367353.98, 0.00029622, "BUY"]
  ]
}
```

#### Field Descriptions:
N/A
### Chart Data

### GET /tradingview/history

#### Description:
Get OHLCV historical data for TradingView chart integration.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| symbol | string | true | The symbol (e.g. BTC_THB) |
| resolution | string | true | Chart resolution: 1, 5, 15, 60, 240, 1D |
| from | int | true | Start timestamp (Unix seconds) |
| to | int | true | End timestamp (Unix seconds) |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/tradingview/history?symbol=BTC_THB&resolution=60&from=1633424427&to=1633427427'
```

#### Response:
```json
{
  "c": [1685000, 1680699.95, 1688998.99, 1692222.22],
  "h": [1685000, 1685000, 1689000, 1692222.22],
  "l": [1680053.22, 1671000, 1680000, 1684995.07],
  "o": [1682500, 1685000, 1680100, 1684995.07],
  "s": "ok",
  "t": [1633424400, 1633425300, 1633426200, 1633427100],
  "v": [4.604352630000001, 8.530631670000005, 4.836581560000002, 2.851018920000002]
}
```

#### Field Descriptions:
N/A
## Secure Endpoints

### User Account & Limits

### POST /api/v3/market/wallet

#### Description:
Get user available balances. For both available and reserved balances use POST /api/v3/market/balances.


#### Required Permission:
N/A

#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location --request POST 'https://api.bitkub.com/api/v3/market/wallet' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "error": 0,
  "result": {
    "THB": 188379.27,
    "BTC": 8.90397323,
    "ETH": 10.1
  }
}
```

#### Field Descriptions:
N/A
### POST /api/v3/market/balances

#### Description:
Get balances info including both available and reserved balances.


#### Required Permission:
N/A

#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location --request POST 'https://api.bitkub.com/api/v3/market/balances' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "error": 0,
  "result": {
    "THB": { "available": 188379.27, "reserved": 0 },
    "BTC": { "available": 8.90397323, "reserved": 0 },
    "ETH": { "available": 10.1, "reserved": 0 }
  }
}
```

#### Field Descriptions:
N/A
### POST /api/v3/user/trading-credits

#### Description:
Check trading credit balance.


#### Required Permission:
N/A

#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location --request POST 'https://api.bitkub.com/api/v3/user/trading-credits' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "error": 0,
  "result": 1000
}
```

#### Field Descriptions:
N/A
### POST /api/v3/user/limits

#### Description:
Check deposit and withdraw limitations and current usage.


#### Required Permission:
N/A

#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location --request POST 'https://api.bitkub.com/api/v3/user/limits' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "error": 0,
  "result": {
    "limits": {
      "crypto": { "deposit": 0.88971929, "withdraw": 0.88971929 },
      "fiat": { "deposit": 200000, "withdraw": 200000 }
    },
    "usage": {
      "crypto": {
        "deposit": 0, "withdraw": 0,
        "deposit_percentage": 0, "withdraw_percentage": 0,
        "deposit_thb_equivalent": 0, "withdraw_thb_equivalent": 0
      },
      "fiat": { "deposit": 0, "withdraw": 0, "deposit_percentage": 0, "withdraw_percentage": 0 }
    },
    "rate": 224790
  }
}
```

#### Field Descriptions:
N/A
### GET /api/v3/user/coin-convert-history

#### Description:
List all coin convert histories (paginated).


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| p | int | false | Page (default = 1) |
| lmt | int | false | Limit (default = 100) |
| sort | int | false | Sort: 1 or -1 (default = 1) |
| status | string | false | Filter: success, fail, all (default = all) |
| sym | string | false | Filter by symbol |
| start | int | false | Start timestamp |
| end | int | false | End timestamp |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/user/coin-convert-history?p=1&lmt=10' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "error": 0,
  "result": [
    {
      "transaction_id": "67ef4ca7ddb88f34ce16a126",
      "status": "success",
      "amount": "0.0134066",
      "from_currency": "KUB",
      "trading_fee_received": "1.34",
      "timestamp": 1743761171000
    }
  ],
  "pagination": { "page": 1, "last": 12, "next": 2 }
}
```

#### Field Descriptions:
N/A
### Trading Operations

### POST /api/v3/market/place-bid

#### Description:
Create a buy order.


#### Required Permission:
N/A
#### Body Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| sym | string | true | The symbol (e.g. btc_thb) |
| amt | float | true | Amount to spend — no trailing zeros (1000 not 1000.00) |
| rat | float | true | Rate — no trailing zeros; use 0 for market order |
| typ | string | true | Order type: limit or market |
| client_id | string | false | Your custom reference ID |
| post_only | bool | false | Post-only flag: true or false |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/market/place-bid' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}' \
--header 'Content-Type: application/json' \
--data '{"sym":"thb_btc","amt":1000,"rat":10,"typ":"limit"}'
```

#### Response:
```json
{
  "error": 0,
  "result": {
    "id": "1",
    "typ": "limit",
    "amt": 1000,
    "rat": 15000,
    "fee": 2.5,
    "cre": 2.5,
    "rec": 0.06666666,
    "ts": "1707220636",
    "ci": "input_client_id"
  }
}
```

#### Field Descriptions:
N/A
### POST /api/v3/market/place-ask

#### Description:
Create a sell order.


#### Required Permission:
N/A
#### Body Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| sym | string | true | The symbol (e.g. btc_thb) |
| amt | float | true | Amount to sell — no trailing zeros (0.1 not 0.10000000) |
| rat | float | true | Rate — no trailing zeros; use 0 for market order |
| typ | string | true | Order type: limit or market |
| client_id | string | false | Your custom reference ID |
| post_only | bool | false | Post-only flag: true or false |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/market/place-ask' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}' \
--header 'Content-Type: application/json' \
--data '{"sym":"btc_thb","amt":0.1,"rat":3000000,"typ":"limit"}'
```

#### Response:
```json
{
  "error": 0,
  "result": {
    "id": "1",
    "typ": "limit",
    "amt": 1.0,
    "rat": 15000,
    "fee": 37.5,
    "cre": 37.5,
    "rec": 15000,
    "ts": "1533834844",
    "ci": "input_client_id"
  }
}
```

#### Field Descriptions:
N/A
### POST /api/v3/market/cancel-order

#### Description:
Cancel an open order.


#### Required Permission:
N/A
#### Body Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| sym | string | true | The symbol (e.g. btc_thb) |
| id | string | true | Order ID to cancel |
| sd | string | true | Order side: buy or sell |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/market/cancel-order' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}' \
--header 'Content-Type: application/json' \
--data '{"sym":"btc_thb","id":"123456","sd":"buy"}'
```

#### Response:
```json
{
  "error": 0
}
```

#### Field Descriptions:
N/A
### GET /api/v3/market/my-open-orders

#### Description:
List all open orders for the given symbol.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| sym | string | true | The symbol (e.g. btc_thb) |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/market/my-open-orders?sym=btc_thb' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "error": 0,
  "result": [
    {
      "id": "278465822",
      "side": "buy",
      "type": "limit",
      "rate": "10",
      "fee": "0.25",
      "credit": "0",
      "amount": "100",
      "receive": "9.975",
      "parent_id": "0",
      "super_id": "0",
      "client_id": "client_id",
      "ts": 1707220636000
    }
  ]
}
```

#### Field Descriptions:
N/A
### GET /api/v3/market/my-order-history

#### Description:
List all orders that have already matched. Supports page-based and keyset (cursor) pagination.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| sym | string | true | The trading symbol (e.g. BTC_THB) |
| p | string | false | Page number (page-based pagination) |
| lmt | string | false | Limit per page (default: 10, min: 1) |
| cursor | string | false | Base64 cursor (keyset pagination) |
| start | string | false | Start timestamp |
| end | string | false | End timestamp |
| pagination_type | string | false | page or keyset (default: page) |

#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/market/my-order-history?sym=BTC_THB&p=1&lmt=10' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Validation Rules:
- `sym` is required and must be a valid trading symbol
- `p` and `cursor` cannot be used together
- `p` requires `pagination_type=page` or no pagination_type specified
- `cursor` requires `pagination_type=keyset`
- `lmt` must be a positive integer >= 1
- `start` and `end` must be valid timestamps if provided
- `start` must be less than `end` if both provided

#### Response (Page-based pagination):
```json
{
  "error": 0,
  "result": [
    {
      "txn_id": "68a82566596d482000f4e4edaa05m0",
      "order_id": "68a82566596d482000f4e4edaa05m0",
      "parent_order_id": "68a82566596d482000f4e4edaa05m0",
      "super_order_id": "68a82566596d482000f4e4edaa05m0",
      "client_id": "CLIENT123",
      "taken_by_me": false,
      "is_maker": true,
      "side": "buy",
      "type": "limit",
      "rate": "2500000.00",
      "fee": "25.00",
      "credit": "0.00",
      "amount": "1000.00",
      "ts": 1755850086843,
      "order_closed_at": 1755850086843
    }
  ],
  "pagination": {
    "page": 1,
    "last": 10,
    "next": 2,
    "prev": null
  }
}
```

#### Response (Keyset-based pagination):
```json
{
  "error": 0,
  "result": [
    {
      "txn_id": "68a82566596d482000f4e4edaa05m0",
      "order_id": "68a82566596d482000f4e4edaa05m0",
      "parent_order_id": "68a82566596d482000f4e4edaa05m0",
      "super_order_id": "68a82566596d482000f4e4edaa05m0",
      "client_id": "CLIENT123",
      "taken_by_me": false,
      "is_maker": true,
      "side": "buy",
      "type": "limit",
      "rate": "2500000.00",
      "fee": "25.00",
      "credit": "0.00",
      "amount": "1000.00",
      "ts": 1755850086843,
      "order_closed_at": 1755850086843
    }
  ],
  "pagination": {
    "cursor": "eyJpZCI6Ik9SRDEyMzQ1Njc4OSIsInRzIjoiMTY3MjUzMTIwMCJ9",
    "has_next": true
  }
}
```

#### Field Descriptions:

| Field | Type | Description |
| ----- | ---- | ----------- |
| txn_id | string | Transaction ID |
| order_id | string | Unique order identifier |
| parent_order_id | string | Parent order ID (for linked orders) |
| super_order_id | string | Super order ID (for grouped orders) |
| client_id | string | Client-provided order ID |
| taken_by_me | boolean | Whether the order was taken by the user |
| is_maker | boolean | Whether the order was a maker order |
| side | string | Order side: buy or sell |
| type | string | Order type: limit or market |
| rate | string | Order price/rate |
| fee | string | Fee paid in THB |
| credit | string | Credit used for fee payment |
| amount | string | Order amount |
| ts | number | Order close timestamp (milliseconds) |
| order_closed_at | number | Order closure timestamp (milliseconds, nullable) |

### GET /api/v3/market/order-info

#### Description:
Get information regarding the specified order.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| sym | string | true | The symbol (e.g. btc_thb) |
| id | string | true | Order ID |
| sd | string | true | Order side: buy or sell |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/market/order-info?sym=btc_thb&id=289&sd=buy' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "error": 0,
  "result": {
    "id": "289",
    "first": "289",
    "parent": "0",
    "last": "316",
    "client_id": "",
    "post_only": false,
    "amount": "4000",
    "rate": 291000,
    "fee": 10,
    "credit": 10,
    "filled": 3999.97,
    "total": 4000,
    "status": "filled",
    "partial_filled": false,
    "remaining": 0,
    "history": [
      {
        "amount": 98.14848,
        "credit": 0.25,
        "fee": 0.25,
        "id": "289",
        "rate": 291000,
        "timestamp": 1702466375000,
        "txn_id": "BTCBUY0003372258"
      }
    ]
  }
}
```

#### Field Descriptions:
N/A
### POST /api/v3/market/wstoken

#### Description:
Generate a token for WebSocket authentication (used with the Public WebSocket API).


#### Required Permission:
N/A

#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location --request POST 'https://api.bitkub.com/api/v3/market/wstoken' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "error": 0,
  "result": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
}
```

#### Field Descriptions:
N/A
## Fiat Operations (DEPRECATED — Migrate to V4 by 2026-06-09)

**⚠️ These endpoints will be permanently removed on 09 June 2026. Please migrate to REST API V4 Fiat endpoints.**

### POST /api/v3/fiat/accounts

#### Description:
List all approved bank accounts.

**⚠️ DEPRECATED:** Use GET /api/v4/fiat/accounts instead.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| p | int | false | Page |
| lmt | int | false | Limit |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location --request POST 'https://api.bitkub.com/api/v3/fiat/accounts' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "error": 0,
  "result": [
    {
      "id": "7262109099",
      "bank": "Kasikorn Bank",
      "name": "Somsak",
      "time": 1570893867
    }
  ],
  "pagination": { "page": 1, "last": 1 }
}
```

#### Field Descriptions:
N/A
### POST /api/v3/fiat/withdraw

#### Description:
Make a withdrawal to an approved bank account.

**⚠️ DEPRECATED:** Use POST /api/v4/fiat/withdraw instead.


#### Required Permission:
N/A
#### Body Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| id | string | true | Bank account ID |
| amt | float | true | Amount to withdraw |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/fiat/withdraw' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}' \
--header 'Content-Type: application/json' \
--data '{"id":"7262109099","amt":1000}'
```

#### Response:
```json
{
  "error": 0,
  "result": {
    "txn": "THBWD0000012345",
    "acc": "7262109099",
    "cur": "THB",
    "amt": 21,
    "fee": 20,
    "rec": 1,
    "ts": 1569999999
  }
}
```

#### Field Descriptions:
N/A
### POST /api/v3/fiat/deposit-history

#### Description:
List fiat deposit history.

**⚠️ DEPRECATED:** Use GET /api/v4/fiat/deposit/history instead.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| p | int | false | Page |
| lmt | int | false | Limit |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location --request POST 'https://api.bitkub.com/api/v3/fiat/deposit-history' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "error": 0,
  "result": [
    {
      "txn_id": "THBDP0000012345",
      "currency": "THB",
      "amount": 5000.55,
      "status": "complete",
      "time": 1570893867
    }
  ],
  "pagination": { "page": 1, "last": 1 }
}
```

#### Field Descriptions:
N/A
### POST /api/v3/fiat/withdraw-history

#### Description:
List fiat withdrawal history.

**⚠️ DEPRECATED:** Use GET /api/v4/fiat/withdraw/history instead.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| p | int | false | Page |
| lmt | int | false | Limit |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location --request POST 'https://api.bitkub.com/api/v3/fiat/withdraw-history' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "error": 0,
  "result": [
    {
      "txn_id": "THBWD0000012345",
      "currency": "THB",
      "amount": "21",
      "fee": 20,
      "status": "complete",
      "time": 1570893493
    }
  ],
  "pagination": { "page": 1, "last": 1 }
}
```

#### Field Descriptions:
N/A
## Additional

N/A — No additional reference information for V3.

## Error Codes

### Status Codes

N/A — V3 uses numeric error codes, not HTTP status-based codes.

### Numeric Errors

| Code | Description |
| ---- | ----------- |
| 0 | No error |
| 1 | Invalid JSON payload |
| 2 | Missing X-BTK-APIKEY |
| 3 | Invalid API key |
| 4 | API pending for activation |
| 5 | IP not allowed |
| 6 | Missing / invalid signature |
| 7 | Missing timestamp |
| 8 | Invalid timestamp |
| 9 | Invalid user / User not found |
| 10 | Invalid parameter |
| 11 | Invalid symbol |
| 12 | Invalid amount |
| 13 | Invalid rate |
| 14 | Improper rate |
| 15 | Amount too low |
| 16 | Failed to get balance |
| 17 | Wallet is empty |
| 18 | Insufficient balance |
| 19 | Failed to insert order into db |
| 20 | Failed to deduct balance |
| 21 | Invalid order for cancellation |
| 22 | Invalid side |
| 23 | Failed to update order status |
| 24 | Invalid order for lookup |
| 25 | KYC level 1 is required to proceed |
| 30 | Limit exceeds |
| 40 | Pending withdrawal exists |
| 41 | Invalid currency for withdrawal |
| 42 | Address is not in whitelist |
| 43 | Failed to deduct crypto |
| 44 | Failed to create withdrawal record |
| 47 | Withdrawal amount exceeds the maximum limit |
| 48 | Invalid bank account |
| 49 | Bank limit exceeds |
| 50 | Pending withdrawal exists / Cannot perform action due to pending transactions |
| 51 | Withdrawal is under maintenance |
| 52 | Invalid permission |
| 53 | Invalid internal address |
| 54 | Address has been deprecated |
| 55 | Cancel only mode |
| 56 | User has been suspended from purchasing |
| 57 | User has been suspended from selling |
| 58 | User bank is not verified |
| 61 | This endpoint does not support broker coins |
| 90 | Server error (please contact support) |

### System Errors

N/A — V3 uses numeric error codes. System errors are covered in Numeric Errors (code 90).

### Business Errors

N/A — V3 uses numeric error codes. Business errors are covered in Numeric Errors above.

### Validation Errors

N/A — V3 uses numeric error codes. Validation errors are covered in Numeric Errors above.

### Authentication Errors

N/A — V3 uses numeric error codes. Authentication errors (codes 2-8) are covered in Numeric Errors above.

## Rate Limits

Exceeding the limit blocks requests for 30 seconds (HTTP 429). Limits apply per user per endpoint regardless of API version.

| Endpoint | Rate Limit |
| -------- | ---------- |
| /api/v3/market/ticker | 100 req/sec |
| /api/v3/market/depth | 10 req/sec |
| /api/v3/market/symbols | 100 req/sec |
| /api/v3/market/trades | 100 req/sec |
| /api/v3/market/bids | 100 req/sec |
| /api/v3/market/asks | 100 req/sec |
| /api/market/order-info | 100 req/sec |
| /api/market/my-open-orders | 150 req/sec |
| /api/market/my-order-history | 100 req/sec |
| /api/market/place-bid | 150 req/sec |
| /api/market/place-ask | 150 req/sec |
| /api/market/cancel-order | 200 req/sec |
| /api/market/balances | 150 req/sec |
| /api/market/wallet | 150 req/sec |
| /api/servertime | 2,000 req/10secs |
| /api/status | 100 req/sec |
| /api/fiat/* | 20 req/sec |
| /api/user/* | 20 req/sec |
| /tradingview/* | 100 req/sec |
