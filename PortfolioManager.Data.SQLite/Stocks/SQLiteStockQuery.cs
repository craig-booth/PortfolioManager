using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Common;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class SQLiteStockQuery: SQLiteQuery, IStockQuery 
    {
        protected internal SQLiteStockQuery(SQLiteStockDatabase database)
            : base(database._Connection, new SQLiteStockEntityCreator(database))
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
            decimal percent = 0.00m;

            var query = EntityQuery.FromTable("RelativeNTAs")
                .Where("[Parent] = @Parent AND [Child] = @Child AND [Date] <= @Date")
                .WithParameter("@Parent", parent)
                .WithParameter("@Child", child)
                .WithParameter("@Date", atDate)
                .OrderBy("[Date] DESC");


            var reader = query.GetFields("[Percentage]");
            if (reader.Read())
                percent = SQLiteUtils.DBToDecimal(reader.GetInt32(0));

            reader.Close();

            return percent;
        }

        public string GetASXCode(Guid id)
        {
            return GetASXCode(id, DateTime.Now);
        }

        public string GetASXCode(Guid id, DateTime atDate)
        {
            var query = EntityQuery.FromTable("Stocks")
                .WithId(id)
                .EffectiveAt(atDate);

            var reader = query.GetFields("[ASXCode]");
            if (!reader.Read())
            {
                reader.Close();
                throw new RecordNotFoundException("");
            }

            string asxCode = reader.GetString(0);
            reader.Close();

            return asxCode;
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
                .Where("[Stock] = @Stock AND [Date] <= @Date")
                .WithParameter("@Stock", stock)
                .WithParameter("@Date", date)
                .OrderBy("[Date] DESC");

            var reader = query.GetFields("Price");
            if (reader.Read())
            {
                price = SQLiteUtils.DBToDecimal(reader.GetInt32(0));
                reader.Close();
                return true;
            }
            else
            {
                price = 0.00m;
                reader.Close();
                return false;
            }
        }

        private bool GetExactClosingPrice(Guid stock, DateTime date, out decimal price)
        {
            var query = EntityQuery.FromTable("StockPrices")
                .Where("[Stock] = @Stock AND[Date] = @Date")
                .WithParameter("@Stock", stock)
                .WithParameter("@Date", date);

            var reader = query.GetFields("Price");
            if (reader.Read())
            {
                price = SQLiteUtils.DBToDecimal(reader.GetInt32(0));
                reader.Close();
                return true;
            }
            else
            {
                price = 0.00m;
                reader.Close();
                return false;
            }

        }

        private SQLiteCommand _GetPricesCommand;
        public Dictionary<DateTime, decimal> GetPrices(Guid stock, DateTime fromDate, DateTime toDate)
        {
            var prices = new Dictionary<DateTime, decimal>();

            if (_GetPricesCommand == null)
            {
                _GetPricesCommand = new SQLiteCommand("SELECT [Date], [Price] FROM [StockPrices] WHERE [Stock] = @Stock AND [Date] BETWEEN @FromDate AND @ToDate ORDER BY [Date]", _Connection);
                _GetPricesCommand.Prepare();
            }

            _GetPricesCommand.Parameters.AddWithValue("@Stock", stock.ToString());
            _GetPricesCommand.Parameters.AddWithValue("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            _GetPricesCommand.Parameters.AddWithValue("@ToDate", toDate.ToString("yyyy-MM-dd"));
            SQLiteDataReader reader = _GetPricesCommand.ExecuteReader();

            while (reader.Read())
            {
                DateTime date = reader.GetDateTime(0);
                decimal price = SQLiteUtils.DBToDecimal(reader.GetInt32(1));

                prices.Add(date, price);
            }

            reader.Close();
            
            return prices;          
        }

        public DateTime GetLatestClosingPrice(Guid stock)
        {
            DateTime date;

            var query = EntityQuery.FromTable("StockPrices")
                .Where("[Stock] = @Stock and [Current] = 0")
                .WithParameter("@Stock", stock)
                .OrderBy("[Date] DESC");

            var reader = query.GetFields("[Date]");
            if (reader.Read())
                date = reader.GetDateTime(0);
            else
                date = DateUtils.NoDate;

            reader.Close();

            return date;
        }

        public bool TradingDay(DateTime date)
        {
            bool result;

            var query = EntityQuery.FromTable("NonTradingDays")
                .Where("[Date] = @Date")
                .WithParameter("@Date", date);

            var reader = query.GetFields("Date");
            result = (!reader.HasRows);

            reader.Close();

            return result;
        }

        public IEnumerable<DateTime> NonTradingDays()
        {
            var nonTradingDays = new List<DateTime>();

            var query = EntityQuery.FromTable("NonTradingDays")
                .OrderBy("Date");

            var reader = query.GetFields("Date");
            while (reader.Read())
            {
                DateTime date = reader.GetDateTime(0);
  
                nonTradingDays.Add(date);
            }

            reader.Close();

            return nonTradingDays;
        }

    }
}
