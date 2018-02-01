using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class SQLiteStockQuery: SQLiteQuery, IStockQuery 
    {
        protected internal SQLiteStockQuery(SqliteTransaction transaction)
            : base(transaction, new SQLiteStockEntityCreator())
        {
        }

        public Stock Get(Guid id, DateTime atDate)
        {
            var query = EntityQuery.FromTable("Stocks")
                .WithId(id)
                .EffectiveAt(atDate);

            return query.CreateEntity<Stock>();
        }

        public IEnumerable<Stock> GetAll()
        {
            var query = EntityQuery.FromTable("Stocks");

            return query.CreateEntities<Stock>();
        }

        
        public IEnumerable<Stock> GetAll(DateTime atDate)
        {
            var query = EntityQuery.FromTable("Stocks")
                        .EffectiveAt(atDate);

            return query.CreateEntities<Stock>();
        }

        public Stock GetByASXCode(string asxCode, DateTime atDate)
        {
            Stock stock;

            if (TryGetByASXCode(asxCode, atDate, out stock))
                return stock;
            else
                throw new RecordNotFoundException("");
        }

        public bool TryGetByASXCode(string asxCode, DateTime atDate, out Stock stock)
        {
            var query = EntityQuery.FromTable("Stocks")
                .Where("[ASXCode] = @ASXCode")
                .WithParameter("@ASXCode", asxCode)
                .EffectiveAt(atDate);

            stock = query.CreateEntity<Stock>();

            return (stock != null);
        }

        public IEnumerable<Stock> GetChildStocks(Guid parent, DateTime atDate)
        {
            var query = EntityQuery.FromTable("Stocks")
                .Where("[Parent] = @Parent")
                .WithParameter("@Parent", parent)
                .EffectiveAt(atDate);

            return query.CreateEntities<Stock>();
        }

        public RelativeNTA GetRelativeNTA(Guid parent, Guid child, DateTime atDate)
        {
            var query = EntityQuery.FromTable("RelativeNTAs")
                .Where("[Parent] = @Parent AND [Child] = @Child AND [Date] = @Date")
                .WithParameter("@Parent", parent)
                .WithParameter("@Child", child)
                .WithParameter("@Date", atDate);

            return query.CreateEntity<RelativeNTA>();
        }

        public IEnumerable<RelativeNTA> GetRelativeNTAs(Guid parent, Guid child)
        {
            var query = EntityQuery.FromTable("ReleativeNTAs")
                .Where("[Parent] = @Parent AND [Child] = @Child")
                .WithParameter("@Parent", parent)
                .WithParameter("@Child", child);

            return query.CreateEntities<RelativeNTA>();
        }

        public decimal PercentOfParentCost(Guid parent, Guid child, DateTime atDate)
        {
            decimal percent;

            var query = EntityQuery.FromTable("RelativeNTAs")
                .Select("Percentage")
                .Where("[Parent] = @Parent AND [Child] = @Child AND [Date] <= @Date")
                .WithParameter("@Parent", parent)
                .WithParameter("@Child", child)
                .WithParameter("@Date", atDate)
                .OrderBy("[Date] DESC");

            
            query.ExecuteScalar(out percent);
            
            return percent; 
        }

        public string GetASXCode(Guid id)
        {
            return GetASXCode(id, DateTime.Now);
        }

        public string GetASXCode(Guid id, DateTime atDate)
        {
            string asxCode;

            var query = EntityQuery.FromTable("Stocks")
                .Select("ASXCode")
                .WithId(id)
                .EffectiveAt(atDate);

            
            if (query.ExecuteScalar(out asxCode))
            {
                return asxCode;
            }
            else
            {
                throw new RecordNotFoundException("");
            }
        }

        public decimal GetPrice(Guid stock, DateTime date)
        {
            return GetPrice(stock, date, false);
        }

        public bool TryGetPrice(Guid stock, DateTime date, out decimal price)
        {
            return TryGetPrice(stock, date, out price, false);
        }

        public decimal GetPrice(Guid stock, DateTime date, bool exact)
        {
            decimal closingPrice;

            if (TryGetPrice(stock, date, out closingPrice, exact))
                return closingPrice;
            else
                return 0.00m;
        }

        public bool TryGetPrice(Guid stock, DateTime date, out decimal price, bool exact)
        {
            if (exact)
                return GetExactClosingPrice(stock, date, out price);
            else
                return GetClosingPrice(stock, date, out price);
        }

        private bool GetClosingPrice(Guid stock, DateTime date, out decimal price)
        {
            var query = EntityQuery.FromTable("StockPrices")
                .Select("Price")
                .Where("[Stock] = @Stock AND [Date] <= @Date")
                .WithParameter("@Stock", stock)
                .WithParameter("@Date", date)
                .OrderBy("[Date] DESC");

            return query.ExecuteScalar(out price);
        }

        private bool GetExactClosingPrice(Guid stock, DateTime date, out decimal price)
        {
            var query = EntityQuery.FromTable("StockPrices")
                .Select("Price")
                .Where("[Stock] = @Stock AND[Date] = @Date")
                .WithParameter("@Stock", stock)
                .WithParameter("@Date", date);

            return query.ExecuteScalar(out price);
        }

        public Dictionary<DateTime, decimal> GetPrices(Guid stock, DateTime fromDate, DateTime toDate)
        {
            var prices = new Dictionary<DateTime, decimal>();

            var query = EntityQuery.FromTable("StockPrices")
                .Select("[Date], [Price]")
                .Where("[Stock] = @Stock and [Date] between @FromDate and @ToDate")
                .WithParameter("@Stock", stock)
                .WithParameter("@FromDate", fromDate)
                .WithParameter("@ToDate", toDate)
                .OrderBy("[Date]");

            using (var reader = query.Execute())
            {
                while (reader.Read())
                {
                    DateTime date = reader.GetDateTime(0);
                    decimal price = SQLiteUtils.DBToDecimal(reader.GetInt32(1));

                    prices.Add(date, price);
                }
            }
            
            return prices;          
        }

        public DateTime GetLatestClosingPrice(Guid stock)
        {
            var query = EntityQuery.FromTable("StockPrices")
                .Select("Date")
                .Where("[Stock] = @Stock and [Current] = 'N'")
                .WithParameter("@Stock", stock)
                .OrderBy("[Date] DESC");

            DateTime date;
            query.ExecuteScalar(out date);

            return date;
        }

        public bool TradingDay(DateTime date)
        {
            var query = EntityQuery.FromTable("NonTradingDays")
                .Select("Date")
                .Where("[Date] = @Date")
                .WithParameter("@Date", date);

            using (var reader = query.ExecuteSingle())
            {
                return !reader.HasRows;
            }
        }

        public IEnumerable<DateTime> NonTradingDays()
        {
            var nonTradingDays = new List<DateTime>();

            var query = EntityQuery.FromTable("NonTradingDays")
                .Select("Date")
                .OrderBy("Date");

            using (var reader = query.Execute())
            {
                while (reader.Read())
                {
                    DateTime date = reader.GetDateTime(0);

                    nonTradingDays.Add(date);
                }
            }

            return nonTradingDays;
        }

    }
}
