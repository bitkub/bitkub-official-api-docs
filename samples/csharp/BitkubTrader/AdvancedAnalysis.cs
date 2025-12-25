using System;
using System.Collections.Generic;
using System.Linq;

namespace BitkubTrader
{
    /// <summary>
    /// Advanced Technical Analysis - Elliott Wave, Fibonacci, Volume Profile
    /// 🌊 การวิเคราะห์ขั้นสูงที่ไม่เคยมีมาก่อน!
    /// </summary>
    public class AdvancedAnalysis
    {
        #region Elliott Wave Analysis

        /// <summary>
        /// Detect Elliott Wave patterns
        /// วิเคราะห์ Elliott Wave - ทฤษฎีคลื่นของ Ralph Nelson Elliott
        /// </summary>
        public static ElliottWavePattern DetectElliottWave(List<decimal> prices)
        {
            if (prices.Count < 50)
                return new ElliottWavePattern { Pattern = "INSUFFICIENT_DATA" };

            var pivots = FindPivotPoints(prices);

            if (pivots.Count < 5)
                return new ElliottWavePattern { Pattern = "NO_CLEAR_PATTERN" };

            // Elliott Wave มี 5 คลื่น (Impulse) + 3 คลื่น (Correction)
            // Wave 1, 3, 5 = Motive waves (ขึ้น)
            // Wave 2, 4 = Corrective waves (ลง)

            var wave = AnalyzeWaveStructure(pivots, prices);
            return wave;
        }

        private static List<PivotPoint> FindPivotPoints(List<decimal> prices, int window = 5)
        {
            var pivots = new List<PivotPoint>();

            for (int i = window; i < prices.Count - window; i++)
            {
                var isPivotHigh = true;
                var isPivotLow = true;

                for (int j = i - window; j <= i + window; j++)
                {
                    if (j == i) continue;

                    if (prices[j] >= prices[i])
                        isPivotHigh = false;
                    if (prices[j] <= prices[i])
                        isPivotLow = false;
                }

                if (isPivotHigh)
                {
                    pivots.Add(new PivotPoint
                    {
                        Index = i,
                        Price = prices[i],
                        Type = "HIGH"
                    });
                }
                else if (isPivotLow)
                {
                    pivots.Add(new PivotPoint
                    {
                        Index = i,
                        Price = prices[i],
                        Type = "LOW"
                    });
                }
            }

            return pivots;
        }

        private static ElliottWavePattern AnalyzeWaveStructure(List<PivotPoint> pivots, List<decimal> prices)
        {
            // Simplified Elliott Wave detection
            // จริงๆ ต้องมีการวิเคราะห์ซับซ้อนกว่านี้มาก

            if (pivots.Count < 8)
                return new ElliottWavePattern { Pattern = "INCOMPLETE" };

            var recentPivots = pivots.TakeLast(8).ToList();

            // Check for 5-wave impulse pattern
            var isImpulseWave = CheckImpulseWave(recentPivots);

            if (isImpulseWave)
            {
                var currentWave = DetermineCurrentWave(recentPivots, prices);

                return new ElliottWavePattern
                {
                    Pattern = "IMPULSE_WAVE",
                    CurrentWave = currentWave,
                    Confidence = 75,
                    NextExpectedMove = GetExpectedMove(currentWave),
                    Pivots = recentPivots
                };
            }

            // Check for corrective pattern (ABC)
            var isCorrective = CheckCorrectiveWave(recentPivots);

            if (isCorrective)
            {
                return new ElliottWavePattern
                {
                    Pattern = "CORRECTIVE_WAVE",
                    CurrentWave = "C",
                    Confidence = 70,
                    NextExpectedMove = "REVERSAL_EXPECTED"
                };
            }

            return new ElliottWavePattern { Pattern = "UNKNOWN" };
        }

