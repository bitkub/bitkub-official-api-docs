# 🎨 Beautiful UI Guide

คู่มือการใช้งาน Beautiful UI Features สำหรับ Bitkub Trader

## 📺 Screenshots & Features

### 🌟 Main Menu
```
╔══════════════════════════════════════════════════════════════════════╗
║                                                                      ║
║   ▄▄▄▄   ▄▄▄█████▓ ██ ▄█▀ █    ██  ▄▄▄▄        ▄▄▄█████▓ ▓█████     ║
║  ▓█████▄ ▓  ██▒ ▓▒ ██▄█▒  ██  ▓██▒▓█████▄      ▓  ██▒ ▓▒ ▓█   ▀     ║
║  ▒██▒ ▄██▒ ▓██░ ▒░▓███▄░ ▓██  ▒██░▒██▒ ▄██     ▒ ▓██░ ▒░ ▒███       ║
║  ▒██░█▀  ░ ▓██▓ ░ ▓██ █▄ ▓▓█  ░██░▒██░█▀       ░ ▓██▓ ░  ▒▓█  ▄     ║
║  ░▓█  ▀█▓  ▒██▒ ░ ▒██▒ █▄▒▒█████▓ ░▓█  ▀█▓       ▒██▒ ░  ░▒████▒    ║
║  ░▒▓███▀▒  ▒ ░░   ▒ ▒▒ ▓▒░▒▓▒ ▒ ▒ ░▒▓███▀▒       ▒ ░░    ░░ ▒░ ░    ║
║                                                                      ║
╚══════════════════════════════════════════════════════════════════════╝
```

Interactive Menu ที่สามารถเลือกได้ด้วยลูกศร:
- 📊 View Market Data
- 💰 Check Balance
- 📈 View Order Book
- 🎯 Place Test Order
- 📜 View Order History
- ⚡ Live Trading Dashboard
- 🤖 Start Trading Bot

## ✨ Features Overview

### 1. 📊 Market Data Display

แสดงข้อมูลราคาแบบสวยงาม:

```
╔═══════════════════════════════════════════════════════╗
║              💰 CURRENT PRICE                         ║
║                                                       ║
║              1,234,567.89                             ║
║                  THB                                  ║
╚═══════════════════════════════════════════════════════╝

╔═══════════════════════════════════════════════════════╗
║              📊 24H CHANGE                            ║
║                                                       ║
║               ▲ +2.45%                                ║
╚═══════════════════════════════════════════════════════╝
```

พร้อม:
- ✅ Color-coded price changes (เขียว = ขึ้น, แดง = ลง)
- ✅ 24h High/Low bars
- ✅ Volume display
- ✅ Bid/Ask spread

### 2. 📈 Live Price Chart

กราฟราคาแบบ real-time:

```
Price History (Last 50 updates)

████████████  1250
██████████    1240
████████      1230
██████        1220
████          1210
```

- อัพเดททุก 2 วินาที
- เก็บประวัติ 50 ช่วงล่าสุด
- สีเปลี่ยนตามทิศทางราคา

### 3. 📚 Order Book Display

แสดง Order Book แบบ split-screen:

```
╔════════════════════╦═════════════════════╗
║   📈 BUY ORDERS    ║   📉 SELL ORDERS    ║
╠════════════════════╬═════════════════════╣
║ Price    Amount    ║ Price    Amount     ║
║ 1,234,500  0.001   ║ 1,234,600  0.002    ║
║ 1,234,400  0.003   ║ 1,234,700  0.001    ║
║ 1,234,300  0.002   ║ 1,234,800  0.005    ║
╚════════════════════╩═════════════════════╝
```

พร้อม Spread Analysis

### 4. 💰 Portfolio Balance

แสดงยอดเงินพร้อมกราฟ:

```
╔═══════════════════════════════════════════╗
║        💰 PORTFOLIO BALANCE               ║
╠═══════════════════════════════════════════╣
║ Currency   Available    Reserved    Total ║
║ ₿ BTC      0.12345678   0.00000000  ...   ║
║ Ξ ETH      1.23456789   0.00000000  ...   ║
║ ฿ THB      50,000.00    0.00        ...   ║
╠═══════════════════════════════════════════╣
║ TOTAL VALUE:               150,000 THB    ║
╚═══════════════════════════════════════════╝
```

