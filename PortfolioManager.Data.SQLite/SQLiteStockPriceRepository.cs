using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.SQLite
{
    class SQLiteStockPriceRepository: IStockPriceRepository 
    {
        protected SQLiteDatabase _Database;
        protected SQLiteConnection _Connection;

        protected internal SQLiteStockPriceRepository(SQLiteDatabase database)
        {
            _Database = database;
            _Connection = database._Connection;
        }

        private SQLiteCommand _GetCommand;
        public decimal Get(Guid stockId, DateTime date)
        {
            if (_GetCommand == null)
            {
                _GetCommand = new SQLiteCommand("SELECT [Price] FROM [StockPrices] WHERE [Stock] = @Stock AND [Date] = @Date", _Connection);
                _GetCommand.Prepare();
            }

            _GetCommand.Parameters.AddWithValue("@Stock", stockId.ToString());
            _GetCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            SQLiteDataReader reader = _GetCommand.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                throw new RecordNotFoundException(stockId);
            }
            var price = SQLiteUtils.DBToDecimal(reader.GetInt64(0));

            reader.Close();

            return price;            
        }

        public bool PriceExists(Guid stockId, DateTime date)
        {
            if (_GetCommand == null)
            {
                _GetCommand = new SQLiteCommand("SELECT [Price] FROM [StockPrices] WHERE [Stock] = @Stock AND [Date] = @Date", _Connection);
                _GetCommand.Prepare();
            }

            _GetCommand.Parameters.AddWithValue("@Stock", stockId.ToString());
            _GetCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            SQLiteDataReader reader = _GetCommand.ExecuteReader();

            var exists = reader.HasRows;
            
            reader.Close();
            return exists;          
        }

        private SQLiteCommand _AddCommand;
        public void Add(Guid stockId, DateTime date, decimal price)
        {
            if (_AddCommand == null)
            {
                _AddCommand = new SQLiteCommand("INSERT INTO [StockPrices] ([Stock], [Date], [Price]) VALUES (@Stock, @Date, @Price)", _Connection);
                _AddCommand.Prepare();
            }

            _AddCommand.Parameters.AddWithValue("@Stock", stockId.ToString());
            _AddCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            _AddCommand.Parameters.AddWithValue("@Price", SQLiteUtils.DecimalToDB(price));
            _AddCommand.ExecuteNonQuery(); 
        }

        private SQLiteCommand _UpdateCommand;
        public void Update(Guid stockId, DateTime date, decimal price)
        {
            if (_UpdateCommand == null)
            {
                _UpdateCommand = new SQLiteCommand("UPDATE [StockPrices] SET [Price] = @Price WHERE [Stock] = @Stock and [Date] = @Date", _Connection);
                _UpdateCommand.Prepare();
            }

            _UpdateCommand.Parameters.AddWithValue("@Stock", stockId.ToString());
            _UpdateCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            _UpdateCommand.Parameters.AddWithValue("@Price", SQLiteUtils.DecimalToDB(price));
            _UpdateCommand.ExecuteNonQuery(); 
        }

        private SQLiteCommand _DeleteCommand;
        public void Delete(Guid stockId, DateTime date)
        {
            if (_DeleteCommand == null)
            {
                _DeleteCommand = new SQLiteCommand("DELETE FROM [StockPrices] WHERE [Stock] = @Stock and [Date] = @Date", _Connection);
                _DeleteCommand.Prepare();
            }

            _DeleteCommand.Parameters.AddWithValue("@Stock", stockId.ToString());
            _DeleteCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            _UpdateCommand.ExecuteNonQuery(); 
        }
       
    }
}
