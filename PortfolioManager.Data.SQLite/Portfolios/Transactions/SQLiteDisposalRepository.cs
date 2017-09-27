using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios.Transactions
{
    class SQLiteDisposalRepository : SQLiteRepository<Transaction>
    {
        public SQLiteDisposalRepository(SqliteTransaction transaction)
            : base(transaction, "Aquisitions", new SQLitePortfolioEntityCreator())
        {

        }

        private SqliteCommand _GetCurrentRecordCommand;
        protected override SqliteCommand GetCurrentRecordCommand()
        {
            if (_GetCurrentRecordCommand == null)
            {
                _GetCurrentRecordCommand = new SqliteCommand("SELECT [Transactions].*, [Disposals].* FROM [Disposals] LEFT OUTER JOIN [Transactions] ON [Disposals].[Id] = [Transactions].[Id] WHERE [Disposals].[Id] = @Id", _Transaction.Connection, _Transaction);
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
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO [Disposals] ([Id], [Units], [AveragePrice], [TransactionCosts], [CGTMethod], [CreateCashTransaction]) VALUES (@Id, @Units, @AveragePrice, @TransactionCosts, @CGTMethod, @CreateCashTransaction)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Units", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@AveragePrice", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@TransactionCosts", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@CGTMethod", SqliteType.Integer);          
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
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE[Disposals] SET [Units] = @Units, [AveragePrice] = @AveragePrice, [TransactionCosts] = @TransactionCosts, [CGTMethod] = @CGTMethod , [CreateCashTransaction] = @CreateCashTransaction WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Units", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@AveragePrice", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@TransactionCosts", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@CGTMethod", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@CreateCashTransaction", SqliteType.Text);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }


        protected override void AddParameters(SqliteParameterCollection parameters, Transaction entity)
        {
            var disposal = entity as Disposal;

            parameters["@Id"].Value = disposal.Id.ToString();
            parameters["@Units"].Value = disposal.Units;
            parameters["@AveragePrice"].Value = SQLiteUtils.DecimalToDB(disposal.AveragePrice);
            parameters["@TransactionCosts"].Value = SQLiteUtils.DecimalToDB(disposal.TransactionCosts);
            parameters["@CGTMethod"].Value = disposal.CGTMethod;
            parameters["@CreateCashTransaction"].Value = SQLiteUtils.BoolToDb(disposal.CreateCashTransaction);
        }

    }
 
}
