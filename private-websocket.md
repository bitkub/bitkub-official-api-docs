# Private WebSocket API

## Overview

The Private WebSocket API provides real-time trading data updates for authenticated users on the Bitkub Exchange platform. This service delivers order updates and match updates through WebSocket connections.

## Endpoint

| Environment | WebSocket URL |
|-------------|---------------|
| Production | `wss://stream.bitkub.com/v3/private` |

## Connection

### Recommended Headers

It is recommended to set a `User-Agent` header when establishing the WebSocket connection. This helps with identification and debugging on the server side.

```
User-Agent: <your-app-name>/<version>
```

**Example:**
```
User-Agent: my-trading-bot/1.0.0
```

## Connection Lifecycle

- **Ping Frequency**: Send ping at least every **5 minutes** to keep the connection alive
- **Maximum Connection Duration**: Connections are automatically terminated after **2 hours**
- **Recommended Ping Interval**: 4 minutes (240 seconds)

---

## Authentication

After establishing a WebSocket connection, you must authenticate using your API credentials.

### Generate Signature

Create an HMAC SHA256 signature using the timestamp as the payload:

```javascript
const timestamp = Date.now();
const payload = [timestamp];
const signature = CryptoJS.HmacSHA256(payload.join(""), apiSecret).toString(CryptoJS.enc.Hex);
```

### Authentication Request

```json
{
    "event": "auth",
    "data": {
        "X-BTK-APIKEY": "your_api_key",
        "X-BTK-SIGN": "generated_signature",
        "X-BTK-TIMESTAMP": "1699123456789"
    }
}
```

### Authentication Response

**Success:**
```json
{
    "event": "auth",
    "code": "200",
    "message": "Success",
    "data": {},
    "connection_id": "Y33pLftYyQ0CEpQ=",
    "timestamp": "2024-01-01T12:00:00.000000000Z"
}
```

**Failure:**
```json
{
    "event": "auth",
    "code": "401",
    "message": "Unauthorized",
    "data": {},
    "connection_id": "Y33pLftYyQ0CEpQ=",
    "timestamp": "2024-01-01T12:00:00.000000000Z"
}
```

---

## Subscription

After successful authentication, subscribe to the channels you want to receive updates from.

### Available Channels

| Channel | Description |
|---------|-------------|
| `order_update` | Real-time order status updates (placed, modified, cancelled, executed) |
| `match_update` | Real-time trade execution notifications |

### Subscribe Request

```json
{
    "event": "subscribe",
    "channel": "order_update"
}
```

```json
{
    "event": "subscribe",
    "channel": "match_update"
}
```

### Subscribe Response

**Success:**
```json
{
    "event": "subscribe",
    "channel": "order_update",
    "code": "200",
    "message": "Success",
    "data": {
        "message": "Subscribed successfully"
    },
    "connection_id": "Y33pLftYyQ0CEpQ=",
    "timestamp": "2024-01-01T12:00:00.000000000Z"
}
```

### Unsubscribe Request

```json
{
    "event": "unsubscribe",
    "channel": "order_update"
}
```

### Unsubscribe Response

**Success:**
```json
{
    "event": "unsubscribe",
    "channel": "order_update",
    "code": "200",
    "message": "Success",
    "data": {
        "message": "Unsubscribed successfully"
    },
    "connection_id": "Y33pLftYyQ0CEpQ=",
    "timestamp": "2024-01-01T12:00:00.000000000Z"
}
```

---

## Ping (Keep-Alive)

Send periodic ping messages to maintain the connection:

### Ping Request

```json
{
    "event": "ping"
}
```

### Ping Response

```json
{
    "event": "ping",
    "code": "200",
    "message": "Success",
    "data": {
        "message": "pong"
    },
    "connection_id": "Y33pLftYyQ0CEpQ=",
    "timestamp": "2024-01-01T12:00:00.000000000Z"
}
```

---

## Events and Responses

### Order Update Event

Received when your order status changes (created, filled, partially filled, cancelled, etc.)

**Response Format:**

```json
{
    "event": "order_update",
    "code": "200",
    "message": "Success",
    "data": {
        "user_id": "string",
        "order_id": "string",
        "client_id": "string | null",
        "symbol": "BTC_THB",
        "side": "buy | sell",
        "type": "limit | stoplimit | market",
        "status": "new | open | rejected | partial_filled | filled | partial_filled_canceled | canceled | untriggered",
        "price": "1000000.00",
        "stop_price": null,
        "order_currency": "THB",
        "order_amount": "10000.00",
        "executed_currency": "THB",
        "executed_amount": "5000.00",
        "received_currency": "BTC",
        "received_amount": "0.005",
        "total_fee": "12.50",
        "credit_used": "0.00",
        "net_fee_paid": "12.50",
        "avg_filled_price": "1000000.00",
        "post_only": false,
        "order_created_at": 1704067200000,
        "order_updated_at": 1704067250000
    },
    "connection_id": "Y33pLftYyQ0CEpQ=",
    "timestamp": "2024-01-01T12:00:00.000000000Z"
}
```

