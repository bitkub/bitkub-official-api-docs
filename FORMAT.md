# Documentation Format Specification

This document specifies the markdown format used across this repository's API documentation files. It exists so that anyone adding a new endpoint, stream, or file follows the same structure the [`bitkub-api-document`](https://github.com/bitkub-online-corp/bitkub-api-document) reader already parses — inconsistent formatting is what made the previous generation of these docs unreliable to render (see the "Why this format exists" section).

If you are about to write or edit documentation in this repo, this is the file to follow. When in doubt, copy the shape of an existing endpoint/stream rather than inventing a new one.

## Contents

- [File inventory](#file-inventory)
- [Why this format exists](#why-this-format-exists)
- [Universal conventions](#universal-conventions)
  - [Heading hierarchy](#heading-hierarchy)
  - [The N/A convention](#the-na-convention)
  - [Section separators](#section-separators)
  - [Tables](#tables)
  - [Internal cross-reference links](#internal-cross-reference-links)
  - [Change Log / Announcement bullet format](#change-log--announcement-bullet-format)
- [REST API files (`rest-v3.md`, `rest-v4.md`)](#rest-api-files-rest-v3md-rest-v4md)
  - [File-level structure](#file-level-structure)
  - [Endpoint Index](#endpoint-index)
  - [Per-endpoint structure](#per-endpoint-structure)
  - [Error Codes section](#error-codes-section)
  - [Rate Limits section](#rate-limits-section)
- [WebSocket files (`websocket-public.md`, `websocket-private.md`)](#websocket-files-websocket-publicmd-websocket-privatemd)
  - [File-level structure](#file-level-structure-1)
  - [Per-stream structure](#per-stream-structure)
  - [Multiple Response variants (one stream, several event shapes)](#multiple-response-variants-one-stream-several-event-shapes)
  - [Order Status Mapping (private only)](#order-status-mapping-private-only)
- [README.md](#readmemd)
- [Deprecated files (`rest-v1.md`, `rest-v2.md`)](#deprecated-files-rest-v1md-rest-v2md)
- [Checklist: adding a new REST endpoint](#checklist-adding-a-new-rest-endpoint)
- [Checklist: adding a new WebSocket stream/event](#checklist-adding-a-new-websocket-streamevent)

---

## File inventory

| File | Role |
|---|---|
| `README.md` | Repo landing page, file index, and the single consolidated `## Announcement` section (see [README.md](#readmemd)) |
| `rest-v3.md` | REST API V3 — current stable REST API |
| `rest-v4.md` | REST API V4 — newer REST API surface (crypto/wallet/fiat) |
| `websocket-public.md` | Public WebSocket API — unauthenticated market-data streams |
| `websocket-private.md` | Private WebSocket API — authenticated order/match streams |
| `rest-v1.md` | REST API V1 — deprecated, historical reference only |
| `rest-v2.md` | REST API V2 — deprecated, historical reference only |

`rest-v1.md`/`rest-v2.md` exist as historical reference pages. The `bitkub-api-document` reader does not fetch or render them today — they follow the same format for consistency, but are not required reading for the live docs site.

## Why this format exists

The previous format (still on this repo's `master` branch) had no shared convention across files: heading levels flipped between endpoints (`###` vs `####` for the identical concept), parameter sections were named three different ways (`Query:`/`Query (URL):`/`Body:`), and most endpoints had no `Field Descriptions` or `Required Permission` sections at all. A regex-based reader cannot parse "whatever the author happened to type" reliably — sections silently dropped, sometimes rendering blank pages with no error.

This format fixes that by giving every file, every endpoint, and every stream the **same heading set, in the same style**, so a reader only needs one set of rules per concept (`#### Response:`, `#### Field Descriptions:`, etc.) instead of one-off pattern-matching per file.

## Universal conventions

### Heading hierarchy

Applies consistently across every file in this repo:

- `#` — file title (one per file)
- `##` — major section (`Change Log`, `Overview`, `Endpoints`, `Error Codes`, `Rate Limits`, `Data Streams`, `Reference`, ...)
- `###` — an individual endpoint (`### GET /api/v3/market/ticker`), an individual stream (`### Live Order Book Stream`), or a subsection within Getting Started/Reference/Error Codes
- `####` — a fixed subsection **within** an endpoint or stream (`#### Description:`, `#### Response:`, `#### Field Descriptions:`, ...)

The same concept is **always** the same heading level everywhere. Never flip `### Description:` vs `#### Description:` between endpoints — this was the single biggest parsing failure in the old format.

### The N/A convention

A heading must never be omitted just because it doesn't apply to a given endpoint/stream/file. Instead, keep the heading and write a literal `N/A` (optionally with a reason after an em dash):

```
#### Field Descriptions:
N/A
```

```
### Authentication Flow

N/A — Public streams do not require authentication.
```

This matters for parsing: a reader can tell "documented as not applicable" apart from "this file/section is broken/missing" only if the heading itself is always present. An omitted heading looks like a parse failure; an `N/A` body does not.

### Section separators

Every endpoint and every stream is separated from the next one by:

```
<br>

---

<br>
```

Exactly one blank line before `<br>`, one blank line between `<br>` and `---`, and one blank line after `---` before the next `<br>`. This is deliberate — a bare `---` with only single newlines around it does not render with visible spacing on GitHub; the `<br>` tags plus double spacing do. Don't collapse this to a single blank line or drop the `<br>` tags.

### Tables

Standard GFM pipe tables: a header row, then a separator row (`| --- | --- |`), then data rows. Column widths/padding in the source don't matter — align them for readability if you like, but a parser must never rely on exact column width.

Two table shapes recur throughout:

- **Parameter tables** (`Query Params:`, `Body Params:`): 4 columns — `| Key | Type | Required | Description |`
- **Field Descriptions tables**: 3 columns — `| Field | Type | Description |` (no `Required` column — every documented field is, by definition, present in the response)

A header row is identified structurally (it's the row immediately before the `| --- |` separator), not by matching specific header text — so don't worry about the header cell wording being "Field" vs "Key" vs "Code" as long as the separator row follows it.

### Internal cross-reference links

Links between endpoints/sections use GitHub's auto-generated heading anchors: lowercase the heading text, strip punctuation, replace spaces with hyphens. Two forms are valid:

- Same-file: `[my-order-history](#get-apiv3marketmy-order-history)`
- Cross-file: `[GET /api/v4/wallet/balances](rest-v4.md#get-apiv4walletbalances)`

**Known limitation — do not link to multi-segment paths by anchor.** GitHub's slugifier strips `/` entirely rather than converting it to a hyphen, so `### GET /api/v4/fiat/deposit/history` and a hypothetical `### GET /api/v4/fiat/deposithistory` would produce the *same* anchor (`get-apiv4fiatdeposithistory`) — the segment boundary is lost and cannot be reconstructed from the anchor alone. If you need to reference an endpoint whose path has more than 3 segments after `/api/vN/`, prefer linking to the file (`[Fiat v4 endpoints](rest-v4.md)`) over a specific anchor, or spell out the fact prose-style instead of relying on the anchor resolving correctly downstream.

**Never link to an anchor that doesn't exist.** If an endpoint is removed, any Change Log/Announcement entry that referenced its anchor must have the link stripped back to plain text — a dangling anchor is broken even when viewed directly on GitHub, not just in the reader app.

### Change Log / Announcement bullet format

Both `## Change Log` (per-file) and `## Announcement` (README.md only, see below) use the same bullet shape:

```
* YYYY-MM-DD Description text, optionally with [inline links](url).
```

The leading `YYYY-MM-DD` is optional — undated entries are allowed (e.g. `* Deposit history records are available for the last 90 days...`) but dated entries should sort newest-first. When a file genuinely has no changelog, keep the heading and write:

```
## Change Log

N/A — No change log available for this API.
```

Bold-prefixed deprecation warnings use `**⚠️ DATE:**` inline at the start of the bullet, e.g.:

```
* **⚠️ 2026-05-18:** `market.trade.<symbol>` stream will be permanently closed. Please migrate to Private WebSocket.
```

---

## REST API files (`rest-v3.md`, `rest-v4.md`)

### File-level structure

In order:

```
# REST API V3                          ← file title
## Change Log                          ← dated bullets, newest first
# Table of contents                     ← manual anchor list, kept in sync by hand
## Overview
## Base URL
## Endpoint Index                       ← categorized summary tables, see below
## Authentication
## Endpoints                            ← wrapper heading, no content of its own
## Non-Secure Endpoints  /  ## Secure Endpoints   (v3), or category headings like ## Crypto Endpoints / ## Wallet Endpoints / ## Fiat Endpoints (v4)
  ### <grouping label, no endpoint of its own>   e.g. "### Server Information", "### Address Management"
  ### GET /api/v3/... | ### POST /api/v3/...      ← one per endpoint, see "Per-endpoint structure"
## Additional                           ← free-form extra notes, or "N/A — ..."
## Error Codes
  ### Status Codes
  ### <category> Errors                 ← one or more category tables, see below
## Rate Limits
```

`v3` and `v4` differ slightly in how endpoints are grouped under `## Endpoints` (v3: Non-Secure/Secure; v4: by domain — Crypto/Wallet/Fiat) and in a couple of optional bare `###` grouping labels used purely as visual dividers with no `#### Name:`-style content underneath them (e.g. `### Market Data (Read-Only)`, `### Trading Operations`, `### Address Management`). These grouping labels are optional prose, not a fixed part of the endpoint format.

### Endpoint Index

A quick-reference table (or set of tables) under `## Endpoint Index`, grouped by category, always with an `Endpoint` column (linking to the endpoint's anchor) and a `Method` column. v3's Secure Endpoints table additionally has `Trade`/`Deposit`/`Withdraw` columns marked with `✅` to show which permission scopes an endpoint needs — this is a human-readable summary, not parsed independently of the endpoint's own `Required Permission` heading.

```
### Non-Secure Endpoints V3

| Market Data Endpoint | Method |
| --------------------------------------------------------------| ------ |
| [GET /api/v3/market/symbols](#get-apiv3marketsymbols)         | GET    |
...
```

### Per-endpoint structure

Every endpoint is a `### GET /api/vN/...` or `### POST /api/vN/...` heading (method + full path, exactly as called), followed by these `####` subsections, in this order:

```
### GET /api/v3/market/ticker

#### Description:
<prose — one or two sentences>

#### Required Permission:
N/A
```
— or, when a permission scope is required, inline on the heading line itself instead of a separate body:
```
#### Required Permission: `is_deposit`
```

```
#### Query Params:                     ← GET endpoints
-- or --
#### Body Params:                      ← POST endpoints

| Key | Type | Required | Description |
| --- | ---- | -------- | ----------- |
| sym | string | false | The symbol (e.g. btc_thb) |
```
(Omit this heading entirely only if the endpoint truly takes no parameters at all, e.g. `GET /api/v3/servertime` — otherwise keep it and write `N/A` in the body.)

```
#### Validation Rules:
N/A
-- or, a bullet list --
- `sym` is required and must be a valid trading symbol
- `p` and `cursor` cannot be used together
```

```
#### Example cURL:
```bash
curl --location 'https://api.bitkub.com/api/v3/market/ticker?sym=btc_thb'
```
```

```
#### Response:
```json
{ "...": "..." }
```
```
(See [Multiple Response variants](#multiple-response-variants-one-stream-several-event-shapes) — the same `(label)` suffix convention applies to REST endpoints too, e.g. `#### Response (Page-based pagination):` / `#### Response (Keyset-based pagination):` on `GET /api/v3/market/my-order-history`.)

```
#### Field Descriptions:
N/A
-- or --
| Field | Type | Description |
| ----- | ---- | ----------- |
| txn_id | string | Transaction ID |
```

**Recommended heading order** is `Description → Required Permission → Query/Body Params → Validation Rules → Example cURL → Response → Field Descriptions`. In practice `Validation Rules` and `Example cURL` sometimes swap order in existing content — the reader matches headings by name, not position, so this doesn't break parsing, but new endpoints should follow the recommended order for readability.

Every endpoint ends with a [section separator](#section-separators) before the next `###` heading.

### Error Codes section

```
## Error Codes

### Status Codes
```

For v3 (numeric error codes, no HTTP-status concept): `N/A — V3 uses numeric error codes, not HTTP status-based codes.`

For v4 (structured string codes that do carry an HTTP status): a real table —

```
| HTTP Status | Meaning |
| ----------- | ------- |
| 200 | OK — The request was processed as expected |
| 400 | INVALID_REQUEST — ... |
```

Followed by one `###` heading per error category, each its own table. **v3 categories** (2-column, no per-code status): `### Authentication Errors`, `### User Errors`, `### Trading Errors`, `### Withdrawal Errors`, `### System Errors` —

```
| Code | Description |
| ---- | ----------- |
| 1 | Invalid JSON payload |
```

**v4 categories** (3-column, includes the HTTP status per code): `### Numeric Errors` (N/A for v4 — it's the inverse convention of v3's `Status Codes`), `### System Errors`, `### Business Errors`, `### Validation Errors`, `### Authentication Errors` —

```
| Code | Status | Message |
| ---- | ------ | ------- |
| B1000-CW | 400 | User account is suspended |
```

A single table cell may contain multiple related messages using `<br>` line breaks and `•` bullets:

```
| 9 | • Invalid user <br> • User not found <br> • Freeze withdrawal |
```

A code whose meaning changed can show the old meaning struck through: `~~Transaction not found~~ <br> User bank is not verified`.

### Rate Limits section

```
## Rate Limits

<one paragraph of prose about the general rule>

| Endpoint | Rate Limit |
| -------- | ---------- |
| /api/v3/market/ticker | 100 req/sec |
```

---

## WebSocket files (`websocket-public.md`, `websocket-private.md`)

### File-level structure

```
# WebSocket API — Public (2023-04-19)     ← file title, optionally dated
## Change Log
## Overview
## Endpoint                                ← table: Environment | WebSocket URL
## Getting Started
  ### Connection Requirements
  ### Authentication Flow
  ### Subscription Management
  ### Keep-Alive
## Data Streams
  ### <Stream Name>                        ← one per stream/event, see "Per-stream structure"
## Reference
  ### Stream Demo
  ### Order Status Values
  ### Error Codes
## Complete Example
  ### JavaScript Implementation
## Security Best Practices
```

`websocket-private.md` additionally nests real content under `### Authentication Flow` (`#### Generate Signature`, `#### Authentication Request`, `#### Response:`) and `### Subscription Management` (`#### Available Channels`, `#### Subscribe Request`, `#### Response:`, `#### Unsubscribe Request`, `#### Unsubscribe Response`) and `### Keep-Alive` (`#### Ping Request`, `#### Response:`) — because the private API actually has a connect/auth/subscribe handshake to document. The public API doesn't, so those same `###` headings are just `N/A — ...` one-liners there.

`websocket-private.md`'s `### Order Status Values` additionally nests [Order Status Mapping](#order-status-mapping-private-only) tables.

### Per-stream structure

Every stream/event is a `###` heading (a human-readable name, not a literal path), followed by:

```
### Live Order Book Stream

#### Name:
orderbook.\<symbol-id\>
```
(Use `\<placeholder\>` — escaped angle brackets — for variable segments in the stream name.)

```
#### Description:
<prose>
```

A stream that is being deprecated gets a bold warning line directly under the `###` heading, before `#### Name:`:
```
### Trade Stream (DEPRECATED - closes 2026-05-18)

**⚠️ This stream will be PERMANENTLY CLOSED on 2026-05-18. Migrate to Private WebSocket.**

#### Name:
...
```

```
#### Response:                          ← or one-or-more #### Response (label): — see below
```json
{ "...": "..." }
```
```

```
#### Field Descriptions:                ← or one-or-more #### Field Descriptions (label): — see below
| Field | Type | Description |
| ----- | ---- | ----------- |
| stream | string | Stream name |
```

Then a [section separator](#section-separators) before the next `###` stream.

### Multiple Response variants (one stream, several event shapes)

Some streams emit more than one distinct event shape (e.g. `orderbook` emits `bidschanged`, `tradeschanged`, `depthchanged`, and `global.ticker`, each a different JSON shape). Document each as its own `#### Response (label):` heading, and give each one its own `#### Field Descriptions (label):` heading with the **same label**, so every response variant has a matching field-description table:

```
#### Response (bidschanged / askschanged):
```json
{ "data": [[121.82, 112510.1, 0.00108283, 0, false, false]], "event": "bidschanged", "pairing_id": 1 }
```

#### Response (tradeschanged):
```json
{ "data": [...], "event": "tradeschanged", "pairing_id": 1 }
```

...

#### Field Descriptions (bidschanged / askschanged):
| Field | Type | Description |
| ----- | ---- | ----------- |
| data[0][0] | float | Volume |

#### Field Descriptions (tradeschanged):
| Field | Type | Description |
| ----- | ---- | ----------- |
| data[0][0] | int | Timestamp |
```

**Field naming for array-shaped data.** When a response's `data` field is a numeric-indexed array (not a named object), do **not** describe fields with a bare index like `0`, `1`, `2` — different response variants often share the exact same tuple shape (e.g. `[volume, rate, amount, reserved, is_new, is_owner]`), so a bare `0` is ambiguous once read outside the context of its own table, and downstream tooling that flattens field descriptions across variants can produce duplicate-looking entries. Instead, name the field after its real JSON path: `data[0][0]`, `data[0][1]`, etc. When two array positions are semantically identical (e.g. `tradeschanged`'s buy-orders array at `data[1]` and sell-orders array at `data[2]` have the same 6-field shape and meaning), combine them into one row instead of duplicating: `data[1][0] / data[2][0]`.

The number of `#### Response (label):` headings and `#### Field Descriptions (label):` headings for a stream must match 1:1 by label — every response variant should have a corresponding field-description table with the same label.

### Order Status Mapping (private only)

Nested under `websocket-private.md`'s `### Order Status Values`:

```
#### Status Values
| New Status | Description |
|------------|-------------|
| `new` | Order created, pending processing |

#### Mapping: Old Status to New Status
| Old Status | New Statuses |
|------------|--------------|
| `unfilled` | `new`, `open`, `partial_filled` |

#### Mapping: New Status to Old Status
| New Status | Old Status |
|------------|------------|
| `new` | `unfilled` |
```

---

## README.md

`README.md` is the repo landing page and holds the **single, consolidated** `## Announcement` section — cross-cutting notices that apply across multiple files/endpoints (deprecations, migrations, upcoming breaking changes) live here, not duplicated inside each individual REST/WS file's own Change Log. It uses the exact same [bullet format](#change-log--announcement-bullet-format) as each file's `## Change Log`.

Above the Announcement section, README.md has the file-index table (`Name | Description`) linking to each of the 6 doc files — keep this in sync whenever a file is added, renamed, or its description changes.

## Deprecated files (`rest-v1.md`, `rest-v2.md`)

Same REST file format as v3/v4 (see above), but these document APIs that are no longer active — they exist purely as historical reference. Do not add new endpoints here, and do not expect `bitkub-api-document` to fetch or render these two files.

---

## Checklist: adding a new REST endpoint

1. Pick the right file (`rest-v3.md` or `rest-v4.md`) and the right `##`/`###` category to nest it under.
2. Add a `### METHOD /api/vN/...` heading with the full path.
3. Add, in order: `#### Description:`, `#### Required Permission:` (N/A or inline scope), `#### Query Params:`/`#### Body Params:` (N/A if none), `#### Validation Rules:` (N/A or bullets), `#### Example cURL:`, `#### Response:` (one or more, with `(label)` if there's more than one shape), `#### Field Descriptions:` (N/A or a table matching each Response label).
4. Add a [section separator](#section-separators) before the next endpoint.
5. Add a row to the file's `## Endpoint Index` table.
6. Add a row to `## Rate Limits` if the endpoint has a specific limit.
7. Add a `## Change Log` bullet noting the addition, dated.

## Checklist: adding a new WebSocket stream/event

1. Add a `### <Stream Name>` heading under `## Data Streams`.
2. Add `#### Name:` (the literal stream/event identifier) and `#### Description:`.
3. Add one `#### Response (label):` per distinct event shape this stream can emit (omit `(label)` if there's only one shape).
4. Add a matching `#### Field Descriptions (label):` for every Response label — using real field names for object data, and `data[i][j]`-style paths (not bare indices) for array-shaped data.
5. Add a [section separator](#section-separators) before the next stream.
6. Add a `## Change Log` bullet noting the addition, dated.