พร้อม Pie Chart แสดงสัดส่วนพอร์ต

### 5. ⚡ Live Trading Dashboard

หน้าจอ real-time แบบเต็มจอ:

```
╔════════════════════════════════════════════════════════════╗
║   ⚡ BITKUB LIVE TRADING DASHBOARD ⚡                      ║
║   THB_BTC │ Uptime: 00:15:32 │ 2025-12-25 10:30:45       ║
╚════════════════════════════════════════════════════════════╝

┌────────────────────────┬────────────────────────┐
│   🚀 PRICE INFO        │   📊 ORDER BOOK        │
│                        │                        │
│   1,234,567.89 THB     │   BID         ASK      │
│   ▲ +2.45% (24h)       │   1,234,500   1,234,600│
│                        │   1,234,400   1,234,700│
│   High: 1,245,000      │                        │
│   Low:  1,220,000      │                        │
│                        │                        │
├────────────────────────┼────────────────────────┤
│   💰 BALANCE           │   📋 OPEN ORDERS (3)   │
│                        │                        │
│   ₿ BTC    0.1234      │   📈 BUY  1,234,000    │
│   ฿ THB    50,000      │   📉 SELL 1,235,000    │
│   TOTAL    200,000 ฿   │   📈 BUY  1,233,000    │
└────────────────────────┴────────────────────────┘

┌────────────────────────────────────────────────┐
│   Press Ctrl+C to exit │ Updates every 2 seconds│
└────────────────────────────────────────────────┘
```

อัพเดทข้อมูลทุก 2 วินาทีแบบ real-time!

### 6. 🎯 Order Placement with Animations

Loading animation ขณะส่งคำสั่ง:

```
⠋ Placing test BUY order...
```

แล้วแสดงผลลัพธ์แบบสวยงาม:

```
╔════════════════════════════════════════╗
║     📈 BUY ORDER PLACED               ║
╠════════════════════════════════════════╣
║ Order ID:      12345                  ║
║ Hash:          fwQ6dnQWQPs...         ║
║ Type:          LIMIT                  ║
║ Amount:        1000.00                ║
║ Rate:          1,234,567 THB          ║
║ Fee:           2.50 THB               ║
║ Will Receive:  0.00081 BTC            ║
╚════════════════════════════════════════╝

✓ Test order successful! (No real transaction)
```

## 🎨 Color Coding

โปรแกรมใช้สีเพื่อความเข้าใจง่าย:

- 🟢 **Green** - ราคาขึ้น, คำสั่งซื้อ, กำไร
- 🔴 **Red** - ราคาลง, คำสั่งขาย, ขาดทุน
- 🟡 **Yellow** - ราคาปัจจุบัน, ข้อมูลสำคัญ
- 🔵 **Cyan** - ข้อมูลทั่วไป, headers
- 🟣 **Magenta** - Volume, คำสั่งที่เปิดอยู่
- ⚪ **White/Dim** - ข้อมูลรอง, timestamps

## ⚡ Animations

### Loading Spinners
```
⠋ Loading...
⠙ Loading...
⠹ Loading...
⠸ Loading...
⠼ Loading...
⠴ Loading...
⠦ Loading...
⠧ Loading...
⠇ Loading...
⠏ Loading...
```

### Progress Bars
```
Downloading data   ████████████░░░░░░░░  60%  ⠸
```

### Live Updates
- Dashboard อัพเดททุก 2 วินาที
- Price chart เพิ่มข้อมูลแบบ real-time
- Order book refresh อัตโนมัติ

## 🎮 Interactive Features

### Menu Navigation
- ใช้ `↑` `↓` เลื่อนเลือก
- กด `Enter` เพื่อยืนยัน
- สีเปลี่ยนเมื่อ highlight

### Input Prompts
```
Enter symbol: │ THB_BTC
              └─ Default value แสดงสีเทา

Your choice: █
            └─ Cursor กระพริบ
```

### Confirmations
```
? Test BUY 1000 THB worth at market price? (y/N): _
```

## 🚀 การใช้งาน Beautiful UI

### วิธีที่ 1: รันโปรแกรมโดยตรง

```bash
cd samples/csharp/BitkubTrader
dotnet run
# จากนั้นเลือก ProgramBeautiful.cs เป็น entry point
```

### วิธีที่ 2: แก้ไข Project File

