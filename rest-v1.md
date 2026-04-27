# REST API V1 — Deprecated

## Announcement

N/A — V1 endpoints have been removed.

## Change Log

* 2024-07-25 Deprecated and removed all V1 endpoints

## Overview

**⚠️ All V1 endpoints have been removed. This document is for historical reference only.**

V1 was the original Bitkub REST API. All endpoints were deprecated in 2024 and fully removed. Please use V3 or V4 equivalents.

## Base URL

N/A — V1 endpoints have been removed.

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
| /api/market/wallet | POST | POST /api/v3/market/wallet |
| /api/market/balances | POST | POST /api/v3/market/balances |
| /api/market/place-bid | POST | POST /api/v3/market/place-bid |
| /api/market/place-ask | POST | POST /api/v3/market/place-ask |
| /api/market/cancel-order | POST | POST /api/v3/market/cancel-order |
| /api/market/my-open-orders | GET | GET /api/v3/market/my-open-orders |
| /api/market/my-order-history | GET | GET /api/v3/market/my-order-history |
| /api/market/order-info | GET | GET /api/v3/market/order-info |

## Additional

N/A — V1 endpoints have been removed.

## Error Codes

### Status Codes

N/A — V1 endpoints have been removed.

### Numeric Errors

N/A — V1 endpoints have been removed.

### System Errors

N/A — V1 endpoints have been removed.

### Business Errors

N/A — V1 endpoints have been removed.

### Validation Errors

N/A — V1 endpoints have been removed.

### Authentication Errors

N/A — V1 endpoints have been removed.

## Rate Limits

N/A — V1 endpoints have been removed.
