using Microsoft.Data.Sqlite;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLiteDRPCashBalanceRepository : SQLiteEffectiveDatedRepository<DRPCashBalance>, IDRPCashBalanceRepository
    {
        protected internal SQLiteDRPCashBalanceRepository(SQLitePortfolioDatabase database)
            : base(database, "DRPCashBalances", new SQLitePortfolioEntityCreator(database))
        {
        }


        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO DRPCashBalances ([Id], [FromDate], [ToDate], [Balance]) VALUES (@Id, @FromDate, @ToDate, @Balance)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE DRPCashBalances SET [ToDate] = @ToDate, [Balance] = @Balance WHERE [Id] = @Id AND [FromDate] = @FromDate", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SqliteCommand command, DRPCashBalance entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@FromDate", entity.FromDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@ToDate", entity.ToDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Balance", SQLiteUtils.DecimalToDB(entity.Balance));
        }
    }
}
