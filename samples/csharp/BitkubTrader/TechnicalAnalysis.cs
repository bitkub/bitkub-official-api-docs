using System;
using System.Collections.Generic;
using System.Linq;

namespace BitkubTrader
{
    /// <summary>
    /// Technical Analysis Indicators for Smart Trading
    /// </summary>
    public class TechnicalAnalysis
    {
        /// <summary>
        /// Calculate RSI (Relative Strength Index)
        /// </summary>
        public static decimal CalculateRSI(List<decimal> prices, int period = 14)
        {
            if (prices.Count < period + 1)
                return 50; // Default neutral value

            var gains = new List<decimal>();
            var losses = new List<decimal>();

            for (int i = 1; i < prices.Count; i++)
            {
                var change = prices[i] - prices[i - 1];
                if (change > 0)
                {
                    gains.Add(change);
                    losses.Add(0);
                }
                else
                {
                    gains.Add(0);
                    losses.Add(Math.Abs(change));
                }
            }

            var avgGain = gains.TakeLast(period).Average();
            var avgLoss = losses.TakeLast(period).Average();

            if (avgLoss == 0)
                return 100;

            var rs = avgGain / avgLoss;
            var rsi = 100 - (100 / (1 + rs));

            return rsi;
        }

        /// <summary>
        /// Calculate SMA (Simple Moving Average)
        /// </summary>
        public static decimal CalculateSMA(List<decimal> prices, int period)
        {
            if (prices.Count < period)
                return prices.Last();

            return prices.TakeLast(period).Average();
        }

        /// <summary>
        /// Calculate EMA (Exponential Moving Average)
        /// </summary>
        public static decimal CalculateEMA(List<decimal> prices, int period)
        {
            if (prices.Count < period)
                return prices.Last();

            var multiplier = 2m / (period + 1);
            var ema = prices.Take(period).Average(); // Start with SMA

            foreach (var price in prices.Skip(period))
            {
                ema = (price - ema) * multiplier + ema;
            }

            return ema;
        }

        /// <summary>
        /// Calculate MACD (Moving Average Convergence Divergence)
        /// </summary>
        public static (decimal macd, decimal signal, decimal histogram) CalculateMACD(
            List<decimal> prices,
            int fastPeriod = 12,
            int slowPeriod = 26,
            int signalPeriod = 9)
        {
            if (prices.Count < slowPeriod)
                return (0, 0, 0);

            var fastEMA = CalculateEMA(prices, fastPeriod);
            var slowEMA = CalculateEMA(prices, slowPeriod);
            var macd = fastEMA - slowEMA;

            // Calculate signal line (EMA of MACD)
            var macdHistory = new List<decimal> { macd };
            var signal = CalculateEMA(macdHistory, signalPeriod);
            var histogram = macd - signal;

            return (macd, signal, histogram);
        }

        /// <summary>
        /// Calculate Bollinger Bands
        /// </summary>
        public static (decimal upper, decimal middle, decimal lower) CalculateBollingerBands(
            List<decimal> prices,
            int period = 20,
            decimal stdDevMultiplier = 2)
        {
            if (prices.Count < period)
                return (prices.Last(), prices.Last(), prices.Last());

            var sma = CalculateSMA(prices, period);
            var recentPrices = prices.TakeLast(period).ToList();

            // Calculate standard deviation
            var variance = recentPrices.Select(p => Math.Pow((double)(p - sma), 2)).Average();
            var stdDev = (decimal)Math.Sqrt(variance);

            var upper = sma + (stdDev * stdDevMultiplier);
            var lower = sma - (stdDev * stdDevMultiplier);

            return (upper, sma, lower);
        }

        /// <summary>
        /// Calculate Stochastic Oscillator
        /// </summary>
        public static (decimal k, decimal d) CalculateStochastic(
            List<decimal> prices,
            int period = 14)
        {
            if (prices.Count < period)
                return (50, 50);

            var recentPrices = prices.TakeLast(period).ToList();
            var currentPrice = prices.Last();
            var lowestLow = recentPrices.Min();
            var highestHigh = recentPrices.Max();

            var k = highestHigh == lowestLow ? 50 :
                    ((currentPrice - lowestLow) / (highestHigh - lowestLow)) * 100;

            // %D is 3-period SMA of %K
            var d = k; // Simplified, should track K history

            return (k, d);
        }

        /// <summary>
        /// Calculate ATR (Average True Range) for volatility
        /// </summary>
        public static decimal CalculateATR(List<decimal> highs, List<decimal> lows, List<decimal> closes, int period = 14)
        {
            if (highs.Count < period || lows.Count < period || closes.Count < period)
                return 0;

            var trueRanges = new List<decimal>();

            for (int i = 1; i < highs.Count; i++)
            {
                var tr = Math.Max(
                    highs[i] - lows[i],
                    Math.Max(
                        Math.Abs(highs[i] - closes[i - 1]),
                        Math.Abs(lows[i] - closes[i - 1])
                    )
                );
                trueRanges.Add(tr);
            }

            return trueRanges.TakeLast(period).Average();
        }