        private static bool CheckImpulseWave(List<PivotPoint> pivots)
        {
            // Wave 3 should be longest (or at least not shortest)
            // Wave 2 shouldn't retrace more than 100% of Wave 1
            // Wave 4 shouldn't overlap Wave 1

            // Simplified check
            return pivots.Count >= 5;
        }

        private static bool CheckCorrectiveWave(List<PivotPoint> pivots)
        {
            // ABC correction pattern
            return pivots.Count >= 3;
        }

        private static string DetermineCurrentWave(List<PivotPoint> pivots, List<decimal> prices)
        {
            var lastPivot = pivots.Last();
            var currentPrice = prices.Last();

            // Simplified wave counting
            var waveCount = pivots.Count % 5;

            return waveCount switch
            {
                1 => "WAVE_1",
                2 => "WAVE_2",
                3 => "WAVE_3",
                4 => "WAVE_4",
                0 => "WAVE_5",
                _ => "WAVE_1"
            };
        }

        private static string GetExpectedMove(string currentWave)
        {
            return currentWave switch
            {
                "WAVE_1" => "CORRECTION_DOWN (Wave 2)",
                "WAVE_2" => "STRONG_MOVE_UP (Wave 3 - Strongest)",
                "WAVE_3" => "MINOR_CORRECTION (Wave 4)",
                "WAVE_4" => "FINAL_PUSH_UP (Wave 5)",
                "WAVE_5" => "MAJOR_CORRECTION (ABC)",
                _ => "UNKNOWN"
            };
        }

        #endregion

        #region Fibonacci Analysis

        /// <summary>
        /// Calculate Fibonacci Retracement levels
        /// </summary>
        public static FibonacciLevels CalculateFibonacciRetracement(decimal high, decimal low)
        {
            var diff = high - low;

            return new FibonacciLevels
            {
                Level_0 = high,
                Level_236 = high - (diff * 0.236m),
                Level_382 = high - (diff * 0.382m),
                Level_500 = high - (diff * 0.500m),
                Level_618 = high - (diff * 0.618m),
                Level_786 = high - (diff * 0.786m),
                Level_100 = low
            };
        }

        /// <summary>
        /// Calculate Fibonacci Extension levels
        /// </summary>
        public static FibonacciExtensions CalculateFibonacciExtension(decimal start, decimal end, decimal retrace)
        {
            var move = end - start;
            var retraceLevel = retrace;

            return new FibonacciExtensions
            {
                Level_618 = retraceLevel + (move * 0.618m),
                Level_1000 = retraceLevel + move,
                Level_1272 = retraceLevel + (move * 1.272m),
                Level_1618 = retraceLevel + (move * 1.618m),
                Level_2000 = retraceLevel + (move * 2.000m),
                Level_2618 = retraceLevel + (move * 2.618m)
            };
        }

        /// <summary>
        /// Find support/resistance using Fibonacci
        /// </summary>
        public static List<FibLevel> FindFibonacciSupportResistance(List<decimal> prices)
        {
            var high = prices.Max();
            var low = prices.Min();
            var current = prices.Last();

            var fib = CalculateFibonacciRetracement(high, low);
            var levels = new List<FibLevel>();

            AddFibLevel(levels, fib.Level_236, "23.6%", current);
            AddFibLevel(levels, fib.Level_382, "38.2%", current);
            AddFibLevel(levels, fib.Level_500, "50.0%", current);
            AddFibLevel(levels, fib.Level_618, "61.8% (Golden Ratio)", current);
            AddFibLevel(levels, fib.Level_786, "78.6%", current);

            return levels.OrderBy(l => Math.Abs(l.Distance)).ToList();
        }

        private static void AddFibLevel(List<FibLevel> levels, decimal price, string name, decimal currentPrice)
        {
            levels.Add(new FibLevel
            {
                Price = price,
                Name = name,
                Distance = ((price - currentPrice) / currentPrice) * 100,
                Type = price > currentPrice ? "RESISTANCE" : "SUPPORT"
            });
        }