**Field Descriptions:**

| Field | Type | Description |
|-------|------|-------------|
| `user_id` | string | User identifier |
| `order_id` | string | Unique order identifier |
| `client_id` | string \| null | Client-provided order identifier |
| `symbol` | string | Trading pair (e.g., `BTC_THB`) |
| `side` | string | Order side: `buy` or `sell` |
| `type` | string | Order type: `limit`, `stoplimit`, or `market` |
| `status` | string | Order status (see Status Mapping below) |
| `price` | string \| null | Limit price (null for market orders) |
| `stop_price` | string \| null | Stop price (for stop-limit orders) |
| `order_currency` | string | Currency used for the order |
| `order_amount` | string | Original order amount |
| `executed_currency` | string | Currency of executed amount |
| `executed_amount` | string | Total executed amount (including fees for buy orders) |
| `received_currency` | string | Currency received |
| `received_amount` | string | Amount received after fees |
| `total_fee` | string | Total fee (wallet + credit) |
| `credit_used` | string | Fee paid using credit |
| `net_fee_paid` | string | Net fee paid from wallet |
| `avg_filled_price` | string | Average filled price |
| `post_only` | boolean | Whether the order is post-only |
| `canceled_by` | string \| null | Cancellation source (if cancelled) |
| `order_created_at` | number | Order creation timestamp (Unix milliseconds) |
| `order_triggered_at` | number \| null | Trigger timestamp for stop orders (Unix milliseconds) |
| `order_updated_at` | number \| null | Last update timestamp (Unix milliseconds) |

---

### Match Update Event

Received when a trade executes (order is matched).

**Response Format:**

```json
{
    "event": "match_update",
    "code": "200",
    "message": "Success",
    "data": {
        "order_id": "string",
        "txn_id": "string",
        "client_id": "string | null",
        "symbol": "BTC_THB",
        "type": "limit | stoplimit | market",
        "status": "partial_filled | filled",
        "side": "buy | sell",
        "is_maker": true,
        "price": "1000000.00",
        "executed_currency": "THB",
        "executed_amount": "5000.00",
        "received_currency": "BTC",
        "received_amount": "0.005",
        "fee_rate": "0.0025",
        "total_fee": "12.50",
        "credit_used": "0.00",
        "net_fee_paid": "12.50",
        "txn_ts": 1704067200
    },
    "connection_id": "Y33pLftYyQ0CEpQ=",
    "timestamp": "2024-01-01T12:00:00.000000000Z"
}
```

**Field Descriptions:**

| Field | Type | Description |
|-------|------|-------------|
| `order_id` | string | Unique order identifier |
| `txn_id` | string | Transaction/trade identifier |
| `client_id` | string \| null | Client-provided order identifier |
| `symbol` | string | Trading pair (e.g., `BTC_THB`) |
| `type` | string | Order type: `limit`, `stoplimit`, or `market` |
| `status` | string | Order status after this match |
| `side` | string | Order side: `buy` or `sell` |
| `is_maker` | boolean | True if this order was the maker |
| `price` | string | Execution price |
| `executed_currency` | string | Currency of executed amount |
| `executed_amount` | string | Amount executed in this trade |
| `received_currency` | string | Currency received |
| `received_amount` | string | Amount received after fees |
| `fee_rate` | string | Fee rate applied |
| `total_fee` | string | Total fee for this trade |
| `credit_used` | string | Fee paid using credit |
| `net_fee_paid` | string | Net fee paid from wallet |
| `txn_ts` | number | Transaction timestamp (Unix seconds) |

---

## Order Status Mapping

The new trade system provides more granular order statuses. Here is the mapping between old and new statuses:

### Status Values

| New Status | Description |
|------------|-------------|
| `new` | Order created, pending processing |
| `open` | Order accepted, waiting in order book |
| `partial_filled` | Order partially executed |
| `filled` | Order fully executed |
| `canceled` | Order cancelled |
| `partial_filled_canceled` | Order partially filled then cancelled |
| `rejected` | Order rejected |
| `untriggered` | Conditional order waiting for trigger (stop-limit) |

### Mapping: Old Status to New Status

| Old Status | New Statuses |
|------------|--------------|
| `unfilled` | `new`, `open`, `partial_filled` |
| `filled` | `filled` |
| `cancel` | `rejected`, `canceled`, `partial_filled_canceled` |
| *(new)* | `untriggered` |

