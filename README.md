# bitkub-official-api-docs
Official Documentation for Bitkub APIs

* The documentation described here is official. This means the documentation is officially supported and maintained by Bitkub's own development team.
* The use of any other projects is **not supported**; please make sure you are visiting **bitkub/bitkub-official-api-docs**.


Name | Description
------------ | ------------
[rest-v1.md](./rest-v1.md)                     | Details on the RESTful API V1 (deprecated)
[rest-v2.md](./rest-v2.md)                     | Details on the RESTful API V2 (deprecated)
[rest-v3.md](./rest-v3.md)                     | Details on the RESTful API V3
[rest-v4.md](./rest-v4.md)                     | Details on the RESTful API V4
[websocket-public.md](./websocket-public.md)   | Details on the Public WebSocket API
[websocket-private.md](./websocket-private.md) | Details on the Private WebSocket API

## Announcement

* Fiat deposit and withdraw history older than 90 days is archived for [deposits](rest-v4.md#get-apiv4fiatdeposithistory) and [withdraws](rest-v4.md#get-apiv4fiatwithdrawhistory). More details [here](https://support.bitkub.com/en/solutions/articles?id=KM000009940).
* Crypto deposit and withdraw history older than 90 days is archived for [deposits](rest-v4.md#get-apiv4cryptodeposits) and [withdraws](rest-v4.md#get-apiv4cryptowithdraws). More details [here](https://support.bitkub.com/en/solutions/articles?id=KM000009898).
* `/api/v3/market/wallet` and `/api/v3/market/balances` are deprecated. Please migrate to [GET /api/v4/wallet/balances](rest-v4.md#get-apiv4walletbalances) and [GET /api/v4/wallet/assets](rest-v4.md#get-apiv4walletassets) as replacements.
* Fiat v3 endpoints will be deprecated on 09 June 2026.** Please migrate to [Fiat v4 endpoints](rest-v4.md) as replacement: [POST /api/v3/fiat/accounts](rest-v3.md#post-apiv3fiataccounts), [POST /api/v3/fiat/withdraw](rest-v3.md#post-apiv3fiatwithdraw), [POST /api/v3/fiat/deposit-history](rest-v3.md#post-apiv3fiatdeposit-history), [POST /api/v3/fiat/withdraw-history](rest-v3.md#post-apiv3fiatwithdraw-history)
* remove status: "cancelled" from [my-order-info](rest-v3.md#get-apiv3marketorder-info) after 3 days period. remove on 9 April 2026
* The following market endpoints will be deprecated on 9 Dec 2025. Please use [v3 endpoints](rest-v3.md#non-secure-endpoints-v3) as replacement: GET /api/market/symbols, GET /api/market/ticker, GET /api/market/trades, GET /api/market/bids, GET /api/market/asks, GET /api/market/books, GET /api/market/depth
* Page-based pagination will be deprecated on 8 Sep 2025 for [my-order-history](rest-v3.md#get-apiv3marketmy-order-history).
* Order history older than 90 days is archived for [my-order-history](rest-v3.md#get-apiv3marketmy-order-history) More details here.
* order_id and txn_id formats of [my-open-orders](rest-v3.md#get-apiv3marketmy-open-orders), [my-order-history](rest-v3.md#get-apiv3marketmy-order-history), [my-order-info](rest-v3.md#get-apiv3marketorder-info), [place-bid](rest-v3.md#post-apiv3marketplace-bid), [place-ask](rest-v3.md#post-apiv3marketplace-ask), [cancel-order](rest-v3.md#post-apiv3marketcancel-order) may change for some symbols due to a system upgrade, See affected symbols and detail : [here](https://support.bitkub.com/en/support/solutions/articles/151000214886-announcement-trading-system-upgrade)
* API Specifications for Crypto Endpoints, please refer to the documentation here: [Crypto Endpoints](rest-v4.md)
* Deprecation of Order Hash for [my-open-orders](rest-v3.md#get-apiv3marketmy-open-orders), [my-order-history](rest-v3.md#get-apiv3marketmy-order-history), [my-order-info](rest-v3.md#get-apiv3marketorder-info), [place-bid](rest-v3.md#post-apiv3marketplace-bid), [place-ask](rest-v3.md#post-apiv3marketplace-ask), [cancel-order](rest-v3.md#post-apiv3marketcancel-order) on 28/02/2025 onwards, More details [here](https://support.bitkub.com/en/support/solutions/articles/151000205895-notice-deprecation-of-order-hash-from-public-api-on-28-02-2025-onwards)
* Deposit history records are available for the last 90 days only for GET /api/v4/crypto/deposits. Records older than 90 days are archived.
* **⚠️ 2026-05-18:** `market.trade.<symbol>` stream will be permanently closed. Please migrate to Private WebSocket.
