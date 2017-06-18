using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite
{
    class SQLiteStockPriceRepository: IStockPriceRepository 
    {
        protected SQLiteDatabase _Database;
        protected SqliteConnection _Connection;

        protected internal SQLiteStockPriceRepository(SQLiteDatabase database)
        {
            _Database = database;
            _Connection = database._Connection;
        }

        private SqliteCommand _GetCommand;
        public decimal Get(Guid stockId, DateTime date)
        {
                    if (_GetCommand == null)
            {
                _GetCommand = new SqliteCommand("SELECT [Price] FROM [StockPrices] WHERE [Stock] = @Stock AND [Date] = @Date", _Connection);
                _GetCommand.Prepare();
            }

            _GetCommand.Parameters.AddWithValue("@Stock", stockId.ToString());
            _GetCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

            decimal price;
            using (SqliteDataReader reader = _GetCommand.ExecuteReader())
            {
                if (!reader.Read())
                {
                    throw new RecordNotFoundException(stockId);
                }
                price = SQLiteUtils.DBToDecimal(reader.GetInt64(0));
            }

            return price;    
        }

        public bool Exists(Guid stockId, DateTime date)
        {
            if (_GetCommand == null)
            {
                _GetCommand = new SqliteCommand("SELECT [Price] FROM [StockPrices] WHERE [Stock] = @Stock AND [Date] = @Date", _Connection);
                _GetCommand.Prepare();
            }

            _GetCommand.Parameters.AddWithValue("@Stock", stockId.ToString());
            _GetCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

            using (SqliteDataReader reader = _GetCommand.ExecuteReader())
            {
                return reader.HasRows;
            }     
        }

        private SqliteCommand _AddCommand;
        public void Add(Guid stockId, DateTime date, decimal price, bool current)
        {
            if (_AddCommand == null)
            {
                _AddCommand = new SqliteCommand("INSERT INTO [StockPrices] ([Stock], [Date], [Price], [Current]) VALUES (@Stock, @Date, @Price, @Current)", _Connection);
                _AddCommand.Prepare();
            }

            _AddCommand.Parameters.AddWithValue("@Stock", stockId.ToString());
            _AddCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            _AddCommand.Parameters.AddWithValue("@Price", SQLiteUtils.DecimalToDB(price));
            _AddCommand.Parameters.AddWithValue("@Current", SQLiteUtils.BoolToDb(current));
            _AddCommand.ExecuteNonQuery(); 
        }

        private SqliteCommand _UpdateCommand;
        public void Update(Guid stockId, DateTime date, decimal price, bool current)
        {
            if (_UpdateCommand == null)
            {
                _UpdateCommand = new SqliteCommand("UPDATE [StockPrices] SET [Price] = @Price, [Current] = @Current WHERE [Stock] = @Stock and [Date] = @Date", _Connection);
                _UpdateCommand.Prepare();
            }

            _UpdateCommand.Parameters.AddWithValue("@Stock", stockId.ToString());
            _UpdateCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            _UpdateCommand.Parameters.AddWithValue("@Price", SQLiteUtils.DecimalToDB(price));
            _UpdateCommand.Parameters.AddWithValue("@Current", SQLiteUtils.BoolToDb(current));
            _UpdateCommand.ExecuteNonQuery(); 
        }

        private SqliteCommand _DeleteCommand;
        public void Delete(Guid stockId, DateTime date)
        {
            if (_DeleteCommand == null)
            {
                _DeleteCommand = new SqliteCommand("DELETE FROM [StockPrices] WHERE [Stock] = @Stock and [Date] = @Date", _Connection);
                _DeleteCommand.Prepare();
            }

            _DeleteCommand.Parameters.AddWithValue("@Stock", stockId.ToString());
            _DeleteCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            _UpdateCommand.ExecuteNonQuery(); 
        }
       
    }
}
