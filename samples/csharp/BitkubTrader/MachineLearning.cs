using System;
using System.Collections.Generic;
using System.Linq;

namespace BitkubTrader
{
    /// <summary>
    /// Machine Learning Price Prediction & Pattern Recognition
    /// 🧠 AI ที่เรียนรู้จากข้อมูลและทำนายราคา
    /// </summary>
    public class MachineLearning
    {
        #region Linear Regression

        /// <summary>
        /// Simple Linear Regression for trend prediction
        /// </summary>
        public static LinearRegressionResult LinearRegression(List<decimal> prices)
        {
            int n = prices.Count;
            if (n < 10)
                return new LinearRegressionResult();

            // Calculate linear regression y = mx + b
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            for (int i = 0; i < n; i++)
            {
                double x = i;
                double y = (double)prices[i];

                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
            }

            double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            double intercept = (sumY - slope * sumX) / n;

            // Predict next values
            var predictions = new List<decimal>();
            for (int i = 0; i < 10; i++)
            {
                double predicted = slope * (n + i) + intercept;
                predictions.Add((decimal)predicted);
            }

            // Calculate R-squared
            double meanY = sumY / n;
            double ssTotal = 0, ssResidual = 0;

            for (int i = 0; i < n; i++)
            {
                double y = (double)prices[i];
                double predicted = slope * i + intercept;

                ssTotal += Math.Pow(y - meanY, 2);
                ssResidual += Math.Pow(y - predicted, 2);
            }

            double rSquared = 1 - (ssResidual / ssTotal);

            return new LinearRegressionResult
            {
                Slope = (decimal)slope,
                Intercept = (decimal)intercept,
                RSquared = (decimal)rSquared,
                Predictions = predictions,
                Trend = slope > 0 ? "UPTREND" : slope < 0 ? "DOWNTREND" : "FLAT"
            };
        }

        #endregion

        #region Moving Average Convergence

        /// <summary>
        /// Predict crossover points using MA
        /// </summary>
        public static MAcrossoverPrediction PredictMACrossover(List<decimal> prices)
        {
            var sma20 = TechnicalAnalysis.CalculateSMA(prices, 20);
            var sma50 = TechnicalAnalysis.CalculateSMA(prices, 50);
            var ema12 = TechnicalAnalysis.CalculateEMA(prices, 12);
            var ema26 = TechnicalAnalysis.CalculateEMA(prices, 26);

            // Calculate convergence/divergence rate
            var smaDiff = sma20 - sma50;
            var emaDiff = ema12 - ema26;

            // Estimate bars until crossover
            int? barsUntilCross = null;
            string expectedCross = "NONE";

            if (smaDiff > 0 && smaDiff < sma50 * 0.01m) // Within 1%
            {
                barsUntilCross = (int)Math.Abs(smaDiff / ((smaDiff / 20)));
                expectedCross = "DEATH_CROSS_IMMINENT";
            }
            else if (smaDiff < 0 && Math.Abs(smaDiff) < sma50 * 0.01m)
            {
                barsUntilCross = (int)Math.Abs(smaDiff / ((smaDiff / 20)));
                expectedCross = "GOLDEN_CROSS_IMMINENT";
            }

            return new MAcrossoverPrediction
            {
                SMA20 = sma20,
                SMA50 = sma50,
                Difference = smaDiff,
                ExpectedCrossover = expectedCross,
                EstimatedBars = barsUntilCross
            };
        }

        #endregion

        #region Pattern Recognition with ML

