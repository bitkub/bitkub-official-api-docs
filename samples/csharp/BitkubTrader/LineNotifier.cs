using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Spectre.Console;

namespace BitkubTrader
{
    /// <summary>
    /// ⚠️ DEPRECATED - LINE Notify บริการนี้ถูกยกเลิกแล้ว!
    ///
    /// กรุณาใช้ LineMessenger.cs แทน (LINE Official Account Messaging API)
    ///
    /// ไฟล์นี้เก็บไว้เพื่อความเข้ากันได้แบบย้อนหลังเท่านั้น
    /// LINE Notify service has been discontinued. Use LineMessenger.cs instead.
    ///
    /// ---
    ///
    /// 📱 LINE Notify Integration - ส่งการแจ้งเตือนไป LINE (เลิกใช้แล้ว)
    ///
    /// วิธีตั้งค่า:
    /// 1. ไปที่ https://notify-bot.line.me/
    /// 2. Login ด้วย LINE
    /// 3. Generate Token
    /// 4. ใส่ Token ใน config
    /// </summary>
    [Obsolete("LINE Notify service has been discontinued. Use LineMessenger class instead.")]
    public class LineNotifier
    {
        private readonly string _accessToken;
        private readonly HttpClient _httpClient;
        private readonly bool _enabled;
        private const string LINE_NOTIFY_API = "https://notify-api.line.me/api/notify";

        public LineNotifier(string accessToken, bool enabled = true)
        {
            _accessToken = accessToken;
            _enabled = enabled && !string.IsNullOrEmpty(accessToken) &&
                       accessToken != "YOUR_LINE_ACCESS_TOKEN";
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// ส่งข้อความ LINE
        /// </summary>
        public async Task<bool> SendMessageAsync(string message)
        {
            if (!_enabled)
            {
                ConsoleUI.ShowWarning("LINE Notify ยังไม่ได้เปิดใช้งาน");
                return false;
            }

            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("message", message)
                });

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

                var response = await _httpClient.PostAsync(LINE_NOTIFY_API, content);

