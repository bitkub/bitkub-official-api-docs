using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Spectre.Console;

namespace BitkubTrader
{
    /// <summary>
    /// 📱 LINE Official Account (LINE OA) Integration
    /// ส่งการแจ้งเตือนผ่าน LINE Messaging API
    ///
    /// วิธีตั้งค่า:
    /// 1. สร้าง LINE Official Account ที่ https://manager.line.biz/
    /// 2. ไปที่ LINE Developers Console: https://developers.line.biz/console/
    /// 3. สร้าง Messaging API channel
    /// 4. คัดลอก Channel Access Token
    /// 5. เพิ่มเพื่อนบัญชี LINE OA ของคุณ
    /// 6. ดึง User ID จาก Webhook หรือใช้เครื่องมือ
    /// 7. ใส่ Channel Access Token และ User ID ใน Config
    /// </summary>
    public class LineMessenger
    {
        private readonly string _channelAccessToken;
        private readonly List<string> _userIds;
        private readonly HttpClient _httpClient;
        private readonly bool _enabled;
        private const string LINE_MESSAGING_API = "https://api.line.me/v2/bot/message/push";

        public LineMessenger(string channelAccessToken, List<string> userIds, bool enabled = true)
        {
            _channelAccessToken = channelAccessToken;
            _userIds = userIds ?? new List<string>();
            _enabled = enabled &&
                      !string.IsNullOrEmpty(channelAccessToken) &&
                      channelAccessToken != "YOUR_CHANNEL_ACCESS_TOKEN" &&
                      _userIds.Count > 0;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// ส่งข้อความ LINE
        /// </summary>
        public async Task<bool> SendMessageAsync(string message)
        {
            if (!_enabled)
            {
                ConsoleUI.ShowWarning("📱 LINE OA ยังไม่ได้เปิดใช้งาน");
                return false;
            }

            bool allSuccess = true;

            foreach (var userId in _userIds)
            {
                try
                {
                    var payload = new
                    {
                        to = userId,
                        messages = new[]
                        {
                            new
                            {
                                type = "text",
                                text = message
                            }
                        }
                    };

                    var json = JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_channelAccessToken}");

                    var response = await _httpClient.PostAsync(LINE_MESSAGING_API, content);

                    if (response.IsSuccessStatusCode)
                    {
                        ConsoleUI.ShowSuccess($"✅ ส่ง LINE ถึง {MaskUserId(userId)} แล้ว");
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        ConsoleUI.ShowError($"❌ ส่ง LINE ไม่สำเร็จ: {response.StatusCode} - {error}");
                        allSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleUI.ShowError($"❌ Error sending LINE to {MaskUserId(userId)}: {ex.Message}");
                    allSuccess = false;
                }
            }

            return allSuccess;
        }

        /// <summary>
        /// ส่งข้อความพร้อม Flex Message (สวยงาม)
        /// </summary>
        public async Task<bool> SendFlexMessageAsync(string altText, object flexMessage)
        {
            if (!_enabled)
            {
                ConsoleUI.ShowWarning("📱 LINE OA ยังไม่ได้เปิดใช้งาน");
                return false;
            }

            bool allSuccess = true;

            foreach (var userId in _userIds)
            {
                try
                {
                    var payload = new
                    {
                        to = userId,
                        messages = new[]
                        {
                            new
                            {
                                type = "flex",
                                altText = altText,
                                contents = flexMessage
                            }
                        }
                    };

                    var json = JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_channelAccessToken}");

                    var response = await _httpClient.PostAsync(LINE_MESSAGING_API, content);

                    if (response.IsSuccessStatusCode)
                    {
                        ConsoleUI.ShowSuccess($"✅ ส่ง Flex Message ถึง {MaskUserId(userId)} แล้ว");
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        ConsoleUI.ShowError($"❌ ส่ง Flex Message ไม่สำเร็จ: {response.StatusCode}");
                        allSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleUI.ShowError($"❌ Error: {ex.Message}");
                    allSuccess = false;
                }
            }

            return allSuccess;
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

            var message = $@"{emoji} สัญญาณการเทรด {emoji}

Symbol: {symbol}
Action: {action}
Master Score: {masterScore}/100

เหตุผล:
{reasoning}

⏰ {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือนเข้า Position (Flex Message สวยงาม)
        /// </summary>
        public async Task NotifyEntryAsync(string symbol, decimal price, decimal amount,
            decimal stopLoss, decimal takeProfit)
        {
            var totalValue = price * amount;
            var slPercent = ((price - stopLoss) / price * 100);
            var tpPercent = ((takeProfit - price) / price * 100);

            // Simple text version (Flex Message requires complex JSON structure)
            var message = $@"🎯 เข้า Position แล้ว!

💱 Symbol: {symbol}
💵 ราคาเข้า: {price:N2} THB
📊 จำนวน: {amount:N8}
💰 มูลค่า: {totalValue:N2} THB

🛑 Stop Loss: {stopLoss:N2} (-{slPercent:N2}%)
🎯 Take Profit: {takeProfit:N2} (+{tpPercent:N2}%)

⏰ {DateTime.Now:dd/MM/yyyy HH:mm:ss}

---
Ultimate Trading System";

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

            var message = $@"{emoji} ออก Position แล้ว!

💱 Symbol: {symbol}
📈 ราคาเข้า: {entryPrice:N2} THB
📉 ราคาออก: {exitPrice:N2} THB
📊 จำนวน: {amount:N8}

{(totalPL >= 0 ? "💰" : "📉")} กำไร/ขาดทุน: {totalPL:N2} THB ({profitLossPercent:+0.00;-0.00}%)
📝 เหตุผล: {reason}

⏰ {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือน Stop Loss
        /// </summary>
        public async Task NotifyStopLossAsync(string symbol, decimal entryPrice, decimal exitPrice, decimal loss)
        {
            var lossPercent = ((exitPrice - entryPrice) / entryPrice * 100);

            var message = $@"🛑 STOP LOSS ถูกกระตุ้น!

💱 Symbol: {symbol}
📈 ราคาเข้า: {entryPrice:N2} THB
📉 ราคาออก: {exitPrice:N2} THB

📉 ขาดทุน: {loss:N2} THB ({lossPercent:N2}%)

⏰ {DateTime.Now:dd/MM/yyyy HH:mm:ss}

⚠️ ทบทวนกลยุทธ์และวิเคราะห์ใหม่";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือน Take Profit
        /// </summary>
        public async Task NotifyTakeProfitAsync(string symbol, decimal entryPrice, decimal exitPrice, decimal profit)
        {
            var profitPercent = ((exitPrice - entryPrice) / entryPrice * 100);

            var message = $@"🎯 TAKE PROFIT ถึงเป้าแล้ว!

💱 Symbol: {symbol}
📈 ราคาเข้า: {entryPrice:N2} THB
💰 ราคาออก: {exitPrice:N2} THB

💰 กำไร: {profit:N2} THB (+{profitPercent:N2}%)

⏰ {DateTime.Now:dd/MM/yyyy HH:mm:ss}

🎉 ยินดีด้วย! เทรดสำเร็จ";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือนราคาผ่านจุดสำคัญ
        /// </summary>
        public async Task NotifyPriceAlertAsync(string symbol, decimal price, string level, string type)
        {
            var emoji = type == "SUPPORT" ? "⬆️" : "⬇️";

            var message = $@"{emoji} ราคาผ่านจุดสำคัญ!

💱 Symbol: {symbol}
💵 ราคาปัจจุบัน: {price:N2} THB

📍 ผ่าน: {level} ({type})

⏰ {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือน Elliott Wave
        /// </summary>
        public async Task NotifyElliottWaveAsync(string symbol, string pattern, string currentWave,
            int confidence, string nextExpected)
        {
            var message = $@"🌊 Elliott Wave Update

💱 Symbol: {symbol}
📊 Pattern: {pattern}
🌊 Current Wave: {currentWave}
✅ Confidence: {confidence}%
➡️ Next Expected: {nextExpected}

⏰ {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือน Pattern ที่พบ
        /// </summary>
        public async Task NotifyPatternAsync(string symbol, string pattern, int confidence)
        {
            var emoji = pattern.Contains("BOTTOM") || pattern.Contains("CUP") ||
                       pattern.Contains("ASCENDING") ? "📈" : "📉";

            var message = $@"{emoji} Chart Pattern ตรวจพบ!

💱 Symbol: {symbol}
📊 Pattern: {pattern}
✅ Confidence: {confidence}%

⏰ {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// สรุปประจำวัน
        /// </summary>
        public async Task SendDailySummaryAsync(DailyStats stats)
        {
            var winRateEmoji = stats.WinRate >= 70 ? "🎉" :
                              stats.WinRate >= 50 ? "👍" : "😐";

            var profitEmoji = stats.NetProfit >= 0 ? "💰" : "📉";

            var message = $@"📊 สรุปการเทรดประจำวัน

📅 วันที่: {DateTime.Now:dd/MM/yyyy}

📈 จำนวน Trades: {stats.TotalTrades}
✅ Win: {stats.WinTrades} ({stats.WinRate:N1}%) {winRateEmoji}
❌ Loss: {stats.LossTrades}

{profitEmoji} กำไร/ขาดทุนรวม: {stats.NetProfit:N2} THB
💰 Largest Win: {stats.LargestWin:N2} THB
📉 Largest Loss: {stats.LargestLoss:N2} THB

🎯 Win Rate: {stats.WinRate:N1}%
📊 Profit Factor: {stats.ProfitFactor:N2}

---
Ultimate Trading System
⏰ {DateTime.Now:HH:mm:ss}";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือน Error
        /// </summary>
        public async Task NotifyErrorAsync(string error, string details = "")
        {
            var message = $@"❌ เกิดข้อผิดพลาด!

🚨 Error: {error}

{(string.IsNullOrEmpty(details) ? "" : $"📝 Details:\n{details.Substring(0, Math.Min(200, details.Length))}\n")}
⏰ {DateTime.Now:dd/MM/yyyy HH:mm:ss}

⚠️ กรุณาตรวจสอบระบบ";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// แจ้งเตือนเหตุการณ์ Custom
        /// </summary>
        public async Task NotifyCustomEventAsync(string eventName, string message)
        {
            var fullMessage = $@"🔔 {eventName}

{message}

⏰ {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

            await SendMessageAsync(fullMessage);
        }

        /// <summary>
        /// ทดสอบการส่ง LINE
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            if (!_enabled)
            {
                AnsiConsole.MarkupLine("[yellow]⚠️ LINE OA ยังไม่ได้เปิดใช้งาน[/]");
                AnsiConsole.MarkupLine("[yellow]ต้องตั้งค่า:[/]");
                AnsiConsole.MarkupLine("[cyan]1. Channel Access Token[/]");
                AnsiConsole.MarkupLine("[cyan]2. User ID (อย่างน้อย 1 คน)[/]");
                return false;
            }

            AnsiConsole.MarkupLine($"[cyan]📱 ทดสอบส่งข้อความไปยัง {_userIds.Count} User(s)...[/]");

            var message = @"✅ ทดสอบการเชื่อมต่อ LINE Official Account

🎛️ Ultimate Trading System
พร้อมใช้งาน!

⏰ " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            var result = await SendMessageAsync(message);

            if (result)
            {
                AnsiConsole.MarkupLine("[green]✅ เชื่อมต่อ LINE OA สำเร็จ![/]");
                AnsiConsole.MarkupLine("[green]ตรวจสอบข้อความใน LINE app ของคุณ[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]❌ เชื่อมต่อ LINE OA ไม่สำเร็จ[/]");
                AnsiConsole.MarkupLine("[yellow]ตรวจสอบ:[/]");
                AnsiConsole.MarkupLine("[cyan]1. Channel Access Token ถูกต้อง[/]");
                AnsiConsole.MarkupLine("[cyan]2. User ID ถูกต้อง[/]");
                AnsiConsole.MarkupLine("[cyan]3. ได้เพิ่มเพื่อน LINE OA แล้ว[/]");
            }

            return result;
        }

        /// <summary>
        /// ซ่อน User ID บางส่วนเพื่อความปลอดภัย
        /// </summary>
        private string MaskUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId) || userId.Length < 8)
                return "***";

            return userId.Substring(0, 4) + "..." + userId.Substring(userId.Length - 4);
        }

        /// <summary>
        /// สร้าง Rich Menu (ต้องเรียกผ่าน LINE Developers Console)
        /// </summary>
        public void ShowRichMenuInstructions()
        {
            AnsiConsole.MarkupLine("\n[yellow]📱 วิธีสร้าง Rich Menu:[/]");
            AnsiConsole.MarkupLine("[cyan]1. ไปที่ LINE Official Account Manager[/]");
            AnsiConsole.MarkupLine("[cyan]2. เลือก Home > Rich menus[/]");
            AnsiConsole.MarkupLine("[cyan]3. Create rich menu[/]");
            AnsiConsole.MarkupLine("[cyan]4. เพิ่มปุ่มสำหรับ: Start Bot, View Stats, Help[/]");
        }
    }

    /// <summary>
    /// Helper สำหรับดึง User ID
    /// </summary>
    public class LineUserIdHelper
    {
        public static void ShowInstructions()
        {
            AnsiConsole.Clear();

            var panel = new Panel(
                new Markup(
                    "[bold yellow]📱 วิธีหา LINE User ID[/]\n\n" +
                    "[cyan]วิธีที่ 1: ใช้ LINE Developers Console[/]\n" +
                    "1. ไปที่ https://developers.line.biz/console/\n" +
                    "2. เลือก Channel ของคุณ\n" +
                    "3. ไปที่ Messaging API > Bot information\n" +
                    "4. สแกน QR Code เพื่อเพิ่มเพื่อน\n" +
                    "5. ส่งข้อความหา Bot\n" +
                    "6. ดู Webhook events เพื่อเห็น User ID\n\n" +
                    "[cyan]วิธีที่ 2: ใช้ Test Tool[/]\n" +
                    "1. ติดตั้ง LINE Official Account\n" +
                    "2. เพิ่มเพื่อน\n" +
                    "3. ไปที่ Settings > Messaging API\n" +
                    "4. ใช้ส่งข้อความทดสอบ\n\n" +
                    "[cyan]วิธีที่ 3: ใช้ Webhook[/]\n" +
                    "1. ตั้งค่า Webhook URL\n" +
                    "2. ส่งข้อความหา Bot\n" +
                    "3. ดู User ID จาก Webhook payload\n\n" +
                    "[bold green]💡 Tips:[/]\n" +
                    "- User ID จะเป็น string ยาวๆ เช่น: Uxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\n" +
                    "- เก็บ User ID ไว้ใน Config หรือ Database\n" +
                    "- สามารถส่งข้อความหลาย User พร้อมกันได้"
                ))
            {
                Header = new PanelHeader("🔍 [bold]LINE User ID Guide[/]", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Yellow)
            };

            AnsiConsole.Write(panel);
        }
    }
}
