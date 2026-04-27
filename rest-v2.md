# REST API V2 — Deprecated

## Announcement

N/A — V2 endpoints have been removed.

## Change Log

* 2024-07-25 Deprecated and removed all V2 secure endpoints

## Overview

**⚠️ All V2 secure endpoints have been removed. This document is for historical reference only.**

V2 introduced secure endpoints with API key authentication. These endpoints were deprecated in July 2024 and fully removed. Please use V3 or V4 equivalents.

## Base URL

N/A — V2 endpoints have been removed.

## Authentication

N/A — V2 passed API key as a query parameter: `?api_key=xxx`. V3+ uses header-based auth: `X-BTK-APIKEY`, `X-BTK-TIMESTAMP`, `X-BTK-SIGN`.

## Endpoints

| Endpoint | Method | Replacement |
| -------- | ------ | ----------- |
| /api/v2/market/wallet | POST | POST /api/v3/market/wallet |
| /api/v2/market/balances | POST | POST /api/v3/market/balances |
| /api/v2/market/place-bid | POST | POST /api/v3/market/place-bid |
| /api/v2/market/place-ask | POST | POST /api/v3/market/place-ask |
| /api/v2/market/cancel-order | POST | POST /api/v3/market/cancel-order |
| /api/v2/market/my-open-orders | GET | GET /api/v3/market/my-open-orders |
| /api/v2/market/my-order-history | GET | GET /api/v3/market/my-order-history |
| /api/v2/market/order-info | GET | GET /api/v3/market/order-info |
| /api/v2/crypto/addresses | POST | POST /api/v4/crypto/addresses |
| /api/v2/crypto/withdraw | POST | POST /api/v4/crypto/withdraws |
| /api/v2/crypto/deposit-history | POST | GET /api/v4/crypto/deposits |
| /api/v2/crypto/withdraw-history | POST | GET /api/v4/crypto/withdraws |
| /api/v2/fiat/accounts | POST | GET /api/v4/fiat/accounts |
| /api/v2/fiat/withdraw | POST | POST /api/v4/fiat/withdraw |
| /api/v2/fiat/deposit-history | POST | GET /api/v4/fiat/deposit/history |
| /api/v2/fiat/withdraw-history | POST | GET /api/v4/fiat/withdraw/history |

## Additional

N/A — V2 endpoints have been removed.

## Error Codes

### Status Codes

N/A — V2 endpoints have been removed.

### Numeric Errors

N/A — V2 endpoints have been removed.

### System Errors

N/A — V2 endpoints have been removed.

### Business Errors

N/A — V2 endpoints have been removed.

### Validation Errors

N/A — V2 endpoints have been removed.

### Authentication Errors

N/A — V2 endpoints have been removed.

## Rate Limits

N/A — V2 endpoints have been removed.
