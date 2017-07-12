using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios.Transactions
{
    class SQLiteOpeningBalanceRepository : SQLiteRepository<Transaction>
    {
        public SQLiteOpeningBalanceRepository(SqliteTransaction transaction)
            : base(transaction, "OpeningBalances", new SQLitePortfolioEntityCreator())
        {

        }

        private SqliteCommand _GetCurrentRecordCommand;
        protected override SqliteCommand GetCurrentRecordCommand()
        {
            if (_GetCurrentRecordCommand == null)
            {
                _GetCurrentRecordCommand = new SqliteCommand("SELECT [Transactions].*, [OpeningBalances].* FROM [OpeningBalances] LEFT OUTER JOIN [Transactions] ON [OpeningBalances].[Id] = [Transactions].[Id] WHERE [OpeningBalances].[Id] = @Id", _Transaction.Connection, _Transaction);
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
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO [OpeningBalances] ([Id], [Units], [CostBase], [AquisitionDate], [PurchaseId]) VALUES (@Id, @Units, @CostBase, @AquisitionDate, @PurchaseId)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Units", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@CostBase", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@AquisitionDate", SqliteType.Text);
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
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE [OpeningBalances] SET [Units] = @Units, [CostBase] = @CostBase, [AquisitionDate] = @AquisitionDate, [PurchaseId] = @PurchaseId WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Units", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@CostBase", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@AquisitionDate", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@PurchaseId", SqliteType.Text);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }


        protected override void AddParameters(SqliteParameterCollection parameters, Transaction entity)
        {
            var openingBalance = entity as OpeningBalance;

            parameters["@Id"].Value = openingBalance.Id.ToString();
            parameters["@Units"].Value = openingBalance.Units;
            parameters["@CostBase"].Value = SQLiteUtils.DecimalToDB(openingBalance.CostBase);
            parameters["@AquisitionDate"].Value = openingBalance.TransactionDate.ToString("yyyy-MM-dd");
            parameters["@PurchaseId"].Value = openingBalance.PurchaseId.ToString();
        }
    }
}