        /// <summary>
        /// Use ML to recognize chart patterns
        /// </summary>
        public static PatternRecognition RecognizePatterns(List<decimal> prices)
        {
            var patterns = new List<string>();
            var confidence = new Dictionary<string, int>();

            // Head and Shoulders
            if (DetectHeadAndShoulders(prices, out int hsConfidence))
            {
                patterns.Add("HEAD_AND_SHOULDERS");
                confidence["HEAD_AND_SHOULDERS"] = hsConfidence;
            }

            // Double Top/Bottom
            if (DetectDoubleTopBottom(prices, out string dtbType, out int dtbConfidence))
            {
                patterns.Add(dtbType);
                confidence[dtbType] = dtbConfidence;
            }

            // Triangle patterns
            if (DetectTriangle(prices, out string triangleType, out int triConfidence))
            {
                patterns.Add(triangleType);
                confidence[triangleType] = triConfidence;
            }

            // Cup and Handle
            if (DetectCupAndHandle(prices, out int cupConfidence))
            {
                patterns.Add("CUP_AND_HANDLE");
                confidence["CUP_AND_HANDLE"] = cupConfidence;
            }

            // Wedge patterns
            if (DetectWedge(prices, out string wedgeType, out int wedgeConfidence))
            {
                patterns.Add(wedgeType);
                confidence[wedgeType] = wedgeConfidence;
            }

            return new PatternRecognition
            {
                Patterns = patterns,
                Confidence = confidence,
                MostLikelyPattern = confidence.Count > 0 ?
                    confidence.OrderByDescending(kv => kv.Value).First().Key : "NONE"
            };
        }

