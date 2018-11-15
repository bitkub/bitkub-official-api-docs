# Websocket API for Bitkub (2018-11-15)

# Table of contents
* [Websocket endpoint](#websocket-endpoint)
* [Stream name](#stream-name)
* [Symbols](#symbols)
* [Websocket API documentation](#web-socket-api-documentation)
* [Demo](#demo)

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
Refer to [RESTful API](https://github.com/bitkub/bitkub-official-api-docs/blob/master/restful-api.md#get-apimarketsymbols) for all available symbols.



# Websocket API documentation
Refer to the following for description of each stream

### Trade stream
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
The ticker stream provides real-time data on ticker of the specified symbol. Ticker for each symbol is re-calculated on trade order creation, cancellation, and match.

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

# Demo
The demo page is available [here](https://api.bitkub.com/websocket-api?streams=) for testing streams subscription.
