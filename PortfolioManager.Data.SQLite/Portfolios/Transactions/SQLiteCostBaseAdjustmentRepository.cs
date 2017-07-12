using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios.Transactions
{
    class SQLiteCostBaseAdjustmentRepository : SQLiteRepository<Transaction>
    {

        public SQLiteCostBaseAdjustmentRepository(SqliteTransaction transaction)
            : base(transaction, "CostBaseAdjustments", new SQLitePortfolioEntityCreator())
        {

        }

        private SqliteCommand _GetCurrentRecordCommand;
        protected override SqliteCommand GetCurrentRecordCommand()
        {
            if (_GetCurrentRecordCommand == null)
            {
                _GetCurrentRecordCommand = new SqliteCommand("SELECT [Transactions].*, [CostBaseAdjustments].* FROM [CostBaseAdjustments] LEFT OUTER JOIN [Transactions] ON [CostBaseAdjustments].[Id] = [Transactions].[Id] WHERE [CostBaseAdjustments].[Id] = @Id", _Transaction.Connection, _Transaction);
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
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO [CostBaseAdjustments] ([Id], [Percentage]) VALUES (@Id, @Percentage)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Percentage", SqliteType.Integer);

                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE[CostBaseAdjustments] SET [Percentage] = @Percentage WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Percentage", SqliteType.Integer);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }


        protected override void AddParameters(SqliteParameterCollection parameters, Transaction entity)
        {
            var costBaseAdjustment = entity as CostBaseAdjustment;

            parameters["@Id"].Value = costBaseAdjustment.Id.ToString();
            parameters["@Percentage"].Value = SQLiteUtils.DecimalToDB(costBaseAdjustment.Percentage);
        }
        

    }
}