### Mapping: New Status to Old Status

For backward compatibility, you can map new statuses to old statuses:

| New Status | Old Status |
|------------|------------|
| `new` | `unfilled` |
| `open` | `unfilled` |
| `partial_filled` | `unfilled` |
| `filled` | `filled` |
| `canceled` | `cancel` |
| `partial_filled_canceled` | `cancel` |
| `rejected` | `cancel` |
| `untriggered` | `untriggered` |

---

## Error Codes

| Code | Description |
|------|-------------|
| `200` | Success |
| `400` | Bad Request - Invalid request format |
| `401` | Unauthorized - Authentication failed |
| `404` | Not Found - Resource not found |
| `500` | Internal Server Error |

---

## Complete Example

### JavaScript Implementation

```javascript
const CryptoJS = require('crypto-js');

const WEBSOCKET_URL = 'wss://stream.bitkub.com/v3/private';
const API_KEY = 'your_api_key';
const API_SECRET = 'your_api_secret';

let ws;
let pingInterval;

// Connect to WebSocket
function connect() {
    // Set User-Agent header to identify your application
    ws = new WebSocket(WEBSOCKET_URL, [], {
        headers: { 'User-Agent': 'my-trading-bot/1.0.0' }
    });

    ws.onopen = () => {
        console.log('Connected to Private WebSocket');
        authenticate();
        startPingInterval();
    };

    ws.onmessage = (event) => {
        const message = JSON.parse(event.data);
        handleMessage(message);
    };

    ws.onclose = (event) => {
        console.log('Connection closed:', event.code, event.reason);
        stopPingInterval();
        // Implement reconnection logic
        setTimeout(connect, 5000);
    };

    ws.onerror = (error) => {
        console.error('WebSocket error:', error);
    };
}

// Authenticate
function authenticate() {
    const timestamp = Date.now();
    const payload = [timestamp];
    const signature = CryptoJS.HmacSHA256(payload.join(""), API_SECRET).toString(CryptoJS.enc.Hex);

    const authMessage = {
        event: "auth",
        data: {
            "X-BTK-APIKEY": API_KEY,
            "X-BTK-SIGN": signature,
            "X-BTK-TIMESTAMP": timestamp.toString()
        }
    };

    ws.send(JSON.stringify(authMessage));
}

// Subscribe to channels
function subscribe(channel) {
    const subscribeMessage = {
        event: "subscribe",
        channel: channel
    };
    ws.send(JSON.stringify(subscribeMessage));
}

// Send ping
function sendPing() {
    if (ws.readyState === WebSocket.OPEN) {
        ws.send(JSON.stringify({ event: "ping" }));
    }
}

// Start ping interval (every 4 minutes)
function startPingInterval() {
    pingInterval = setInterval(sendPing, 4 * 60 * 1000);
}

// Stop ping interval
function stopPingInterval() {
    if (pingInterval) {
        clearInterval(pingInterval);
        pingInterval = null;
    }
}

// Handle incoming messages
function handleMessage(message) {
    switch (message.event) {
        case 'auth':
            if (message.code === '200') {
                console.log('Authentication successful');
                subscribe('order_update');
                subscribe('match_update');
            } else {
                console.error('Authentication failed:', message.message);
            }
            break;

        case 'subscribe':
            console.log(`Subscribed to ${message.channel}`);
            break;

        case 'order_update':
            handleOrderUpdate(message.data);
            break;

        case 'match_update':
            handleMatchUpdate(message.data);
            break;

        case 'ping':
            console.log('Pong received:', message.data.message);
            break;

        default:
            console.log('Unknown message:', message);
    }
}

// Handle order updates
function handleOrderUpdate(data) {
    console.log('Order Update:', {
        orderId: data.order_id,
        symbol: data.symbol,
        side: data.side,
        status: data.status,
        executedAmount: data.executed_amount,
        receivedAmount: data.received_amount
    });
}

// Handle match updates
function handleMatchUpdate(data) {
    console.log('Match Update:', {
        orderId: data.order_id,
        txnId: data.txn_id,
        symbol: data.symbol,
        side: data.side,
        price: data.price,
        isMaker: data.is_maker
    });
}

// Start connection
connect();
```

---

## Security Best Practices

1. **Never expose API secrets**: Keep your API secret secure and never log it
2. **Use secure connections**: Always use `wss://` (WebSocket Secure)
3. **Validate signatures**: Ensure proper HMAC SHA256 signature generation
4. **Handle disconnections**: Implement proper reconnection logic
5. **Monitor connection health**: Track pings/pongs and reconnect if needed
6. **Set a User-Agent header**: Include a descriptive `User-Agent` when connecting to help with server-side identification and debugging
