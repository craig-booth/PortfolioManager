using System;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class SQLiteNonTradingDayRepository: INonTradingDayRepository
    {
        protected SQLiteDatabase _Database;
        protected SqliteConnection _Connection;

        protected internal SQLiteNonTradingDayRepository(SQLiteDatabase database)
        {
            _Database = database;
            _Connection = database._Connection;
        }

        private SqliteCommand _AddRecordCommand;
        public void Add(DateTime date)
        {
            if (_AddRecordCommand == null)
            {
                _AddRecordCommand = new SqliteCommand("INSERT OR IGNORE INTO [NonTradingDays] ([Date]) VALUES (@Date)", _Connection);
                _AddRecordCommand.Prepare();
            }

            _AddRecordCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            _AddRecordCommand.ExecuteNonQuery();
        }

        private SqliteCommand _DeleteRecordCommand;
        public void Delete(DateTime date)
        {
            if (_DeleteRecordCommand == null)
            {
                _DeleteRecordCommand = new SqliteCommand("DELETE FROM [NonTradingDays] WHERE [Date] = @Date", _Connection);
                _DeleteRecordCommand.Prepare();
            }

            _DeleteRecordCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            _DeleteRecordCommand.ExecuteNonQuery();
        }
    }
}
