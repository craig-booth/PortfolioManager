using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    public class SQLiteStockRepository : SQLiteEffectiveDatedRepository<Stock>, IStockRepository
    {
        protected internal SQLiteStockRepository(SqliteTransaction transaction, IEntityCreator entityCreator) 
            : base(transaction, "Stocks", entityCreator)
        {
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO Stocks ([Id], [FromDate], [ToDate], [ASXCode], [Name], [Type], [Parent], [DividendRounding], [DRPMethod], [Category]) VALUES (@Id, @FromDate, @ToDate, @ASXCode, @Name, @Type, @Parent, @DividendRounding, @DRPMethod, @Category)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@FromDate", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@ToDate", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@ASXCode", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Name", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Type", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@Parent", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@DividendRounding", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@DRPMethod", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@Category", SqliteType.Integer);

                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE Stocks SET [ToDate] = @ToDate, [ASXCode] = @ASXCode, [Name] = @Name, [Type] = @Type, [Parent] = @Parent, [DividendRounding] = @DividendRounding, [DRPMethod] = @DRPMethod, [Category] = @Category WHERE [Id] = @Id AND [FromDate] = @FromDate", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@FromDate", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@ToDate", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@ASXCode", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Name", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Type", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@Parent", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@DividendRounding", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@DRPMethod", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@Category", SqliteType.Integer);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SqliteParameterCollection parameters, Stock entity)
        {
            parameters["@Id"].Value = entity.Id.ToString();
            parameters["@FromDate"].Value = entity.FromDate.ToString("yyyy-MM-dd");
            parameters["@ToDate"].Value = entity.ToDate.ToString("yyyy-MM-dd");
            parameters["@ASXCode"].Value = entity.ASXCode;
            parameters["@Name"].Value = entity.Name;
            parameters["@Type"].Value = entity.Type;
            parameters["@Parent"].Value = entity.ParentId.ToString();
            parameters["@DividendRounding"].Value = entity.DividendRoundingRule;
            parameters["@DRPMethod"].Value = entity.DRPMethod;
            parameters["@Category"].Value = entity.Category;
        }

    }
}
