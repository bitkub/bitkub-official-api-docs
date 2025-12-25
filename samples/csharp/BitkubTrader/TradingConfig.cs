using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BitkubTrader
{
    /// <summary>
    /// ⚙️ ระบบตั้งค่ากลยุทธ์การเทรด - ปรับได้ทุกอย่าง!
    /// ใช้งานง่าย เข้าใจง่าย แก้ไขได้ในไฟล์ JSON
    /// </summary>
    public class TradingConfig
    {
        // === GENERAL SETTINGS ===
        public GeneralSettings General { get; set; } = new();

        // === ENTRY LOGIC ===
        public EntryLogic Entry { get; set; } = new();

        // === EXIT LOGIC ===
        public ExitLogic Exit { get; set; } = new();

        // === RISK MANAGEMENT ===
        public RiskManagement Risk { get; set; } = new();

        // === INDICATOR SETTINGS ===
        public IndicatorSettings Indicators { get; set; } = new();

        // === NOTIFICATION SETTINGS ===
        public NotificationSettings Notifications { get; set; } = new();

        /// <summary>
        /// โหลด Config จากไฟล์ JSON
        /// </summary>
        public static TradingConfig Load(string path = "trading_config.json")
        {
            try
            {
                if (!File.Exists(path))
                {
                    // สร้าง config ตัวอย่างใหม่
                    var defaultConfig = CreateDefault();
                    defaultConfig.Save(path);
                    return defaultConfig;
                }

                var json = File.ReadAllText(path);
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<TradingConfig>(json, options)
                       ?? CreateDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error loading config: {ex.Message}");
                return CreateDefault();
            }
        }

        /// <summary>
        /// บันทึก Config ลงไฟล์
        /// </summary>
        public void Save(string path = "trading_config.json")
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            };

            var json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// สร้าง Config ตัวอย่าง
        /// </summary>
        public static TradingConfig CreateDefault()
        {
            return new TradingConfig
            {
                General = new GeneralSettings
                {
                    TradingMode = "BALANCED",
                    Symbol = "THB_BTC",
                    UpdateInterval = 30,
                    MaxOpenPositions = 3,
                    EnableAutoTrading = false
                },

                Entry = new EntryLogic
                {
                    MinMasterScore = 75,
                    RequireConfirmation = true,

                    // ต้องมีอย่างน้อย X ตัวชี้วัดบอกซื้อ
                    MinBullishIndicators = 5,

                    // เงื่อนไข Technical
                    TechnicalConditions = new TechnicalConditions
                    {
                        RSI_Min = 30,
                        RSI_Max = 70,
                        MACD_MustBePositive = true,
                        RequireBollingerBounce = true
                    },

                    // เงื่อนไข Elliott Wave
                    ElliottWaveConditions = new ElliottWaveConditions
                    {
                        PreferredWaves = new List<string> { "WAVE_3", "WAVE_5" },
                        MinConfidence = 70
                    },

                    // เงื่อนไข Fibonacci
                    FibonacciConditions = new FibonacciConditions
                    {
                        MaxDistanceFromSupport = 2.0m, // ห่างจาก support ไม่เกิน 2%
                        PreferredLevels = new List<string> { "61.8%", "50%" }
                    },

                    // เงื่อนไข Volume Profile
                    VolumeProfileConditions = new VolumeProfileConditions
                    {
                        RequireAbovePOC = true,
                        MaxDistanceFromVAL = 5.0m
                    },

                    // เงื่อนไข Market Structure
                    MarketStructureConditions = new MarketStructureConditions
                    {
                        RequiredTrend = "UPTREND",
                        AllowBOS = true,
                        AllowCHoCH = false // ไม่เข้าถ้าเปลี่ยนทิศทาง
                    },

                    // เงื่อนไข Order Flow
                    OrderFlowConditions = new OrderFlowConditions
                    {
                        MinBidAskRatio = 1.2m, // ฝั่งซื้อต้องมากกว่าฝั่งขายอย่างน้อย 1.2 เท่า
                        RequireBuyingPressure = true
                    }
                },

                Exit = new ExitLogic
                {
                    // Stop Loss
                    StopLossPercent = 3.0m, // ขาดทุน 3% ขายทิ้ง
                    StopLossType = "PERCENTAGE", // หรือ "ATR", "FIBONACCI"

                    // Take Profit
                    TakeProfitPercent = 5.0m, // กำไร 5% ขาย
                    TakeProfitType = "PERCENTAGE", // หรือ "FIBONACCI", "RESISTANCE"

                    // Trailing Stop
                    EnableTrailingStop = true,
                    TrailingStopPercent = 2.0m,

                    // Time-based Exit
                    MaxHoldingHours = 72, // ถือครองสูงสุด 72 ชั่วโมง

                    // Signal-based Exit
                    ExitOnMasterScoreBelow = 40, // ถ้าคะแนนต่ำกว่า 40 ให้ออก
                    ExitOnTrendChange = true // ออกเมื่อเทรนด์เปลี่ยน
                },

                Risk = new RiskManagement
                {
                    MaxRiskPerTradePercent = 2.0m, // เสี่ยงไม่เกิน 2% ต่อ trade
                    PositionSizeType = "RISK_BASED", // หรือ "FIXED_AMOUNT", "PERCENTAGE"
                    MaxDailyLossPercent = 5.0m, // ขาดทุนวันละไม่เกิน 5%
                    MaxDailyTrades = 10,

                    // Pyramiding (เพิ่มออเดอร์ตามกำไร)
                    EnablePyramiding = false,
                    MaxPyramidLevels = 3
                },

                Indicators = new IndicatorSettings
                {
                    RSI_Period = 14,
                    RSI_Overbought = 70,
                    RSI_Oversold = 30,

                    MACD_Fast = 12,
                    MACD_Slow = 26,
                    MACD_Signal = 9,

                    BollingerBands_Period = 20,
                    BollingerBands_StdDev = 2.0m,

                    EMA_Periods = new List<int> { 9, 21, 50, 200 },

                    ATR_Period = 14,
                    ATR_Multiplier = 2.0m
                },

                Notifications = new NotificationSettings
                {
                    EnableLineNotify = true,
                    LineAccessToken = "YOUR_LINE_ACCESS_TOKEN",

                    NotifyOnSignal = true,
                    NotifyOnEntry = true,
                    NotifyOnExit = true,
                    NotifyOnStopLoss = true,
                    NotifyOnTakeProfit = true,
                    NotifyOnError = true,

                    DailySummaryTime = "18:00",

                    CustomEvents = new List<CustomNotificationEvent>
                    {
                        new CustomNotificationEvent
                        {
                            Name = "Strong Buy Signal",
                            Condition = "MasterScore >= 85",
                            Message = "🔥 STRONG BUY! Master Score: {score}"
                        },
                        new CustomNotificationEvent
                        {
                            Name = "Price Alert",
                            Condition = "Price > 2000000",
                            Message = "📈 BTC ทะลุ 2 ล้านบาทแล้ว!"
                        }
                    }
                }
            };
        }

        /// <summary>
        /// ตรวจสอบว่า Config ถูกต้องหรือไม่
        /// </summary>
        public (bool IsValid, List<string> Errors) Validate()
        {
            var errors = new List<string>();

            // ตรวจสอบค่าพื้นฐาน
            if (Entry.MinMasterScore < 0 || Entry.MinMasterScore > 100)
                errors.Add("MinMasterScore ต้องอยู่ระหว่าง 0-100");

            if (Exit.StopLossPercent <= 0)
                errors.Add("StopLossPercent ต้องมากกว่า 0");

            if (Exit.TakeProfitPercent <= 0)
                errors.Add("TakeProfitPercent ต้องมากกว่า 0");

            if (Risk.MaxRiskPerTradePercent <= 0 || Risk.MaxRiskPerTradePercent > 100)
                errors.Add("MaxRiskPerTradePercent ต้องอยู่ระหว่าง 0-100");

            if (Notifications.EnableLineNotify &&
                string.IsNullOrEmpty(Notifications.LineAccessToken))
                errors.Add("ต้องใส่ LINE Access Token ถ้าเปิดใช้งาน LINE Notify");

            return (errors.Count == 0, errors);
        }
    }

    // ========================================
    // SETTINGS CLASSES
    // ========================================

    public class GeneralSettings
    {
        public string TradingMode { get; set; } = "BALANCED"; // ULTRA_AGGRESSIVE, AGGRESSIVE, BALANCED, CONSERVATIVE
        public string Symbol { get; set; } = "THB_BTC";
        public int UpdateInterval { get; set; } = 30; // วินาที
        public int MaxOpenPositions { get; set; } = 3;
        public bool EnableAutoTrading { get; set; } = false;
    }

    public class EntryLogic
    {
        public int MinMasterScore { get; set; } = 75;
        public bool RequireConfirmation { get; set; } = true;
        public int MinBullishIndicators { get; set; } = 5;

        public TechnicalConditions TechnicalConditions { get; set; } = new();
        public ElliottWaveConditions ElliottWaveConditions { get; set; } = new();
        public FibonacciConditions FibonacciConditions { get; set; } = new();
        public VolumeProfileConditions VolumeProfileConditions { get; set; } = new();
        public MarketStructureConditions MarketStructureConditions { get; set; } = new();
        public OrderFlowConditions OrderFlowConditions { get; set; } = new();
    }

    public class TechnicalConditions
    {
        public decimal RSI_Min { get; set; } = 30;
        public decimal RSI_Max { get; set; } = 70;
        public bool MACD_MustBePositive { get; set; } = true;
        public bool RequireBollingerBounce { get; set; } = true;
    }

    public class ElliottWaveConditions
    {
        public List<string> PreferredWaves { get; set; } = new();
        public int MinConfidence { get; set; } = 70;
    }

    public class FibonacciConditions
    {
        public decimal MaxDistanceFromSupport { get; set; } = 2.0m;
        public List<string> PreferredLevels { get; set; } = new();
    }

    public class VolumeProfileConditions
    {
        public bool RequireAbovePOC { get; set; } = true;
        public decimal MaxDistanceFromVAL { get; set; } = 5.0m;
    }

    public class MarketStructureConditions
    {
        public string RequiredTrend { get; set; } = "UPTREND";
        public bool AllowBOS { get; set; } = true;
        public bool AllowCHoCH { get; set; } = false;
    }

    public class OrderFlowConditions
    {
        public decimal MinBidAskRatio { get; set; } = 1.2m;
        public bool RequireBuyingPressure { get; set; } = true;
    }

    public class ExitLogic
    {
        public decimal StopLossPercent { get; set; } = 3.0m;
        public string StopLossType { get; set; } = "PERCENTAGE";

        public decimal TakeProfitPercent { get; set; } = 5.0m;
        public string TakeProfitType { get; set; } = "PERCENTAGE";

        public bool EnableTrailingStop { get; set; } = true;
        public decimal TrailingStopPercent { get; set; } = 2.0m;

        public int MaxHoldingHours { get; set; } = 72;

        public int ExitOnMasterScoreBelow { get; set; } = 40;
        public bool ExitOnTrendChange { get; set; } = true;
    }

    public class RiskManagement
    {
        public decimal MaxRiskPerTradePercent { get; set; } = 2.0m;
        public string PositionSizeType { get; set; } = "RISK_BASED";
        public decimal MaxDailyLossPercent { get; set; } = 5.0m;
        public int MaxDailyTrades { get; set; } = 10;

        public bool EnablePyramiding { get; set; } = false;
        public int MaxPyramidLevels { get; set; } = 3;
    }

    public class IndicatorSettings
    {
        public int RSI_Period { get; set; } = 14;
        public decimal RSI_Overbought { get; set; } = 70;
        public decimal RSI_Oversold { get; set; } = 30;

        public int MACD_Fast { get; set; } = 12;
        public int MACD_Slow { get; set; } = 26;
        public int MACD_Signal { get; set; } = 9;

        public int BollingerBands_Period { get; set; } = 20;
        public decimal BollingerBands_StdDev { get; set; } = 2.0m;

        public List<int> EMA_Periods { get; set; } = new();

        public int ATR_Period { get; set; } = 14;
        public decimal ATR_Multiplier { get; set; } = 2.0m;
    }

    public class NotificationSettings
    {
        public bool EnableLineNotify { get; set; } = true;
        public string LineAccessToken { get; set; } = "";

        public bool NotifyOnSignal { get; set; } = true;
        public bool NotifyOnEntry { get; set; } = true;
        public bool NotifyOnExit { get; set; } = true;
        public bool NotifyOnStopLoss { get; set; } = true;
        public bool NotifyOnTakeProfit { get; set; } = true;
        public bool NotifyOnError { get; set; } = true;

        public string DailySummaryTime { get; set; } = "18:00";

        public List<CustomNotificationEvent> CustomEvents { get; set; } = new();
    }

    public class CustomNotificationEvent
    {
        public string Name { get; set; } = "";
        public string Condition { get; set; } = "";
        public string Message { get; set; } = "";
    }
}
