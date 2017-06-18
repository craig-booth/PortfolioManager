using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.Sqlite;

using PortfolioManager.Data;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{

    class SQLiteCGTEventRepository : SQLiteRepository<CGTEvent>, ICGTEventRepository
    {
        protected internal SQLiteCGTEventRepository(SQLitePortfolioDatabase database)
            : base(database, "CGTEvents", new SQLitePortfolioEntityCreator(database))
        {
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO CGTEvents ([Id], [Stock], [Units], [EventDate], [CostBase], [AmountReceived], [CapitalGain], [CGTMethod]) VALUES (@Id, @Stock, @Units, @EventDate, @CostBase, @AmountReceived, @CapitalGain, @CGTMethod)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE CGTEvents SET [Stock] = @Stock, [Units] = @Units, [EventDate] = @EventDate, [CostBase] = @CostBase, [AmountReceived] = @AmountReceived, [CapitalGain] = @CapitalGain, [CGTMethod] = @CGTMethod WHERE [Id] = @Id", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SqliteCommand command, CGTEvent entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@Stock", entity.Stock.ToString());
            command.Parameters.AddWithValue("@Units", entity.Units);
            command.Parameters.AddWithValue("@EventDate", entity.EventDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@CostBase", SQLiteUtils.DecimalToDB(entity.CostBase));
            command.Parameters.AddWithValue("@AmountReceived", SQLiteUtils.DecimalToDB(entity.AmountReceived));
            command.Parameters.AddWithValue("@CapitalGain", SQLiteUtils.DecimalToDB(entity.CapitalGain));
            command.Parameters.AddWithValue("@CGTMethod", entity.CGTMethod);
        }

    }
}
