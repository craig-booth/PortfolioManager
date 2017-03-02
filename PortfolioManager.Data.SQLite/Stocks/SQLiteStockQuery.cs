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
    class SQLiteStockQuery: IStockQuery 
    {
        protected SQLiteDatabase _Database;
        protected SQLiteConnection _Connection;

        protected internal SQLiteStockQuery(SQLiteDatabase database)
        {
            _Database = database;
            _Connection = database._Connection;
        }

        private SQLiteCommand _GetStockByIdandDate;
        public Stock Get(Guid id, DateTime atDate)
        {
            if (_GetStockByIdandDate == null)
            {
                _GetStockByIdandDate = new SQLiteCommand("SELECT * FROM [Stocks] WHERE [Id] = @Id AND @Date BETWEEN [FromDate] AND [ToDate]", _Connection);
                _GetStockByIdandDate.Prepare();
            }

            _GetStockByIdandDate.Parameters.AddWithValue("@Id", id.ToString());
            _GetStockByIdandDate.Parameters.AddWithValue("@Date", atDate.ToString("yyyy-MM-dd"));

            var stock = GetStock(_GetStockByIdandDate);
            if (stock == null)
                throw new RecordNotFoundException("");
            return stock;
        }

        private SQLiteCommand _GetAllStocks;
        public IReadOnlyCollection<Stock> GetAll()
        {
            var list = new List<Stock>();

            if (_GetAllStocks == null)
            {
                _GetAllStocks = new SQLiteCommand("SELECT * FROM [Stocks]", _Connection);
                _GetAllStocks.Prepare();
            }

            SQLiteDataReader reader = _GetAllStocks.ExecuteReader();
            while (reader.Read())
            {
                Stock stock = SQLiteStockEntityCreator.CreateStock(_Database as SQLiteStockDatabase, reader);
                list.Add(stock);
            }
            reader.Close();

            return list.AsReadOnly();
        }

        private SQLiteCommand _GetAllStocksAtDate;
        public IReadOnlyCollection<Stock> GetAll(DateTime atDate)
        {
            var list = new List<Stock>();

            if (_GetAllStocksAtDate == null)
            {
                _GetAllStocksAtDate = new SQLiteCommand("SELECT * FROM [Stocks] WHERE @Date BETWEEN [FromDate] AND [ToDate]", _Connection);
                _GetAllStocksAtDate.Prepare();
            }

            _GetAllStocksAtDate.Parameters.AddWithValue("@Date", atDate.ToString("yyyy-MM-dd"));

            SQLiteDataReader reader = _GetAllStocksAtDate.ExecuteReader();
            while (reader.Read())
            {
                Stock stock = SQLiteStockEntityCreator.CreateStock(_Database as SQLiteStockDatabase, reader);
                list.Add(stock);
            }
            reader.Close();

            return list.AsReadOnly();
        }

        private SQLiteCommand _GetStockByASXCodeandDate;
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
            if (_GetStockByASXCodeandDate == null)
            {
                _GetStockByASXCodeandDate = new SQLiteCommand("SELECT * FROM [Stocks] WHERE [ASXCode] = @ASXCode AND @Date BETWEEN [FromDate] AND [ToDate] ORDER BY [ASXCode]", _Connection);
                _GetStockByASXCodeandDate.Prepare();
            }

            _GetStockByASXCodeandDate.Parameters.AddWithValue("@ASXCode", asxCode);
            _GetStockByASXCodeandDate.Parameters.AddWithValue("@Date", atDate.ToString("yyyy-MM-dd"));

           stock = GetStock(_GetStockByASXCodeandDate);
           return (stock != null);
        }

        private SQLiteCommand _GetChildStocks;
        public IReadOnlyCollection<Stock> GetChildStocks(Guid parent, DateTime atDate)
        {
            var list = new List<Stock>();

            if (_GetChildStocks == null)
            {
                _GetChildStocks = new SQLiteCommand("SELECT * FROM [Stocks] WHERE [Parent] = @Parent AND @Date BETWEEN [FromDate] AND [ToDate]", _Connection);
                _GetChildStocks.Prepare();
            }

            _GetChildStocks.Parameters.AddWithValue("@Parent", parent.ToString());
            _GetChildStocks.Parameters.AddWithValue("@Date", atDate.ToString("yyyy-MM-dd"));

            SQLiteDataReader reader = _GetChildStocks.ExecuteReader();
            while (reader.Read())
            {
                Stock stock = SQLiteStockEntityCreator.CreateStock(_Database as SQLiteStockDatabase, reader);
                list.Add(stock);
            }       
            reader.Close();

            return list.AsReadOnly();
        }

        private SQLiteCommand _GetRelativeNTA;
        public RelativeNTA GetRelativeNTA(Guid parent, Guid child, DateTime atDate)
        {
            if (_GetRelativeNTA == null)
            {
                _GetRelativeNTA = new SQLiteCommand("SELECT * FROM [RelativeNTAs] WHERE [Parent] = @Parent AND [Child] = @Child AND [Date] = @Date", _Connection);
                _GetRelativeNTA.Prepare();
            }

            _GetRelativeNTA.Parameters.AddWithValue("@Parent", parent.ToString());
            _GetRelativeNTA.Parameters.AddWithValue("@Child", child.ToString());
            _GetRelativeNTA.Parameters.AddWithValue("@Date", atDate.ToString("yyyy-MM-dd"));

            return GetRelativeNTA(_GetRelativeNTA);
        }

        private SQLiteCommand _GetRelativeNTAs;
        public IReadOnlyCollection<RelativeNTA> GetRelativeNTAs(Guid parent, Guid child)
        {
            var list = new List<RelativeNTA>();

            if (_GetRelativeNTAs == null)
            {
                _GetRelativeNTAs = new SQLiteCommand("SELECT * FROM [RelativeNTAs] WHERE [Parent] = @Parent AND [Child] = @Child AND [Date] = @Date", _Connection);
                _GetRelativeNTAs.Prepare();
            }

            _GetRelativeNTAs.Parameters.AddWithValue("@Parent", parent.ToString());
            _GetRelativeNTAs.Parameters.AddWithValue("@Child", child.ToString());

            SQLiteDataReader reader = _GetRelativeNTA.ExecuteReader();
            while (reader.Read())
            {
                list.Add(SQLiteStockEntityCreator.CreateRelativeNTA(_Database as SQLiteStockDatabase, reader));
            }
            reader.Close();

            return list.AsReadOnly();
        }

        private SQLiteCommand _PercentOfParentCost;
        public decimal PercentOfParentCost(Guid parent, Guid child, DateTime atDate)
        {
            decimal percent = 0.00m;

            if (_PercentOfParentCost == null)
            {
                _PercentOfParentCost = new SQLiteCommand("SELECT [Percentage] FROM [RelativeNTAs] WHERE [Parent] = @Parent AND [Child] = @Child AND [Date] <= @Date ORDER BY [Date] DESC", _Connection);
                _PercentOfParentCost.Prepare();
            }

            _PercentOfParentCost.Parameters.AddWithValue("@Parent", parent.ToString());
            _PercentOfParentCost.Parameters.AddWithValue("@Child", child.ToString());
            _PercentOfParentCost.Parameters.AddWithValue("@Date", atDate.ToString("yyyy-MM-dd"));

            SQLiteDataReader reader = _PercentOfParentCost.ExecuteReader();
            if (reader.Read())
                percent = DBToDecimal(reader.GetInt32(0));

            reader.Close();

            return percent;
        }

        public string GetASXCode(Guid id)
        {
            return GetASXCode(id, DateTime.Now);
        }

        private SQLiteCommand _GetASXCodeAtDate;
        public string GetASXCode(Guid id, DateTime atDate)
        {
            if (_GetASXCodeAtDate == null)
            {
                _GetASXCodeAtDate = new SQLiteCommand("SELECT [ASXCode] FROM [Stocks] WHERE [Id] = @Id AND @Date BETWEEN [FromDate] AND [ToDate]", _Connection);
                _GetASXCodeAtDate.Prepare();
            }

            _GetASXCodeAtDate.Parameters.AddWithValue("@Id", id.ToString());
            _GetASXCodeAtDate.Parameters.AddWithValue("@Date", atDate.ToString("yyyy-MM-dd"));

            SQLiteDataReader reader = _GetASXCodeAtDate.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                throw new RecordNotFoundException("");
            }

            string asxCode = reader.GetString(0);
            reader.Close();

            return asxCode;
        }

        private Stock GetStock(SQLiteCommand command)
        {
          SQLiteDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                return null;
            }

            Stock stock = SQLiteStockEntityCreator.CreateStock(_Database as SQLiteStockDatabase, reader);
            reader.Close();

            return stock;
        }

        private RelativeNTA GetRelativeNTA(SQLiteCommand command)
        {
            SQLiteDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                throw new RecordNotFoundException("");
            }

            RelativeNTA nta = SQLiteStockEntityCreator.CreateRelativeNTA(_Database as SQLiteStockDatabase, reader);
            reader.Close();

            return nta;
        }


        public decimal DBToDecimal(int value)
        {
            return (decimal)value / 100000;
        }

        public decimal GetClosingPrice(Guid stock, DateTime date)
        {
            return GetClosingPrice(stock, date, false);
        }

        public bool TryGetClosingPrice(Guid stock, DateTime date, out decimal price)
        {
            return TryGetClosingPrice(stock, date, out price, false);
        }


        public decimal GetClosingPrice(Guid stock, DateTime date, bool exact)
        {
            decimal closingPrice;

            if (TryGetClosingPrice(stock, date, out closingPrice, exact))
                return closingPrice;
            else
                return 0.00m;
        }

        public bool TryGetClosingPrice(Guid stock, DateTime date, out decimal price, bool exact)
        {
            if (exact)
                return GetExactClosingPrice(stock, date, out price);
            else
                return GetClosingPrice(stock, date, out price);
        }

        private SQLiteCommand _GetClosingPriceCommand;
        private bool GetClosingPrice(Guid stock, DateTime date, out decimal price)
        {
            if (_GetClosingPriceCommand == null)
            {
                _GetClosingPriceCommand = new SQLiteCommand("SELECT [Price] FROM [StockPrices] WHERE [Stock] = @Stock AND [Date] <= @Date ORDER BY [Date] DESC LIMIT 1", _Connection);
                _GetClosingPriceCommand.Prepare();
            }

            _GetClosingPriceCommand.Parameters.AddWithValue("@Stock", stock.ToString());
            _GetClosingPriceCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            SQLiteDataReader reader = _GetClosingPriceCommand.ExecuteReader();

            if (reader.Read())
            {
                price = DBToDecimal(reader.GetInt32(0));
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


        private SQLiteCommand _GetExactClosingPriceCommand;
        private bool GetExactClosingPrice(Guid stock, DateTime date, out decimal price)
        {
            if (_GetExactClosingPriceCommand == null)
            {
                _GetExactClosingPriceCommand = new SQLiteCommand("SELECT [Price] FROM [StockPrices] WHERE [Stock] = @Stock AND [Date] = @Date", _Connection);
                _GetExactClosingPriceCommand.Prepare();
            }

            _GetExactClosingPriceCommand.Parameters.AddWithValue("@Stock", stock.ToString());
            _GetExactClosingPriceCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            SQLiteDataReader reader = _GetExactClosingPriceCommand.ExecuteReader();

            if (reader.Read())
            {
                price = DBToDecimal(reader.GetInt32(0));
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

        private SQLiteCommand _GetClosingPricesCommand;
        public Dictionary<DateTime, decimal> GetClosingPrices(Guid stock, DateTime fromDate, DateTime toDate)
        {
            var prices = new Dictionary<DateTime, decimal>();

            if (_GetClosingPricesCommand == null)
            {
                _GetClosingPricesCommand = new SQLiteCommand("SELECT [Date], [Price] FROM [StockPrices] WHERE [Stock] = @Stock AND [Date] BETWEEN @FromDate AND @ToDate ORDER BY [Date]", _Connection);
                _GetClosingPricesCommand.Prepare();
            }

            _GetClosingPricesCommand.Parameters.AddWithValue("@Stock", stock.ToString());
            _GetClosingPricesCommand.Parameters.AddWithValue("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            _GetClosingPricesCommand.Parameters.AddWithValue("@ToDate", toDate.ToString("yyyy-MM-dd"));
            SQLiteDataReader reader = _GetClosingPricesCommand.ExecuteReader();

            while (reader.Read())
            {
                DateTime date = reader.GetDateTime(0);
                decimal price = DBToDecimal(reader.GetInt32(1));

                prices.Add(date, price);
            }

            reader.Close();
            
            return prices;          
        }


        private SQLiteCommand _GetLatestClosingPriceCommand;
        public DateTime GetLatestClosingPrice(Guid stock)
        {
            DateTime date;

            if (_GetLatestClosingPriceCommand == null)
            {
                _GetLatestClosingPriceCommand = new SQLiteCommand("SELECT[Date] FROM[StockPrices] WHERE[Stock] = @Stock ORDER BY[Date] DESC LIMIT 1", _Connection);
                _GetLatestClosingPriceCommand.Prepare();
            }

            _GetLatestClosingPriceCommand.Parameters.AddWithValue("@Stock", stock.ToString());
            SQLiteDataReader reader = _GetLatestClosingPriceCommand.ExecuteReader();

            if (reader.Read())
                date = reader.GetDateTime(0);
            else
                date = DateUtils.NoDate;

            reader.Close();

            return date;
        }

    }
}
