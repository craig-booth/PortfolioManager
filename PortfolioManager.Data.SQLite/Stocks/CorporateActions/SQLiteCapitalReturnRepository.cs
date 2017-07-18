using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks.CorporateActions
{
    class SQLiteCapitalReturnRepository : SQLiteRepository<CorporateAction>
    {

        public SQLiteCapitalReturnRepository(SqliteTransaction transaction, IEntityCreator entityCreator)
            : base(transaction, "CapitalReturns", entityCreator)
        {

        }

        private SqliteCommand _AddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_AddRecordCommand == null)
            {
                _AddRecordCommand = new SqliteCommand("INSERT INTO [CapitalReturns] ([Id], [PaymentDate], [Amount]) VALUES (@Id, @PaymentDate, @Amount)", _Transaction.Connection, _Transaction);

                _AddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _AddRecordCommand.Parameters.Add("@PaymentDate", SqliteType.Text);
                _AddRecordCommand.Parameters.Add("@Amount", SqliteType.Integer);

                _AddRecordCommand.Prepare();
            }       

            return _AddRecordCommand;
        }

        private SqliteCommand _UpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_UpdateRecordCommand == null)
            {
                _UpdateRecordCommand = new SqliteCommand("UPDATE [CapitalReturns] SET [PaymentDate] = @PaymentDate, [Amount] = @Amount WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _UpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _UpdateRecordCommand.Parameters.Add("@PaymentDate", SqliteType.Text);
                _UpdateRecordCommand.Parameters.Add("@Amount", SqliteType.Integer);

                _UpdateRecordCommand.Prepare();
            }

            return _UpdateRecordCommand;
        }

        protected override void AddParameters(SqliteParameterCollection parameters, CorporateAction entity)
        {
            var capitalReturn = entity as CapitalReturn;

            parameters["@Id"].Value = capitalReturn.Id.ToString();
            parameters["@PaymentDate"].Value = capitalReturn.PaymentDate.ToString("yyyy-MM-dd");
            parameters["@Amount"].Value = SQLiteUtils.DecimalToDB(capitalReturn.Amount);
        }
    }
}
