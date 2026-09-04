<div align="center">

# 🌊 Bitkub Official API Docs 🌊

### 📚 The one and only official documentation for Bitkub's trading APIs 📚

[![Official](https://img.shields.io/badge/Bitkub-OFFICIAL-1AB759.svg?style=for-the-badge)](https://github.com/bitkub/bitkub-official-api-docs)
[![REST API](https://img.shields.io/badge/REST-V3%20%26%20V4-0A66C2.svg?style=for-the-badge)](./rest-v4.md)
[![WebSocket](https://img.shields.io/badge/WebSocket-Realtime-8B5CF6.svg?style=for-the-badge)](./websocket-public.md)

**✅ Officially supported & maintained by Bitkub's own development team**

</div>

> ⚠️ **Accept no substitutes.** The use of any other projects is **not supported** — please make sure you are visiting **[bitkub/bitkub-official-api-docs](https://github.com/bitkub/bitkub-official-api-docs)**.

<br>

## Documentation

| 📄 Reference | Description |
| ------------ | ----------- |
| [rest-v1.md](./rest-v1.md) | Details on the RESTful API V1 (deprecated) |
| [rest-v2.md](./rest-v2.md) | Details on the RESTful API V2 (deprecated) |
| [rest-v3.md](./rest-v3.md) | Details on the RESTful API V3 |
| [rest-v4.md](./rest-v4.md) | Details on the RESTful API V4 |
| [websocket-public.md](./websocket-public.md) | Details on the Public WebSocket API |
| [websocket-private.md](./websocket-private.md) | Details on the Private WebSocket API |


---

<br>

## Announcement

* Fiat deposit and withdraw history older than 90 days is archived for [deposits](rest-v4.md#get-apiv4fiatdeposithistory) and [withdraws](rest-v4.md#get-apiv4fiatwithdrawhistory). More details [here](https://support.bitkub.com/en/solutions/articles?id=KM000009940).
* Crypto deposit and withdraw history older than 90 days is archived for [deposits](rest-v4.md#get-apiv4cryptodeposits) and [withdraws](rest-v4.md#get-apiv4cryptowithdraws). More details [here](https://support.bitkub.com/en/solutions/articles?id=KM000009898).
* `/api/v3/market/wallet` and `/api/v3/market/balances` are deprecated. Please migrate to [GET /api/v4/wallet/balances](rest-v4.md#get-apiv4walletbalances) and [GET /api/v4/wallet/assets](rest-v4.md#get-apiv4walletassets) as replacements.
* Fiat v3 endpoints will be deprecated on 09 June 2026.** Please migrate to [Fiat v4 endpoints](rest-v4.md) as replacement: POST /api/v3/fiat/accounts, POST /api/v3/fiat/withdraw, POST /api/v3/fiat/deposit-history, POST /api/v3/fiat/withdraw-history
* remove status: "cancelled" from [my-order-info](rest-v3.md#get-apiv3marketorder-info) after 3 days period. remove on 9 April 2026
* The following market endpoints will be deprecated on 9 Dec 2025. Please use [v3 endpoints](rest-v3.md#non-secure-endpoints-v3) as replacement: GET /api/market/symbols, GET /api/market/ticker, GET /api/market/trades, GET /api/market/bids, GET /api/market/asks, GET /api/market/books, GET /api/market/depth
* Page-based pagination will be deprecated on 8 Sep 2025 for [my-order-history](rest-v3.md#get-apiv3marketmy-order-history).
* Order history older than 90 days is archived for [my-order-history](rest-v3.md#get-apiv3marketmy-order-history) More details here.
* order_id and txn_id formats of [my-open-orders](rest-v3.md#get-apiv3marketmy-open-orders), [my-order-history](rest-v3.md#get-apiv3marketmy-order-history), [my-order-info](rest-v3.md#get-apiv3marketorder-info), [place-bid](rest-v3.md#post-apiv3marketplace-bid), [place-ask](rest-v3.md#post-apiv3marketplace-ask), [cancel-order](rest-v3.md#post-apiv3marketcancel-order) may change for some symbols due to a system upgrade, See affected symbols and detail : [here](https://support.bitkub.com/en/support/solutions/articles/151000214886-announcement-trading-system-upgrade)
* API Specifications for Crypto Endpoints, please refer to the documentation here: [Crypto Endpoints](rest-v4.md)
* Deprecation of Order Hash for [my-open-orders](rest-v3.md#get-apiv3marketmy-open-orders), [my-order-history](rest-v3.md#get-apiv3marketmy-order-history), [my-order-info](rest-v3.md#get-apiv3marketorder-info), [place-bid](rest-v3.md#post-apiv3marketplace-bid), [place-ask](rest-v3.md#post-apiv3marketplace-ask), [cancel-order](rest-v3.md#post-apiv3marketcancel-order) on 28/02/2025 onwards, More details [here](https://support.bitkub.com/en/support/solutions/articles/151000205895-notice-deprecation-of-order-hash-from-public-api-on-28-02-2025-onwards)
* Deposit history records are available for the last 90 days only for GET /api/v4/crypto/deposits. Records older than 90 days are archived.
* **⚠️ 2026-05-18:** `market.trade.<symbol>` stream will be permanently closed. Please migrate to Private WebSocket.

<!-- HIDDEN until the launch blog post is published — restore by deleting this comment wrapper.

---
<br>
<br>
<br>


<div align="center">

# ⚡ Official Bitkub Trading MCP Server ⚡

### 🚀 Connect Claude, Cursor, or any MCP client straight to live Thai crypto markets — in natural language 🚀

[![npm](https://img.shields.io/npm/v/bitkub-trading-mcp-official?color=CB3837&label=npm&logo=npm&style=for-the-badge)](https://www.npmjs.com/package/bitkub-trading-mcp-official)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)
[![Node.js 20+](https://img.shields.io/badge/node-20.0+-339933.svg?style=for-the-badge&logo=node.js&logoColor=white)](https://nodejs.org/)
[![MCP](https://img.shields.io/badge/MCP-compatible-4A154B.svg?style=for-the-badge)](https://modelcontextprotocol.io)

**✅ OFFICIAL — built & maintained by Bitkub's own development team**

📦 **[bitkub-trading-mcp-official on npm →](https://www.npmjs.com/package/bitkub-trading-mcp-official)**

</div>

> ⚠️ **THIS SERVER CAN MOVE REAL MONEY.** Alongside read-only market-data and account tools, it exposes **trading, withdrawal, and deposit-address** endpoints. Every one of them sits behind a **two-step confirmation gate** — the first call only previews the exact request and returns a one-time token; nothing reaches Bitkub until a second call replays that token. Set `BITKUB_ENABLE_WRITE=false` to switch the whole write group off, and scope your API key to only the permissions you actually want reachable.

### ✨ What you get

| 🎯 | Capability |
| -- | ---------- |
| 📊 | **9 public REST tools** — status, servertime, symbols, ticker, bids/asks, depth, trades, TradingView history |
| 🔐 | **12 secure REST tools** (read-only) — wallet balances, crypto/fiat deposits & withdraws history, addresses, open orders, order history, user limits & trading credits |
| ⚠️ | **6 write tools** (gated) — place bid/ask, cancel order, generate crypto address, crypto withdraw, fiat withdraw |
| 📡 | **3 WebSocket snapshot tools** — public ticker + private order/match updates |
| 🧠 | **Ask in plain language** — *"What's in my Bitkub wallet?"*, *"Show me the BTC order book"* |

### ⚡ Quick install — one command (public data, no API key)

```bash
claude mcp add bitkub-trading-mcp-official -- npx -y bitkub-trading-mcp-official
```

### 🔑 With API key (full features — your wallet, orders & matches)

```bash
claude mcp add bitkub-trading-mcp-official \
  -e BITKUB_API_KEY=your_real_key_here \
  -e BITKUB_API_SECRET=your_real_secret_here \
  -- npx -y bitkub-trading-mcp-official
```

### 🛡️ Read-only mode (recommended if you only want an AI-readable view)

```bash
claude mcp add bitkub-trading-mcp-official \
  -e BITKUB_API_KEY=your_real_key_here \
  -e BITKUB_API_SECRET=your_real_secret_here \
  -e BITKUB_ENABLE_WRITE=false \
  -- npx -y bitkub-trading-mcp-official
```

> 💡 Generate a key at **<https://www.bitkub.com/en/api-management>**. Your key's permissions are the real boundary: leave **Trade**, **Deposit**, and **Withdraw** unchecked for a read-only key; grant only the scopes you intend to use otherwise. Crypto withdrawals additionally require the destination to be on your **trusted address** list.

📖 Full docs, write-tool reference & the confirmation-gate spec: **<https://github.com/bitkub/bitkub-trading-mcp-official>**

-->
