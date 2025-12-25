using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace BitkubTrader
{
    /// <summary>
    /// News Analyzer for Thai Financial Websites
    /// วิเคราะห์ข่าวสารจากเว็บไซต์การเงินของไทย
    /// </summary>
    public class NewsAnalyzer
    {
        private readonly HttpClient _httpClient;
        private readonly List<string> _cryptoKeywords;
        private readonly Dictionary<string, List<string>> _sentimentWords;

        public NewsAnalyzer()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            // Crypto-related keywords in Thai
            _cryptoKeywords = new List<string>
            {
                "บิทคอยน์", "bitcoin", "btc",
                "อีเธอเรียม", "ethereum", "eth",
                "คริปโต", "crypto", "cryptocurrency",
                "เหรียญดิจิทัล", "ดิจิทัลแอสเซท",
                "บล็อกเชน", "blockchain",
                "บิทคับ", "bitkub"
            };

            // Sentiment words
            _sentimentWords = new Dictionary<string, List<string>>
            {
                ["POSITIVE"] = new List<string>
                {
                    "ขึ้น", "เพิ่ม", "สูง", "กำไร", "บวก", "ดี", "แข็งแกร่ง",
                    "ราคาสูงขึ้น", "ทำนิวไฮ", "ปรับตัวขึ้น", "recovery", "rally",
                    "bullish", "เติบโต", "พุ่ง", "ทะยาน", "โต", "เพิ่มขึ้น",
                    "ฟื้นตัว", "แข็งค่า", "คึกคัก", "แรง"
                },
                ["NEGATIVE"] = new List<string>
                {
                    "ลง", "ลด", "ต่ำ", "ขาดทุน", "ลบ", "แย่", "อ่อนแอ",
                    "ราคาตก", "ดิ่ง", "ปรับตัวลง", "crash", "drop",
                    "bearish", "หดตัว", "ร่วง", "วูบ", "ตก", "ลดลง",
                    "ทรุด", "อ่อนค่า", "เงียบ", "อ่อนแอ", "แม่", "เสี่ยง"
                },
                ["NEUTRAL"] = new List<string>
                {
                    "คงที่", "ปกติ", "เท่าเดิม", "sideways", "consolidate",
                    "ranging", "รอ", "ยืน", "คงที่", "มิดเทิม"
                }
            };
        }

        /// <summary>
        /// Fetch and analyze crypto news from Thai websites
        /// </summary>
        public async Task<List<NewsArticle>> AnalyzeNewsAsync()
        {
            var articles = new List<NewsArticle>();

            // Fetch from multiple sources
            articles.AddRange(await FetchFromThairath());
            articles.AddRange(await FetchFromSanook());
            articles.AddRange(await FetchFromManagerOnline());

            // Analyze sentiment for each article
            foreach (var article in articles)
            {
                AnalyzeSentiment(article);
            }

            return articles.OrderByDescending(a => a.Timestamp).ToList();
        }

        #region News Sources

        private async Task<List<NewsArticle>> FetchFromThairath()
        {
            var articles = new List<NewsArticle>();

            try
            {
                // Thairath Finance section (example URL - may need adjustment)
                var url = "https://www.thairath.co.th/tags/crypto";
                var html = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var articleNodes = doc.DocumentNode.SelectNodes("//article") ?? new HtmlNodeCollection(null);

                foreach (var node in articleNodes.Take(5))
                {
                    var titleNode = node.SelectSingleNode(".//h2 | .//h3 | .//a[@class='title']");
                    var linkNode = node.SelectSingleNode(".//a[@href]");

                    if (titleNode != null && ContainsCryptoKeyword(titleNode.InnerText))
                    {
                        articles.Add(new NewsArticle
                        {
                            Title = CleanText(titleNode.InnerText),
                            Url = linkNode?.GetAttributeValue("href", ""),
                            Source = "Thairath",
                            Timestamp = DateTime.Now
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching from Thairath: {ex.Message}");
            }

            return articles;
        }

        private async Task<List<NewsArticle>> FetchFromSanook()
        {
            var articles = new List<NewsArticle>();

            try
            {
                var url = "https://www.sanook.com/finance/";
                var html = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var articleNodes = doc.DocumentNode.SelectNodes("//article | //div[contains(@class, 'news-item')]") ?? new HtmlNodeCollection(null);

                foreach (var node in articleNodes.Take(5))
                {
                    var titleNode = node.SelectSingleNode(".//h2 | .//h3 | .//a");
                    var linkNode = node.SelectSingleNode(".//a[@href]");

                    if (titleNode != null && ContainsCryptoKeyword(titleNode.InnerText))
                    {
                        articles.Add(new NewsArticle
                        {
                            Title = CleanText(titleNode.InnerText),
                            Url = linkNode?.GetAttributeValue("href", ""),
                            Source = "Sanook",
                            Timestamp = DateTime.Now
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching from Sanook: {ex.Message}");
            }

            return articles;
        }

        private async Task<List<NewsArticle>> FetchFromManagerOnline()
        {
            var articles = new List<NewsArticle>();

            try
            {
                var url = "https://mgronline.com/stock/";
                var html = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var articleNodes = doc.DocumentNode.SelectNodes("//article | //div[contains(@class, 'article')]") ?? new HtmlNodeCollection(null);

                foreach (var node in articleNodes.Take(5))
                {
                    var titleNode = node.SelectSingleNode(".//h2 | .//h3");
                    var linkNode = node.SelectSingleNode(".//a[@href]");

                    if (titleNode != null && ContainsCryptoKeyword(titleNode.InnerText))
                    {
                        articles.Add(new NewsArticle
                        {
                            Title = CleanText(titleNode.InnerText),
                            Url = linkNode?.GetAttributeValue("href", ""),
                            Source = "Manager Online",
                            Timestamp = DateTime.Now
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching from Manager: {ex.Message}");
            }

            return articles;
        }

        #endregion

        /// <summary>
        /// Analyze sentiment of news article
        /// </summary>
        private void AnalyzeSentiment(NewsArticle article)
        {
            var text = article.Title.ToLower();
            var scores = new Dictionary<string, int>
            {
                ["POSITIVE"] = 0,
                ["NEGATIVE"] = 0,
                ["NEUTRAL"] = 0
            };

            // Count sentiment words
            foreach (var sentiment in _sentimentWords)
            {
                foreach (var word in sentiment.Value)
                {
                    if (text.Contains(word.ToLower()))
                    {
                        scores[sentiment.Key]++;
                    }
                }
            }

            // Determine overall sentiment
            var maxScore = scores.Values.Max();
            if (maxScore == 0)
            {
                article.Sentiment = "NEUTRAL";
                article.SentimentScore = 0;
            }
            else
            {
                article.Sentiment = scores.First(x => x.Value == maxScore).Key;
                article.SentimentScore = maxScore * (article.Sentiment == "POSITIVE" ? 0.3m :
                                                    article.Sentiment == "NEGATIVE" ? -0.3m : 0);
            }

            // Extract keywords
            article.Keywords = string.Join(", ",
                _cryptoKeywords.Where(k => text.Contains(k.ToLower())).Distinct());
        }

        /// <summary>
        /// Get overall market sentiment from recent news
        /// </summary>
        public async Task<MarketSentiment> GetMarketSentimentAsync()
        {
            var articles = await AnalyzeNewsAsync();

            if (articles.Count == 0)
            {
                return new MarketSentiment
                {
                    OverallSentiment = "NEUTRAL",
                    SentimentScore = 0,
                    TotalArticles = 0
                };
            }

            var positive = articles.Count(a => a.Sentiment == "POSITIVE");
            var negative = articles.Count(a => a.Sentiment == "NEGATIVE");
            var neutral = articles.Count(a => a.Sentiment == "NEUTRAL");

            var score = (positive * 1 + negative * -1) / (decimal)articles.Count;

            string overall;
            if (score > 0.3m)
                overall = "VERY_POSITIVE";
            else if (score > 0.1m)
                overall = "POSITIVE";
            else if (score < -0.3m)
                overall = "VERY_NEGATIVE";
            else if (score < -0.1m)
                overall = "NEGATIVE";
            else
                overall = "NEUTRAL";

            return new MarketSentiment
            {
                OverallSentiment = overall,
                SentimentScore = score,
                TotalArticles = articles.Count,
                PositiveCount = positive,
                NegativeCount = negative,
                NeutralCount = neutral,
                RecentArticles = articles.Take(5).ToList()
            };
        }

        /// <summary>
        /// Check if text contains crypto-related keywords
        /// </summary>
        private bool ContainsCryptoKeyword(string text)
        {
            var lowerText = text.ToLower();
            return _cryptoKeywords.Any(keyword => lowerText.Contains(keyword.ToLower()));
        }

        /// <summary>
        /// Clean HTML text
        /// </summary>
        private string CleanText(string text)
        {
            // Remove HTML tags
            text = Regex.Replace(text, "<[^>]+>", "");
            // Remove extra whitespace
            text = Regex.Replace(text, @"\s+", " ");
            // Decode HTML entities
            text = System.Net.WebUtility.HtmlDecode(text);
            return text.Trim();
        }

        /// <summary>
        /// Generate trading recommendation based on news sentiment
        /// </summary>
        public string GetTradingRecommendation(MarketSentiment sentiment)
        {
            if (sentiment.SentimentScore > 0.4m && sentiment.PositiveCount > sentiment.NegativeCount * 2)
                return "STRONG_BUY - News sentiment is very positive";

            if (sentiment.SentimentScore > 0.2m && sentiment.PositiveCount > sentiment.NegativeCount)
                return "BUY - News sentiment is positive";

            if (sentiment.SentimentScore < -0.4m && sentiment.NegativeCount > sentiment.PositiveCount * 2)
                return "STRONG_SELL - News sentiment is very negative";

            if (sentiment.SentimentScore < -0.2m && sentiment.NegativeCount > sentiment.PositiveCount)
                return "SELL - News sentiment is negative";

            return "HOLD - News sentiment is neutral";
        }
    }

    public class NewsArticle
    {
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
        public string Source { get; set; } = "";
        public string Sentiment { get; set; } = "NEUTRAL";
        public decimal SentimentScore { get; set; }
        public string Keywords { get; set; } = "";
        public string Summary { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }

    public class MarketSentiment
    {
        public string OverallSentiment { get; set; } = "NEUTRAL";
        public decimal SentimentScore { get; set; }
        public int TotalArticles { get; set; }
        public int PositiveCount { get; set; }
        public int NegativeCount { get; set; }
        public int NeutralCount { get; set; }
        public List<NewsArticle> RecentArticles { get; set; } = new();
    }
}
