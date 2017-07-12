using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios.Transactions
{
    class SQLiteReturnOfCapitalRepository : SQLiteRepository<Transaction>
    {
        public SQLiteReturnOfCapitalRepository(SqliteTransaction transaction)
            : base(transaction, "ReturnsOfCapital", new SQLitePortfolioEntityCreator())
        {

        }

        private SqliteCommand _GetCurrentRecordCommand;
        protected override SqliteCommand GetCurrentRecordCommand()
        {
            if (_GetCurrentRecordCommand == null)
            {
                _GetCurrentRecordCommand = new SqliteCommand("SELECT [Transactions].*, [ReturnsOfCapital].* FROM [ReturnsOfCapital] LEFT OUTER JOIN [Transactions] ON [ReturnsOfCapital].[Id] = [Transactions].[Id] WHERE [ReturnsOfCapital].[Id] = @Id", _Transaction.Connection, _Transaction);
                _GetCurrentRecordCommand.Parameters.Add("@Id", SqliteType.Text);

                _GetCurrentRecordCommand.Prepare();
            }

            return _GetCurrentRecordCommand;
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO [ReturnsOfCapital] ([Id], [Amount], [CreateCashTransaction]) VALUES (@Id, @Amount, @CreateCashTransaction)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Amount", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@CreateCashTransaction", SqliteType.Text);

                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE [ReturnsOfCapital] SET [Amount] = @Amount, [CreateCashTransaction] = @CreateCashTransaction WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Amount", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@CreateCashTransaction", SqliteType.Text);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }


        protected override void AddParameters(SqliteParameterCollection parameters, Transaction entity)
        {
            var returnOfCapital = entity as ReturnOfCapital;

            parameters["@Id"].Value = returnOfCapital.Id.ToString();
            parameters["@Amount"].Value = SQLiteUtils.DecimalToDB(returnOfCapital.Amount);
            parameters["@CreateCashTransaction"].Value = SQLiteUtils.BoolToDb(returnOfCapital.CreateCashTransaction);
        }
    }

}
