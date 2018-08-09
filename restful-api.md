
# RESTful API for Bitkub (2018-08-09)

# Base URL
* The base URL is: https://api.bitkub.com

# Endpoint types
### Non-secure endpoints
All non-secure endpoints do not need authentication and use the method GET.
* GET /api/servertime
* GET /api/market/symbols
* GET /api/market/trades
* GET /api/market/bids
* GET /api/market/asks

### Secure endpoints
All secure endpoints require authentication and use the method POST.
* POST /api/market/wallet
* POST /api/market/balances
* POST /api/market/place-bid
* POST /api/market/place-ask
* POST /api/market/cancel-order
* POST /api/market/my-open-orders
* POST /api/market/my-order-history

# Constructing the request
### Request header
Authentication requires API KEY and API SECRET. Every request to the server must contain the following in the request header:
* Accepts: application/json
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

### GET /api/market/symbols

#### Description:
List all available symbols.

#### Query:
-

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
List open sell orders.

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
