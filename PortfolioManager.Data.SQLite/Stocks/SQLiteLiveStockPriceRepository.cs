using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class SQLiteLiveStockPriceRepository : ILiveStockPriceRepository
    {
        protected SQLiteDatabase _Database;
        protected SQLiteConnection _Connection;


        protected internal SQLiteLiveStockPriceRepository(SQLiteLiveStockPriceDatabase database)
        {
            _Database = database;
            _Connection = database._Connection;
        }

        private SQLiteCommand _GetRecordCommand;
        public decimal Get(Guid stockId)
        {
            if (_GetRecordCommand == null)
            {
                _GetRecordCommand = new SQLiteCommand("SELECT [Price] FROM [LivePrices] WHERE [Stock] = @Stock", _Connection);
                _GetRecordCommand.Prepare();
            }

            _GetRecordCommand.Parameters.AddWithValue("@Stock", stockId.ToString());
            var price =_GetRecordCommand.ExecuteScalar();

            return SQLiteUtils.DBToDecimal((long)price);
        }

        private SQLiteCommand _UpdateRecordCommand;
        public void Update(Guid stockId, decimal price)
        {
            if (_UpdateRecordCommand == null)
            {
                _UpdateRecordCommand = new SQLiteCommand("INSERT OR REPLACE INTO [LivePrices] ([Stock], [Price]) VALUES (@Stock, @Price)", _Connection);
                _UpdateRecordCommand.Prepare();
            }

            _UpdateRecordCommand.Parameters.AddWithValue("@Stock", stockId.ToString());
            _UpdateRecordCommand.Parameters.AddWithValue("@Price", SQLiteUtils.DecimalToDB(price));
            _UpdateRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _DeleteRecordCommand;
        public void Delete(Guid stockId)
        {
            if (_DeleteRecordCommand == null)
            {
                _DeleteRecordCommand = new SQLiteCommand("DELETE FROM [LivePrices] WHERE [Stock] = @Stock", _Connection);
                _DeleteRecordCommand.Prepare();
            }

            _DeleteRecordCommand.Parameters.AddWithValue("@Stock", stockId.ToString());

            _UpdateRecordCommand.ExecuteNonQuery();
        }
    }
}
