using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios.Transactions
{
    class SQLiteCashTransactionsRepository :SQLiteRepository<Transaction>
    {
        public SQLiteCashTransactionsRepository(SqliteTransaction transaction)
            : base(transaction, "CashTransactions", new SQLitePortfolioEntityCreator())
        {

        }

        private SqliteCommand _GetCurrentRecordCommand;
        protected override SqliteCommand GetCurrentRecordCommand()
        {
            if (_GetCurrentRecordCommand == null)
            {
                _GetCurrentRecordCommand = new SqliteCommand("SELECT [Transactions].*, [CashTransactions].* FROM [CashTransactions] LEFT OUTER JOIN [Transactions] ON [CashTransactions].[Id] = [Transactions].[Id] WHERE [CashTransactions].[Id] = @Id", _Transaction.Connection, _Transaction);
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
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO [CashTransactions] ([Id], [Type], [Amount]) VALUES (@Id, @Type, @Amount)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Type", SqliteType.Integer);
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
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE [CashTransactions] SET [Type] = @Type, [Amount] = @Amount WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Type", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@Amount", SqliteType.Integer);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }


        protected override void AddParameters(SqliteParameterCollection parameters, Transaction entity)
        {
            var cashTransaction = entity as CashTransaction;

            parameters["@Id"].Value = cashTransaction.Id.ToString();
            parameters["@Type"].Value = cashTransaction.Type;
            parameters["@Amount"].Value = SQLiteUtils.DecimalToDB(cashTransaction.Amount);
        }
    }

}
