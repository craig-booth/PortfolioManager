
using Microsoft.Data.Sqlite;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{

    class SQLiteCGTEventRepository : SQLiteRepository<CGTEvent>, ICGTEventRepository
    {
        protected internal SQLiteCGTEventRepository(SqliteTransaction transaction)
            : base(transaction,  "CGTEvents", new SQLitePortfolioEntityCreator())
        {
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO CGTEvents ([Id], [Stock], [Units], [EventDate], [CostBase], [AmountReceived], [CapitalGain], [CGTMethod]) VALUES (@Id, @Stock, @Units, @EventDate, @CostBase, @AmountReceived, @CapitalGain, @CGTMethod)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Stock", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Units", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@EventDate", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@CostBase", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@AmountReceived", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@CapitalGain", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@CGTMethod", SqliteType.Integer);

                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE CGTEvents SET [Stock] = @Stock, [Units] = @Units, [EventDate] = @EventDate, [CostBase] = @CostBase, [AmountReceived] = @AmountReceived, [CapitalGain] = @CapitalGain, [CGTMethod] = @CGTMethod WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Stock", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Units", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@EventDate", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@CostBase", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@AmountReceived", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@CapitalGain", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@CGTMethod", SqliteType.Integer);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SqliteParameterCollection parameters, CGTEvent entity)
        {
            parameters["@Id"].Value = entity.Id.ToString();
            parameters["@Stock"].Value = entity.Stock.ToString();
            parameters["@Units"].Value = entity.Units;
            parameters["@EventDate"].Value = entity.EventDate.ToString("yyyy-MM-dd");
            parameters["@CostBase"].Value = SQLiteUtils.DecimalToDB(entity.CostBase);
            parameters["@AmountReceived"].Value = SQLiteUtils.DecimalToDB(entity.AmountReceived);
            parameters["@CapitalGain"].Value = SQLiteUtils.DecimalToDB(entity.CapitalGain);
            parameters["@CGTMethod"].Value = entity.CGTMethod;
        }

    }
}