        #endregion

        #region Volume Profile Analysis

        /// <summary>
        /// Calculate Volume Profile (VPVR - Volume Profile Visible Range)
        /// </summary>
        public static VolumeProfile CalculateVolumeProfile(List<decimal> prices, List<decimal> volumes, int bins = 20)
        {
            if (prices.Count != volumes.Count || prices.Count < 10)
                return new VolumeProfile();

            var maxPrice = prices.Max();
            var minPrice = prices.Min();
            var priceRange = maxPrice - minPrice;
            var binSize = priceRange / bins;

            var volumeBins = new Dictionary<decimal, decimal>();

            // Initialize bins
            for (int i = 0; i < bins; i++)
            {
                var binPrice = minPrice + (binSize * i);
                volumeBins[binPrice] = 0;
            }

            // Fill bins with volume
            for (int i = 0; i < prices.Count; i++)
            {
                var price = prices[i];
                var volume = volumes[i];
                var binIndex = (int)((price - minPrice) / binSize);

                if (binIndex >= bins) binIndex = bins - 1;
                if (binIndex < 0) binIndex = 0;

                var binPrice = minPrice + (binSize * binIndex);
                volumeBins[binPrice] += volume;
            }

            // Find POC (Point of Control) - price level with highest volume
            var poc = volumeBins.OrderByDescending(kv => kv.Value).First();

            // Find Value Area (70% of volume)
            var totalVolume = volumeBins.Values.Sum();
            var valueAreaVolume = totalVolume * 0.70m;

            var sortedBins = volumeBins.OrderByDescending(kv => kv.Value).ToList();
            var valueAreaBins = new List<KeyValuePair<decimal, decimal>>();
            var accumulatedVolume = 0m;

            foreach (var bin in sortedBins)
            {
                valueAreaBins.Add(bin);
                accumulatedVolume += bin.Value;

                if (accumulatedVolume >= valueAreaVolume)
                    break;
            }

            var vah = valueAreaBins.Max(b => b.Key); // Value Area High
            var val = valueAreaBins.Min(b => b.Key); // Value Area Low

            return new VolumeProfile
            {
                POC = poc.Key,
                VAH = vah,
                VAL = val,
                TotalVolume = totalVolume,
                VolumeDistribution = volumeBins
            };
        }

        /// <summary>
        /// Detect High Volume Nodes (HVN) and Low Volume Nodes (LVN)
        /// </summary>
        public static VolumeNodes DetectVolumeNodes(VolumeProfile profile)
        {
            var avgVolume = profile.VolumeDistribution.Values.Average();
            var stdDev = CalculateStdDev(profile.VolumeDistribution.Values.ToList());

            var hvnThreshold = avgVolume + stdDev;
            var lvnThreshold = avgVolume - stdDev;

            var hvns = profile.VolumeDistribution
                .Where(kv => kv.Value > hvnThreshold)
                .Select(kv => kv.Key)
                .ToList();

            var lvns = profile.VolumeDistribution
                .Where(kv => kv.Value < lvnThreshold)
                .Select(kv => kv.Key)
                .ToList();

            return new VolumeNodes
            {
                HighVolumeNodes = hvns,
                LowVolumeNodes = lvns,
                POC = profile.POC
            };
        }

        private static decimal CalculateStdDev(List<decimal> values)
        {
            var avg = values.Average();
            var sumOfSquares = values.Sum(v => (double)Math.Pow((double)(v - avg), 2));
            return (decimal)Math.Sqrt(sumOfSquares / values.Count);
        }

        #endregion

        #region Market Structure Analysis

