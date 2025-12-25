# Bitkub Trading Bot - C# Example

โปรแกรมเทรด Cryptocurrency บน Bitkub Exchange ด้วย C# (.NET 6.0+)

🎨 **พิเศษ! เวอร์ชั่น Beautiful UI พร้อม Animations และ Live Dashboard**

## คุณสมบัติ

### 🚀 Core Features
- ✅ ดึงข้อมูลตลาด (Market Data) - Ticker, Order Book, Symbols
- ✅ จัดการบัญชี - ตรวจสอบยอดเงิน (Balances)
- ✅ การเทรด - สั่งซื้อ (Buy), สั่งขาย (Sell), ยกเลิกคำสั่ง (Cancel)
- ✅ ประวัติการเทรด - คำสั่งที่เปิดอยู่, ประวัติคำสั่งที่จับคู่แล้ว
- ✅ Test Mode - ทดสอบการสั่งซื้อขายโดยไม่หักยอดเงินจริง
- ✅ HMAC SHA-256 Authentication
- ✅ Async/Await pattern
- ✅ Type-safe models

### 🎨 Beautiful UI Features
- ✨ **ASCII Art Logo** - โลโก้สวยงามตอนเปิดโปรแกรม
- 🌈 **Color-coded Display** - สีสันสดใส แยกแยะข้อมูลง่าย
- 📊 **Interactive Tables** - ตารางข้อมูลที่สวยงาม
- 📈 **Live Charts** - กราฟราคาแบบ real-time
- ⚡ **Animations** - Loading spinners และ progress bars
- 📺 **Live Dashboard** - หน้าจอแสดงข้อมูลแบบ real-time update
- 🎯 **Interactive Menus** - เมนูเลือกแบบ interactive
- 💫 **Visual Effects** - เอฟเฟคสวยงามทั่วโปรแกรม
- 📉 **Order Book Display** - แสดง Order Book แบบสวยงาม
- 💰 **Portfolio Charts** - กราฟแสดงสัดส่วนพอร์ต

### 🤖 AI Trading Features (EPIC!)
- 🧠 **AI Trading Engine** - AI วิเคราะห์และตัดสินใจเทรดอัตโนมัติ
- 📊 **Technical Analysis** - RSI, MACD, Bollinger Bands, SMA/EMA, Stochastic, ATR
- 🎯 **Smart Signals** - STRONG_BUY, BUY, HOLD, SELL, STRONG_SELL พร้อม Confidence Score
- 📰 **News Sentiment Analysis** - วิเคราะห์ข่าวจากเว็บไทย (Thairath, Sanook, Manager)
- 💾 **SQLite Database** - เก็บสถิติการเทรด, ราคา, signals, performance ทั้งหมด
- 🎚️ **Multiple Trading Modes**:
  - ⚡ **Aggressive** - เทรดบ่อย ความเสี่ยงสูง ผลตอบแทนสูง
  - ⚖️ **Balanced** - สมดุลระหว่างความเสี่ยงและผลตอบแทน
  - 🛡️ **Conservative** - เทรดระมัดระวัง ความเสี่ยงต่ำ
- 💰 **Risk Management System**:
  - 🎯 Auto Take Profit - กำไรถึงเป้าขายอัตโนมัติ
  - 🛑 Auto Stop Loss - ขาดทุนถึงจุดหยุดขายอัตโนมัติ
  - 📊 Position Sizing - คำนวณขนาดคำสั่งตามความเสี่ยง
- 📈 **Performance Analytics**:
  - 📊 Win Rate, Profit Factor, Sharpe Ratio
  - 💰 Total P/L, Average Win/Loss
  - 📉 Max Drawdown, Largest Win/Loss
- 🔍 **Pattern Detection** - Double Top/Bottom, Head & Shoulders, Trends
- 🎯 **Multi-Indicator Scoring** - รวม indicators หลายตัวเพื่อความแม่นยำสูง