                if (response.IsSuccessStatusCode)
                {
                    ConsoleUI.ShowSuccess($"✅ ส่ง LINE แล้ว: {message.Substring(0, Math.Min(50, message.Length))}...");
                    return true;
                }
                else
                {
                    ConsoleUI.ShowError($"❌ ส่ง LINE ไม่สำเร็จ: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ConsoleUI.ShowError($"❌ Error sending LINE: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// แจ้งเตือนสัญญาณซื้อขาย
        /// </summary>
        public async Task NotifySignalAsync(string symbol, string action, int masterScore, string reasoning)
        {
            var emoji = action switch
            {
                "STRONG_BUY" => "🔥🔥🔥",
                "BUY" => "✅",
                "SELL" => "⚠️",
                "STRONG_SELL" => "🚨🚨🚨",
                _ => "ℹ️"
            };

            var message = $@"
{emoji} สัญญาณการเทรด {emoji}

Symbol: {symbol}
Action: {action}
Master Score: {masterScore}/100

เหตุผล:
{reasoning}

เวลา: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือนเข้า Position
        /// </summary>
        public async Task NotifyEntryAsync(string symbol, decimal price, decimal amount, decimal stopLoss, decimal takeProfit)
        {
            var message = $@"
🎯 เข้า Position แล้ว!

Symbol: {symbol}
ราคาเข้า: {price:N2} THB
จำนวน: {amount:N8}
มูลค่า: {(price * amount):N2} THB

🛑 Stop Loss: {stopLoss:N2} (-{((price - stopLoss) / price * 100):N2}%)
🎯 Take Profit: {takeProfit:N2} (+{((takeProfit - price) / price * 100):N2}%)

เวลา: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือนออก Position
        /// </summary>
        public async Task NotifyExitAsync(string symbol, decimal entryPrice, decimal exitPrice,
            decimal amount, string reason)
        {
            var profitLoss = exitPrice - entryPrice;
            var profitLossPercent = (profitLoss / entryPrice) * 100;
            var emoji = profitLoss >= 0 ? "💰" : "📉";
            var totalPL = profitLoss * amount;

            var message = $@"
{emoji} ออก Position แล้ว!

Symbol: {symbol}
ราคาเข้า: {entryPrice:N2} THB
ราคาออก: {exitPrice:N2} THB
จำนวน: {amount:N8}

กำไร/ขาดทุน: {totalPL:N2} THB ({profitLossPercent:+0.00;-0.00}%)
เหตุผล: {reason}

เวลา: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือน Stop Loss
        /// </summary>
        public async Task NotifyStopLossAsync(string symbol, decimal entryPrice, decimal exitPrice, decimal loss)
        {
            var message = $@"
🛑 STOP LOSS ถูกกระตุ้น!

Symbol: {symbol}
ราคาเข้า: {entryPrice:N2} THB
ราคาออก: {exitPrice:N2} THB

ขาดทุน: {loss:N2} THB ({((exitPrice - entryPrice) / entryPrice * 100):N2}%)

เวลา: {DateTime.Now:dd/MM/yyyy HH:mm:ss}

⚠️ ทบทวนกลยุทธ์และวิเคราะห์ใหม่
";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือน Take Profit
        /// </summary>
        public async Task NotifyTakeProfitAsync(string symbol, decimal entryPrice, decimal exitPrice, decimal profit)
        {
            var message = $@"
🎯 TAKE PROFIT ถึงเป้าแล้ว!

Symbol: {symbol}
ราคาเข้า: {entryPrice:N2} THB
ราคาออก: {exitPrice:N2} THB

กำไร: {profit:N2} THB ({((exitPrice - entryPrice) / entryPrice * 100):N2}%)

เวลา: {DateTime.Now:dd/MM/yyyy HH:mm:ss}

🎉 ยินดีด้วย! เทรดสำเร็จ
";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือนราคาผ่านจุดสำคัญ
        /// </summary>
        public async Task NotifyPriceAlertAsync(string symbol, decimal price, string level, string type)
        {
            var emoji = type == "SUPPORT" ? "⬆️" : "⬇️";

            var message = $@"
{emoji} ราคาผ่านจุดสำคัญ!

Symbol: {symbol}
ราคาปัจจุบัน: {price:N2} THB

ผ่าน: {level} ({type})

เวลา: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือน Elliott Wave
        /// </summary>
        public async Task NotifyElliottWaveAsync(string symbol, string pattern, string currentWave,
            int confidence, string nextExpected)
        {
            var message = $@"
🌊 Elliott Wave Update

Symbol: {symbol}
Pattern: {pattern}
Current Wave: {currentWave}
Confidence: {confidence}%
Next Expected: {nextExpected}

เวลา: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือน Pattern ที่พบ
        /// </summary>
        public async Task NotifyPatternAsync(string symbol, string pattern, int confidence)
        {
            var emoji = pattern.Contains("BOTTOM") || pattern.Contains("CUP") ||
                       pattern.Contains("ASCENDING") ? "📈" : "📉";

            var message = $@"
{emoji} Chart Pattern ตรวจพบ!

Symbol: {symbol}
Pattern: {pattern}
Confidence: {confidence}%

เวลา: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// สรุปประจำวัน
        /// </summary>
        public async Task SendDailySummaryAsync(DailyStats stats)
        {
            var message = $@"
📊 สรุปการเทรดประจำวัน

วันที่: {DateTime.Now:dd/MM/yyyy}

จำนวน Trades: {stats.TotalTrades}
✅ Win: {stats.WinTrades} ({stats.WinRate:N1}%)
❌ Loss: {stats.LossTrades}

💰 กำไร/ขาดทุน: {stats.NetProfit:N2} THB
📈 Largest Win: {stats.LargestWin:N2} THB
📉 Largest Loss: {stats.LargestLoss:N2} THB

🎯 Win Rate: {stats.WinRate:N1}%
📊 Profit Factor: {stats.ProfitFactor:N2}

---
Ultimate Trading System
";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือน Error
        /// </summary>
        public async Task NotifyErrorAsync(string error, string details = "")
        {
            var message = $@"
❌ เกิดข้อผิดพลาด!

Error: {error}

{(string.IsNullOrEmpty(details) ? "" : $"Details: {details}\n")}
เวลา: {DateTime.Now:dd/MM/yyyy HH:mm:ss}

⚠️ กรุณาตรวจสอบระบบ
";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือนเหตุการณ์ Custom
        /// </summary>
        public async Task NotifyCustomEventAsync(string eventName, string message)
        {
            var fullMessage = $@"
🔔 {eventName}

{message}

เวลา: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
";

            await SendMessageAsync(fullMessage);
        }

        /// <summary>
        /// ทดสอบการส่ง LINE
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            if (!_enabled)
            {
                AnsiConsole.MarkupLine("[yellow]⚠️ LINE Notify ยังไม่ได้เปิดใช้งาน[/]");
                return false;
            }

            var message = @"
✅ ทดสอบการเชื่อมต่อ LINE Notify

Ultimate Trading System
พร้อมใช้งาน!

เวลา: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            var result = await SendMessageAsync(message);

            if (result)
            {
                AnsiConsole.MarkupLine("[green]✅ เชื่อมต่อ LINE สำเร็จ![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]❌ เชื่อมต่อ LINE ไม่สำเร็จ[/]");
            }

            return result;
        }
    }

    /// <summary>
    /// สถิติประจำวัน
    /// </summary>
    public class DailyStats
    {
        public int TotalTrades { get; set; }
        public int WinTrades { get; set; }
        public int LossTrades { get; set; }
        public decimal WinRate => TotalTrades > 0 ? (decimal)WinTrades / TotalTrades * 100 : 0;

        public decimal NetProfit { get; set; }
        public decimal LargestWin { get; set; }
        public decimal LargestLoss { get; set; }

        public decimal TotalWinAmount { get; set; }
        public decimal TotalLossAmount { get; set; }
        public decimal ProfitFactor => TotalLossAmount != 0 ?
            Math.Abs(TotalWinAmount / TotalLossAmount) : 0;
    }
}