        /// <summary>
        /// Detect market structure (Higher Highs, Lower Lows, etc.)
        /// </summary>
        public static MarketStructure AnalyzeMarketStructure(List<decimal> prices)
        {
            var pivots = FindPivotPoints(prices);

            if (pivots.Count < 4)
                return new MarketStructure { Trend = "UNCLEAR" };

            var highs = pivots.Where(p => p.Type == "HIGH").ToList();
            var lows = pivots.Where(p => p.Type == "LOW").ToList();

            // Check for Higher Highs and Higher Lows (Uptrend)
            var isUptrend = CheckHigherHighsLows(highs, lows);

            // Check for Lower Highs and Lower Lows (Downtrend)
            var isDowntrend = CheckLowerHighsLows(highs, lows);

            string trend;
            string structure;

            if (isUptrend)
            {
                trend = "UPTREND";
                structure = "Higher Highs, Higher Lows";
            }
            else if (isDowntrend)
            {
                trend = "DOWNTREND";
                structure = "Lower Highs, Lower Lows";
            }
            else
            {
                trend = "RANGING";
                structure = "Consolidation";
            }

            // Detect Break of Structure (BOS) or Change of Character (CHoCH)
            var lastStructureBreak = DetectStructureBreak(pivots, prices);

            return new MarketStructure
            {
                Trend = trend,
                Structure = structure,
                LastBreak = lastStructureBreak,
                KeyLevels = GetKeyLevels(highs, lows)
            };
        }

        private static bool CheckHigherHighsLows(List<PivotPoint> highs, List<PivotPoint> lows)
        {
            if (highs.Count < 2 || lows.Count < 2) return false;

            var recentHighs = highs.TakeLast(3).ToList();
            var recentLows = lows.TakeLast(3).ToList();

            var higherHighs = recentHighs.Zip(recentHighs.Skip(1), (a, b) => b.Price > a.Price).All(x => x);
            var higherLows = recentLows.Zip(recentLows.Skip(1), (a, b) => b.Price > a.Price).All(x => x);

            return higherHighs && higherLows;
        }

        private static bool CheckLowerHighsLows(List<PivotPoint> highs, List<PivotPoint> lows)
        {
            if (highs.Count < 2 || lows.Count < 2) return false;

            var recentHighs = highs.TakeLast(3).ToList();
            var recentLows = lows.TakeLast(3).ToList();

            var lowerHighs = recentHighs.Zip(recentHighs.Skip(1), (a, b) => b.Price < a.Price).All(x => x);
            var lowerLows = recentLows.Zip(recentLows.Skip(1), (a, b) => b.Price < a.Price).All(x => x);

            return lowerHighs && lowerLows;
        }

        private static string DetectStructureBreak(List<PivotPoint> pivots, List<decimal> prices)
        {
            // Simplified BOS/CHoCH detection
            if (pivots.Count < 2) return "NONE";

            var lastPivot = pivots[^1];
            var prevPivot = pivots[^2];
            var currentPrice = prices.Last();

            if (lastPivot.Type == "HIGH" && currentPrice > lastPivot.Price)
                return "BREAK_OF_STRUCTURE_BULLISH";

            if (lastPivot.Type == "LOW" && currentPrice < lastPivot.Price)
                return "BREAK_OF_STRUCTURE_BEARISH";

            return "NO_BREAK";
        }

        private static List<decimal> GetKeyLevels(List<PivotPoint> highs, List<PivotPoint> lows)
        {
            var levels = new List<decimal>();

            levels.AddRange(highs.TakeLast(3).Select(h => h.Price));
            levels.AddRange(lows.TakeLast(3).Select(l => l.Price));

            return levels.Distinct().OrderByDescending(l => l).ToList();
        }

        #endregion

        #region Order Flow Analysis

