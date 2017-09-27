using Microsoft.Data.Sqlite;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLiteParcelAuditRepository : SQLiteRepository<ShareParcelAudit>, IParcelAuditRepository
    {

        protected internal SQLiteParcelAuditRepository(SqliteTransaction transaction)
            :base(transaction, "ParcelAudit", new SQLitePortfolioEntityCreator())
        {
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO ParcelAudit ([Id], [Parcel], [Date], [Transaction], [UnitCount], [CostBaseChange], [AmountChange]) VALUES (@Id, @Parcel, @Date, @Transaction, @UnitCount, @CostBaseChange, @AmountChange)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Parcel", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Date", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Transaction", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@UnitCount", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@CostBaseChange", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@AmountChange", SqliteType.Integer);

                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE ParcelAudit SET [Parcel] = @Parcel, [Date] = @Date, [Transaction] = @Transaction, [UnitCount] = @UnitCount, [CostBaseChange] = @CostBaseChange, [AmountChange] = @AmountChange WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Parcel", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Date", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Transaction", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@UnitCount", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@CostBaseChange", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@AmountChange", SqliteType.Integer);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SqliteParameterCollection parameters, ShareParcelAudit entity)
        {
            parameters["@Id"].Value = entity.Id.ToString();
            parameters["@Parcel"].Value = entity.Parcel.ToString();
            parameters["@Date"].Value = entity.Date.ToString("yyyy-MM-dd");
            parameters["@Transaction"].Value = entity.Transaction.ToString();
            parameters["@UnitCount"].Value = entity.UnitCount;
            parameters["@CostBaseChange"].Value = SQLiteUtils.DecimalToDB(entity.CostBaseChange);
            parameters["@AmountChange"].Value = SQLiteUtils.DecimalToDB(entity.AmountChange);
         }

    }
}
