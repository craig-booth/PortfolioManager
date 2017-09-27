using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios.Transactions
{
    public class SQLiteAquisitionRepository : SQLiteRepository<Transaction>
    {

        public SQLiteAquisitionRepository(SqliteTransaction transaction)
            : base(transaction, "Aquisitions", new SQLitePortfolioEntityCreator())
        {

        }

        private SqliteCommand _GetCurrentRecordCommand;
        protected override SqliteCommand GetCurrentRecordCommand()
        {
            if (_GetCurrentRecordCommand == null)
            {
                _GetCurrentRecordCommand = new SqliteCommand("SELECT [Transactions].*, [Aquisitions].* FROM [Aquisitions] LEFT OUTER JOIN [Transactions] ON [Aquisitions].[Id] = [Transactions].[Id] WHERE [Aquisitions].[Id] = @Id", _Transaction.Connection, _Transaction);
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
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO [Aquisitions] ([Id], [Units], [AveragePrice], [TransactionCosts], [CreateCashTransaction]) VALUES (@Id, @Units, @AveragePrice, @TransactionCosts, @CreateCashTransaction)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Units", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@AveragePrice", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@TransactionCosts", SqliteType.Integer);
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
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE [Aquisitions] SET [Units] = @Units, [AveragePrice] = @AveragePrice, [TransactionCosts] = @TransactionCosts, [CreateCashTransaction] = @CreateCashTransaction WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Units", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@AveragePrice", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@TransactionCosts", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@CreateCashTransaction", SqliteType.Text);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }


        protected override void AddParameters(SqliteParameterCollection parameters, Transaction entity)
        {
            var aquisition = entity as Aquisition;

            parameters["@Id"].Value = aquisition.Id.ToString();
            parameters["@Units"].Value = aquisition.Units;
            parameters["@AveragePrice"].Value = SQLiteUtils.DecimalToDB(aquisition.AveragePrice);
            parameters["@TransactionCosts"].Value = SQLiteUtils.DecimalToDB(aquisition.TransactionCosts);
            parameters["@CreateCashTransaction"].Value = SQLiteUtils.BoolToDb(aquisition.CreateCashTransaction);
        }
    }
}