        private static bool DetectHeadAndShoulders(List<decimal> prices, out int confidence)
        {
            confidence = 0;

            if (prices.Count < 30) return false;

            // Find three peaks
            var pivots = AdvancedAnalysis.FindPivotPoints(prices.TakeLast(30).ToList(), 3);
            var peaks = pivots.Where(p => p.Type == "HIGH").ToList();

            if (peaks.Count >= 3)
            {
                var leftShoulder = peaks[0].Price;
                var head = peaks[1].Price;
                var rightShoulder = peaks[2].Price;

                // Head should be higher than both shoulders
                if (head > leftShoulder && head > rightShoulder)
                {
                    // Shoulders should be approximately equal
                    var shoulderDiff = Math.Abs(leftShoulder - rightShoulder) / leftShoulder;

                    if (shoulderDiff < 0.05m) // Within 5%
                    {
                        confidence = 85;
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool DetectDoubleTopBottom(List<decimal> prices, out string type, out int confidence)
        {
            type = "NONE";
            confidence = 0;

            if (prices.Count < 20) return false;

            var pivots = AdvancedAnalysis.FindPivotPoints(prices.TakeLast(20).ToList(), 2);

            // Double Top
            var highs = pivots.Where(p => p.Type == "HIGH").ToList();
            if (highs.Count >= 2)
            {
                var diff = Math.Abs(highs[^1].Price - highs[^2].Price) / highs[^1].Price;
                if (diff < 0.02m) // Within 2%
                {
                    type = "DOUBLE_TOP";
                    confidence = 80;
                    return true;
                }
            }

            // Double Bottom
            var lows = pivots.Where(p => p.Type == "LOW").ToList();
            if (lows.Count >= 2)
            {
                var diff = Math.Abs(lows[^1].Price - lows[^2].Price) / lows[^1].Price;
                if (diff < 0.02m)
                {
                    type = "DOUBLE_BOTTOM";
                    confidence = 80;
                    return true;
                }
            }

            return false;
        }

        private static bool DetectTriangle(List<decimal> prices, out string type, out int confidence)
        {
            type = "NONE";
            confidence = 0;

            if (prices.Count < 30) return false;

            var highs = new List<decimal>();
            var lows = new List<decimal>();

            // Get highs and lows over time
            for (int i = 5; i < prices.Count; i += 5)
            {
                highs.Add(prices.Skip(i - 5).Take(5).Max());
                lows.Add(prices.Skip(i - 5).Take(5).Min());
            }

            var highSlope = CalculateSlope(highs);
            var lowSlope = CalculateSlope(lows);

            // Ascending Triangle: highs flat, lows rising
            if (Math.Abs(highSlope) < 0.001m && lowSlope > 0.001m)
            {
                type = "ASCENDING_TRIANGLE";
                confidence = 75;
                return true;
            }

            // Descending Triangle: lows flat, highs falling
            if (Math.Abs(lowSlope) < 0.001m && highSlope < -0.001m)
            {
                type = "DESCENDING_TRIANGLE";
                confidence = 75;
                return true;
            }

            // Symmetrical Triangle: both converging
            if (highSlope < -0.001m && lowSlope > 0.001m)
            {
                type = "SYMMETRICAL_TRIANGLE";
                confidence = 70;
                return true;
            }

            return false;
        }

        private static bool DetectCupAndHandle(List<decimal> prices, out int confidence)
        {
            confidence = 0;

            if (prices.Count < 40) return false;

            var firstHalf = prices.Take(prices.Count / 2).ToList();
            var secondHalf = prices.Skip(prices.Count / 2).ToList();

            // Cup: U-shaped bottom
            var cupLow = firstHalf.Min();
            var cupLeftHigh = firstHalf.First();
            var cupRightHigh = firstHalf.Last();

            // Handle: slight pullback
            var handleLow = secondHalf.Min();
            var handleHigh = secondHalf.Max();

            // Check if pattern matches
            if (cupLeftHigh > cupLow && cupRightHigh > cupLow &&
                handleLow > cupLow && handleHigh < cupRightHigh)
            {
                confidence = 70;
                return true;
            }

            return false;
        }

        private static bool DetectWedge(List<decimal> prices, out string type, out int confidence)
        {
            type = "NONE";
            confidence = 0;

            if (prices.Count < 20) return false;

            var highs = new List<decimal>();
            var lows = new List<decimal>();

            for (int i = 3; i < prices.Count; i += 3)
            {
                highs.Add(prices.Skip(i - 3).Take(3).Max());
                lows.Add(prices.Skip(i - 3).Take(3).Min());
            }

            var highSlope = CalculateSlope(highs);
            var lowSlope = CalculateSlope(lows);

            // Rising Wedge: both rising but converging
            if (highSlope > 0 && lowSlope > 0 && lowSlope > highSlope)
            {
                type = "RISING_WEDGE";
                confidence = 70;
                return true;
            }

            // Falling Wedge: both falling but converging
            if (highSlope < 0 && lowSlope < 0 && highSlope < lowSlope)
            {
                type = "FALLING_WEDGE";
                confidence = 70;
                return true;
            }

            return false;
        }

        private static decimal CalculateSlope(List<decimal> values)
        {
            if (values.Count < 2) return 0;

            int n = values.Count;
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            for (int i = 0; i < n; i++)
            {
                sumX += i;
                sumY += (double)values[i];
                sumXY += i * (double)values[i];
                sumX2 += i * i;
            }

            double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            return (decimal)slope;
        }

        #endregion

        #region Price Prediction

        /// <summary>
        /// Predict next N prices using weighted average of multiple methods
        /// </summary>
        public static List<decimal> PredictNextPrices(List<decimal> prices, int count = 5)
        {
            // Method 1: Linear Regression
            var linearPred = LinearRegression(prices);

            // Method 2: EMA projection
            var ema = TechnicalAnalysis.CalculateEMA(prices, 12);
            var emaChange = ema - prices[^2];

            // Method 3: Pattern-based
            var pattern = TechnicalAnalysis.DetectPattern(prices);

            var predictions = new List<decimal>();

            for (int i = 0; i < count; i++)
            {
                // Weighted combination
                var linearVal = linearPred.Predictions[i];
                var emaVal = ema + (emaChange * (i + 1));

                // Pattern adjustment
                var patternFactor = pattern switch
                {
                    "UPTREND" => 1.005m,
                    "DOWNTREND" => 0.995m,
                    _ => 1.0m
                };

                var predicted = (linearVal * 0.5m + emaVal * 0.5m) * patternFactor;
                predictions.Add(predicted);
            }

            return predictions;
        }

        #endregion
    }

    #region Data Models

    public class LinearRegressionResult
    {
        public decimal Slope { get; set; }
        public decimal Intercept { get; set; }
        public decimal RSquared { get; set; }
        public List<decimal> Predictions { get; set; } = new();
        public string Trend { get; set; } = "";
    }

    public class MAcrossoverPrediction
    {
        public decimal SMA20 { get; set; }
        public decimal SMA50 { get; set; }
        public decimal Difference { get; set; }
        public string ExpectedCrossover { get; set; } = "";
        public int? EstimatedBars { get; set; }
    }

    public class PatternRecognition
    {
        public List<string> Patterns { get; set; } = new();
        public Dictionary<string, int> Confidence { get; set; } = new();
        public string MostLikelyPattern { get; set; } = "";
    }

    #endregion
}