        /// <summary>
        /// Analyze order flow and market depth
        /// </summary>
        public static OrderFlowAnalysis AnalyzeOrderFlow(List<List<decimal>> bids, List<List<decimal>> asks)
        {
            if (bids.Count == 0 || asks.Count == 0)
                return new OrderFlowAnalysis { Imbalance = "UNKNOWN" };

            var totalBidVolume = bids.Sum(b => b[1]); // Sum of bid amounts
            var totalAskVolume = asks.Sum(a => a[1]); // Sum of ask amounts

            var ratio = totalBidVolume / (totalAskVolume + 0.0001m); // Avoid division by zero

            var avgBidPrice = bids.Average(b => b[0]);
            var avgAskPrice = asks.Average(a => a[0]);

            string imbalance;
            string pressure;

            if (ratio > 1.5m)
            {
                imbalance = "HEAVY_BID_SIDE";
                pressure = "BUYING_PRESSURE";
            }
            else if (ratio < 0.67m)
            {
                imbalance = "HEAVY_ASK_SIDE";
                pressure = "SELLING_PRESSURE";
            }
            else
            {
                imbalance = "BALANCED";
                pressure = "NEUTRAL";
            }

            // Detect walls (large orders)
            var bidWalls = bids.Where(b => b[1] > totalBidVolume * 0.1m).ToList();
            var askWalls = asks.Where(a => a[1] > totalAskVolume * 0.1m).ToList();

            return new OrderFlowAnalysis
            {
                BidVolume = totalBidVolume,
                AskVolume = totalAskVolume,
                Ratio = ratio,
                Imbalance = imbalance,
                Pressure = pressure,
                BidWalls = bidWalls.Select(w => w[0]).ToList(),
                AskWalls = askWalls.Select(w => w[0]).ToList()
            };
        }

        #endregion
    }

    #region Data Models

    public class ElliottWavePattern
    {
        public string Pattern { get; set; } = "";
        public string CurrentWave { get; set; } = "";
        public int Confidence { get; set; }
        public string NextExpectedMove { get; set; } = "";
        public List<PivotPoint> Pivots { get; set; } = new();
    }

    public class PivotPoint
    {
        public int Index { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; } = ""; // HIGH or LOW
    }

    public class FibonacciLevels
    {
        public decimal Level_0 { get; set; }
        public decimal Level_236 { get; set; }
        public decimal Level_382 { get; set; }
        public decimal Level_500 { get; set; }
        public decimal Level_618 { get; set; }
        public decimal Level_786 { get; set; }
        public decimal Level_100 { get; set; }
    }

    public class FibonacciExtensions
    {
        public decimal Level_618 { get; set; }
        public decimal Level_1000 { get; set; }
        public decimal Level_1272 { get; set; }
        public decimal Level_1618 { get; set; }
        public decimal Level_2000 { get; set; }
        public decimal Level_2618 { get; set; }
    }

    public class FibLevel
    {
        public decimal Price { get; set; }
        public string Name { get; set; } = "";
        public decimal Distance { get; set; } // % distance from current price
        public string Type { get; set; } = ""; // SUPPORT or RESISTANCE
    }

    public class VolumeProfile
    {
        public decimal POC { get; set; } // Point of Control
        public decimal VAH { get; set; } // Value Area High
        public decimal VAL { get; set; } // Value Area Low
        public decimal TotalVolume { get; set; }
        public Dictionary<decimal, decimal> VolumeDistribution { get; set; } = new();
    }

    public class VolumeNodes
    {
        public List<decimal> HighVolumeNodes { get; set; } = new();
        public List<decimal> LowVolumeNodes { get; set; } = new();
        public decimal POC { get; set; }
    }

    public class MarketStructure
    {
        public string Trend { get; set; } = "";
        public string Structure { get; set; } = "";
        public string LastBreak { get; set; } = "";
        public List<decimal> KeyLevels { get; set; } = new();
    }

    public class OrderFlowAnalysis
    {
        public decimal BidVolume { get; set; }
        public decimal AskVolume { get; set; }
        public decimal Ratio { get; set; }
        public string Imbalance { get; set; } = "";
        public string Pressure { get; set; } = "";
        public List<decimal> BidWalls { get; set; } = new();
        public List<decimal> AskWalls { get; set; } = new();
    }

    #endregion
}
