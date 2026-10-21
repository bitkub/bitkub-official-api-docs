# REST API V4

## Change Log

* 2026-10-21 Added new Market endpoints GET /api/v4/market/open-orders, GET /api/v4/market/orders and GET /api/v4/market/matched
* 2026-05-06 Added new Wallet endpoints GET /api/v4/wallet/balances and GET /api/v4/wallet/assets
* 2026-04-07 Introducing Fiat V4 Endpoints
* 2025-05-27 Added GET /api/v4/crypto/compensations, updated spec for crypto/withdraws and crypto/deposits
* 2025-04-08 Added new error codes: B1016-CW Deposit is frozen, V1015-CW Coin not found
* 2025-04-03 Added GET /api/v4/crypto/coins
* 2025-02-03 Introducing Crypto V4 Endpoints

# Table of contents
* [Overview](#overview)
* [Base URL](#base-url)
* [Endpoint Index](#endpoint-index)
* [Authentication](#authentication)
* [Endpoints](#endpoints)
* [Error Codes](#error-codes)
* [Rate Limits](#rate-limits)

## Overview

V4 is the current REST API for crypto and fiat deposit/withdrawal operations, wallet balances, and order/trade history. It uses the same HMAC authentication as V3 but returns structured error codes (e.g. `V1007-CW`) instead of numeric codes.

## Base URL

`https://api.bitkub.com`

## Endpoint Index

### Secure Endpoints V4
All secure endpoints require [authentication](#authentication).

| Crypto V4 Endpoints                                           | Method | Deposit | Withdraw | Trade |
| --------------------------------------------------------------| ------ | ------- | -------- | ----- |
| [/api/v4/crypto/addresses](#get-apiv4cryptoaddresses)         | GET    |         |          |       |
| [/api/v4/crypto/addresses](#post-apiv4cryptoaddresses)        | POST   | ✅      |          |       |
| [/api/v4/crypto/deposits](#get-apiv4cryptodeposits)           | GET    |         |          |       |
| [/api/v4/crypto/withdraws](#get-apiv4cryptowithdraws)         | GET    |         |          |       |
| [/api/v4/crypto/withdraws](#post-apiv4cryptowithdraws)        | POST   |         | ✅       |       |
| [/api/v4/crypto/coins](#get-apiv4cryptocoins)                 | GET    |         |          |       |
| [/api/v4/crypto/compensations](#get-apiv4cryptocompensations) | GET    |         |          |       |

| Wallet V4 Endpoints                                     | Method |
| --------------------------------------------------------| ------ |
| [/api/v4/wallet/balances](#get-apiv4walletbalances)     | GET    |
| [/api/v4/wallet/assets](#get-apiv4walletassets)         | GET    |

| Market V4 Endpoints                                         | Method |
| ----------------------------------------------------------- | ------ |
| [/api/v4/market/open-orders](#get-apiv4marketopen-orders)   | GET    |
| [/api/v4/market/orders](#get-apiv4marketorders)             | GET    |
| [/api/v4/market/matched](#get-apiv4marketmatched)           | GET    |

| Fiat V4 Endpoints                                              | Method | Deposit | Withdraw | Trade |
| ----------------------------------------------------------------| ------ | ------- | -------- | ----- |
| [/api/v4/fiat/accounts](#get-apiv4fiataccounts)                | GET    |         | ✅       |       |
| [/api/v4/fiat/deposit/history](#get-apiv4fiatdeposithistory)   | GET    |         |          |       |
| [/api/v4/fiat/withdraw/history](#get-apiv4fiatwithdrawhistory) | GET    |         |          |       |
| [/api/v4/fiat/withdraw](#post-apiv4fiatwithdraw)               | POST   |         |          |       |

## Authentication

All secure endpoints require the following headers:

| Header | Description |
| ------ | ----------- |
| `Accept` | `application/json` |
| `Content-Type` | `application/json` |
| `X-BTK-APIKEY` | Your API key |
| `X-BTK-TIMESTAMP` | Timestamp in milliseconds (from GET /api/v3/servertime) |
| `X-BTK-SIGN` | HMAC SHA-256 signature in hex format |

GET requests require parameters as a **query string** in the URL (e.g. `?symbol=ATOM&limit=10`). POST requests require a JSON payload (`application/json`) — the payload is always JSON.

You must get a new timestamp in milliseconds from [GET /api/v3/servertime](rest-v3.md#get-apiv3servertime).

**Signature format:** `{timestamp}{METHOD}{/api/path}{?query or body}`

```javascript
// GET example
1699381086593GET/api/v4/crypto/addresses?symbol=ATOM

// POST example
1699376552354POST/api/v4/crypto/addresses{"symbol":"ATOM","network":"ATOM"}
```

## Endpoints

## Crypto Endpoints

### Address Management

### GET /api/v4/crypto/addresses

#### Description:
List all crypto deposit addresses (paginated).


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| page | int | false | Page (default = 1) |
| limit | int | false | Limit (default = 100, max = 200) |
| symbol | string | false | Coin symbol (e.g. ATOM) |
| network | string | false | Network (e.g. ATOM) |
| memo | string | false | Memo filter |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/crypto/addresses?symbol=ATOM' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "code": "0",
  "message": "success",
  "data": {
    "page": 1,
    "total_page": 1,
    "total_item": 1,
    "items": [
      {
        "symbol": "ATOM",
        "network": "ATOM",
        "address": "cosmos1jcslcmz2lpsy7uq5u2ktn459qce2chqapey7gh",
        "memo": "107467228685",
        "created_at": "2022-03-18T05:41:40.199Z"
      }
    ]
  }
}
```

#### Field Descriptions:
N/A

<br>

---

<br>

### POST /api/v4/crypto/addresses

#### Description:
Generate a new crypto deposit address. If an address already exists for the symbol/network, returns the existing address.

#### Required Permission: `is_deposit`

#### Body Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| symbol | string | true | Coin symbol (e.g. ATOM) |
| network | string | true | Network (e.g. ATOM) |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/crypto/addresses' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}' \
--header 'Content-Type: application/json' \
--data '{"symbol":"ETH","network":"ETH"}'
```

#### Response:
```json
{
  "code": "0",
  "message": "success",
  "data": [
    {
      "symbol": "ETH",
      "network": "ETH",
      "address": "0x520165471daa570ab632dd504c6af257bd36edfb",
      "memo": ""
    }
  ]
}
```

#### Field Descriptions:
N/A

<br>

---

<br>

### Transaction History

### GET /api/v4/crypto/deposits

#### Description:
List crypto deposit history. Only records within the last 90 days are returned.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| page | int | false | Page (default = 1) |
| limit | int | false | Limit (default = 100, max = 200) |
| symbol | string | false | Coin symbol (e.g. BTC, ETH) |
| status | string | false | Deposit status: pending, rejected, complete |
| created_start | string | false | Start of creation time range (e.g. 2025-01-11T10:00:00.000Z) |
| created_end | string | false | End of creation time range (e.g. 2025-01-11T10:00:00.000Z) |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/crypto/deposits?limit=10' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "code": "0",
  "message": "success",
  "data": {
    "page": 1,
    "total_page": 1,
    "total_item": 1,
    "items": [
      {
        "hash": "XRPWD0000100276",
        "symbol": "XRP",
        "network": "XRP",
        "amount": "5.75111474",
        "from_address": "0xDaCd17d1E77604aaFB6e47F5Ffa1F7E35F83fDa7",
        "to_address": "0x2b0849d47a90e3c4784a5b1130a14305a099d828",
        "confirmations": 1,
        "status": "complete",
        "created_at": "2022-03-18T05:41:40.199Z",
        "completed_at": "2022-03-18T05:45:50.199Z"
      }
    ]
  }
}
```

#### Field Descriptions:
N/A

<br>

---

<br>

### GET /api/v4/crypto/withdraws

#### Description:
List crypto withdrawal history.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| page | int | false | Page (default = 1) |
| limit | int | false | Limit (default = 100, max = 200) |
| symbol | string | false | Coin symbol (e.g. BTC, ETH) |
| status | string | false | Withdrawal status: pending, processing, reported, rejected, complete |
| created_start | string | false | Start of creation time range (e.g. 2025-01-11T10:00:00.000Z) |
| created_end | string | false | End of creation time range (e.g. 2025-01-11T10:00:00.000Z) |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/crypto/withdraws?limit=10' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "code": "0",
  "message": "success",
  "data": {
    "page": 1,
    "total_page": 1,
    "total_item": 2,
    "items": [
      {
        "txn_id": "RDNTWD0000804050",
        "external_ref": "XX_1111111111",
        "hash": null,
        "symbol": "RDNT",
        "network": "ARB",
        "amount": "2.00000000",
        "fee": "4.36",
        "address": "0xDaCd17d1E77604aaFB6e47F5Ffa1F7E35F83fDa7",
        "memo": "",
        "status": "processing",
        "created_at": "2024-09-01T10:02:43.211Z",
        "completed_at": "2024-09-01T10:02:45.031Z"
      },
      {
        "txn_id": "BTCWD1321312683",
        "external_ref": "XX_1111111112",
        "hash": "0x8891b79c79f0842c9a654db47745fe0291fba222b290d22cabc93f8ae4490303",
        "symbol": "BTC",
        "network": "BTC",
        "amount": "0.10000000",
        "fee": "0.0025",
        "address": "0xDaCd17d1E77604aaFB6e47F5Ffa1F7E35F83fDa7",
        "memo": "",
        "status": "complete",
        "created_at": "2024-09-01T10:02:43.211Z",
        "completed_at": "2024-09-01T10:02:45.031Z"
      }
    ]
  }
}
```

#### Field Descriptions:
N/A

<br>

---

<br>

### GET /api/v4/crypto/compensations

#### Description:
List crypto compensation history (network issue refunds and adjustments).


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| page | int | false | Page (default = 1) |
| limit | int | false | Limit (default = 100, max = 200) |
| symbol | string | false | Coin symbol (e.g. BTC, ETH) |
| type | string | false | Compensation type: COMPENSATE or DECOMPENSATE |
| status | string | false | Status: complete |
| created_start | string | false | Start of creation time range (e.g. 2025-01-11T10:00:00.000Z) |
| created_end | string | false | End of creation time range (e.g. 2025-01-11T10:00:00.000Z) |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/crypto/compensations?symbol=ATOM' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "code": "0",
  "message": "Success",
  "data": {
    "page": 1,
    "total_page": 1,
    "total_item": 1,
    "items": [
      {
        "txn_id": "XRPCP0000001234",
        "symbol": "XRP",
        "type": "DECOMPENSATE",
        "amount": "-1",
        "status": "complete",
        "created_at": "2024-02-09T12:00:00Z",
        "completed_at": "2024-02-09T13:00:00Z",
        "user_id": "1234"
      }
    ]
  }
}
```

#### Field Descriptions:
N/A

<br>

---

<br>

### Asset Operations

### GET /api/v4/crypto/coins

#### Description:
Get all supported coins with their networks, deposit/withdrawal settings, and fee information.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| symbol | string | false | Coin symbol (e.g. BTC, ETH) |
| network | string | false | Network filter |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/crypto/coins?symbol=BTC' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "code": "0",
  "message": "success",
  "data": {
    "items": [
      {
        "name": "Bitcoin",
        "symbol": "BTC",
        "networks": [
          {
            "name": "Bitcoin",
            "network": "BTC",
            "address_regex": "^[13][a-km-zA-HJ-NP-Z1-9]{26,35}$|^(tb1)[0-9A-Za-z]{39,59}$",
            "memo_regex": "",
            "explorer": "https://www.blockchain.com/btc/tx/",
            "contract_address": "",
            "withdraw_min": "0.0002",
            "withdraw_fee": "0.0001",
            "withdraw_internal_min": "",
            "withdraw_internal_fee": "",
            "withdraw_decimal_places": 8,
            "min_confirm": 3,
            "decimal": 8,
            "deposit_enable": true,
            "withdraw_enable": true,
            "is_memo": false
          }
        ],
        "deposit_enable": true,
        "withdraw_enable": true
      }
    ]
  }
}
```

#### Field Descriptions:
N/A

<br>

---

<br>

### POST /api/v4/crypto/withdraws

#### Description:
Make a withdrawal to a trusted address.

#### Required Permission: `is_withdraw`

#### Body Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| symbol | string | true | Coin symbol (e.g. BTC, ETH) |
| amount | string | true | Amount to withdraw |
| address | string | true | Destination address |
| memo | string | false | Memo or destination tag |
| network | string | true | Network to use |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/crypto/withdraws' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}' \
--header 'Content-Type: application/json' \
--data '{"symbol":"RDNT","amount":"2.00000000","address":"0xDaCd17d1E77604aaFB6e47F5Ffa1F7E35F83fDa7","network":"ARB"}'
```

#### Response:
```json
{
  "code": "0",
  "message": "success",
  "data": {
    "txn_id": "RDNTWD0000804050",
    "symbol": "RDNT",
    "network": "ARB",
    "amount": "2.00000000",
    "fee": "4.36",
    "address": "0xDaCd17d1E77604aaFB6e47F5Ffa1F7E35F83fDa7",
    "memo": "",
    "created_at": "2024-09-01T10:02:43.211Z"
  }
}
```

#### Field Descriptions:
N/A

<br>

---

<br>

## Wallet Endpoints

### GET /api/v4/wallet/balances

#### Description:
Get wallet balances.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| segment | enum(funding) | false | Segment (default: funding) |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/wallet/balances' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "code": "0",
  "message": "success",
  "data": [
    {
      "currency": "BTC",
      "available": "1000",
      "reserved": "100",
      "total": "1100"
    },
    {
      "currency": "THB",
      "available": "10",
      "reserved": "10",
      "total": "20"
    }
  ]
}
```

#### Field Descriptions:
N/A

<br>

---

<br>

### GET /api/v4/wallet/assets

#### Description:
Get wallet assets.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| segment | enum(funding) | false | Segment (default: funding) |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/wallet/assets' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "code": "0",
  "message": "success",
  "data": {
    "BTC": "1000",
    "ETH": "100"
  }
}
```

#### Field Descriptions:
N/A

<br>

---

<br>

## Fiat Endpoints

### Account Management

### GET /api/v4/fiat/accounts

#### Description:
List approved bank accounts for the authenticated user.

#### Required Permission: `withdraw`

#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| page | int | false | Page (default = 1, min = 1) |
| limit | int | false | Limit (default = 25, max = 100) |


#### Validation Rules:
- `page` and `limit` must be provided together
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/fiat/accounts?page=1&limit=25' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "code": 0,
  "message": "success",
  "data": [
    {
      "id": "123456",
      "bank": "KBANK",
      "name": "John Doe",
      "time": 1706745600
    }
  ],
  "pagination": { "page": 1, "limit": 25 }
}
```

#### Field Descriptions:
N/A

<br>

---

<br>

### Transaction History

### GET /api/v4/fiat/deposit/history

#### Description:
List fiat deposit transaction history. Only deposit records within the last 90 days will be returned.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| page | int | false | Page (default = 1, min = 1) |
| limit | int | false | Limit (default = 25, max = 100) |


#### Validation Rules:
- `page` and `limit` must be provided together
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/fiat/deposit/history?page=1&limit=25' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "code": 0,
  "message": "success",
  "data": [
    {
      "txn_id": "THBDPXXXXXXXXXX",
      "currency": "THB",
      "amount": "10000.50",
      "status": "complete",
      "time": 1706745600
    }
  ],
  "pagination": { "page": 1, "limit": 25 }
}
```

#### Field Descriptions:
N/A

<br>

---

<br>

### GET /api/v4/fiat/withdraw/history

#### Description:
List fiat withdrawal transaction history. Only withdraw records within the last 90 days will be returned.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| page | int | false | Page (default = 1, min = 1) |
| limit | int | false | Limit (default = 25, max = 100) |


#### Validation Rules:
- `page` and `limit` must be provided together
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/fiat/withdraw/history?page=1&limit=25' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "code": 0,
  "message": "success",
  "data": [
    {
      "txn_id": "THBDPXXXXXXXXXX",
      "currency": "THB",
      "amount": "5000.00",
      "fee": "10.00",
      "status": "complete",
      "time": 1706745600
    }
  ],
  "pagination": { "page": 1, "limit": 25 }
}
```

#### Field Descriptions:
N/A

<br>

---

<br>

### Fiat Operations

### POST /api/v4/fiat/withdraw

#### Description:
Submit a fiat withdrawal request to an approved bank account.

#### Required Permission: `withdraw`

#### Body Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| bank_account_no | string | true | Approved bank account number |
| amount | double | true | Withdrawal amount (must be > 0) |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/fiat/withdraw' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}' \
--header 'Content-Type: application/json' \
--data '{"bank_account_no":"1234567890","amount":5000.00}'
```

#### Response:
```json
{
  "code": 0,
  "message": "success",
  "data": {
    "txn_id": "THBDPXXXXXXXXXX",
    "bank_account_no": "******7890",
    "currency": "THB",
    "amount": "5000.00",
    "fee": "10.00",
    "receive": "4990.00",
    "time": 1706745600
  }
}
```

#### Field Descriptions:
N/A

<br>

---

<br>

## Market Endpoints

All three market endpoints use cursor-based pagination. When `has_next` is `true`, pass the returned `cursor.id` as `next_id` and `cursor.ts` as `next_ts` to fetch the next page. `next_id` and `next_ts` must always be sent together.

### GET /api/v4/market/open-orders

#### Description:
List open orders (pending or partially filled) for the authenticated user, with optional filters and cursor-based pagination.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| symbol | string | false | Trading pair symbol (e.g. BTC_THB) |
| order_id | string | false | Filter by order ID. Requires `symbol` |
| side | string | false | Order side: buy, sell |
| type | string | false | Order type: limit, market, stoplimit |
| contain_client_id | string | false | Return only orders whose `client_id` contains this value |
| limit | int | false | Number of results (default = 10, min = 1, max = 1000) |
| next_id | string | false | Cursor ID for the next page (alphanumeric). Must be used with `next_ts` |
| next_ts | string | false | Cursor timestamp for the next page. Must be used with `next_id` |
| start_time | int | false | Start time (Unix timestamp in milliseconds) |
| end_time | int | false | End time (Unix timestamp in milliseconds) |


#### Validation Rules:
- `symbol` is required when `order_id` is provided
- `next_id` and `next_ts` must be provided together
- `next_id` must be alphanumeric
- `limit` must be between 1 and 1000
- `start_time` and `end_time` must be positive integers (milliseconds) if provided
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/market/open-orders?symbol=BTC_THB&limit=10' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "code": "0",
  "message": "success",
  "data": {
    "limit": 10,
    "has_next": true,
    "cursor": {
      "id": "68a82566596d482000f4e4ed",
      "ts": 1755850086843
    },
    "items": [
      {
        "order_id": "68a82566596d482000f4e4ed",
        "client_id": "my-order-001",
        "symbol": "BTC_THB",
        "side": "buy",
        "type": "limit",
        "status": "partial_filled",
        "partial_filled": true,
        "price": "2500000",
        "order_currency": "THB",
        "order_amount": "1000",
        "executed_currency": "THB",
        "executed_amount": "400",
        "received_currency": "BTC",
        "received_amount": "0.0001596",
        "total_fee": "1",
        "credit_used": "0",
        "fee_currency": "THB",
        "net_fee_paid": "1",
        "avg_filled_price": "2500000",
        "post_only": false,
        "order_created_at": 1755850086843,
        "order_updated_at": 1755850090120,
        "quote_amount": "1000"
      }
    ]
  }
}
```

#### Field Descriptions:
See [Order Item Fields](#order-item-fields) and [Pagination Fields](#pagination-fields).

<br>

---

<br>

### GET /api/v4/market/orders

#### Description:
List order history (all statuses) for the authenticated user, with optional filters and cursor-based pagination.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| symbol | string | false | Trading pair symbol (e.g. BTC_THB) |
| order_id | string | false | Filter by order ID. Requires `symbol` |
| side | string | false | Order side: buy, sell |
| type | string | false | Order type: limit, market, stoplimit |
| status | string | false | Order status: new, open, rejected, partial_filled, partial_filled_canceled, filled, canceled, untriggered |
| limit | int | false | Number of results (default = 10, min = 1, max = 1000) |
| next_id | string | false | Cursor ID for the next page (alphanumeric). Must be used with `next_ts` |
| next_ts | string | false | Cursor timestamp for the next page. Must be used with `next_id` |
| start_time | int | false | Start time (Unix timestamp in milliseconds) |
| end_time | int | false | End time (Unix timestamp in milliseconds) |


#### Validation Rules:
- `symbol` is required when `order_id` is provided
- `next_id` and `next_ts` must be provided together
- `next_id` must be alphanumeric
- `limit` must be between 1 and 1000
- `start_time` and `end_time` must be positive integers (milliseconds) if provided
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/market/orders?symbol=BTC_THB&status=filled&limit=10' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "code": "0",
  "message": "success",
  "data": {
    "limit": 10,
    "has_next": false,
    "cursor": {
      "id": "",
      "ts": 0
    },
    "items": [
      {
        "order_id": "68a82566596d482000f4e4ee",
        "client_id": "my-order-002",
        "symbol": "BTC_THB",
        "side": "sell",
        "type": "limit",
        "status": "filled",
        "partial_filled": false,
        "price": "2500000",
        "order_currency": "BTC",
        "order_amount": "0.001",
        "executed_currency": "BTC",
        "executed_amount": "0.001",
        "received_currency": "THB",
        "received_amount": "2493.75",
        "total_fee": "6.25",
        "credit_used": "0",
        "fee_currency": "THB",
        "net_fee_paid": "6.25",
        "avg_filled_price": "2500000",
        "post_only": false,
        "order_created_at": 1755850086843,
        "order_updated_at": 1755850087001,
        "base_amount": "0.001",
        "quote_amount": "2500"
      }
    ]
  }
}
```

#### Field Descriptions:

##### Order Item Fields

Shared by [GET /api/v4/market/open-orders](#get-apiv4marketopen-orders) and [GET /api/v4/market/orders](#get-apiv4marketorders). Optional fields are omitted from the response when they have no value.

| Field | Type | Description |
| ----- | ---- | ----------- |
| order_id | string | Order ID |
| client_id | string | Client-provided order ID (omitted if not set) |
| symbol | string | Trading pair symbol (e.g. BTC_THB) |
| side | string | Order side: buy or sell |
| type | string | Order type: limit, market or stoplimit |
| status | string | Order status: new, open, rejected, partial_filled, partial_filled_canceled, filled, canceled, untriggered |
| partial_filled | boolean | Whether the order has been partially filled |
| price | string | Limit price (omitted for market orders) |
| stop_price | string | Stop trigger price (stoplimit orders only) |
| order_currency | string | Currency of `order_amount` (quote currency for buy, base currency for sell) |
| order_amount | string | Amount placed on the order |
| executed_currency | string | Currency of `executed_amount` |
| executed_amount | string | Amount executed so far |
| received_currency | string | Currency of `received_amount` |
| received_amount | string | Amount received so far |
| total_fee | string | Total fee charged |
| credit_used | string | Trading credit used to pay the fee |
| fee_currency | string | Currency the fee is charged in |
| net_fee_paid | string | Fee paid after trading credit (`total_fee` - `credit_used`) |
| avg_filled_price | string | Average filled price |
| post_only | boolean | Whether the order is post-only |
| canceled_by | string | Who canceled the order (omitted if not canceled) |
| order_created_at | number | Order creation time (milliseconds) |
| order_triggered_at | number | Stop order trigger time (milliseconds, stoplimit orders only) |
| order_updated_at | number | Last update time (milliseconds) |
| base_amount | string | Order amount in base currency (omitted if not applicable) |
| condition | string | Stop trigger condition (stoplimit orders only) |
| quote_amount | string | Order amount in quote currency |

##### Pagination Fields

| Field | Type | Description |
| ----- | ---- | ----------- |
| limit | number | Page size used for this request |
| has_next | boolean | Whether more records are available |
| cursor.id | string | Pass as `next_id` to fetch the next page |
| cursor.ts | number | Pass as `next_ts` to fetch the next page |
| items | array | List of records |

<br>

---

<br>

### GET /api/v4/market/matched

#### Description:
List executed trades (matches) for the authenticated user, with optional filters and cursor-based pagination.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| symbol | string | false | Trading pair symbol (e.g. BTC_THB) |
| order_id | string | false | Filter by order ID. Requires `symbol` |
| txn_id | string | false | Filter by transaction ID (alphanumeric) |
| side | string | false | Order side: buy, sell |
| type | string | false | Order type: limit, market, stoplimit |
| limit | int | false | Number of results (default = 20, min = 1, max = 100) |
| next_id | string | false | Cursor ID for the next page (alphanumeric). Must be used with `next_ts` |
| next_ts | string | false | Cursor timestamp for the next page. Must be used with `next_id` |
| start_time | int | false | Start time (Unix timestamp in milliseconds) |
| end_time | int | false | End time (Unix timestamp in milliseconds) |
| archive | boolean | false | Query archived records (default = false) |


#### Validation Rules:
- `symbol` is required when `order_id` is provided
- `next_id` and `next_ts` must be provided together
- `next_id` and `txn_id` must be alphanumeric
- `limit` must be between 1 and 100
- `start_time` and `end_time` must be positive integers (milliseconds) if provided
- `start_time` must be less than `end_time` if both are provided
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v4/market/matched?symbol=BTC_THB&limit=20' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: {YOUR_API_KEY}' \
--header 'X-BTK-SIGN: {YOUR_SIGNATURE}'
```

#### Response:
```json
{
  "code": "0",
  "message": "success",
  "data": {
    "limit": 20,
    "has_next": true,
    "cursor": {
      "id": "68a82566596d482000f4e4f0",
      "ts": 1755850087001
    },
    "items": [
      {
        "order_id": "68a82566596d482000f4e4ee",
        "txn_id": "68a82566596d482000f4e4f0",
        "client_id": "my-order-002",
        "symbol": "BTC_THB",
        "type": "limit",
        "status": "filled",
        "side": "sell",
        "is_maker": true,
        "price": "2500000",
        "executed_currency": "BTC",
        "executed_amount": "0.001",
        "received_currency": "THB",
        "received_amount": "2493.75",
        "fee_rate": "0.0025",
        "total_fee": "6.25",
        "credit_used": "0",
        "fee_currency": "THB",
        "net_fee_paid": "6.25",
        "txn_ts": 1755850087001
      }
    ]
  }
}
```

#### Field Descriptions:

| Field | Type | Description |
| ----- | ---- | ----------- |
| order_id | string | Order ID |
| txn_id | string | Transaction (match) ID |
| client_id | string | Client-provided order ID (omitted if not set) |
| symbol | string | Trading pair symbol (e.g. BTC_THB) |
| type | string | Order type: limit, market or stoplimit |
| status | string | Order status |
| side | string | Order side: buy or sell |
| is_maker | boolean | Whether this side of the trade was the maker |
| price | string | Execution price |
| executed_currency | string | Currency of `executed_amount` |
| executed_amount | string | Amount executed in this trade |
| received_currency | string | Currency of `received_amount` |
| received_amount | string | Amount received from this trade |
| fee_rate | string | Fee rate applied |
| total_fee | string | Total fee charged |
| credit_used | string | Trading credit used to pay the fee |
| fee_currency | string | Currency the fee is charged in |
| net_fee_paid | string | Fee paid after trading credit (`total_fee` - `credit_used`) |
| txn_ts | number | Trade execution time (milliseconds) |

See [Pagination Fields](#pagination-fields) for `limit`, `has_next` and `cursor`.

<br>

---

<br>

## Additional

For the use of coins and networks, please use coin or network symbol for any API request. Please refer to the link below for available coins and networks: https://www.bitkub.com/fee/cryptocurrency

Note the following exceptions:

| Currency / Network  | Symbol  |
| ------------------- | ------- |
| Terra Classic (LUNC) | `LUNA`  |
| Terra 2.0 (LUNA)    | `LUNA2` |

## Error Codes

Error response format:

```json
{
  "code": "V1007-CW",
  "message": "Symbol not found",
  "data": {}
}
```

### Status Codes

| HTTP Status | Meaning |
| ----------- | ------- |
| 200 | OK — The request was processed as expected |
| 400 | INVALID_REQUEST — Request is not well-formed, violates schema, or has incorrect fields |
| 401 | NOT_AUTHORIZED — API key doesn't match the signature or lacks permissions |
| 403 | FORBIDDEN — API key doesn't have necessary permissions |
| 404 | NOT_FOUND — The requested resource doesn't exist |
| 5XX | Internal Server Error |

### Authentication Errors

| Code | Status | Message |
| ---- | ------ | ------- |
| A1000-CW | 401 | Unauthorized Access |
| A1001-CW | 403 | Permission denied |

### User Errors

N/A — V4 has no User error category.

### Trading Errors

N/A — V4 has no Trading error category.

### Withdrawal Errors

N/A — V4 has no Withdrawal error category.

### System Errors

| Code | Status | Message |
| ---- | ------ | ------- |
| S1000-CW | 500 | Internal service error |

### Business Errors

| Code | Status | Message |
| ---- | ------ | ------- |
| B1000-CW | 400 | User account is suspended |
| B1001-CW | 400 | Network is disabled |
| B1002-CW | 400 | CWS Wallet not found |
| B1003-CW | 400 | Insufficient balance |
| B1004-CW | 400 | User mismatch condition |
| B1005-CW | 400 | Duplicate key |
| B1006-CW | 400 | Airdrop already transfer |
| B1007-CW | 400 | Symbol required |
| B1008-CW | 400 | Event Symbol mismatched |
| B1009-CW | 400 | Pending withdrawal exists |
| B1010-CW | 400 | User account is frozen |
| B1011-CW | 400 | Withdrawal exceeds daily limit |
| B1012-CW | 400 | Address is not trusted |
| B1013-CW | 400 | Withdrawal is frozen |
| B1014-CW | 400 | Address is not whitelisted |
| B1015-CW | 400 | Request is processing |
| B1016-CW | 400 | Deposit is frozen |
| B1017-CW | 400 | The amount is higher than the maximum withdrawal amount |

### Market Errors

Market endpoints (`/api/v4/market/*`) return error codes with the `-MK` suffix.

| Code | Status | Message |
| ---- | ------ | ------- |
| A1000-MK | 401 | Unauthorized |
| A1001-MK | 403 | IP address not allowed |
| A1002-MK | 401 | API key is missing |
| A1003-MK | 401 | Invalid API key |
| A1004-MK | 401 | Signature is missing |
| A1005-MK | 401 | Invalid signature |
| A1006-MK | 401 | Invalid timestamp |
| A1007-MK | 401 | Timestamp expired |
| A1008-MK | 429 | Rate limit exceeded |
| B1000-MK | 404 | Order not found |
| V1000-MK | 400 | Invalid parameter |
| V1001-MK | 404 | Symbol not found |
| V1002-MK | 400 | next_id and next_ts must be provided together |
| V1003-MK | 400 | symbol is required when order_id is provided |
| V1004-MK | 400 | start_time must be less than end_time |
| V1006-MK | 400 | invalid side — must be buy or sell |
| V1008-MK | 400 | invalid order type |
| V1010-MK | 400 | invalid status value |
| V1012-MK | 400 | limit must be at least 1 |
| V1013-MK | 400 | limit exceeds maximum allowed value |
| V1015-MK | 400 | start_time must be greater than zero |
| V1017-MK | 400 | end_time must be greater than zero |
| S1000-MK | 500 | Internal server error |

### Validation Errors

| Code | Status | Message |
| ---- | ------ | ------- |
| V1000-CW | 404 | User not found |
| V1001-CW | 404 | Asset not found |
| V1002-CW | 404 | Event not found |
| V1003-CW | 400 | Invalid signature |
| V1004-CW | 401 | Signature has expired |
| V1005-CW | 404 | Transaction not found |
| V1006-CW | 400 | Invalid parameter |
| V1007-CW | 404 | Symbol not found |
| V1008-CW | 400 | Address not yet generated for this symbol |
| V1009-CW | 404 | Memo not found for this address |
| V1010-CW | 404 | Address not found |
| V1011-CW | 400 | Address already exists |
| V1012-CW | 400 | Destination address not active |
| V1015-CW | 404 | Coin not found |

## Rate Limits

Exceeding the limit blocks requests for 30 seconds (HTTP 429). Limits apply per user per endpoint regardless of API version.

| Endpoint | Rate Limit |
| -------- | ---------- |
| /api/v4/crypto/* | 250 req/10secs |
| /api/v4/wallet/balances | 150 req/sec |
| /api/v4/wallet/assets | 150 req/sec |
