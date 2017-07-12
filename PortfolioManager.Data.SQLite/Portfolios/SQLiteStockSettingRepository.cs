using Microsoft.Data.Sqlite;
using PortfolioManager.Data.Portfolios;


namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLiteStockSettingRepository : SQLiteEffectiveDatedRepository<StockSetting>, IStockSettingRepository
    {
 
        protected internal SQLiteStockSettingRepository(SqliteTransaction transaction)
            : base(transaction, "StockSettings", new SQLitePortfolioEntityCreator())
        {
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO StockSettings ([Id], [FromDate], [ToDate], [ParticipateinDRP]) VALUES (@Id, @FromDate, @ToDate, @ParticipateinDRP)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@FromDate", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@ToDate", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@ParticipateinDRP", SqliteType.Text);

                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE StockSettings SET [ToDate] = @ToDate, [ParticipateinDRP] = @ParticipateinDRP WHERE [Id] = @Id AND [FromDate] = @FromDate", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@FromDate", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@ToDate", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@ParticipateinDRP", SqliteType.Text);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SqliteParameterCollection parameters, StockSetting entity)
        {
            parameters["@Id"].Value = entity.Id.ToString();
            parameters["@FromDate"].Value = entity.FromDate.ToString("yyyy-MM-dd");
            parameters["@ToDate"].Value = entity.ToDate.ToString("yyyy-MM-dd");
            parameters["@ParticipateinDRP"].Value = SQLiteUtils.BoolToDb(entity.ParticipateinDRP);
        }
    }
}
