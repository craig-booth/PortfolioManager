using System;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite
{
    class SQLiteStockPriceRepository: IStockPriceRepository 
    {
        protected SqliteTransaction _Transaction;

        protected internal SQLiteStockPriceRepository(SqliteTransaction transaction)
        {
            _Transaction = transaction;
        }

        private SqliteCommand _GetCommand;
        private SqliteCommand GetRecordCommand()
        {          
            if (_GetCommand == null)
            {
                _GetCommand = new SqliteCommand("SELECT [Price] FROM [StockPrices] WHERE [Stock] = @Stock AND [Date] = @Date", _Transaction.Connection, _Transaction);
                _GetCommand.Parameters.Add("@Stock", SqliteType.Text);
                _GetCommand.Parameters.Add("@Date", SqliteType.Text);
                _GetCommand.Prepare();
            }

            return _GetCommand;
        }

        public decimal Get(Guid stockId, DateTime date)
        {
            var command = GetRecordCommand();

            command.Parameters["@Stock"].Value = stockId.ToString();
            command.Parameters["@Date"].Value = date.ToString("yyyy-MM-dd");

            decimal price;
            using (SqliteDataReader reader = command.ExecuteReader())
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
            var command = GetRecordCommand();

            command.Parameters["@Stock"].Value = stockId.ToString();
            command.Parameters["@Date"].Value = date.ToString("yyyy-MM-dd");

            using (SqliteDataReader reader = command.ExecuteReader())
            {
                return reader.HasRows;
            }     
        }

        private SqliteCommand _AddCommand;
        private SqliteCommand GetAddRecordCommand()
        {
            if (_AddCommand == null)
            {
                _AddCommand = new SqliteCommand("INSERT INTO [StockPrices] ([Stock], [Date], [Price], [Current]) VALUES (@Stock, @Date, @Price, @Current)", _Transaction.Connection, _Transaction);
                _AddCommand.Parameters.Add("@Stock", SqliteType.Text);
                _AddCommand.Parameters.Add("@Date", SqliteType.Text);
                _AddCommand.Parameters.Add("@Price", SqliteType.Integer);
                _AddCommand.Parameters.Add("@Current", SqliteType.Text);
                _AddCommand.Prepare();
            }

            return _AddCommand;
        }

        public void Add(Guid stockId, DateTime date, decimal price, bool current)
        {
            var command = GetAddRecordCommand();

            command.Parameters["@Stock"].Value = stockId.ToString();
            command.Parameters["@Date"].Value = date.ToString("yyyy-MM-dd");
            command.Parameters["@Price"].Value = SQLiteUtils.DecimalToDB(price);
            command.Parameters["@Current"].Value =  SQLiteUtils.BoolToDb(current);
            command.ExecuteNonQuery(); 
        }

        private SqliteCommand _UpdateCommand;
        private SqliteCommand GetUpdateCommand()
        {
            if (_UpdateCommand == null)
            {
                _UpdateCommand = new SqliteCommand("UPDATE [StockPrices] SET [Price] = @Price, [Current] = @Current WHERE [Stock] = @Stock and [Date] = @Date", _Transaction.Connection, _Transaction);
                _UpdateCommand.Parameters.Add("@Stock", SqliteType.Text);
                _UpdateCommand.Parameters.Add("@Date", SqliteType.Text);
                _UpdateCommand.Parameters.Add("@Price", SqliteType.Integer);
                _UpdateCommand.Parameters.Add("@Current", SqliteType.Text);
                _UpdateCommand.Prepare();
            }

            return _UpdateCommand;
        }

        public void Update(Guid stockId, DateTime date, decimal price, bool current)
        {
            var command = GetUpdateCommand();

            command.Parameters["@Stock"].Value = stockId.ToString();
            command.Parameters["@Date"].Value = date.ToString("yyyy-MM-dd");
            command.Parameters["@Price"].Value = SQLiteUtils.DecimalToDB(price);
            command.Parameters["@Current"].Value = SQLiteUtils.BoolToDb(current);
            command.ExecuteNonQuery();
        }

        private SqliteCommand _DeleteCommand;
        public void Delete(Guid stockId, DateTime date)
        {
            if (_DeleteCommand == null)
            {
                _DeleteCommand = new SqliteCommand("DELETE FROM [StockPrices] WHERE [Stock] = @Stock and [Date] = @Date", _Transaction.Connection, _Transaction);
                _DeleteCommand.Parameters.Add("@Stock", SqliteType.Text);
                _DeleteCommand.Parameters.Add("@Date", SqliteType.Text);
                _DeleteCommand.Prepare();
            }

            _DeleteCommand.Parameters["@Stock"].Value = stockId.ToString();
            _DeleteCommand.Parameters["@Date"].Value = date.ToString("yyyy-MM-dd");
            _UpdateCommand.ExecuteNonQuery(); 
        }
       
    }
}
