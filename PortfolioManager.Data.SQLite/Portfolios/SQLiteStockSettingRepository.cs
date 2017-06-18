using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;
using PortfolioManager.Data;
using PortfolioManager.Data.Portfolios;


namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLiteStockSettingRepository : SQLiteEffectiveDatedRepository<StockSetting>, IStockSettingRepository
    {
 
        protected internal SQLiteStockSettingRepository(SQLitePortfolioDatabase database)
            : base(database, "StockSettings", new SQLitePortfolioEntityCreator(database))
        {
            _Database = database;
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO StockSettings ([Id], [FromDate], [ToDate], [ParticipateinDRP]) VALUES (@Id, @FromDate, @ToDate, @ParticipateinDRP)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE StockSettings SET [ToDate] = @ToDate, [ParticipateinDRP] = @ParticipateinDRP WHERE [Id] = @Id AND [FromDate] = @FromDate", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SqliteCommand command, StockSetting entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@FromDate", entity.FromDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@ToDate", entity.ToDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@ParticipateinDRP", SQLiteUtils.BoolToDb(entity.ParticipateinDRP));
        }
    }
}
