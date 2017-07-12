using Microsoft.Data.Sqlite;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLiteCashAccountRepository : SQLiteRepository<CashAccountTransaction>, ICashAccountRepository
    {
  
        protected internal SQLiteCashAccountRepository(SqliteTransaction transaction)
            : base(transaction, "CashAccountTransactions", new SQLitePortfolioEntityCreator())
        {
        }


        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO CashAccountTransactions ([Id], [Type], [Date], [Description], [Amount]) VALUES (@Id, @Type, @Date, @Description, @Amount)", _Transaction.Connection, _Transaction);
                
                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Type", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@Date", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Description", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Amount", SqliteType.Integer);

                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE CashAccountTransactions SET [Type] = @Type, [Date] = @Date, [Description] = @Description, [Amount] = @Amount WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Type", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@Date", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Description", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Amount", SqliteType.Integer);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SqliteParameterCollection parameters, CashAccountTransaction entity)
        {
            parameters["@Id"].Value = entity.Id.ToString();
            parameters["@Type"].Value = entity.Type;
            parameters["@Date"].Value = entity.Date.ToString("yyyy-MM-dd");
            parameters["@Amount"].Value = SQLiteUtils.DecimalToDB(entity.Amount);
        }

    }
}
