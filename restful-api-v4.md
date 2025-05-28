# RESTful API for Bitkub V4 (2025-02-03)

# Announcement

- Introducing the New Public API v4 for Crypto Endpoints

# Change log

- 2025-05-27 Added new Crypto endpoint [GET /api/v4/crypto/compensations](#get-apiv4cryptocompensations) and update api specification for [GET /api/v4/crypto/withdraws](#get-apiv4cryptowithdraws) and [GET /api/v4/crypto/deposits](#get-apiv4cryptodeposits)
- 2025-04-08 Added new error codes: [B1016-CW] Deposit is frozen, [V1015-CW] Coin not found
- 2025-04-03 Added new Crypto endpoint [GET /api/v4/crypto/coins](#get-apiv4cryptocoins)
- 2025-02-03 Introducing Crypto V4 Endpoints

# Table of contents

- [Base URL](#base-url)
- [Endpoint types](#endpoint-types)
- [Constructing the request](#constructing-the-request)
- [API documentation](#api-documentation)
- [Error codes](#error-codes)
- [Rate limits](#rate-limits)

# Base URL

- The base URL is: https://api.bitkub.com

# Endpoint types

### Secure endpoints V4

All secure endpoints require [authentication](#constructing-the-request).

| Crypto V4 Endpoints                                           | Method | Deposit | Withdraw | Trade |
| ------------------------------------------------------------- | ------ | ------- | -------- | ----- |
| [/api/v4/crypto/addresses](#get-apiv4cryptoaddresses)         | GET    |         |          |       |
| [/api/v4/crypto/addresses](#post-apiv4cryptoaddresses)        | POST   | ✅      |          |       |
| [/api/v4/crypto/deposits](#get-apiv4cryptodeposits)           | GET    |         |          |       |
| [/api/v4/crypto/withdraws](#get-apiv4cryptowithdraws)         | GET    |         |          |       |
| [/api/v4/crypto/withdraws](#post-apiv4cryptowithdraws)        | POST   |         | ✅       |       |
| [/api/v4/crypto/coins](#get-apiv4cryptocoins)                 | GET    |         |          |       |
| [/api/v4/crypto/compensations](#get-apiv4cryptocompensations) | GET    |         |          |       |

# Constructing the request

### GET/POST request

- GET requests require parameters as **query string** in the URL (e.g. ?symbol=BTC&limit=10).
- POST requests require JSON payload (application/json).

### Request headers (Secure Endpoints)

Authentication requires API KEY and API SECRET. Every request to the server must contain the following in the request header:

- Accept: application/json
- Content-type: application/json
- X-BTK-APIKEY: {YOUR API KEY}
- X-BTK-TIMESTAMP: {Timestamp i.e. 1699376552354 }
- X-BTK-SIGN: [Signature](#signature)

### Signature

Generate the signature from the timestamp, the request method, API path, query parameter, and JSON payload using HMAC SHA-256. Use the API Secret as the secret key for generating the HMAC variant of JSON payload. The signature is in **hex** format. The user has to attach the signature via the Request Header
You must get a new timestamp in millisecond from [/api/v3/servertime](restful-api.md#get-apiv3servertime). The old one is in second.

#### Example string for signing a signature:

```javascript
//Example for Get Method
1699381086593GET/api/v4/crypto/addresses?symbol=ATOM

// Example for Post Method
1699376552354POST/api/v4/crypto/addresses{"symbol":"ATOM","network": "ATOM"}
```

#### Example cURL:

```javascript
curl --location 'https://api.bitkub.com/api/v4/crypto/addresses?symbol=ATOM' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: e286825bda3497ae2d03aa3a30c420d603060cb4edbdd3ec711910c86966e9ba' \
--header 'X-BTK-SIGN: f5884963865a6e868ddbd58c9fb9ea4bd013076e8a8fa51d38b86c38d707cb8a'
```

```javascript
curl --location 'https://api.bitkub.com/api/v4/crypto/addresses' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: e286825bda3497ae2d03aa3a30c420d603060cb4edbdd3ec711910c86966e9ba' \
--header 'X-BTK-SIGN: f5884963865a6e868ddbd58c9fb9ea4bd013076e8a8fa51d38b86c38d707cb8a' \
--header 'Content-Type: application/json' \
--data '{
    "symbol": "ATOM",
    "network": "ATOM",
}'
```

# API documentation

Refer to the following for description of each endpoint

### GET /api/v4/crypto/addresses

#### Description:

List all crypto addresses (paginated).

#### Path Params: -

#### Query Params:

| Key     | Type   | Required | Description                      |
| ------- | ------ | -------- | -------------------------------- |
| page    | int    | false    | Page (default = 1)               |
| limit   | int    | false    | Limit (default = 100, max = 200) |
| symbol  | String | false    | e.g. ATOM                        |
| network | String | false    | e.g. ATOM                        |
| memo    | String | false    | e.g. 107467228685                |

#### Body Params: -

#### Example cURL:

```javascript
curl --location 'https://api.bitkub.com/api/v4/crypto/addresses?symbol=ATOM' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: e286825bda3497ae2d03aa3a30c420d603060cb4edbdd3ec711910c86966e9ba' \
--header 'X-BTK-SIGN: f5884963865a6e868ddbd58c9fb9ea4bd013076e8a8fa51d38b86c38d707cb8a'
```

#### Response:

```javascript
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
      },
      {
        "symbol": "ATOM",
        "network": "ATOM",
        "address": "cosmos1jcslcmz2lpsy7uq5u2ktn459qce2chqapey7gh",
        "memo": "104010164476",
        "created_at": "2022-03-18T05:46:34.113Z"
      }
    ]
  }
}
```

### POST /api/v4/crypto/addresses

#### Description:

Generate a new crypto address (if an address exists; will return the existing address).

#### Required Permission: `is_deposit`

#### Path Params: -

#### Query Params: -

#### Body Params:

| Key     | Type   | Required | Description |
| ------- | ------ | -------- | ----------- |
| symbol  | String | true     | e.g. ATOM   |
| network | String | true     | e.g. ATOM   |

#### Example cURL:

```javascript
curl --location 'https://api.bitkub.com/api/v4/crypto/addresses' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: e286825bda3497ae2d03aa3a30c420d603060cb4edbdd3ec711910c86966e9ba' \
--header 'X-BTK-SIGN: f5884963865a6e868ddbd58c9fb9ea4bd013076e8a8fa51d38b86c38d707cb8a' \
--header 'Content-Type: application/json' \
--data '{
    "symbol": "ETH",
    "network": "ETH",
}'
```

#### Response:

```javascript
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

### GET /api/v4/crypto/deposits

#### Description:

List crypto deposit history.

#### Path Params: -

#### Query Params:

| Key           | Type   | Required | Description                                                                                                                                                                |
| ------------- | ------ | -------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| page          | int    | false    | Page (default = 1)                                                                                                                                                         |
| limit         | int    | false    | Limit (default = 100, max = 200)                                                                                                                                           |
| created_start | String | false    | The start of the time range for the transaction creation timestamp. Only transactions created on or after this timestamp will be included. (e.g. 2025-01-11T10:00:00.000Z) |
| created_end   | String | false    | The end of the time range for the transaction creation timestamp. Only transactions created on or before this timestamp will be included. (e.g. 2025-01-11T10:00:00.000Z)  |

#### Body Params: -

#### Example cURL:

```javascript
curl --location 'https://api.bitkub.com/api/v4/crypto/deposits?limit=10' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: e286825bda3497ae2d03aa3a30c420d603060cb4edbdd3ec711910c86966e9ba' \
--header 'X-BTK-SIGN: f5884963865a6e868ddbd58c9fb9ea4bd013076e8a8fa51d38b86c38d707cb8a'
```

#### Response:

```javascript
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
        "from_address": "0x8b5B4E70BFCB3784f1c1157A50bd5f103c4b0102",
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

### GET /api/v4/crypto/withdraws

#### Description:

List crypto withdrawal history.

#### Path Params: -

#### Query Params:

| Key           | Type   | Required | Description                                                                                                                                                                |
| ------------- | ------ | -------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| page          | int    | false    | Page (default = 1)                                                                                                                                                         |
| limit         | int    | false    | Limit (default = 100, max = 200)                                                                                                                                           |
| created_start | String | false    | The start of the time range for the transaction creation timestamp. Only transactions created on or after this timestamp will be included. (e.g. 2025-01-11T10:00:00.000Z) |
| created_end   | String | false    | The end of the time range for the transaction creation timestamp. Only transactions created on or before this timestamp will be included. (e.g. 2025-01-11T10:00:00.000Z)  |

#### Body Params: -

#### Example cURL:

```javascript
curl --location 'https://api.bitkub.com/api/v4/crypto/withdraws?limit=10' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: e286825bda3497ae2d03aa3a30c420d603060cb4edbdd3ec711910c86966e9ba' \
--header 'X-BTK-SIGN: f5884963865a6e868ddbd58c9fb9ea4bd013076e8a8fa51d38b86c38d707cb8a'
```

#### Response:

```javascript
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
        "address": "0x8b5B4E70BFCB3784f1c1157A50bd5f103c4b0102",
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
        "address": "0x8b5B4E70BFCB3784f1c1157A50bd5f103c4b0102",
        "memo": "",
        "status": "complete",
        "created_at": "2024-09-01T10:02:43.211Z",
        "completed_at": "2024-09-01T10:02:45.031Z"
      }
    ]
  }
}
```

### POST /api/v4/crypto/withdraws

#### Description:

Make a withdrawal to a trusted address.

#### Required Permission: `is_withdraw`

#### Path Params: -

#### Query Params: -

#### Body Params:

| Key     | Type   | Required | Description                           |
| ------- | ------ | -------- | ------------------------------------- |
| symbol  | String | true     | Symbol for withdrawal (e.g. BTC, ETH) |
| amount  | String | true     | Amount to withdraw                    |
| address | String | true     | Address to withdraw                   |
| memo    | String | false    | Memo or destination tag to withdraw   |
| network | String | true     | Network to withdraw                   |

#### Example cURL:

```javascript
curl --location 'https://api.bitkub.com/api/v4/crypto/withdraws' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: e286825bda3497ae2d03aa3a30c420d603060cb4edbdd3ec711910c86966e9ba' \
--header 'X-BTK-SIGN: f5884963865a6e868ddbd58c9fb9ea4bd013076e8a8fa51d38b86c38d707cb8a' \
--header 'Content-Type: application/json' \
--data '{
    "symbol": "RDNT",
    "amount": "2.00000000",
    "address": "0x8b5B4E70BFCB3784f1c1157A50bd5f103c4b0102",
    "network": "ARB"
}'
```

#### Response:

```javascript
{
  "code": "0",
  "message": "success",
  "data": {
    "txn_id": "RDNTWD0000804050",
    "symbol": "RDNT",
    "network": "ARB",
    "amount": "2.00000000",
    "fee": "4.36",
    "address": "0x8b5B4E70BFCB3784f1c1157A50bd5f103c4b0102",
    "memo": "",
    "created_at": "2024-09-01T10:02:43.211Z"
  }
}
```

### GET /api/v4/crypto/coins

#### Description:

Get all coins (available for deposit and withdraw).

#### Path Params: -

#### Query Params:

| Key     | Type   | Required | Description                 |
| ------- | ------ | -------- | --------------------------- |
| symbol  | String | false    | Coin Symbol (e.g. BTC, ETH) |
| network | String | false    | Network                     |

#### Body Params: -

#### Example cURL:

```javascript
curl --location 'https://api.bitkub.com/api/v4/crypto/coin?symbol=ATOM' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: e286825bda3497ae2d03aa3a30c420d603060cb4edbdd3ec711910c86966e9ba' \
--header 'X-BTK-SIGN: f5884963865a6e868ddbd58c9fb9ea4bd013076e8a8fa51d38b86c38d707cb8a'
```

#### Response:

```javascript
{
  "code": "0",
  "message": "success",
  "data": {
    "items":[
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

### GET /api/v4/crypto/compensations

#### Description:

List crypto compensations history.

#### Path Params: -

#### Query Params:

| Key           | Type   | Required | Description                                                                                                                                                                |
| ------------- | ------ | -------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| page          | int    | false    | Page (default = 1)                                                                                                                                                         |
| limit         | int    | false    | Limit (default = 100, max = 200)                                                                                                                                           |
| symbol        | String | false    | Coin Symbol (e.g. BTC, ETH)                                                                                                                                                |
| type          | String | false    | Compensation Type (COMPENSATE,DECOMPENSATE)                                                                                                                                |
| status        | String | false    | Transaction Compensation Status (PENDING, COMPLETED)                                                                                                                       |
| created_start | String | false    | The start of the time range for the transaction creation timestamp. Only transactions created on or after this timestamp will be included. (e.g. 2025-01-11T10:00:00.000Z) |
| created_end   | String | false    | The end of the time range for the transaction creation timestamp. Only transactions created on or before this timestamp will be included. (e.g. 2025-01-11T10:00:00.000Z)  |

#### Body Params: -

#### Example cURL:

```javascript
curl --location 'https://api.bitkub.com/api/v4/crypto/compensations?symbol=ATOM' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: e286825bda3497ae2d03aa3a30c420d603060cb4edbdd3ec711910c86966e9ba' \
--header 'X-BTK-SIGN: f5884963865a6e868ddbd58c9fb9ea4bd013076e8a8fa51d38b86c38d707cb8a'
```

#### Response:

```javascript
 {
  "code": "0",
  "message": "success",
  "data": {
    "page": 1,
    "total_page": 1,
    "total_item": 21,
    "items": [
      {
        "txn_id": "XRPCP1234",
        "symbol": "XRP",
        "type": "DECOMPENSATE",
        "amount": "-1",
        "status": "COMPLETED",
        "created_at": "2024-02-09T12:00:00.000+00:00",
        "completed_at": "2024-02-09T13:00:00.000+00:00"
      },
      {
        "txn_id": "BLUECP1234",
        "symbol": "BLUE",
        "type": "COMPENSATE",
        "amount": "20",
        "status": "COMPLETED",
        "created_at": "2025-04-09T18:30:04.000+07:00",
        "completed_at": "2025-04-09T18:30:04.000+07:00"
      }
    ]
  }
}
```

## Additional

For the use of coins and networks, please use coin or network symbol for any APIs request. Please be cautious of these cryptocurrency when you specified on the request.\
\
Please refer to the link below for the available coins and networks.\
https://www.bitkub.com/fee/cryptocurrency \
\
Note the following exceptions for the coin and network:

| Currency / Network  | Symbol  |
| ------------------- | ------- |
| Terra Classic(LUNC) | `LUNA`  |
| Terra 2.0 (LUNA)    | `LUNA2` |

# Error codes

The following is the JSON payload for the Response Error:

```javascript
{
   "code": "V1007-CW",
   "message": "Symbol not found",
   "data": {}
 }
```

## Status Codes

#### 200 (OK)

The request was processed as expected.

#### 400 (INVALID_REQUEST)

The request is not well-formed, violates schema, or has incorrect fields.

#### 401 (NOT_AUTHORIZED)

The API key doesn't match the signature or have the necessary permissions to perform the request.

#### 403 (FORBIDDEN)

The API key doesn't have the necessary permissions to complete the request.

#### 404 (NOT_FOUND)

The requested resource doesn't exist.

#### 5XX

Internal Server Error.

| Code     | Status | Message                |
| -------- | ------ | ---------------------- |
| S1000-CW | 500    | Internal service error |

### Business Error

| Code     | Status | Message                        |
| -------- | ------ | ------------------------------ |
| B1000-CW | 400    | User account is suspended      |
| B1001-CW | 400    | Network is disabled            |
| B1002-CW | 400    | CWS Wallet not found           |
| B1003-CW | 400    | Insufficient balance           |
| B1004-CW | 400    | User mismatch condition        |
| B1005-CW | 400    | Duplicate key                  |
| B1006-CW | 400    | Airdrop already transfer       |
| B1007-CW | 400    | Symbol required                |
| B1008-CW | 400    | Event Symbol mismatched        |
| B1009-CW | 400    | Pending withdrawal exists      |
| B1010-CW | 400    | User account is frozen         |
| B1011-CW | 400    | Withdrawal exceeds daily limit |
| B1012-CW | 400    | Address is not trusted         |
| B1013-CW | 400    | Withdrawal is frozen           |
| B1014-CW | 400    | Address is not whitelisted     |
| B1015-CW | 400    | Request is processing          |
| B1016-CW | 400    | Deposit is frozen              |

### Validation Error

| Code     | Status | Message                                   |
| -------- | ------ | ----------------------------------------- |
| V1000-CW | 404    | User not found                            |
| V1001-CW | 404    | Asset not found                           |
| V1002-CW | 404    | Event not found                           |
| V1003-CW | 400    | Invalid signature                         |
| V1004-CW | 401    | Signature has expired                     |
| V1005-CW | 404    | Transaction not found                     |
| V1006-CW | 400    | Invalid parameter                         |
| V1007-CW | 404    | Symbol not found                          |
| V1008-CW | 400    | Address not yet generated for this symbol |
| V1009-CW | 404    | Memo not found for this address           |
| V1010-CW | 404    | Address not found                         |
| V1011-CW | 400    | Address already exists                    |
| V1012-CW | 400    | Destination address not active            |
| V1015-CW | 404    | Coin not found                            |

### Authentication Error

| Code     | Status | Message             |
| -------- | ------ | ------------------- |
| A1000-CW | 401    | Unauthorized Access |
| A1001-CW | 403    | Permission denied   |

# Rate limits

If the request rate exceeds the limit in any endpoints, the request will be blocked for 30 seconds. When blocked, HTTP response is 429 Too Many Requests. The limits apply to individual user accessing the API. **_The rate limit is applied to each endpoint regardless the API version._**

| Endpoint          | Rate Limit        |
| ----------------- | ----------------- |
| /api/v4/crypto/\* | 250 req / 10 secs |
