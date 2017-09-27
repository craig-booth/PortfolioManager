using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks.CorporateActions
{
    class SQLiteSplitConsolidationRepository : SQLiteRepository<CorporateAction>
    {

        public SQLiteSplitConsolidationRepository(SqliteTransaction transaction, IEntityCreator entityCreator)
            : base(transaction, "SplitConsolidations", entityCreator)
        {

        }

        private SqliteCommand _AddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_AddRecordCommand == null)
            {
                _AddRecordCommand = new SqliteCommand("INSERT INTO [SplitConsolidations] ([Id], [OldUnits], [NewUnits]) VALUES (@Id, @OldUnits, @NewUnits)", _Transaction.Connection, _Transaction);

                _AddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _AddRecordCommand.Parameters.Add("@OldUnits", SqliteType.Integer);
                _AddRecordCommand.Parameters.Add("@NewUnits", SqliteType.Integer);

                _AddRecordCommand.Prepare();
            }

            return _AddRecordCommand;
        }

        private SqliteCommand _UpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_UpdateRecordCommand == null)
            {
                _UpdateRecordCommand = new SqliteCommand("UPDATE [SplitConsolidations] SET [OldUnits] = @OldUnits, [NewUnits] = @NewUnits WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _UpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _UpdateRecordCommand.Parameters.Add("@OldUnits", SqliteType.Integer);
                _UpdateRecordCommand.Parameters.Add("@NewUnits", SqliteType.Integer);

                _UpdateRecordCommand.Prepare();
            }

            return _UpdateRecordCommand;
        }

        protected override void AddParameters(SqliteParameterCollection parameters, CorporateAction entity)
        {
            var splitConsolidation = entity as SplitConsolidation;

            parameters["@Id"].Value = splitConsolidation.Id.ToString();
            parameters["@OldUnits"].Value = splitConsolidation.OldUnits;
            parameters["@NewUnits"].Value = splitConsolidation.NewUnits;
        }
    }
}