## ความต้องการของระบบ

- .NET 6.0 SDK หรือใหม่กว่า
- Bitkub API Key และ API Secret ([สมัครที่นี่](https://www.bitkub.com/))

## การติดตั้ง

### 1. Clone repository

```bash
git clone https://github.com/bitkub/bitkub-official-api-docs.git
cd bitkub-official-api-docs/samples/csharp/BitkubTrader
```

### 2. Restore packages

```bash
dotnet restore
```

### 3. ตั้งค่า API Credentials

แก้ไขไฟล์ `Program.cs` และใส่ API Key และ Secret ของคุณ:

```csharp
private const string API_KEY = "YOUR_API_KEY";
private const string API_SECRET = "YOUR_API_SECRET";
```

**⚠️ คำเตือน:** ห้ามเปิดเผย API Key และ Secret ในที่สาธารณะ!

## การใช้งาน

### รันโปรแกรม AI Trading (🚀 แนะนำ!)

```bash
# รัน AI Trading System - เทพที่สุด!
dotnet run
# แล้วแก้ไข Program.cs ให้ชี้ไปที่ ProgramAI.Main()
```

หรือแก้ไข `BitkubTrader.csproj` เปลี่ยน entry point:
```xml
<PropertyGroup>
  <StartupObject>BitkubTrader.ProgramAI</StartupObject>
</PropertyGroup>
```

**AI Trading System มี:**
- 🤖 Start AI Trading Bot (เลือกโหมดได้)
- 📊 View Performance Analytics
- 📰 Analyze Market News (ข่าวไทย)
- 🔍 Technical Analysis
- 💾 Database Statistics

### รันโปรแกรมแบบ Beautiful UI 🎨

```bash
# แก้ไข BitkubTrader.csproj ให้ StartupObject เป็น ProgramBeautiful
dotnet run
```

### รันโปรแกรมตัวอย่างแบบธรรมดา

```bash
# แก้ไข BitkubTrader.csproj ให้ StartupObject เป็น Program
dotnet run
```

โปรแกรมจะแสดง:
1. เวลาของเซิร์ฟเวอร์
2. สัญลักษณ์การเทรดที่มี
3. ราคาปัจจุบันของ BTC
4. Order Book (Bids/Asks)
5. ยอดเงินในบัญชี
6. คำสั่งที่เปิดอยู่
7. ประวัติการเทรด
8. ทดสอบคำสั่งซื้อ (Test Mode)
9. ทดสอบคำสั่งขาย (Test Mode)

### ตัวอย่างการใช้งานในโค้ด

#### 1. สร้าง Client

```csharp
using var client = new BitkubClient("YOUR_API_KEY", "YOUR_API_SECRET");
```

#### 2. ดึงข้อมูลราคา

```csharp
// Get ticker for specific symbol
var ticker = await client.GetTickerAsync("THB_BTC");
var btcPrice = ticker["THB_BTC"].Last;
Console.WriteLine($"BTC Price: {btcPrice:N2} THB");

// Get all tickers
var allTickers = await client.GetTickerAsync();
```

#### 3. ดู Order Book

```csharp
var depth = await client.GetDepthAsync("THB_BTC", limit: 10);
Console.WriteLine("Top 10 Bids:");
foreach (var bid in depth.Bids)
{
    Console.WriteLine($"Price: {bid[0]}, Amount: {bid[1]}");
}
```

#### 4. ตรวจสอบยอดเงิน

```csharp
var balances = await client.GetBalancesAsync();
foreach (var balance in balances.Result)
{
    Console.WriteLine($"{balance.Key}: {balance.Value.Available}");
}
```

#### 5. สั่งซื้อ (Buy Order)

```csharp
// Limit Order - ซื้อ BTC ที่ราคา 1,000,000 THB/BTC ด้วยเงิน 1,000 THB
var buyOrder = await client.PlaceBidAsync(
    symbol: "THB_BTC",
    amount: 1000,      // จำนวนเงิน THB ที่ต้องการใช้
    rate: 1000000,     // ราคาที่ต้องการซื้อ
    type: "limit"
);

if (buyOrder.Error == 0)
{
    Console.WriteLine($"Order ID: {buyOrder.Result.Id}");
    Console.WriteLine($"Will receive: {buyOrder.Result.Receive} BTC");
}

// Market Order - ซื้อทันทีที่ราคาตลาด
var marketBuy = await client.PlaceBidAsync(
    symbol: "THB_BTC",
    amount: 1000,
    rate: 0,           // 0 = ราคาตลาด
    type: "market"
);
```

#### 6. สั่งขาย (Sell Order)

```csharp
// Limit Order - ขาย 0.001 BTC ที่ราคา 1,200,000 THB/BTC
var sellOrder = await client.PlaceAskAsync(
    symbol: "THB_BTC",
    amount: 0.001m,    // จำนวน BTC ที่ต้องการขาย
    rate: 1200000,     // ราคาที่ต้องการขาย
    type: "limit"
);

if (sellOrder.Error == 0)
{
    Console.WriteLine($"Order ID: {sellOrder.Result.Id}");
    Console.WriteLine($"Will receive: {sellOrder.Result.Receive} THB");
}
```

#### 7. ทดสอบคำสั่งก่อนส่งจริง (Test Mode)

```csharp
// ทดสอบโดยไม่หักยอดเงินจริง
var testOrder = await client.PlaceBidTestAsync(
    symbol: "THB_BTC",
    amount: 1000,
    rate: 1000000,
    type: "limit"
);
// ตรวจสอบผลลัพธ์ก่อนสั่งจริง
```

#### 8. ยกเลิกคำสั่ง

```csharp
// ยกเลิกด้วย Order ID
await client.CancelOrderAsync(
    symbol: "THB_BTC",
    orderId: 12345,
    side: "buy"
);

// หรือยกเลิกด้วย Order Hash
await client.CancelOrderByHashAsync("order_hash_here");
```

#### 9. ดูคำสั่งที่เปิดอยู่

```csharp
var openOrders = await client.GetOpenOrdersAsync("THB_BTC");
foreach (var order in openOrders.Result)
{
    Console.WriteLine($"Order #{order.Id}: {order.Side} {order.Amount} @ {order.Rate}");
}
```

#### 10. ดูประวัติการเทรด

```csharp
var history = await client.GetOrderHistoryAsync(
    symbol: "THB_BTC",
    page: 1,
    limit: 20
);

foreach (var order in history.Result)
{
    Console.WriteLine($"{order.TxnId}: {order.Side} {order.Amount} @ {order.Rate}");
}
```

## โครงสร้างโปรเจค

```
BitkubTrader/
├── BitkubTrader.csproj    # Project file
├── BitkubClient.cs        # Main API client
├── Models.cs              # Data models
└── Program.cs             # Example usage
```

## API Methods

### Market Data (Public - ไม่ต้อง API Key)

| Method | Description |
|--------|-------------|
| `GetServerTimestampAsync()` | ดึงเวลาเซิร์ฟเวอร์ |
| `GetSymbolsAsync()` | ดึงรายการคู่เทรดทั้งหมด |
| `GetTickerAsync(symbol?)` | ดึงข้อมูลราคา |
| `GetDepthAsync(symbol, limit)` | ดึง Order Book |

### Account (Secure - ต้องใช้ API Key)

| Method | Description |
|--------|-------------|
| `GetWalletAsync()` | ดึงยอดเงินที่ใช้ได้ |
| `GetBalancesAsync()` | ดึงยอดเงินทั้งหมด (available + reserved) |

### Trading (Secure - ต้องใช้ API Key)

| Method | Description |
|--------|-------------|
| `PlaceBidAsync()` | สั่งซื้อ |
| `PlaceAskAsync()` | สั่งขาย |
| `PlaceBidTestAsync()` | ทดสอบคำสั่งซื้อ |
| `PlaceAskTestAsync()` | ทดสอบคำสั่งขาย |
| `CancelOrderAsync()` | ยกเลิกคำสั่ง |
| `CancelOrderByHashAsync()` | ยกเลิกคำสั่งด้วย hash |
| `GetOpenOrdersAsync()` | ดูคำสั่งที่เปิดอยู่ |
| `GetOrderHistoryAsync()` | ดูประวัติคำสั่ง |

## Error Codes

| Code | Description |
|------|-------------|
| 0 | สำเร็จ |
| 1 | JSON payload ไม่ถูกต้อง |
| 2 | ไม่มี API key |
| 3 | API key ไม่ถูกต้อง |
| 6 | Signature ไม่ถูกต้อง |
| 10 | Parameter ไม่ถูกต้อง |
| 11 | Symbol ไม่ถูกต้อง |
| 18 | ยอดเงินไม่เพียงพอ |

ดู Error codes ทั้งหมดได้ที่ [RESTful API Documentation](../../restful-api.md#error-codes)

## คำเตือนด้านความปลอดภัย

1. **ห้ามแชร์ API Key และ Secret** - อย่าเปิดเผยใน Git, Discord, หรือที่ใดๆ
2. **ใช้ Test Mode ก่อน** - ทดสอบด้วย `PlaceBidTestAsync()` และ `PlaceAskTestAsync()` ก่อนสั่งจริง
3. **จำกัดสิทธิ์ API Key** - ตั้งค่าให้ใช้ได้เฉพาะ IP ที่ต้องการ
4. **เก็บ API Key ใน Environment Variables** - อย่า hardcode ในโค้ด

### ตัวอย่างการใช้ Environment Variables

```csharp
var apiKey = Environment.GetEnvironmentVariable("BITKUB_API_KEY");
var apiSecret = Environment.GetEnvironmentVariable("BITKUB_API_SECRET");
var client = new BitkubClient(apiKey, apiSecret);
```

```bash
# Linux/Mac
export BITKUB_API_KEY="your_key"
export BITKUB_API_SECRET="your_secret"

# Windows
set BITKUB_API_KEY=your_key
set BITKUB_API_SECRET=your_secret
```

## ตัวอย่าง Trading Strategy

```csharp
public async Task SimpleScalpingStrategy()
{
    using var client = new BitkubClient(API_KEY, API_SECRET);

    while (true)
    {
        // Get current price
        var ticker = await client.GetTickerAsync("THB_BTC");
        var currentPrice = ticker["THB_BTC"].Last;

        // Strategy: Buy 1% below, sell 1% above
        decimal buyPrice = currentPrice * 0.99m;
        decimal sellPrice = currentPrice * 1.01m;

        // Place orders
        await client.PlaceBidAsync("THB_BTC", 1000, buyPrice, "limit");

        // Wait and check if order filled
        await Task.Delay(TimeSpan.FromSeconds(10));

        var openOrders = await client.GetOpenOrdersAsync("THB_BTC");
        // Check and manage orders...
    }
}
```

## การสนับสนุน

- 📖 [Bitkub API Documentation](https://github.com/bitkub/bitkub-official-api-docs)
- 🌐 [Bitkub Website](https://www.bitkub.com/)
- 💬 [Bitkub Support](https://support.bitkub.com/)

## License

MIT License - ใช้งานได้อย่างอิสระ แต่ความเสี่ยงเป็นของผู้ใช้เอง

## คำเตือน

การเทรด Cryptocurrency มีความเสี่ยงสูง อาจสูญเสียเงินลงทุนได้ทั้งหมด กรุณาศึกษาและทำความเข้าใจก่อนลงทุน

**ผู้พัฒนาไม่รับผิดชอบต่อความสูญเสียใดๆ ที่เกิดจากการใช้งานโปรแกรมนี้**
