# REST API V4

## Announcement

* Deposit history records are available for the last 90 days only for GET /api/v4/crypto/deposits. Records older than 90 days are archived.

## Change Log

* 2026-04-07 Introducing Fiat V4 Endpoints
* 2025-05-27 Added GET /api/v4/crypto/compensations, updated spec for crypto/withdraws and crypto/deposits
* 2025-04-08 Added new error codes: B1016-CW Deposit is frozen, V1015-CW Coin not found
* 2025-04-03 Added GET /api/v4/crypto/coins
* 2025-02-03 Introducing Crypto V4 Endpoints

## Overview

V4 is the current REST API for crypto and fiat deposit/withdrawal operations. It uses the same HMAC authentication as V3 but returns structured error codes (e.g. `V1007-CW`) instead of numeric codes.

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
    "total_item": 2,
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
      }
    ]
  }
}
```

#### Field Descriptions:
N/A
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
    "total_item": 2,
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
## Fiat Endpoints

### Account Management

### GET /api/v4/fiat/accounts

#### Description:
List approved bank accounts for the authenticated user. Both `page` and `limit` must be provided together.

#### Required Permission: `withdraw`

#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| page | int | false | Page (default = 1, min = 1) |
| limit | int | false | Limit (default = 25, max = 100) |


#### Validation Rules:
N/A
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
### Transaction History

### GET /api/v4/fiat/deposit/history

#### Description:
List fiat deposit transaction history. Both `page` and `limit` must be provided together.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| page | int | false | Page (default = 1, min = 1) |
| limit | int | false | Limit (default = 25, max = 100) |


#### Validation Rules:
N/A
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
### GET /api/v4/fiat/withdraw/history

#### Description:
List fiat withdrawal transaction history. Both `page` and `limit` must be provided together.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| page | int | false | Page (default = 1, min = 1) |
| limit | int | false | Limit (default = 25, max = 100) |


#### Validation Rules:
N/A
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

### Numeric Errors

N/A — V4 uses structured string error codes (e.g. `V1007-CW`), not numeric codes.

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

### Authentication Errors

| Code | Status | Message |
| ---- | ------ | ------- |
| A1000-CW | 401 | Unauthorized Access |
| A1001-CW | 403 | Permission denied |

## Rate Limits

Exceeding the limit blocks requests for 30 seconds (HTTP 429). Limits apply per user per endpoint regardless of API version.

| Endpoint | Rate Limit |
| -------- | ---------- |
| /api/v4/crypto/* | 250 req/10secs |
| /api/v4/fiat/* | 250 req/10secs |