        /// <summary>
        /// Detect Chart Patterns
        /// </summary>
        public static string DetectPattern(List<decimal> prices)
        {
            if (prices.Count < 20)
                return "INSUFFICIENT_DATA";

            var recent = prices.TakeLast(20).ToList();
            var trend = CalculateTrend(recent);

            // Double Top/Bottom
            if (IsDoubleTop(recent))
                return "DOUBLE_TOP";
            if (IsDoubleBottom(recent))
                return "DOUBLE_BOTTOM";

            // Head and Shoulders
            if (IsHeadAndShoulders(recent))
                return "HEAD_AND_SHOULDERS";

            // Simple trend
            if (trend > 0.02m)
                return "UPTREND";
            if (trend < -0.02m)
                return "DOWNTREND";

            return "SIDEWAYS";
        }

        private static decimal CalculateTrend(List<decimal> prices)
        {
            if (prices.Count < 2)
                return 0;

            var firstHalf = prices.Take(prices.Count / 2).Average();
            var secondHalf = prices.Skip(prices.Count / 2).Average();

            return (secondHalf - firstHalf) / firstHalf;
        }

        private static bool IsDoubleTop(List<decimal> prices)
        {
            // Simplified detection
            var max1 = prices.Take(prices.Count / 2).Max();
            var max2 = prices.Skip(prices.Count / 2).Max();
            var diff = Math.Abs(max1 - max2) / max1;

            return diff < 0.02m; // Within 2%
        }

        private static bool IsDoubleBottom(List<decimal> prices)
        {
            var min1 = prices.Take(prices.Count / 2).Min();
            var min2 = prices.Skip(prices.Count / 2).Min();
            var diff = Math.Abs(min1 - min2) / min1;

            return diff < 0.02m;
        }

        private static bool IsHeadAndShoulders(List<decimal> prices)
        {
            // Simplified: Look for peak in middle higher than sides
            var third = prices.Count / 3;
            var leftPeak = prices.Take(third).Max();
            var middlePeak = prices.Skip(third).Take(third).Max();
            var rightPeak = prices.Skip(third * 2).Max();

            return middlePeak > leftPeak && middlePeak > rightPeak &&
                   Math.Abs(leftPeak - rightPeak) / leftPeak < 0.05m;
        }

        /// <summary>
        /// Generate Trading Signal based on multiple indicators
        /// </summary>
        public static TradingSignal GenerateSignal(List<decimal> prices)
        {
            if (prices.Count < 30)
                return new TradingSignal { Action = "HOLD", Confidence = 0, Reason = "Insufficient data" };

            var score = 0;
            var reasons = new List<string>();

            // RSI Signal
            var rsi = CalculateRSI(prices);
            if (rsi < 30)
            {
                score += 2;
                reasons.Add($"RSI oversold ({rsi:N1})");
            }
            else if (rsi > 70)
            {
                score -= 2;
                reasons.Add($"RSI overbought ({rsi:N1})");
            }

            // MACD Signal
            var (macd, signal, histogram) = CalculateMACD(prices);
            if (histogram > 0 && macd > signal)
            {
                score += 2;
                reasons.Add("MACD bullish crossover");
            }
            else if (histogram < 0 && macd < signal)
            {
                score -= 2;
                reasons.Add("MACD bearish crossover");
            }

            // Bollinger Bands
            var (upper, middle, lower) = CalculateBollingerBands(prices);
            var currentPrice = prices.Last();
            if (currentPrice < lower)
            {
                score += 1;
                reasons.Add("Price below lower Bollinger Band");
            }
            else if (currentPrice > upper)
            {
                score -= 1;
                reasons.Add("Price above upper Bollinger Band");
            }

            // Moving Average Crossover
            var sma20 = CalculateSMA(prices, 20);
            var sma50 = CalculateSMA(prices, 50);
            if (sma20 > sma50)
            {
                score += 1;
                reasons.Add("Golden cross (SMA20 > SMA50)");
            }
            else if (sma20 < sma50)
            {
                score -= 1;
                reasons.Add("Death cross (SMA20 < SMA50)");
            }

            // Pattern Recognition
            var pattern = DetectPattern(prices);
            if (pattern == "UPTREND")
            {
                score += 1;
                reasons.Add("Uptrend detected");
            }
            else if (pattern == "DOWNTREND")
            {
                score -= 1;
                reasons.Add("Downtrend detected");
            }

            // Determine action and confidence
            string action;
            int confidence;

            if (score >= 4)
            {
                action = "STRONG_BUY";
                confidence = Math.Min(90, 60 + score * 5);
            }
            else if (score >= 2)
            {
                action = "BUY";
                confidence = 55 + score * 5;
            }
            else if (score <= -4)
            {
                action = "STRONG_SELL";
                confidence = Math.Min(90, 60 + Math.Abs(score) * 5);
            }
            else if (score <= -2)
            {
                action = "SELL";
                confidence = 55 + Math.Abs(score) * 5;
            }
            else
            {
                action = "HOLD";
                confidence = 50;
            }

            return new TradingSignal
            {
                Action = action,
                Confidence = confidence,
                Reason = string.Join(", ", reasons),
                RSI = rsi,
                MACD = macd,
                Pattern = pattern,
                Score = score
            };
        }
    }

    public class TradingSignal
    {
        public string Action { get; set; } = "HOLD";
        public int Confidence { get; set; }
        public string Reason { get; set; } = "";
        public decimal RSI { get; set; }
        public decimal MACD { get; set; }
        public string Pattern { get; set; } = "";
        public int Score { get; set; }
    }
}
