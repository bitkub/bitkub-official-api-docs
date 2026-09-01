# REST API V3

## Change Log

* 2026-06-09 Removed deprecated Fiat v3 endpoints: POST /api/v3/fiat/accounts, POST /api/v3/fiat/withdraw, POST /api/v3/fiat/deposit-history, POST /api/v3/fiat/withdraw-history. Please migrate to [Fiat v4 endpoints](rest-v4.md).
* 2026-05-26 Removed deprecated endpoints `/api/v3/market/wallet` and `/api/v3/market/balances`. Use [GET /api/v4/wallet/balances](rest-v4.md#get-apiv4walletbalances) and [GET /api/v4/wallet/assets](rest-v4.md#get-apiv4walletassets) instead.
* 2026-04-07 Announce Fiat v4 API and deprecation of Fiat v3 endpoints on 09 June 2026
* 2025-09-08 Update API [my-order-history](#get-apiv3marketmy-order-history) spec
* 2025-01-07 Update FIAT Withdraw error code
* 2025-04-03 Deprecated Crypto Endpoint v3 and Remove from the Document.
* 2024-12-20 Introducing the Enhanced Market Data Endpoint [Ticker, Depth, Bids, Asks, Trades](#non-secure-endpoints-v3)
* 2024-07-25 Deprecated Secure Endpoint V1/V2 and Remove from the Document.
* 2024-07-05 Update rate-limits of place-bid, place-ask, cancel-order, my-open-orders  [Rate-Limits](#rate-limits)
* 2024-07-05 Update rate-limits which will be apply on 17 July 2024 [Rate-Limits](#rate-limits)
* 2024-06-11 Updated API request of POST /api/v3/crypto/internal-withdraw and edited API response of POST /api/v3/crypto/withdraw-history
* 2024-06-11 Added new error code 58 - Transaction Not Found
* 2024-05-16 Release: Post-Only Functionality Added to [POST /api/v3/market/place-bid](#post-apiv3marketplace-bid) and [POST /api/v3/market/place-ask](#post-apiv3marketplace-ask)
* 2024-03-06 Edited Request field for POST /api/v3/crypto/withdraw
* 2024-02-15 Edited Endpoint permission [Permission Table](#secure-endpoints-v3)

# Table of contents
* [Overview](#overview)
* [Base URL](#base-url)
* [Endpoint Index](#endpoint-index)
* [Authentication](#authentication)
* [Endpoints](#endpoints)
* [Error Codes](#error-codes)
* [Rate Limits](#rate-limits)

## Overview

V3 is the current stable REST API for market data and trading operations. It introduces header-based HMAC authentication replacing the older query-string approach.

## Base URL

`https://api.bitkub.com`

## Endpoint Index

### Non-Secure Endpoints V3

| Market Data Endpoint | Method |
| --------------------------------------------------------------| ------ |
| [GET /api/v3/market/symbols](#get-apiv3marketsymbols)         | GET    |
| [GET /api/v3/market/ticker](#get-apiv3marketticker)           | GET    |
| [GET /api/v3/market/bids](#get-apiv3marketbids)               | GET    |
| [GET /api/v3/market/asks](#get-apiv3marketasks)               | GET    |
| [GET /api/v3/market/depth](#get-apiv3marketdepth)             | GET    |
| [GET /api/v3/market/trades](#get-apiv3markettrades)           | GET    |

| Exchange Information Endpoint | Method |
| --------------------------------------------------------------| ------ |
| [GET /api/v3/servertime](#get-apiv3servertime)                | GET    |

### Secure Endpoints V3
All secure endpoints require [authentication](#authentication).

| User Endpoint                                                             | Method | Trade | Deposit | Withdraw |
| ------------------------------------------------------------------------- | ------ | ----- | ------- | -------- |
| [/api/v3/user/trading-credits](#post-apiv3usertrading-credits)            | POST   |       |         |          |
| [/api/v3/user/limits](#post-apiv3userlimits)                              | POST   |       |         |          |
| [/api/v3/user/coin-convert-history](#get-apiv3usercoin-convert-history)   | GET    |       |         |          |

| Trading Endpoint                                                     | Method | Trade | Deposit | Withdraw |
| --------------------------------------------------------------------| ------ | ----- | ------- | -------- |
| [/api/v3/market/place-bid](#post-apiv3marketplace-bid)              | POST   | ✅     |         |          |
| [/api/v3/market/place-ask](#post-apiv3marketplace-ask)              | POST   | ✅     |         |          |
| [/api/v3/market/cancel-order](#post-apiv3marketcancel-order)        | POST   | ✅     |         |          |
| [/api/v3/market/wstoken](#post-apiv3marketwstoken)                  | POST   | ✅     |         |          |
| [/api/v3/market/my-open-orders](#get-apiv3marketmy-open-orders)     | GET    |        |         |          |
| [/api/v3/market/my-order-history](#get-apiv3marketmy-order-history) | GET    |        |         |          |
| [/api/v3/market/order-info](#get-apiv3marketorder-info)             | GET    |       |         |          |

## Authentication

All secure endpoints require the following headers:

| Header | Description |
| ------ | ----------- |
| `Accept` | `application/json` |
| `Content-Type` | `application/json` |
| `X-BTK-APIKEY` | Your API key |
| `X-BTK-TIMESTAMP` | Timestamp in milliseconds (from GET /api/v3/servertime) |
| `X-BTK-SIGN` | HMAC SHA-256 signature in hex format |

GET requests require parameters as a **query string** in the URL (e.g. `?sym=THB_BTC&lmt=10`). POST requests require a JSON payload (`application/json`) — the payload is always JSON.

You must get a new timestamp in milliseconds from [GET /api/v3/servertime](#get-apiv3servertime).

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

<br>

---

<br>

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

<br>

---

<br>

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

<br>

---

<br>

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

<br>

---

<br>

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

<br>

---

<br>

### GET /api/v3/market/depth

#### Description:
Get depth information (order book snapshot).


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| sym | string | true | The symbol (e.g. btc_thb) |
| lmt | int | true | Depth size |


#### Validation Rules:
- `lmt` value can not be less than 1
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

| Field | Type | Description |
| ---------- | ----- | ----------- |
| asks[n][0] | float | Price |
| asks[n][1] | float | Size |
| bids[n][0] | float | Price |
| bids[n][1] | float | Size |

<br>

---

<br>

### GET /api/v3/market/trades

#### Description:
List recent trades.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| sym | string | true | The symbol (e.g. btc_thb) |
| lmt | int | true | Limit number of results |


#### Validation Rules:
- `lmt` value can not be less than 1
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

<br>

---

<br>

## Secure Endpoints

### User Account & Limits

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

<br>

---

<br>

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

| Field | Type | Description |
| ----- | ---- | ----------- |
| limits | object | Limitations by KYC level |
| limits.crypto.deposit | number | BTC value equivalent |
| limits.crypto.withdraw | number | BTC value equivalent |
| limits.fiat.deposit | number | THB value equivalent |
| limits.fiat.withdraw | number | THB value equivalent |
| usage | object | Today's usage |
| usage.crypto.deposit | number | BTC value equivalent |
| usage.crypto.withdraw | number | BTC value equivalent |
| usage.crypto.deposit_thb_equivalent | number | THB value equivalent |
| usage.crypto.withdraw_thb_equivalent | number | THB value equivalent |
| usage.fiat.deposit | number | THB value equivalent |
| usage.fiat.withdraw | number | THB value equivalent |
| rate | number | Current THB rate used to calculate |

<br>

---

<br>

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
| sym | string | false | Filter by symbol (e.g. KUB) |
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

<br>

---

<br>

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

| Field | Type | Description |
| ----- | ---- | ----------- |
| id | string | Order ID |
| typ | string | Order type |
| amt | number | Spending amount |
| rat | number | Rate |
| fee | number | Fee |
| cre | number | Fee credit used |
| rec | number | Amount to receive |
| ts | string | Timestamp |
| ci | string | Input ID for reference |

<br>

---

<br>

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

| Field | Type | Description |
| ----- | ---- | ----------- |
| id | string | Order ID |
| typ | string | Order type |
| amt | number | Selling amount |
| rat | number | Rate |
| fee | number | Fee |
| cre | number | Fee credit used |
| rec | number | Amount to receive |
| ts | string | Timestamp |
| ci | string | Input ID for reference |

<br>

---

<br>

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

<br>

---

<br>

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
      "id": "2",
      "side": "sell",
      "type": "limit",
      "rate": "15000",
      "fee": "35.01",
      "credit": "35.01",
      "amount": "0.93333334",
      "receive": "14000",
      "parent_id": "1",
      "super_id": "1",
      "client_id": "client_id",
      "ts": 1702543272000
    },
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

The first entry is a sell order and the second a buy order — note that `amount` and `receive` swap meaning between them.

#### Field Descriptions:

| Field | Type | Description |
| ----- | ---- | ----------- |
| id | string | Order ID |
| side | string | Order side: buy or sell |
| type | string | Order type |
| rate | string | Rate |
| fee | string | Fee |
| credit | string | Credit used |
| amount | string | On a buy order the THB amount; on a sell order the crypto quantity |
| receive | string | On a buy order the crypto quantity; on a sell order the THB amount |
| parent_id | string | Parent order ID |
| super_id | string | Super parent order ID |
| client_id | string | Client ID |
| ts | number | Timestamp (milliseconds) |

<br>

---

<br>

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
| amount | string | Order amount — quote quantity for buy orders, base quantity for sell orders |
| ts | number | Order close timestamp (milliseconds) |
| order_closed_at | number | Order closure timestamp (milliseconds, nullable) |
| page | number | Current page number |
| last | number | Total number of pages |
| next | number | Next page number (nullable) |
| prev | number | Previous page number (nullable) |
| cursor | string | Base64 encoded cursor for the next page |
| has_next | boolean | Whether there are more records |

#### Cursor Encoding Details:

The `cursor` parameter uses Base64 encoding of a JSON object containing pagination state.

**Cursor structure:**
```json
{
  "id": "ORDER_ID_STRING",
  "ts": "TIMESTAMP_DECIMAL"
}
```

**Encoding process:**
1. Create a JSON object with `id` (order ID) and `ts` (timestamp as decimal)
2. Convert the JSON to a string
3. Encode the string using Base64 standard encoding

**Example:**
```json
// Original cursor object
{
  "id": "ORD123456789",
  "ts": "1672531200"
}

// JSON string
'{"id":"ORD123456789","ts":"1672531200"}'

// Base64 encoded
"eyJpZCI6Ik9SRDEyMzQ1Njc4OSIsInRzIjoiMTY3MjUzMTIwMCJ9"
```

**Custom cursor creation:**
1. Take the last item's `order_id` and `ts` from the previous response
2. Create the JSON: `{"id":"LAST_ORDER_ID","ts":"LAST_TIMESTAMP"}`
3. Base64 encode the JSON string
4. Use the encoded string as the `cursor` parameter

**Empty cursor:**
- Default empty cursor: `e30=` (Base64 of `{}`)
- Used when no cursor is provided in keyset pagination

<br>

---

<br>

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

| Field | Type | Description |
| ----- | ---- | ----------- |
| id | string | Order ID |
| first | string | First order ID |
| parent | string | Parent order ID |
| last | string | Last order ID |
| client_id | string | Your ID for reference |
| post_only | boolean | Post-only flag: true or false |
| amount | string | Order amount — on a buy order the THB amount; on a sell order the crypto amount |
| rate | number | Order rate |
| fee | number | Order fee |
| credit | number | Order fee credit used |
| filled | number | Filled amount |
| total | number | Total amount |
| status | string | Order status: filled, unfilled or cancelled |
| partial_filled | boolean | True when the order has been partially filled; false when not filled or fully filled |
| remaining | number | Remaining amount to be executed |
| history | array | Per-fill history entries for this order |

<br>

---

<br>

## Additional

N/A — No additional reference information for V3.

## Error Codes

### Status Codes

N/A — V3 uses numeric error codes, not HTTP status-based codes.

### Authentication Errors

| Code | Description |
| ---- | ----------- |
| 1 | Invalid JSON payload |
| 2 | Missing X-BTK-APIKEY |
| 3 | Invalid API key |
| 4 | API pending for activation |
| 5 | IP not allowed |
| 6 | Missing / invalid signature |
| 7 | Missing timestamp |
| 8 | Invalid timestamp |

### User Errors

| Code | Description |
| ---- | ----------- |
| 9 | • Invalid user <br> • User not found <br> • Freeze withdrawal <br> • User is not allowed to perform this action within the last 24 hours <br> • User has suspicious withdraw crypto txn |
| 10 | • Invalid parameter <br> • Invalid response: Code not found in response <br> • Validate params <br> • Default |
| 11 | Invalid symbol |
| 12 | • Invalid amount <br> • Withdrawal amount is below the minimum threshold |
| 13 | Invalid rate |
| 14 | Improper rate |
| 15 | Amount too low |

### Trading Errors

| Code | Description |
| ---- | ----------- |
| 16 | Failed to get balance |
| 17 | Wallet is empty |
| 18 | Insufficient balance |
| 19 | Failed to insert order into db |
| 20 | Failed to deduct balance |
| 21 | Invalid order for cancellation (Unable to find OrderID or Symbol.) |
| 22 | Invalid side |
| 23 | Failed to update order status |
| 24 | • Invalid order for lookup <br> • Invalid kyc level |
| 25 | KYC level 1 is required to proceed |
| 30 | Limit exceeds |

### Withdrawal Errors

| Code | Description |
| ---- | ----------- |
| 40 | Pending withdrawal exists |
| 41 | Invalid currency for withdrawal |
| 42 | Address is not in whitelist |
| 43 | • Failed to deduct crypto <br> • Insufficient balance <br> • Deduct balance failed |
| 44 | Failed to create withdrawal record |
| 47 | Withdrawal amount exceeds the maximum limit |
| 48 | • Invalid bank account <br> • User bank id is not found <br> • User bank is unavailable |
| 49 | Bank limit exceeds |
| 50 | • Pending withdrawal exists <br> • Cannot perform the action due to pending transactions |
| 51 | Withdrawal is under maintenance |

### System Errors

| Code | Description |
| ---- | ----------- |
| 0 | No error |
| 52 | Invalid permission |
| 53 | Invalid internal address |
| 54 | Address has been deprecated |
| 55 | Cancel only mode |
| 56 | User has been suspended from purchasing |
| 57 | User has been suspended from selling |
| 58 | ~~Transaction not found~~ <br> User bank is not verified |
| 61 | This endpoint doesn't support broker coins ('source' = broker). You can check 'source' of each symbol in /api/v3/market/symbols. |
| 90 | Server error (please contact support) |

### Business Errors

N/A — V3 has no Business error category.

### Validation Errors

N/A — V3 has no Validation error category.

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
| /api/servertime | 2,000 req/10secs |
| /api/status | 100 req/sec |
| /api/fiat/* | 20 req/sec |
| /api/user/* | 20 req/sec |
| /tradingview/* | 100 req/sec |
