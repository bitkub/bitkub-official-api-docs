# Websocket API for Bitkub (2023-04-19)

# Changelog
* 2023-04-19 Changed the webSocket
market.trade.symbol. Field ```bid, sid``` changed type from ```Integer to String```.
* 2023-01-16 Update `Live Order Book`, added a new event info.
* 2022-08-31 Deprecated the authentication to `Live Order Book` websocket.
# Table of contents
* [Websocket endpoint](#websocket-endpoint)
* [Stream name](#stream-name)
* [Symbols](#symbols)
* [Websocket API documentation](#web-socket-api-documentation)
* [Stream Demo](#stream-demo)
* [Live Order Book](#live-order-book)

# Websocket endpoint
* The websocket endpoint is: **wss://api.bitkub.com/websocket-api/[\<streamName\>](#stream-name)**

# Stream name
Stream name requires 3 parts: **service name**, **service type**, and **symbol**, delimited by **dot (.)**, and is **case-insensitive**.

#### Stream name format:
```javascript
<serviceName>.<serviceType>.<symbol>
```

#### Stream name example:
```javascript
market.trade.thb_btc
```
Above example stream name provides real-time data from the **market** service, type **trade**, of symbol **THB_BTC**.



### Multiple streams subscription:
You can combine multiple streams by using **comma (,)** as the delimeter.

#### Multiple stream names format:
```javascript
<streamName>,<streamName>,<streamName>
```

#### Multiple stream names example:
```javascript
market.trade.thb_btc,market.ticker.thb_btc,market.trade.thb_eth,market.ticker.thb_eth
```
Above subscription provides real-time data from trade and ticker streams of symbols THB_BTC and THB_ETH.



# Symbols
Refer to [RESTful API](https://github.com/bitkub/bitkub-official-api-docs/blob/master/restful-api.md#get-apimarketsymbols) for all available symbols and symbol ids).



# Websocket API documentation
Refer to the following for description of each stream

### Trade stream
<span style="color:white;background:red;"> ⚠️ After April 18th, 2023 at 18:00PM(GMT+7)</span>

* Response field ```bid, sid``` change type from ```Integer to String```.
* Ref: [Announcement](#announcement)

#### Name:
market.trade.\<symbol\>

#### Description:
The trade stream provides real-time data on matched orders. Each trade contains buy order id and sell order id. Order id is unique by the order side (buy/sell) and symbol.

#### Response:
```javascript
{
  "stream": "market.trade.thb_eth", // stream name
  "sym":"THB_ETH", // symbol
  "txn": "ETHSELL0000074282", // transaction id
  "rat": "5977.00", // rate matched
  "amt": 1.556539, // amount matched
  "bid": 2048451, // buy order id
  "sid": 2924729, // sell order id
  "ts": 1542268567 // trade timestamp
}
```

### Ticker stream
#### Name:
market.ticker.\<symbol\>

#### Description:
The ticker stream provides real-time data on ticker of the specified symbol. Ticker for each symbol is re-calculated on trade order creation, cancellation, and fulfillment.

#### Response:
```javascript
{
  "stream": "market.ticker.thb_bch", // stream name
  "id": 6, // symbol id
  "last": "15425.00",
  "lowestAsk": "17799.00",
  "highestBid": "15425.00",
  "percentChange": "-10.15",
  "baseVolume": "55.76940380",
  "quoteVolume": "905856.89",
  "isFrozen": "0",
  "high24hr": "17980.00",
  "low24hr": "15409.00"
 }
```

# Stream Demo
The demo page is available [here](https://api.bitkub.com/websocket-api?streams=) for testing streams subscription.

# Live Order Book
#### Description:
Use symbol id (numeric id) to get real-time data of order book: **wss://api.bitkub.com/websocket-api/orderbook/[\<symbol-id\>](#symbols)**.

#### Message data:
```javascript
{
    "data": (data),
    "event": (event type)
}
```
There are 4 event types: **bidschanged**, **askschanged**, **tradeschanged**, and **global.ticker**
* **bidschanged** occurs when any buy order has changed (opened/closed/cancelled). Data is array of buy orders after the change (max. 30 orders).
* **askschanged** occurs when any sell order has changed (opened/closed/cancelled). Data is array of sell orders after the change (max. 30 orders).
* **tradeschanged** occurs when buy and sell orders have been matched. Data is array containing 3 arrays: array of latest trades, array of buy orders, and array of sell orders (each max. 30 orders). You get this event as the initial data upon successful subscription.
* **global.ticker** occurs every time when either **bidschanged**, **askschanged**, or **tradeschanged** is fired by any symbol in the exchange.

#### Example response (bidschanged or askschanged):
```javascript
{
   "data":[
      [
         121.82, // vol
         112510.1, // rate
         0.00108283, // amount
         0, // reserved, always 0
         false, // is new order
         false // user is owner (deprecated)
      ]
   ],
   "event":"bidschanged"
}
```

#### Example response (tradeschanged):
```javascript
{
   "data":[
      [
         [
            1550320593, // timestamp
            113587, // rate
            0.12817027, // amount
            "BUY", // side
            0, // reserved, always 0
            0, // reserved, always 0
            true, // is new order
            false, // user is buyer (available when authenticated)
            false // user is seller (available when authenticated)
         ]
      ],
      [
         [
            121.82, // vol
            112510.1, // bid rate
            0.00108283, // bid amount
            0, // reserved, always 0
            false, // is new order
            false // user is owner (available when authenticated)
         ]
      ],
      [
         [
            51247.13, // vol
            113699, // ask rate
            0.45072632, // ask amount
            0, // reserved, always 0
            false, // is new order
            false // user is owner (available when authenticated)
         ]
      ]
   ],
   "event":"tradeschanged"
}
```

#### Example response (global.ticker):
```javascript
{
   "data":{
      "baseVolume":106302.39237032, // amount of crypto
      "change":0.16, // difference of price compare to the latest
      "close":15.9, // close price
      "high24hr":16.72, // the highest bidding price taken in the last 24 hours
      "highestBid":15.81, // the highest bidding price
      "highestBidSize":5640.39911448, // the amount of the highest bidding order
      "id":139, // symbol id
      "isFrozen":0, // symbol trade status
      "last":15.9, // the latest price
      "low24hr":15.7, // the lowest price taken in the last 24 hours
      "lowestAsk":16.22, // the lowest asking price
      "lowestAskSize":1582, // the amount of the lowest asking order
      "open":15.74, // open price
      "percentChange":1.02, // difference of price compare to the latest in percent
      "quoteVolume":1715566.77, //  amount of fiat
      "stream":"market.ticker.thb_1inch" // stream name
   },
   "event":"global.ticker" // event name
}
```
