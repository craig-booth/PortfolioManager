using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class SQLiteLiveStockPriceQuery : ILiveStockPriceQuery
    {
        protected SQLiteDatabase _Database;
        protected SQLiteConnection _Connection;

        protected internal SQLiteLiveStockPriceQuery(SQLiteLiveStockPriceDatabase database)
        {
            _Database = database;
            _Connection = database._Connection;
        }

        private SQLiteCommand _GetPriceCommand;
        public decimal GetPrice(Guid stock)
        {
            if (_GetPriceCommand == null)
            {
                _GetPriceCommand = new SQLiteCommand("SELECT [Price] FROM [LivePrices] WHERE [Stock] = @Stock", _Connection);
                _GetPriceCommand.Prepare();
            }

            _GetPriceCommand.Parameters.AddWithValue("@Stock", stock.ToString());
            var price = _GetPriceCommand.ExecuteScalar();

            if (price != null)
                return SQLiteUtils.DBToDecimal((long)price);
            else
                return 10.00m;
        }

        public IDictionary<Guid, decimal> GetPrice(IEnumerable<Guid> stocks)
        {
            var prices = new Dictionary<Guid, decimal>();

            foreach(var stock in stocks)
            {
                prices.Add(stock, GetPrice(stock));
            }

            return prices;
        }

    }
}
