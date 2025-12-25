using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace BitkubTrader
{
    /// <summary>
    /// SQLite Database Manager for Trading Statistics
    /// </summary>
    public class DatabaseManager : IDisposable
    {
        private readonly string _connectionString;
        private SqliteConnection? _connection;

        public DatabaseManager(string dbPath = "bitkub_trading.db")
        {
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase().Wait();
        }

        private async Task InitializeDatabase()
        {
            _connection = new SqliteConnection(_connectionString);
            await _connection.OpenAsync();

            // Create tables
            await CreateTablesAsync();
        }

        private async Task CreateTablesAsync()
        {
            var commands = new[]
            {
                // Price History Table
                @"CREATE TABLE IF NOT EXISTS price_history (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    symbol TEXT NOT NULL,
                    price REAL NOT NULL,
                    high_24h REAL,
                    low_24h REAL,
                    volume REAL,
                    percent_change REAL,
                    timestamp INTEGER NOT NULL
                )",

                // Trading Signals Table
                @"CREATE TABLE IF NOT EXISTS trading_signals (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    symbol TEXT NOT NULL,
                    action TEXT NOT NULL,
                    confidence INTEGER NOT NULL,
                    rsi REAL,
                    macd REAL,
                    pattern TEXT,
                    reason TEXT,
                    price REAL NOT NULL,
                    timestamp INTEGER NOT NULL
                )",

                // Orders Table
                @"CREATE TABLE IF NOT EXISTS orders (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    order_id INTEGER NOT NULL,
                    symbol TEXT NOT NULL,
                    side TEXT NOT NULL,
                    type TEXT NOT NULL,
                    amount REAL NOT NULL,
                    rate REAL NOT NULL,
                    fee REAL,
                    status TEXT NOT NULL,
                    filled_amount REAL DEFAULT 0,
                    profit_loss REAL DEFAULT 0,
                    timestamp INTEGER NOT NULL
                )",

                // Trades Table
                @"CREATE TABLE IF NOT EXISTS trades (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    symbol TEXT NOT NULL,
                    side TEXT NOT NULL,
                    entry_price REAL NOT NULL,
                    exit_price REAL,
                    amount REAL NOT NULL,
                    profit_loss REAL,
                    profit_percent REAL,
                    fees REAL,
                    duration_seconds INTEGER,
                    strategy TEXT,
                    entry_timestamp INTEGER NOT NULL,
                    exit_timestamp INTEGER
                )",

                // Performance Metrics Table
                @"CREATE TABLE IF NOT EXISTS performance (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    symbol TEXT NOT NULL,
                    total_trades INTEGER DEFAULT 0,
                    winning_trades INTEGER DEFAULT 0,
                    losing_trades INTEGER DEFAULT 0,
                    total_profit REAL DEFAULT 0,
                    total_loss REAL DEFAULT 0,
                    win_rate REAL DEFAULT 0,
                    avg_profit REAL DEFAULT 0,
                    avg_loss REAL DEFAULT 0,
                    largest_win REAL DEFAULT 0,
                    largest_loss REAL DEFAULT 0,
                    sharpe_ratio REAL DEFAULT 0,
                    max_drawdown REAL DEFAULT 0,
                    timestamp INTEGER NOT NULL
                )",

                // News Sentiment Table
                @"CREATE TABLE IF NOT EXISTS news_sentiment (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    title TEXT NOT NULL,
                    url TEXT,
                    source TEXT,
                    sentiment TEXT NOT NULL,
                    sentiment_score REAL,
                    keywords TEXT,
                    summary TEXT,
                    timestamp INTEGER NOT NULL
                )",

                // AI Bot Sessions Table
                @"CREATE TABLE IF NOT EXISTS bot_sessions (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    session_id TEXT NOT NULL UNIQUE,
                    mode TEXT NOT NULL,
                    symbol TEXT NOT NULL,
                    initial_balance REAL NOT NULL,
                    current_balance REAL NOT NULL,
                    total_trades INTEGER DEFAULT 0,
                    profit_loss REAL DEFAULT 0,
                    status TEXT NOT NULL,
                    start_timestamp INTEGER NOT NULL,
                    end_timestamp INTEGER
                )",

                // Create indexes
                "CREATE INDEX IF NOT EXISTS idx_price_symbol ON price_history(symbol)",
                "CREATE INDEX IF NOT EXISTS idx_price_timestamp ON price_history(timestamp)",
                "CREATE INDEX IF NOT EXISTS idx_signals_symbol ON trading_signals(symbol)",
                "CREATE INDEX IF NOT EXISTS idx_signals_timestamp ON trading_signals(timestamp)",
                "CREATE INDEX IF NOT EXISTS idx_orders_symbol ON orders(symbol)",
                "CREATE INDEX IF NOT EXISTS idx_trades_symbol ON trades(symbol)",
                "CREATE INDEX IF NOT EXISTS idx_news_timestamp ON news_sentiment(timestamp)"
            };

            foreach (var sql in commands)
            {
                using var cmd = new SqliteCommand(sql, _connection);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        #region Price History

        public async Task SavePriceAsync(string symbol, decimal price, decimal high24h, decimal low24h,
            decimal volume, decimal percentChange)
        {
            var sql = @"INSERT INTO price_history (symbol, price, high_24h, low_24h, volume, percent_change, timestamp)
                       VALUES (@symbol, @price, @high24h, @low24h, @volume, @percentChange, @timestamp)";

            using var cmd = new SqliteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@symbol", symbol);
            cmd.Parameters.AddWithValue("@price", (double)price);
            cmd.Parameters.AddWithValue("@high24h", (double)high24h);
            cmd.Parameters.AddWithValue("@low24h", (double)low24h);
            cmd.Parameters.AddWithValue("@volume", (double)volume);
            cmd.Parameters.AddWithValue("@percentChange", (double)percentChange);
            cmd.Parameters.AddWithValue("@timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<decimal>> GetRecentPricesAsync(string symbol, int count = 50)
        {
            var sql = @"SELECT price FROM price_history
                       WHERE symbol = @symbol
                       ORDER BY timestamp DESC
                       LIMIT @count";

            var prices = new List<decimal>();

            using var cmd = new SqliteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@symbol", symbol);
            cmd.Parameters.AddWithValue("@count", count);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                prices.Add((decimal)reader.GetDouble(0));
            }

            prices.Reverse(); // Oldest to newest
            return prices;
        }

        #endregion

        #region Trading Signals

        public async Task SaveSignalAsync(string symbol, TradingSignal signal, decimal currentPrice)
        {
            var sql = @"INSERT INTO trading_signals
                       (symbol, action, confidence, rsi, macd, pattern, reason, price, timestamp)
                       VALUES (@symbol, @action, @confidence, @rsi, @macd, @pattern, @reason, @price, @timestamp)";

            using var cmd = new SqliteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@symbol", symbol);
            cmd.Parameters.AddWithValue("@action", signal.Action);
            cmd.Parameters.AddWithValue("@confidence", signal.Confidence);
            cmd.Parameters.AddWithValue("@rsi", (double)signal.RSI);
            cmd.Parameters.AddWithValue("@macd", (double)signal.MACD);
            cmd.Parameters.AddWithValue("@pattern", signal.Pattern);
            cmd.Parameters.AddWithValue("@reason", signal.Reason);
            cmd.Parameters.AddWithValue("@price", (double)currentPrice);
            cmd.Parameters.AddWithValue("@timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            await cmd.ExecuteNonQueryAsync();
        }

        #endregion

        #region Orders & Trades

        public async Task SaveOrderAsync(long orderId, string symbol, string side, string type,
            decimal amount, decimal rate, decimal fee, string status = "OPEN")
        {
            var sql = @"INSERT INTO orders (order_id, symbol, side, type, amount, rate, fee, status, timestamp)
                       VALUES (@orderId, @symbol, @side, @type, @amount, @rate, @fee, @status, @timestamp)";

            using var cmd = new SqliteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@orderId", orderId);
            cmd.Parameters.AddWithValue("@symbol", symbol);
            cmd.Parameters.AddWithValue("@side", side);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@amount", (double)amount);
            cmd.Parameters.AddWithValue("@rate", (double)rate);
            cmd.Parameters.AddWithValue("@fee", (double)fee);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task SaveTradeAsync(string symbol, string side, decimal entryPrice, decimal exitPrice,
            decimal amount, decimal profitLoss, decimal fees, int durationSeconds, string strategy)
        {
            var profitPercent = (exitPrice - entryPrice) / entryPrice * 100;
            if (side == "SELL") profitPercent *= -1;

            var sql = @"INSERT INTO trades
                       (symbol, side, entry_price, exit_price, amount, profit_loss, profit_percent,
                        fees, duration_seconds, strategy, entry_timestamp, exit_timestamp)
                       VALUES (@symbol, @side, @entryPrice, @exitPrice, @amount, @profitLoss, @profitPercent,
                               @fees, @duration, @strategy, @entryTime, @exitTime)";

            using var cmd = new SqliteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@symbol", symbol);
            cmd.Parameters.AddWithValue("@side", side);
            cmd.Parameters.AddWithValue("@entryPrice", (double)entryPrice);
            cmd.Parameters.AddWithValue("@exitPrice", (double)exitPrice);
            cmd.Parameters.AddWithValue("@amount", (double)amount);
            cmd.Parameters.AddWithValue("@profitLoss", (double)profitLoss);
            cmd.Parameters.AddWithValue("@profitPercent", (double)profitPercent);
            cmd.Parameters.AddWithValue("@fees", (double)fees);
            cmd.Parameters.AddWithValue("@duration", durationSeconds);
            cmd.Parameters.AddWithValue("@strategy", strategy);
            cmd.Parameters.AddWithValue("@entryTime", DateTimeOffset.UtcNow.ToUnixTimeSeconds() - durationSeconds);
            cmd.Parameters.AddWithValue("@exitTime", DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            await cmd.ExecuteNonQueryAsync();
        }

        #endregion

        #region Performance Analytics

        public async Task<PerformanceMetrics> GetPerformanceAsync(string symbol)
        {
            var sql = @"SELECT
                           COUNT(*) as total_trades,
                           SUM(CASE WHEN profit_loss > 0 THEN 1 ELSE 0 END) as winning_trades,
                           SUM(CASE WHEN profit_loss < 0 THEN 1 ELSE 0 END) as losing_trades,
                           SUM(CASE WHEN profit_loss > 0 THEN profit_loss ELSE 0 END) as total_profit,
                           SUM(CASE WHEN profit_loss < 0 THEN ABS(profit_loss) ELSE 0 END) as total_loss,
                           AVG(CASE WHEN profit_loss > 0 THEN profit_loss ELSE NULL END) as avg_profit,
                           AVG(CASE WHEN profit_loss < 0 THEN profit_loss ELSE NULL END) as avg_loss,
                           MAX(profit_loss) as largest_win,
                           MIN(profit_loss) as largest_loss
                       FROM trades
                       WHERE symbol = @symbol AND exit_price IS NOT NULL";

            using var cmd = new SqliteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@symbol", symbol);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var totalTrades = reader.GetInt32(0);
                var winningTrades = reader.GetInt32(1);
                var losingTrades = reader.GetInt32(2);
                var totalProfit = reader.IsDBNull(3) ? 0 : (decimal)reader.GetDouble(3);
                var totalLoss = reader.IsDBNull(4) ? 0 : (decimal)reader.GetDouble(4);
                var avgProfit = reader.IsDBNull(5) ? 0 : (decimal)reader.GetDouble(5);
                var avgLoss = reader.IsDBNull(6) ? 0 : (decimal)reader.GetDouble(6);
                var largestWin = reader.IsDBNull(7) ? 0 : (decimal)reader.GetDouble(7);
                var largestLoss = reader.IsDBNull(8) ? 0 : (decimal)reader.GetDouble(8);

                return new PerformanceMetrics
                {
                    Symbol = symbol,
                    TotalTrades = totalTrades,
                    WinningTrades = winningTrades,
                    LosingTrades = losingTrades,
                    TotalProfit = totalProfit,
                    TotalLoss = totalLoss,
                    WinRate = totalTrades > 0 ? (decimal)winningTrades / totalTrades * 100 : 0,
                    AvgProfit = avgProfit,
                    AvgLoss = avgLoss,
                    LargestWin = largestWin,
                    LargestLoss = largestLoss,
                    NetProfit = totalProfit - totalLoss,
                    ProfitFactor = totalLoss > 0 ? totalProfit / totalLoss : 0
                };
            }

            return new PerformanceMetrics { Symbol = symbol };
        }

        public async Task SavePerformanceSnapshotAsync(PerformanceMetrics metrics)
        {
            var sql = @"INSERT INTO performance
                       (symbol, total_trades, winning_trades, losing_trades, total_profit, total_loss,
                        win_rate, avg_profit, avg_loss, largest_win, largest_loss, timestamp)
                       VALUES (@symbol, @totalTrades, @winningTrades, @losingTrades, @totalProfit, @totalLoss,
                               @winRate, @avgProfit, @avgLoss, @largestWin, @largestLoss, @timestamp)";

            using var cmd = new SqliteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@symbol", metrics.Symbol);
            cmd.Parameters.AddWithValue("@totalTrades", metrics.TotalTrades);
            cmd.Parameters.AddWithValue("@winningTrades", metrics.WinningTrades);
            cmd.Parameters.AddWithValue("@losingTrades", metrics.LosingTrades);
            cmd.Parameters.AddWithValue("@totalProfit", (double)metrics.TotalProfit);
            cmd.Parameters.AddWithValue("@totalLoss", (double)metrics.TotalLoss);
            cmd.Parameters.AddWithValue("@winRate", (double)metrics.WinRate);
            cmd.Parameters.AddWithValue("@avgProfit", (double)metrics.AvgProfit);
            cmd.Parameters.AddWithValue("@avgLoss", (double)metrics.AvgLoss);
            cmd.Parameters.AddWithValue("@largestWin", (double)metrics.LargestWin);
            cmd.Parameters.AddWithValue("@largestLoss", (double)metrics.LargestLoss);
            cmd.Parameters.AddWithValue("@timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            await cmd.ExecuteNonQueryAsync();
        }

        #endregion

        #region News Sentiment

        public async Task SaveNewsAsync(string title, string url, string source, string sentiment,
            decimal sentimentScore, string keywords, string summary)
        {
            var sql = @"INSERT INTO news_sentiment
                       (title, url, source, sentiment, sentiment_score, keywords, summary, timestamp)
                       VALUES (@title, @url, @source, @sentiment, @score, @keywords, @summary, @timestamp)";

            using var cmd = new SqliteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@url", url ?? "");
            cmd.Parameters.AddWithValue("@source", source);
            cmd.Parameters.AddWithValue("@sentiment", sentiment);
            cmd.Parameters.AddWithValue("@score", (double)sentimentScore);
            cmd.Parameters.AddWithValue("@keywords", keywords);
            cmd.Parameters.AddWithValue("@summary", summary);
            cmd.Parameters.AddWithValue("@timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<string> GetRecentSentimentAsync(int hoursAgo = 24)
        {
            var since = DateTimeOffset.UtcNow.AddHours(-hoursAgo).ToUnixTimeSeconds();
            var sql = @"SELECT sentiment, COUNT(*) as count
                       FROM news_sentiment
                       WHERE timestamp > @since
                       GROUP BY sentiment
                       ORDER BY count DESC
                       LIMIT 1";

            using var cmd = new SqliteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@since", since);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetString(0);
            }

            return "NEUTRAL";
        }

        #endregion

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }

    public class PerformanceMetrics
    {
        public string Symbol { get; set; } = "";
        public int TotalTrades { get; set; }
        public int WinningTrades { get; set; }
        public int LosingTrades { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal TotalLoss { get; set; }
        public decimal WinRate { get; set; }
        public decimal AvgProfit { get; set; }
        public decimal AvgLoss { get; set; }
        public decimal LargestWin { get; set; }
        public decimal LargestLoss { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitFactor { get; set; }
    }
}
