
# RESTful API for Bitkub (2018-12-16)

# Releases
* 2018-12-16 Updated documentation
* 2018-08-09 V1 Release

# Table of contents
* [Base URL](#base-url)
* [Endpoint types](#endpoint-types)
* [Constructing the request](#constructing-the-request)
* [API documentation](#api-documentation)
* [Error codes](#error-codes)

# Base URL
* The base URL is: https://api.bitkub.com

# Endpoint types
### Non-secure endpoints
All non-secure endpoints do not need authentication and use the method GET.
* [GET /api/servertime](#get-apiservertime)
* [GET /api/market/symbols](#get-apimarketsymbols)
* [GET /api/market/ticker](#get-apimarketticker)
* [GET /api/market/trades](#get-apimarkettrades)
* [GET /api/market/bids](#get-apimarketbids)
* [GET /api/market/asks](#get-apimarketasks)
* [GET /api/market/trading-view](#get-apimarkettradingview)

### Secure endpoints
All secure endpoints require [authentication](#constructing-the-request) and use the method POST.
* [POST /api/market/wallet](#post-apimarketwallet)
* [POST /api/market/balances](#post-apimarketbalances)
* [POST /api/market/place-bid](#post-apimarketplace-bid)
* [POST /api/market/place-ask](#post-apimarketplace-ask)
* [POST /api/market/cancel-order](#post-apimarketcancel-order)
* [POST /api/market/my-open-orders](#post-apimarketmy-open-orders)
* [POST /api/market/my-order-history](#post-apimarketmy-order-history)

# Constructing the request
### Request header
Authentication requires API KEY and API SECRET. Every request to the server must contain the following in the request header:
* Accept: application/json
* Content-type: application/json
* X-BTK-APIKEY: {YOUR API KEY}

### Payload
The payload is always JSON. **Always include timestamp in the payload; use `ts` as the key name for timestamp**.

### Signature
Generate the signature from the JSON payload using HMAC SHA-256. Use the API SECRET as the secret key for generating the HMAC variant of JSON payload.

#### Example payload:
```javascript
{"sym":"THB_BTC","amt":0.1,"rat":10000,"typ":"limit","ts":1529490685}
```

#### Example payload with signature:
```javascript
{"sym":"THB_BTC","amt":0.1,"rat":10000,"typ":"limit","ts":1529490685,"sig":"d0c66fabb816c46953270e4a442836ca449711e143c8658dd03103c90b2d0fb7"}
```

#### Example cURL:
```javascript
curl -X POST \
  https://api.bitkub.com/api/market/place-ask \
  -H 'accept: application/json' \
  -H 'content-type: application/json' \
  -H 'x-btk-apikey: 6da634977495306b2206eee7772532cb' \
  -d '{"sym":"THB_BTC","amt":0.1,"rat":10000,"typ":"limit","ts":1529490685,"sig":"d0c66fabb816c46953270e4a442836ca449711e143c8658dd03103c90b2d0fb7"}'
```

# API documentation
Refer to the following for description of each endpoint

### GET /api/servertime

#### Description:
Get server timestamp.

#### Query:
-

#### Response:
```javascript
1529999999
```

### GET /api/market/symbols

#### Description:
List all available symbols.

#### Query:
-

#### Response:
```javascript
{
  error: 0,
  result: [
    {
      id: 1,
      symbol: "THB_BTC",
      info: "Thai Baht to Bitcoin"
    },
    {
      id: 2,
      symbol: "THB_ETH",
      info: "Thai Baht to Ethereum"
    }
  ]
}
```

### GET /api/market/ticker

#### Description:
Get ticker information.

#### Query:
-

#### Response:
```javascript
{
  THB_BTC: {
    id: 1,
    last: "216415.00",
    lowestAsk: "216678.00",
    highestBid: "215000.00",
    percentChange: "1.91",
    baseVolume: "71.02603946",
    quoteVolume: "15302897.99",
    isFrozen: "0",
    high24hr: "221396.00",
    low24hr: "206414.00"
  },
  THB_ETH: {
    id: 2,
    last: "11878.00",
    lowestAsk: "12077.00",
    highestBid: "11893.00",
    percentChange: "-0.49",
    baseVolume: "455.17839270",
    quoteVolume: "5505664.42",
    isFrozen: "0",
    high24hr: "12396.00",
    low24hr: "11645.00"
  }
}
```

### GET /api/market/trades

#### Description:
List recent trades.

#### Query:
* `sym`		**string** The symbol
* `lmt`		**int** No. of limit to query recent trades

#### Response:
```javascript
{
  "error": 0,
  "result": [
    [
      1529516287, // timestamp
      "10000.00", // rate
      "0.09975000", // amount
      "BUY" // side
    ]
  ]
}
```

### GET /api/market/bids

#### Description:
List open buy orders.

#### Query:
* `sym` **string** The symbol
* `lmt` **int** No. of limit to query open buy orders

#### Response:
```javascript
{
  "error": 0,
  "result": [
    [
      1, // order id
      1529453033, // timestamp
      "997.50", // volume
      "10000.00", // rate
      "0.09975000" // amount
    ]
  ]
}

```

### GET /api/market/asks

#### Description:
List open sell orders.

#### Query:
* `sym` **string** The symbol
* `lmt` **int** No. of limit to query open sell orders

#### Response:
```javascript
{
  "error": 0,
  "result": [
    [
      680, // order id
      1529491094, // timestamp
      "997.50", // volume
      "10000.00", // rate
      "0.09975000" // amount
    ]
  ]
}
```

### GET /api/market/tradingview
#### Description:
Get tradingview data for displaying tradingview graph.

#### Query:
* `sym`		**string**		The symbol
* `int`		**int**		Chart interval in minutes
* `frm`		**int**		Timestamp of the starting time (from)
* `to`		**int**		Timestamp of the ending time (to)

#### Response:
```javascript
{
  "error": 0,
  "result": [
    o: [
      207500
    ],
    c: [
      205000
    ],
    h: [
      207500
    ],
    l: [
      205000
    ],
    s: "ok",
    v: [
      0.11205317
    ],
    t: [
      1530460800
    ]
  ]
}
```

### POST /api/market/wallet

#### Description:
Get user wallet info.

#### Query:
-

#### Response:
```javascript
{
  "error": 0,
  "result": {
    "THB": 188379.27,
    "BTC": 8.90397323,
    "ETH": 10.1
  }
}
```

### POST /api/market/balances

#### Description:
Get balances info: this includes both available and reserved balances.

#### Query:
-

#### Response:
```javascript
{
  "error": 0,
  "result": {
    "THB":  {
      "available": 188379.27,
      "reserved": 0
    },
    "BTC": {
      "available": 8.90397323,
      "reserved": 0
    },
    "ETH": {
      "available": 10.1,
      "reserved": 0
    }
  }
}
```

### POST /api/market/place-bid

#### Description:
Create a buy order.

#### Query:
* `sym`		**string**		The symbol
* `amt`		**float**		Amount you want to spend with no trailing zero (e.g 1000.00 is invalid, 1000 is ok)
* `rat`		**float**		Rate you want for the order with no trailing zero (e.g 1000.00 is invalid, 1000 is ok)
* `typ`		**string**		Order type: limit or market

#### Response:
```javascript
{
  "error":0,
  "result":
  {
    "id": 1, // order id
    "typ": "limit", // order type
    "amt": 1000, // spending amount
    "rat": 15000, // rate
    "fee": 2.5, // fee
    "cre": 2.5, // fee credit used
    "rec": 0.06666666, // amount to receive
    "ts": 1533834547 // timestamp
  }
}
```

### POST /api/market/place-ask

#### Description:
Create a sell order.

#### Query:
* `sym`		**string**		The symbol
* `amt`		**float**		Amount you want to sell with no trailing zero (e.g 0.10000000 is invalid, 0.1 is ok)
* `rat`		**float**		Rate you want for the order with no trailing zero (e.g 1000.00 is invalid, 1000 is ok)
* `typ`		**string**		Order type: limit or market

#### Response:
```javascript
{
  "error": 0,
  "result": {
    "id": 1, // order id
    "typ": "limit", // order type
    "amt": 1.00000000, // selling amount
    "rat": 15000, // rate
    "fee": 37.5, // fee
    "cre": 37.5, // fee credit used
    "rec": 15000, // amount to receive
    "ts": 1533834844 // timestamp
  }
}
```

### POST /api/market/cancel-order

### Description:
Cancel an open order.

### Query:
* `sym`		**string**		The symbol
* `id`		**int**		Order id you wish to cancel
* `sd`		**string**		Order side: buy or sell

### Response:
```javascript
{
  "error": 0
}
```

### POST /api/market/my-open-orders

### Description:
List all open orders of the given symbol.

### Query:
* `sym`		**string**		The symbol

### Response:
```javascript
{
  "error": 0,
  "result": [
    {
      "id": 2, // order id
      "side": "SELL", // order side
      "type": "limit", // order type
      "rate": 15000, // rate
      "fee": 35.01, // fee
      "credit": 35.01, // credit used
      "amount": 0.93333334, // amount
      "receive": 14000, // amount to receive
      "parent_id": 1, // parent order id
      "super_id": 1, // super parent order id
      "ts": 1533834844 // timestamp
    }
  ]
}
```

### POST /api/market/my-order-history

### Description:
List all orders that have already matched.

### Query:
* `sym` **string** The symbol
* `p` **int** Page (optional)
* `lmt` **int** Limit (optional)

### Response:
```javascript
{
  "error": 0,
  "result": [
    {
      "txn_id": "ETHBUY0000000197",
      "order_id": 240,
      "parent_order_id": 0,
      "super_order_id": 0,
      "taken_by_me": true,
      "side": "buy",
      "type": "limit",
      "rate": 13335.57,
      "fee": 0.34,
      "credit": 0.34,
      "amount": 0.00999987,
      "ts": 1531513395
    }
  ],
  "pagination": {
      "page": 2,
      "last": 3,
      "next": 3,
      "prev": 1
  }
}
```


# Error codes
Refer to the following descriptions:

Code | Description
------------ | ------------
0 | No error
1 | Invalid JSON payload
2 | Missing X-BTK-APIKEY
3 | Invalid API key
4 | API pending for activation
5 | IP not allowed
6 | Missing / invalid signature
7 | Missing timestamp
8 | Invalid timestamp
9 | Invalid user
10 | Invalid parameter
11 | Invalid symbol
12 | Invalid amount
13 | Invalid rate
14 | Improper rate
15 | Amount too low
16 | Failed to get balance
17 | Wallet is empty
18 | Insufficient balance
19 | Failed to insert order into db
20 | Failed to deduct balance
21 | Invalid order for cancellation
22 | Invalid side
23 | Failed to update order status
30 | Limit exceeds
40 | Pending withdrawal exists
41 | Invalid currency for withdrawal
42 | Address is not in whitelist
43 | Failed to deduct crypto
44 | Failed to create withdrawal record
