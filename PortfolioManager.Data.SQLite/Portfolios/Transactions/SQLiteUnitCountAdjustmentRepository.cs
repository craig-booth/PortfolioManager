using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios.Transactions
{
    class SQLiteUnitCountAdjustmentRepository: SQLiteRepository<Transaction>
    {
        public SQLiteUnitCountAdjustmentRepository(SqliteTransaction transaction)
            : base(transaction, "UnitCountAdjustments", new SQLitePortfolioEntityCreator())
        {

        }

        private SqliteCommand _GetCurrentRecordCommand;
        protected override SqliteCommand GetCurrentRecordCommand()
        {
            if (_GetCurrentRecordCommand == null)
            {
                _GetCurrentRecordCommand = new SqliteCommand("SELECT [Transactions].*, [UnitCountAdjustments].* FROM [UnitCountAdjustments] LEFT OUTER JOIN [Transactions] ON [UnitCountAdjustments].[Id] = [Transactions].[Id] WHERE [UnitCountAdjustments].[Id] = @Id", _Transaction.Connection, _Transaction);
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
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO [UnitCountAdjustments] ([Id], [OriginalUnits], [NewUnits]) VALUES (@Id, @OriginalUnits, @NewUnits)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@OriginalUnits", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@NewUnits", SqliteType.Integer);

                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE [UnitCountAdjustments] SET [OriginalUnits] = @OriginalUnits, [NewUnits] = @NewUnits WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@OriginalUnits", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@NewUnits", SqliteType.Integer);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }


        protected override void AddParameters(SqliteParameterCollection parameters, Transaction entity)
        {
            var unitCountAdjustment = entity as UnitCountAdjustment;

            parameters["@Id"].Value = unitCountAdjustment.Id.ToString();
            parameters["@OriginalUnits"].Value = unitCountAdjustment.OriginalUnits;
            parameters["@NewUnits"].Value = unitCountAdjustment.NewUnits;
        }
    }
    
}