แก้ไข `BitkubTrader.csproj`:

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
  <TargetFramework>net6.0</TargetFramework>
  <StartupObject>BitkubTrader.ProgramBeautiful</StartupObject>
</PropertyGroup>
```

แล้วรัน:
```bash
dotnet run
```

### วิธีที่ 3: ใช้ผ่าน Code

```csharp
// Show beautiful logo
ConsoleUI.ShowLogo();

// Show market data with animations
var ticker = await ConsoleUI.WithSpinnerAsync(
    "📡 Fetching market data...",
    async () => await client.GetTickerAsync("THB_BTC")
);

// Show menu
var choice = ConsoleUI.ShowMenu(
    "Select option",
    new[] { "Option 1", "Option 2", "Option 3" }
);

// Show live dashboard
await ConsoleUI.ShowLiveDashboard(client, "THB_BTC", cancellationToken);
```

## 📱 Live Dashboard Features

### หน้าจอแบ่งเป็น 4 ส่วน:

1. **Top Header** - Symbol, Uptime, Current Time
2. **Left Panel** - Price Info + Chart + Order Book
3. **Right Panel** - Balance + Open Orders
4. **Bottom Footer** - Controls and Warnings

### การอัพเดท Real-time:
- ⚡ ทุก 2 วินาที
- 📊 ไม่กระพริบหน้าจอ (smooth update)
- 🔄 Fetch ข้อมูลแบบ parallel
- 💾 เก็บประวัติราคา 50 ช่วงล่าสุด

## 🎯 Tips & Tricks

### 1. Terminal Settings
สำหรับ UI ที่สวยที่สุด:
- ใช้ terminal ที่รองรับ Unicode (Windows Terminal, iTerm2, etc.)
- ตั้งขนาดหน้าต่างอย่างน้อย 120 x 40 characters
- ใช้ font ที่รองรับ emoji (Cascadia Code, Fira Code, etc.)

### 2. Color Themes
โปรแกรมจะปรับสีให้เข้ากับ terminal theme ของคุณอัตโนมัติ

### 3. Performance
- Live dashboard ใช้ CPU น้อย (async/await)
- อัพเดทเฉพาะส่วนที่เปลี่ยน
- Smooth animations

## ⚙️ Customization

### เปลี่ยนสี

แก้ไขใน `ConsoleUI.cs`:

```csharp
// เปลี่ยนสีหลัก
var primaryColor = Color.Cyan1;  // เปลี่ยนเป็นสีที่ต้องการ

// สีสำหรับ up/down
var upColor = Color.Green;
var downColor = Color.Red;
```

### เปลี่ยน Update Interval

แก้ไขใน `LiveDashboard.cs`:

```csharp
// เปลี่ยนจาก 2 วินาที เป็น 5 วินาที
await Task.Delay(5000, cancellationToken);
```

### เปลี่ยน Chart Size

```csharp
// เพิ่มจำนวนข้อมูลในกราฟ
if (_priceHistory.Count > 100)  // เปลี่ยนจาก 50 เป็น 100
    _priceHistory.RemoveAt(0);
```

## 🐛 Troubleshooting

### ปัญหา: สีไม่แสดง
**วิธีแก้:** ใช้ terminal ที่รองรับ ANSI colors (Windows Terminal, iTerm2)

### ปัญหา: Unicode/Emoji แสดงผิด
**วิธีแก้:** ตั้ง encoding เป็น UTF-8 และใช้ font ที่รองรับ

### ปัญหา: Layout พัง
**วิธีแก้:** ขยายหน้าต่าง terminal ให้กว้างขึ้น (แนะนำ 120+ columns)

### ปัญหา: Dashboard กระพริบ
**วิธีแก้:** ใช้ Spectre.Console version ล่าสุด และ .NET 6.0+

## 📚 Libraries Used

- **Spectre.Console** - Beautiful console UI framework
  - Tables, Charts, Progress bars
  - Live displays, Panels, Grids
  - Color support, Markup formatting
  - Interactive prompts, Menu selections

## 🌟 Examples

ดูตัวอย่างเพิ่มเติมใน:
- `ProgramBeautiful.cs` - Main program with beautiful UI
- `LiveDashboard.cs` - Advanced live dashboard
- `ConsoleUI.cs` - UI helper methods
- `AdvancedExample.cs` - Advanced strategies with UI

---

**Happy Trading with Beautiful UI! 🚀💰**
