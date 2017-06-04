using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class SQLiteNonTradingDayRepository: INonTradingDayRepository
    {
        protected SQLiteDatabase _Database;
        protected SQLiteConnection _Connection;

        protected internal SQLiteNonTradingDayRepository(SQLiteDatabase database)
        {
            _Database = database;
            _Connection = database._Connection;
        }

        private SQLiteCommand _AddRecordCommand;
        public void Add(DateTime date)
        {
            if (_AddRecordCommand == null)
            {
                _AddRecordCommand = new SQLiteCommand("INSERT OR IGNORE INTO [NonTradingDays] ([Date]) VALUES (@Date)", _Connection);
                _AddRecordCommand.Prepare();
            }

            _AddRecordCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            _AddRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _DeleteRecordCommand;
        public void Delete(DateTime date)
        {
            if (_DeleteRecordCommand == null)
            {
                _DeleteRecordCommand = new SQLiteCommand("DELETE FROM [NonTradingDays] WHERE [Date] = @Date", _Connection);
                _DeleteRecordCommand.Prepare();
            }

            _DeleteRecordCommand.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            _DeleteRecordCommand.ExecuteNonQuery();
        }
    }
}
