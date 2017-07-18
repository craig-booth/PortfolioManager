using System;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class SQLiteNonTradingDayRepository: INonTradingDayRepository
    {
        protected SqliteTransaction _Transaction;

        protected internal SQLiteNonTradingDayRepository(SqliteTransaction transaction)
        {
            _Transaction = transaction;
        }

        private SqliteCommand _AddRecordCommand;
        public void Add(DateTime date)
        {
            if (_AddRecordCommand == null)
            {
                _AddRecordCommand = new SqliteCommand("INSERT OR IGNORE INTO [NonTradingDays] ([Date]) VALUES (@Date)", _Transaction.Connection, _Transaction);
                _AddRecordCommand.Parameters.Add("@Date", SqliteType.Text);
                _AddRecordCommand.Prepare();
            }

            _AddRecordCommand.Parameters["@Date"].Value = date.ToString("yyyy-MM-dd");
            _AddRecordCommand.ExecuteNonQuery();
        }

        private SqliteCommand _DeleteRecordCommand;
        public void Delete(DateTime date)
        {
            if (_DeleteRecordCommand == null)
            {
                _DeleteRecordCommand = new SqliteCommand("DELETE FROM [NonTradingDays] WHERE [Date] = @Date", _Transaction.Connection, _Transaction);
                _DeleteRecordCommand.Parameters.Add("@Date", SqliteType.Text);
                _DeleteRecordCommand.Prepare();
            }

            _DeleteRecordCommand.Parameters["@Date"].Value = date.ToString("yyyy-MM-dd");
            _DeleteRecordCommand.ExecuteNonQuery();
        }
    }
}
