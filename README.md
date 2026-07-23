<div align="center">

# 🌊 Bitkub Official API Docs 🌊

### 📚 The one and only official documentation for Bitkub's trading APIs 📚

[![Official](https://img.shields.io/badge/Bitkub-OFFICIAL-1AB759.svg?style=for-the-badge)](https://github.com/bitkub/bitkub-official-api-docs)
[![REST API](https://img.shields.io/badge/REST-V3%20%26%20V4-0A66C2.svg?style=for-the-badge)](./restful-api-v4.md)
[![WebSocket](https://img.shields.io/badge/WebSocket-Realtime-8B5CF6.svg?style=for-the-badge)](./websocket-api.md)

**✅ Officially supported & maintained by Bitkub's own development team**

</div>

> ⚠️ **Accept no substitutes.** The use of any other projects is **not supported** — please make sure you are visiting **[bitkub/bitkub-official-api-docs](https://github.com/bitkub/bitkub-official-api-docs)**.

## 📖 Documentation

| 📄 Reference | Description |
| ------------ | ----------- |
| 🟢 **[restful-api.md](./restful-api.md)** | Details on the RESTful API **V3** (`/api`) |
| 🔵 **[restful-api-v4.md](./restful-api-v4.md)** | Details on the RESTful API **V4** (`/api`) |
| 🟣 **[websocket-api.md](./websocket-api.md)** | Details on the WebSocket API (`/websocket-api`) |
| 🔒 **[private-websocket.md](./private-websocket.md)** | Details on the Private WebSocket streams |

---

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

> 🔒 **READ-ONLY BY DESIGN.** This server **never** places orders, cancels orders, generates addresses, or initiates withdrawals. Only data-retrieval and observation endpoints are exposed — even a key with full trading permissions cannot move money or submit orders.

### ✨ What you get

| 🎯 | Capability |
| -- | ---------- |
| 📊 | **9 public REST tools** — status, symbols, ticker, bids/asks, depth, trades, TradingView history |
| 🔐 | **12 secure REST tools** — wallet balances, crypto/fiat deposits & withdraws, open orders, order history, user limits |
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

> 💡 Generate a key at **<https://www.bitkub.com/en/api-management>** — enable **read-only** permissions only.

📖 Full docs, manual config & tool reference: **<https://github.com/bitkub/bitkub-trading-mcp-official>**

