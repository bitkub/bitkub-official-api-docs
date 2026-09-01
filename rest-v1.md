# REST API V1 — Deprecated

## Change Log

* 2024-07-25 Deprecated the V1 secure endpoints and removed them from the document

## Overview

**⚠️ The V1 `/api/market/*` endpoints have been removed. The three unversioned endpoints documented below — `/api/status`, `/api/servertime` and `/tradingview/history` — remain available, but for enhanced security and performance we strongly recommend using the [V3 non-secure endpoints](rest-v3.md#non-secure-endpoints-v3).**

V1 was the original Bitkub REST API. Its `/api/market/*` endpoints were deprecated and removed; please use the V3 or V4 equivalents listed below.

## Base URL

`https://api.bitkub.com` — for the three unversioned endpoints that remain available. The removed V1 `/api/market/*` endpoints have no base URL.

## Authentication

N/A — V1 used a different authentication mechanism (API key in query string). V3+ uses header-based auth: `X-BTK-APIKEY`, `X-BTK-TIMESTAMP`, `X-BTK-SIGN`.

## Endpoints

| Endpoint | Method | Replacement |
| -------- | ------ | ----------- |
| /api/market/symbols | GET | GET /api/v3/market/symbols |
| /api/market/ticker | GET | GET /api/v3/market/ticker |
| /api/market/trades | GET | GET /api/v3/market/trades |
| /api/market/bids | GET | GET /api/v3/market/bids |
| /api/market/asks | GET | GET /api/v3/market/asks |
| /api/market/books | GET | GET /api/v3/market/depth |
| /api/market/depth | GET | GET /api/v3/market/depth |
| /api/market/wallet | POST | GET /api/v4/wallet/assets |
| /api/market/balances | POST | GET /api/v4/wallet/balances |
| /api/market/place-bid | POST | POST /api/v3/market/place-bid |
| /api/market/place-ask | POST | POST /api/v3/market/place-ask |
| /api/market/cancel-order | POST | POST /api/v3/market/cancel-order |
| /api/market/my-open-orders | GET | GET /api/v3/market/my-open-orders |
| /api/market/my-order-history | GET | GET /api/v3/market/my-order-history |
| /api/market/order-info | GET | GET /api/v3/market/order-info |
| /api/servertime | GET | GET /api/v3/servertime |
| /api/status | GET | N/A — unversioned, no replacement |
| /tradingview/history | GET | N/A — unversioned, no replacement |

### GET /api/status

#### Description:
Get endpoint status. When status is not `ok`, it is highly recommended to wait until the status changes back to `ok`.


#### Required Permission:
N/A

#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/status'
```

#### Response:
```json
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

#### Field Descriptions:
N/A

<br>

---

<br>

### GET /api/servertime

#### Description:
Get server timestamp.

**⚠️ DEPRECATED:** Cannot be used with secure endpoint V3 signing. Use [GET /api/v3/servertime](rest-v3.md#get-apiv3servertime) instead.


#### Required Permission:
N/A

#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/servertime'
```

#### Response:
```json
1707220534359
```

#### Field Descriptions:
N/A

<br>

---

<br>

### GET /tradingview/history

#### Description:
Get OHLCV historical data for TradingView chart integration.


#### Required Permission:
N/A
#### Query Params:

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| symbol | string | true | The symbol (e.g. BTC_THB) |
| resolution | string | true | Chart resolution: 1, 5, 15, 60, 240, 1D |
| from | int | true | Start timestamp (Unix seconds) |
| to | int | true | End timestamp (Unix seconds) |


#### Validation Rules:
N/A
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/tradingview/history?symbol=BTC_THB&resolution=60&from=1633424427&to=1633427427'
```

#### Response:
```json
{
  "c": [1685000, 1680699.95, 1688998.99, 1692222.22],
  "h": [1685000, 1685000, 1689000, 1692222.22],
  "l": [1680053.22, 1671000, 1680000, 1684995.07],
  "o": [1682500, 1685000, 1680100, 1684995.07],
  "s": "ok",
  "t": [1633424400, 1633425300, 1633426200, 1633427100],
  "v": [4.604352630000001, 8.530631670000005, 4.836581560000002, 2.851018920000002]
}
```

#### Field Descriptions:
N/A

<br>

---

<br>

## Additional

N/A — V1 endpoints have been removed.

## Error Codes

### Status Codes

N/A — V1 endpoints have been removed.

### Authentication Errors

N/A — V1 endpoints have been removed.

### User Errors

N/A — V1 endpoints have been removed.

### Trading Errors

N/A — V1 endpoints have been removed.

### Withdrawal Errors

N/A — V1 endpoints have been removed.

### System Errors

N/A — V1 endpoints have been removed.

### Business Errors

N/A — V1 endpoints have been removed.

### Validation Errors

N/A — V1 endpoints have been removed.

## Rate Limits

The removed V1 `/api/market/*` endpoints have no rate limits. Limits for the three unversioned endpoints that remain available are listed in [rest-v3.md](rest-v3.md#rate-limits).
