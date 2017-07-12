using Microsoft.Data.Sqlite;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLiteParcelRepository : SQLiteEffectiveDatedRepository<ShareParcel>, IParcelRepository
    {

        protected internal SQLiteParcelRepository(SqliteTransaction transaction, IEntityCreator entityCreator)
            : base(transaction, "Parcels", entityCreator)
        {
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO Parcels ([Id], [FromDate], [ToDate], [Stock], [AquisitionDate], [Units], [UnitPrice], [Amount], [CostBase], [PurchaseId]) VALUES (@Id, @FromDate, @ToDate, @Stock, @AquisitionDate, @Units, @UnitPrice, @Amount, @CostBase, @PurchaseId)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@FromDate", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@ToDate", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Stock", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@AquisitionDate", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Units", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@UnitPrice", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@Amount", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@CostBase", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@PurchaseId", SqliteType.Text);

                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE Parcels SET [ToDate] = @ToDate, [Stock] = @Stock, [AquisitionDate] = @AquisitionDate, [Units] = @Units, [UnitPrice] = @UnitPrice, [Amount] = @Amount, [CostBase] = @CostBase, [PurchaseId] = @PurchaseId WHERE [Id] = @Id AND [FromDate] = @FromDate", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@FromDate", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@ToDate", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Stock", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@AquisitionDate", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Units", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@UnitPrice", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@Amount", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@CostBase", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@PurchaseId", SqliteType.Text);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SqliteParameterCollection parameters, ShareParcel entity)
        {
            parameters["@Id"].Value = entity.Id.ToString();
            parameters["@FromDate"].Value = entity.FromDate.ToString("yyyy-MM-dd");
            parameters["@ToDate"].Value = entity.ToDate.ToString("yyyy-MM-dd");
            parameters["@Stock"].Value = entity.Stock.ToString();
            parameters["@AquisitionDate"].Value = entity.AquisitionDate.ToString("yyyy-MM-dd");
            parameters["@Units"].Value = entity.Units;
            parameters["@UnitPrice"].Value = SQLiteUtils.DecimalToDB(entity.UnitPrice);
            parameters["@Amount"].Value = SQLiteUtils.DecimalToDB(entity.Amount);
            parameters["@CostBase"].Value = SQLiteUtils.DecimalToDB(entity.CostBase);
            parameters["@PurchaseId"].Value = entity.PurchaseId.ToString();
        }

    }
}
