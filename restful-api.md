
# RESTful API for Bitkub (2025-04-18)

# Announcement
* order_id and txn_id formats of my-open-orders, my-order-history, my-order-info, place-bid, place-ask, cancel-order may change for some symbols due to a system upgrade, See affected symbols and detail : [here](https://support.bitkub.com/en/support/solutions/articles/151000214886-announcement-trading-system-upgrade)
* API Specifications for Crypto Endpoints, please refer to the documentation here: [Crypto Endpoints](restful-api-v4.md)
* Deprecation of Order Hash for [my-open-orders](#get-apiv3marketmy-open-orders), [my-order-history](#get-apiv3marketmy-order-history), [my-order-info](#get-apiv3marketorder-info), [place-bid](#post-apiv3marketplace-bid), [place-ask](#post-apiv3marketplace-ask), [cancel-order](#post-apiv3marketcancel-order) on 28/02/2025 onwards, More details [here](https://support.bitkub.com/en/support/solutions/articles/151000205895-notice-deprecation-of-order-hash-from-public-api-on-28-02-2025-onwards)

# Change log
* 2025-01-07 Update FIAT Withdraw error code
* 2025-04-03 Deprecated Crypto Endpoint v3 and Remove from the Document.
* 2024-12-20 Introducing the Enhanced Market Data Endpoint [Ticker, Depth, Bids, Asks, Trades](#non-secure-endpoints-v3)
* 2024-07-25 Deprecated Secure Endpoint V1/V2 and Remove from the Document.
* 2024-07-05 Update rate-limits of place-bid, place-ask, cancel-order, my-open-orders  [Rate-Limits](#rate-limits)
* 2024-07-05 Update rate-limits which will be apply on 17 July 2024 [Rate-Limits](#rate-limits)
* 2024-06-11 Updated API request of [POST /api/v3/crypto/internal-withdraw](#post-apiv3cryptointernal-withdraw) and edited API response of [POST /api/v3/crypto/withdraw-history](#post-apiv3cryptowithdraw-history)
* 2024-06-11 Added new error code 58 - Transaction Not Found
* 2024-05-16 Release: Post-Only Functionality Added to [POST /api/v3/market/place-bid](#post-apiv3marketplace-bid) and [POST /api/v3/market/place-ask](#post-apiv3marketplace-ask)
* 2024-03-06 Edited Request field for [POST /api/v3/crypto/withdraw](#post-apiv3cryptowithdraw)
* 2024-02-15 Edited Endpoint permission [Permission Table](#secure-endpoints-v3)


# Table of contents
* [Base URL](#base-url)
* [Endpoint types](#endpoint-types)
* [Constructing the request](#constructing-the-request)
* [API documentation](#api-documentation)
* [Error codes](#error-codes)
* [Rate limits](#rate-limits)


# Base URL
* The base URL is: https://api.bitkub.com

# Endpoint types
### Non-secure endpoints
Our existing endpoints remain available for use. However, for enhanced security and performance, we strongly recommend utilizing the new [Non-Secure Endpoint V3](#non-secure-endpoints-v3).
* [GET /api/status](#get-apistatus)
* [GET /api/servertime](#get-apiservertime)
* [GET /api/market/symbols](#get-apimarketsymbols)
* [GET /api/market/ticker](#get-apimarketticker)
* [GET /api/market/trades](#get-apimarkettrades)
* [GET /api/market/bids](#get-apimarketbids)
* [GET /api/market/asks](#get-apimarketasks)
* [GET /api/market/books](#get-apimarketbooks)
* [GET /api/market/depth](#get-apimarketdepth)
* [GET /tradingview/history](#get-tradingviewhistory)
* [GET /api/v3/servertime](#get-apiv3servertime)

### Non-secure endpoints V3
| Market Data Endpoint                                          | Method |
| --------------------------------------------------------------| ------ |
| [GET /api/v3/market/ticker](#get-apiv3marketticker)           | GET    |
| [GET /api/v3/market/bids](#get-apiv3marketbids)               | GET    |
| [GET /api/v3/market/asks](#get-apiv3marketasks)               | GET    |
| [GET /api/v3/market/depth](#get-apiv3marketdepth)             | GET    |
| [GET /api/v3/market/trades](#get-apiv3markettrades)           | GET    |

| Exchange Information Endpoint                                 | Method |
| --------------------------------------------------------------| ------ |
| [GET /api/v3/servertime](#get-apiv3servertime)                | GET    |


### Secure endpoints V3
All secure endpoints require [authentication](#constructing-the-request).

| User Endpoint                                                             | Method | Trade | Deposit | Withdraw |
| ------------------------------------------------------------------------- | ------ | ----- | ------- | -------- |
| [/api/v3/user/trading-credits](#post-apiv3usertrading-credits)            | POST   |       |         |          |
| [/api/v3/user/limits](#post-apiv3userlimits)                              | POST   |       |         |          |
| [/api/v3/user/coin-convert-history](#get-apiv3usercoin-convert-history)   | GET    |       |         |          |

| Trading Endpoint                                                     | Method | Trade | Deposit | Withdraw |
| ------------------------------------------------------------------- | ------ | ----- | ------- | -------- |
| [/api/v3/market/wallet](#post-apiv3marketwallet)                    | POST   |       |         |          |
| [/api/v3/market/balances](#post-apiv3marketbalances)                | POST   |       |         |          |
| [/api/v3/market/place-bid](#post-apiv3marketplace-bid)              | POST   | ✅     |         |          |
| [/api/v3/market/place-ask](#post-apiv3marketplace-ask)              | POST   | ✅     |         |          |
| [/api/v3/market/cancel-order](#post-apiv3marketcancel-order)        | POST   | ✅     |         |          |
| [/api/v3/market/wstoken](#post-apiv3marketwstoken)                  | POST   | ✅     |         |          |
| [/api/v3/market/my-open-orders](#get-apiv3marketmy-open-orders)     | GET    |        |         |          |
| [/api/v3/market/my-order-history](#get-apiv3marketmy-order-history) | GET    |        |         |          |
| [/api/v3/market/order-info](#get-apiv3marketorder-info)             | GET    |       |         |          |

| Fiat Endpoint                                                    | Method | Trade | Deposit | Withdraw |
| ---------------------------------------------------------------- | ------ | ----- | ------- | -------- |
| [/api/v3/fiat/accounts](#post-apiv3fiataccounts)                 | POST   |       |         | ✅       |
| [/api/v3/fiat/withdraw](#post-apiv3fiatwithdraw)                 | POST   |       |         |          |
| [/api/v3/fiat/deposit-history](#post-apiv3fiatdeposit-history)   | POST   |       |         |          |
| [/api/v3/fiat/withdraw-history](#post-apiv3fiatwithdraw-history) | POST   |       |         |          |

# Constructing the request
### GET/POST request
* GET requests require parameters as **query string** in the URL (e.g. ?sym=THB_BTC&lmt=10). 
* POST requests require JSON payload (application/json).

### Request headers (Secure Endpoints)
Authentication requires API KEY and API SECRET. Every request to the server must contain the following in the request header:
* Accept: application/json
* Content-type: application/json
* X-BTK-APIKEY: {YOUR API KEY}
* X-BTK-TIMESTAMP: {Timestamp i.e. 1699376552354 }
* X-BTK-SIGN: [Signature](#signature)

### Payload (POST)
The payload is always JSON.

### Signature
Generate the signature from the timestamp, the request method, API path, query parameter, and JSON payload using HMAC SHA-256. Use the API Secret as the secret key for generating the HMAC variant of JSON payload. The signature is in **hex**  format. The user has to attach the signature via the Request Header
You must get a new timestamp in millisecond from [/api/v3/servertime](#get-apiv3servertime). The old one is in second.

#### Example string for signing a signature:
```javascript
//Example for Get Method
1699381086593GET/api/v3/market/my-order-history?sym=BTC_THB

// Example for Post Method
1699376552354POST/api/v3/market/place-bid{"sym":"thb_btc","amt": 1000,"rat": 10,"typ": "limit"}
```

#### Example cURL:
```javascript
curl --location 'https://api.bitkub.com/api/v3/market/place-bid' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: e286825bda3497ae2d03aa3a30c420d603060cb4edbdd3ec711910c86966e9ba' \
--header 'X-BTK-SIGN: f5884963865a6e868ddbd58c9fb9ea4bd013076e8a8fa51d38b86c38d707cb8a' \
--header 'Content-Type: application/json' \
--data '{
  "sym": "thb_btc",
  "amt": 1000,
  "rat": 10,
  "typ": "limit",
}'
```
```javascript
curl --location 'https://api.bitkub.com/api/v3/market/my-open-orders?sym=BTC_THB' \
--header 'X-BTK-TIMESTAMP: 1699381086593' \
--header 'X-BTK-APIKEY: e286825bda3497ae2d03aa3a30c420d603060cb4edbdd3ec711910c86966e9ba' \
--header 'X-BTK-SIGN: f5884963865a6e868ddbd58c9fb9ea4bd013076e8a8fa51d38b86c38d707cb8a'

```

# API documentation
Refer to the following for description of each endpoint

### GET /api/status

#### Description:
Get endpoint status. When status is not `ok`, it is highly recommended to wait until the status changes back to `ok`.

#### Query:
- n/a

#### Response:
```javascript
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

### GET /api/servertime

#### Description:
Get server timestamp. This can't use with secure endpoint V3. Please use [/api/v3/servertime](#get-apiv3servertime).

#### Query:
- n/a

#### Response:
```javascript
1707220534359
```

### GET /api/v3/servertime

#### Description:
Get server timestamp.

#### Query:
- n/a

#### Response:
```javascript
1701251212273
```

### GET /api/market/symbols

#### Description:
List all available symbols.

#### Query:
- n/a

#### Response:
```javascript
{
  "error": 0,
  "result": [
    {
      "id": 1,
      "symbol": "THB_BTC",
      "info": "Thai Baht to Bitcoin"
    },
    {
      "id": 2,
      "symbol": "THB_ETH",
      "info": "Thai Baht to Ethereum"
    }
  ]
}
```

### GET /api/market/ticker

#### Description:
Get ticker information.

#### Query:
* `sym` **string** The symbol (optional)
  * e.g. thb_btc

#### Response:
```javascript
{
  "THB_BTC": {
    "id": 1,
    "last": 216415.00,
    "lowestAsk": 216678.00,
    "highestBid": 215000.00,
    "percentChange": 1.91,
    "baseVolume": 71.02603946,
    "quoteVolume": 15302897.99,
    "isFrozen": 0,
    "high24hr": 221396.00,
    "low24hr": 206414.00
  },
  "THB_ETH": {
    "id": 2,
    "last": 11878.00,
    "lowestAsk": 12077.00,
    "highestBid": 11893.00,
    "percentChange": -0.49,
    "baseVolume": 455.17839270,
    "quoteVolume": 5505664.42,
    "isFrozen": 0,
    "high24hr": 12396.00,
    "low24hr": 11645.00
  }
}
```

### GET /api/market/trades

#### Description:
List recent trades.

#### Query:
* `sym`		**string** The symbol (e.g. thb_btc)
* `lmt`		**int** No. of limit to query recent trades

#### Response:
```javascript
{
  "error": 0,
  "result": [
    [
      1529516287, // timestamp
      10000.00, // rate
      0.09975000, // amount
      "BUY" // side
    ]
  ]
}
```

### GET /api/market/bids
#### Description:
List open buy orders.

#### Query:
* `sym` **string** The symbol (e.g. thb_btc)
* `lmt` **int** No. of limit to query open buy orders

#### Response:
```javascript
{
  "error": 0,
  "result": [
    [
      "1", // order id
      1529453033, // timestamp
      997.50, // volume
      10000.00, // rate
      0.09975000 // amount
    ]
  ]
}

```

### GET /api/market/asks


#### Description:
List open sell orders.

#### Query:
* `sym` **string** The symbol (e.g. thb_btc)
* `lmt` **int** No. of limit to query open sell orders

#### Response:
```javascript
{
  "error": 0,
  "result": [
    [
      "680", // order id
      1529491094, // timestamp
      997.50, // volume
      10000.00, // rate
      0.09975000 // amount
    ]
  ]
}
```

### GET /api/market/books
#### Description:
List all open orders.

#### Query:
* `sym` **string** The symbol (e.g. thb_btc)
* `lmt` **int** No. of limit to query open orders

#### Response:
```javascript
{
  "error": 0,
  "result": {
    "bids": [
      [
        "1", // order id
        1529453033, // timestamp
        997.50, // volume
        10000.00, // rate
        0.09975000 // amount
      ]
    ],
    "asks": [
      [
        "680", // order id
        1529491094, // timestamp
        997.50, // volume
        10000.00, // rate
        0.09975000 // amount
      ]
    ]
  }
}
```

### GET /tradingview/history
#### Description:
Get historical data for TradingView chart.

#### Query:
* `symbol`		**string**		The symbol (e.g. BTC_THB)
* `resolution`		**string**		Chart resolution (1, 5, 15, 60, 240, 1D)
* `from`		**int**		Timestamp of the starting time (e.g. 1633424427)
* `to`		**int**		Timestamp of the ending time (e.g. 1633427427)

#### Response:
```javascript
{
  "c": [
    1685000,
    1680699.95,
    1688998.99,
    1692222.22
  ],
  "h": [
    1685000,
    1685000,
    1689000,
    1692222.22
  ],
  "l": [
    1680053.22,
    1671000,
    1680000,
    1684995.07
  ],
  "o": [
    1682500,
    1685000,
    1680100,
    1684995.07
  ],
  "s": "ok",
  "t": [
    1633424400,
    1633425300,
    1633426200,
    1633427100
  ],
  "v": [
    4.604352630000001,
    8.530631670000005,
    4.836581560000002,
    2.8510189200000022
  ]
}
```

### GET /api/market/depth

#### Description:
Get depth information.

#### Query:
* `sym` **string** The symbol (e.g. thb_btc)
* `lmt` **int** Depth size

#### Response:
```javascript
{
  "asks": [
    [
      262600,
      0.61905798
    ],
    [
      263000,
      0.00210796
    ],
    [
      263200,
      0.89555545
    ],
    [
      263422.5,
      0.03796183
    ],
    [
      263499,
      0.4123439
    ]
  ],
  "bids": [
    [
      262510,
      0.38038703
    ],
    [
      262100.01,
      1.22519999
    ],
    [
      262100,
      0.00381533
    ],
    [
      262024.88,
      0.00352718
    ],
    [
      262001,
      0.09999961
    ]
  ]
}
```


## Market Data Endpoint

### GET /api/v3/market/ticker

### Description:
Get ticker information.

### Query (URL):
* `sym` **string** The symbol (e.g. btc_thb)

### Response:
```javascript
[
    {
        "symbol": "ADA_THB",
        "base_volume": "1875227.0489781",
        "high_24_hr": "38",
        "highest_bid": "37.33",
        "last": "37.36",
        "low_24_hr": "35.76",
        "lowest_ask": "37.39",
        "percent_change": "2.69",
        "quote_volume": "69080877.73"
    },
    {
        "symbol": "CRV_THB",
        "base_volume": "1811348.93318162",
        "high_24_hr": "39",
        "highest_bid": "38.4",
        "last": "38.42",
        "low_24_hr": "35.51",
        "lowest_ask": "38.42",
        "percent_change": "4.52",
        "quote_volume": "67340316.65"
    }
]
```


### GET /api/v3/market/bids
#### Description:
List open buy orders.

#### Query:
* `sym` **string** The symbol (e.g. btc_thb)
* `lmt` **int** No. of limit to query open buy orders

#### Response:
```javascript
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
    },
    {
      "order_id": "365357190",
      "price": "3330100.13",
      "side": "buy",
      "size": "0.00314952",
      "timestamp": 1734689476000,
      "volume": "10488.24"
    }
  ]
}

```

### GET /api/v3/market/asks


#### Description:
List open sell orders.

#### Query:
* `sym` **string** The symbol (e.g. btc_thb)
* `lmt` **int** No. of limit to query open sell orders

#### Response:
```javascript
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
    },
    {
      "order_id": "303536239",
      "price": "3334889.31",
      "side": "sell",
      "size": "0.129",
      "timestamp": 1734714713000,
      "volume": "430200.72"
    }
  ]
}
```

### GET /api/v3/market/depth

#### Description:
Get depth information.

#### Query:
* `sym` **string** The symbol (e.g. btc_thb)
* `lmt` **int** Depth size

#### Response:
```javascript
{
  "error": 0,
  "result": {
    "asks": [
      [
       3338932.98, // price
       0.00619979, //size
      ],
      [
       3341006.36, // price
       0.00134854 //size
      ]
    ],
    "bids": [
      [
        3334907.27, // price
        0.00471255 //size
      ],
      [
        3334907.26, // price
        0.36895805 //size
      ]
    ]
  }
}
```

### GET /api/v3/market/trades

#### Description:
List recent trades.

#### Query:
* `sym`		**string** The symbol (e.g. btc_thb)
* `lmt`		**int** No. of limit to query recent trades

#### Response:
```javascript
{
    "error": 0,
    "result": [
        [
            1734661894000,
            3367353.98,
            0.00148484,
            "BUY"
        ],
        [
            1734661893000,
            3367353.98,
            0.00029622,
            "BUY"
        ]
    ]
}
```
## Trading Endpoint V3


### POST /api/v3/market/wallet

#### Description:
Get user available balances (for both available and reserved balances please use [POST /api/v3/market/balances](#post-apiv3marketbalances)).

#### Query:
- n/a

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
### POST /api/v3/user/trading-credits

### Description:
Check trading credit balance.

### Query (URL):
-

### Response:
```javascript
{
   "error": 0,
   "result": 1000
}
```

### POST /api/v3/market/place-bid

#### Description:
Create a buy order.

#### Body:
* `sym`   **string**    The symbol you want to trade (e.g. btc_thb).
* `amt`   **float**   Amount you want to spend with no trailing zero (e.g. 1000.00 is invalid, 1000 is ok)
* `rat`   **float**   Rate you want for the order with no trailing zero (e.g. 1000.00 is invalid, 1000 is ok)
* `typ`   **string**    Order type: limit or market (for market order, please specify rat as 0)
* `client_id` **string**    your id for reference ( not required )
* `post_only`   **bool**    Postonly flag: true or false ( not required )

#### Response:
```javascript
{
  "error": 0,
  "result": {
    "id": "1", // order id
    "typ": "limit", // order type
    "amt": 1000, // spending amount
    "rat": 15000, // rate
    "fee": 2.5, // fee
    "cre": 2.5, // fee credit used
    "rec": 0.06666666, // amount to receive
    "ts": "1707220636" // timestamp
    "ci": "input_client_id" // input id for reference
  }
}
```

### POST /api/v3/market/place-ask

#### Description:
Create a sell order.

#### Body:
* `sym`   **string**    The symbol. The symbol you want to trade (e.g. btc_thb).
* `amt`   **float**   Amount you want to sell with no trailing zero (e.g. 0.10000000 is invalid, 0.1 is ok)
* `rat`   **float**   Rate you want for the order with no trailing zero (e.g. 1000.00 is invalid, 1000 is ok)
* `typ`   **string**    Order type: limit or market (for market order, please specify rat as 0)
* `client_id`   **string**    your id for reference ( not required )
* `post_only`   **bool**    Postonly flag: true or false ( not required )


#### Response:
```javascript
{
  "error": 0,
  "result": {
    "id": "1", // order id
    "typ": "limit", // order type
    "amt": 1.00000000, // selling amount
    "rat": 15000, // rate
    "fee": 37.5, // fee
    "cre": 37.5, // fee credit used
    "rec": 15000, // amount to receive
    "ts": "1533834844" // timestamp
    "ci": "input_client_id" // input id for reference
  }
}
```

### POST /api/v3/market/cancel-order

### Description:
Cancel an open order.

### Body:
* `sym`   **string**    The symbol. ***Please note that the current endpoint requires the symbol thb_btc. However, it will be changed to btc_thb soon and you will need to update the configurations accordingly for uninterrupted API functionality.***
* `id`    **string**   Order id you wish to cancel
* `sd`    **string**    Order side: buy or sell

### Response:
```javascript
{
  "error": 0
}
```
### POST /api/v3/market/balances

#### Description:
Get balances info: this includes both available and reserved balances.

#### Query:
- n/a

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
### GET /api/v3/market/my-open-orders

### Description:
List all open orders of the given symbol.

### Query:
* `sym`		**string**		The symbol (e.g. btc_thb)

### Response:
```javascript
{
  "error": 0,
  "result": [
    { // Example of sell order
      "id": "2", // order id
      "side": "sell", // order side
      "type": "limit", // order type
      "rate": "15000", // rate
      "fee": "35.01", // fee
      "credit": "35.01", // credit used
      "amount": "0.93333334", // amount of crypto quantity
      "receive": "14000", // amount of THB 
      "parent_id": "1", // parent order id
      "super_id": "1", // super parent order id
      "client_id": "client_id" // client id
      "ts": 1702543272000 // timestamp
    },
    { // Example of buy order
      "id": "278465822",
      "side": "buy",
      "type": "limit",
      "rate": "10",
      "fee": "0.25",
      "credit": "0",
      "amount": "100", // amount of THB 
      "receive": "9.975", // amount of crypto quantity
      "parent_id": "0",
      "super_id": "0",
      "client_id": "client_id",
      "ts": 1707220636000
    },
  ]
}
```
Note : The ```client_id``` of this API response is the input body field name ```client_id``` , was inputted by the user of APIs 
* [api/v3/market/place-bid](#post-apiv3marketplace-bid)
* [api/v3/market/place-ask](#post-apiv3marketplace-ask)



### GET /api/v3/market/my-order-history

### Description:
List all orders that have already matched.

### Query:
* `sym` **string** The symbol (e.g. btc_thb)
* `p` **int** Page (optional)
* `lmt` **int** Limit (optional)
* `start` **int** Start timestamp (optional)
* `end` **int** End timestamp (optional)

### Response:
```javascript
{
  "error": 0,
  "result": [
    {
      "txn_id": "BTCSELL0021206932",
      "order_id": "241407793",
      "parent_order_id": "0",
      "super_order_id": "0",
      "client_id": "",
      "taken_by_me": false,
      "is_maker": false,
      "side": "sell",
      "type": "market",
      "rate": "1525096.27",
      "fee": "0.04",
      "credit": "0",
      "amount": "0.00001", // crypto amount
      "ts": 1707221396584
    },
    {
      "txn_id": "BTCBUY0021182426",
      "order_id": "277231907",
      "parent_order_id": "0",
      "super_order_id": "0",
      "client_id": "client_id",
      "taken_by_me": false,
      "is_maker": false,
      "side": "buy",
      "type": "market",
      "rate": "1497974.74",
      "fee": "0.03",
      "credit": "0",
      "amount": "11", // THB amount
      "ts": 1706775718739
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

### GET /api/v3/market/order-info
### Description:
Get information regarding the specified order.

### Query:
* `sym`		**string**		The symbol (e.g. btc_thb)
* `id`		**string**		Order id
* `sd`		**string**		Order side: buy or sell

### Response:
```javascript
{
    "error": 0,
    "result": {
        "id": "289", // order id
        "first": "289", // first order id
        "parent": "0", // parent order id
        "last": "316", // last order id
        "client_id": "", // your id for reference
        "post_only": false, // post_only: true, false
        "amount": "4000", // order amount THB amount if it Buy side. And Crypto Amount if it sell side
        "rate": 291000, // order rate
        "fee": 10, // order fee
        "credit": 10, // order fee credit used
        "filled": 3999.97, // filled amount
        "total": 4000, // total amount
        "status": "filled", // order status: filled, unfilled, cancelled
        "partial_filled": false, // true when order has been partially filled, false when not filled or fully filled
        "remaining": 0, // remaining amount to be executed
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

## Fiat Endpoint

### POST /api/v3/fiat/accounts

### Description:
List all approved bank accounts.

### Query (URL):
* `p` **int** Page (optional)
* `lmt` **int** Limit (optional)

### Response:
```javascript
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
   "pagination": {
      "page": 1,
      "last": 1
   }
}
```

### POST /api/v3/fiat/withdraw

### Description:
Make a withdrawal to an **approved** bank account.

### Query:
* `id`		**string**	Bank account id
* `amt`		**float**		Amount you want to withdraw

### Response:
```javascript
{
    "error": 0,
    "result": {
        "txn": "THBWD0000012345", // local transaction id
        "acc": "7262109099", // bank account id
        "cur": "THB", // currency
        "amt": 21, // withdraw amount
        "fee": 20, // withdraw fee
        "rec": 1, // amount to receive
        "ts": 1569999999 // timestamp
    }
}
```

### POST /api/v3/fiat/deposit-history

### Description:
List fiat deposit history.

### Query (URL):
* `p` **int** Page (optional)
* `lmt` **int** Limit (optional)

### Response:
```javascript
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
   "pagination": {
      "page": 1,
      "last": 1
   }
}
```

### POST /api/v3/fiat/withdraw-history

### Description:
List fiat withdrawal history.

### Query (URL):
* `p` **int** Page (optional)
* `lmt` **int** Limit (optional)

### Response:
```javascript
{
   "error":0,
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
   "pagination": {
      "page": 1,
      "last": 1
   }
}
```

## User information Endpoint

### POST /api/v3/user/limits

### Description:
Check deposit/withdraw limitations and usage.

### Query (URL):
-

### Response:
```javascript
{
   "error": 0,
   "result": { 
       "limits": { // limitations by kyc level
          "crypto": { 
             "deposit": 0.88971929, // BTC value equivalent
             "withdraw": 0.88971929 // BTC value equivalent
          },
          "fiat": { 
             "deposit": 200000, // THB value equivalent
             "withdraw": 200000 // THB value equivalent
          }
       },
       "usage": { // today's usage
          "crypto": { 
             "deposit": 0, // BTC value equivalent
             "withdraw": 0, // BTC value equivalent
             "deposit_percentage": 0,
             "withdraw_percentage": 0,
             "deposit_thb_equivalent": 0, // THB value equivalent
             "withdraw_thb_equivalent": 0 // THB value equivalent
          },
          "fiat": { 
             "deposit": 0, // THB value equivalent
             "withdraw": 0, // THB value equivalent
             "deposit_percentage": 0,
             "withdraw_percentage": 0
          }
       },
       "rate": 224790 // current THB rate used to calculate
    }
}
```
### GET /api/v3/user/coin-convert-history
### Description:
List all coin convert histories (paginated).

### Query (URL):
* `p` **int** Page default = 1 (optional)
* `lmt` **int** Limit default = 100 (optional)
* `sort` **int** Sort [1, -1] default = 1 (optional)
* `status` **string** Status [success, fail, all] (default = all) (optional)
* `sym` **string** The symbol (optional)
  * e.g. KUB
* `start` **int** Start timestamp (optional)
* `end` **int** End timestamp (optional)


### Response:
```javascript
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
        },
        {
            "transaction_id": "6707a7426fb3370035725c03",
            "status": "fail",
            "amount": "0.000006",
            "from_currency": "KUB",
            "trading_fee_received": "0",
            "timestamp": 1728580016000
        }
    ],
    "pagination": {
        "page": 1,
        "last": 12,
        "next": 2
    }
}
```


# Error codes
Refer to the following descriptions:

| Code | Message                | Description                                                |
| ---- | ---------------------- | ---------------------------------------------------------- |
| 0    | | No error                                                           |
| 1    | | Invalid JSON payload                                               |
| 2    | | Missing X-BTK-APIKEY                                               |
| 3    | | Invalid API key                                                    |
| 4    | | API pending for activation                                         |
| 5    | | IP not allowed                                                     |
| 6    | | Missing / invalid signature                                        |
| 7    | | Missing timestamp                                                  |
| 8    | | Invalid timestamp                                                  |
| 9    | | • Invalid user <br> • User not found <br> • Freeze withdrawal <br> • User is not allowed to perform this action within the last 24 hours <br> • User has suspicious withdraw crypto txn |
| 10   | | • Invalid parameter <br> • Invalid response: Code not found in response <br> • Validate params <br> • Default |
| 11   | | Invalid symbol                                                     |
| 12   | | • Invalid amount <br> • Withdrawal amount is below the minimum threshold |
| 13   | | Invalid rate                                                       |
| 14   | | Improper rate                                                      |
| 15   | | Amount too low                                                     |
| 16   | | Failed to get balance                                              |
| 17   | | Wallet is empty                                                    |
| 18   | | Insufficient balance                                               |
| 19   | | Failed to insert order into db                                     |
| 20   | | Failed to deduct balance                                           |
| 21   | | Invalid order for cancellation (Unable to find OrderID or Symbol.) |
| 22   | | Invalid side                                                       |
| 23   | | Failed to update order status                                      |
| 24   | | • Invalid order for lookup <br> • Invalid kyc level |
| 25   | | KYC level 1 is required to proceed                                 |
| 30   | | Limit exceeds                                                      |
| 40   | | Pending withdrawal exists                                          |
| 41   | | Invalid currency for withdrawal                                    |
| 42   | | Address is not in whitelist                                        |
| 43   | | • Failed to deduct crypto <br> • Insufficient balance <br> • Deduct balance failed |
| 44   | | Failed to create withdrawal record                                 |
| 47   | | Withdrawal amount exceeds the maximum limit                                           |
| 48   | | • Invalid bank account <br> • User bank id is not found <br> • User bank is unavailable |
| 49   | | Bank limit exceeds                                                 |
| 50   | | • Pending withdrawal exists <br> • Cannot perform the action due to pending transactions |
| 51   | | Withdrawal is under maintenance                                    |
| 52   | | Invalid permission                                                 |
| 53   | | Invalid internal address                                           |
| 54   | | Address has been deprecated                                        |
| 55   | | Cancel only mode                                                   |
| 56   | | User has been suspended from purchasing                            |
| 57   | | User has been suspended from selling                               |
| 58   | | ~~Transaction not found~~ <br> User bank is not verified           |
| 90   | | Server error (please contact support)                              |

# Rate limits 
If the request rate exceeds the limit in any endpoints, the request will be blocked for 30 seconds. When blocked, HTTP response is 429 Too Many Requests. The limits apply to individual user accessing the API. ***The rate limit is applied to each endpoint regardless the API version.***

| Endpoint                     | Rate Limit       |
| ---------------------------- | ---------------- |
| /api/market/ticker           | 100 req/sec      |
| /api/market/depth            | 10 req/sec       |
| /api/market/symbols          | 100 req/sec      |
| /api/market/trades           | 100 req/sec      |
| /api/market/bids             | 100 req/sec      |
| /api/market/asks             | 100 req/sec      |
| /api/market/books            | 100 req/sec      |
| /api/market/order-info       | 100 req/sec      |
| /api/market/my-open-orders   | 150 req/sec      |
| /api/market/my-order-history | 100 req/sec      | 
| /api/market/place-bid        | 150 req/sec       |
| /api/market/place-ask        | 150 req/sec       |
| /api/market/cancel-order     | 200 req/sec      |
| /api/market/balances         | 150 req/sec      |
| /api/market/wallet           | 150 req/sec      |
| /api/servertime              | 2,000 req/10secs |
| /api/status                  | 100 req/sec      |
| /api/fiat/*                  | 20 req/sec       |
| /api/user/*                  | 20 req/sec       |
| /tradingview/*               | 100 req/sec      |
