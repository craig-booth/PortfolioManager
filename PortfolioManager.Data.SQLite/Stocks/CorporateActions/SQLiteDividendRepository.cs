using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks.CorporateActions
{
    class SQLiteDividendRepository : SQLiteRepository<CorporateAction>
    {

        public SQLiteDividendRepository(SqliteTransaction transaction, IEntityCreator entityCreator)
            : base(transaction, "Dividends", entityCreator)
        {

        }

        private SqliteCommand _AddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_AddRecordCommand == null)
            {
                _AddRecordCommand = new SqliteCommand("INSERT INTO [Dividends] ([Id], [PaymentDate], [DividendAmount], [CompanyTaxRate], [PercentFranked], [DRPPrice]) VALUES (@Id, @PaymentDate, @DividendAmount, @CompanyTaxRate, @PercentFranked, @DRPPrice)", _Transaction.Connection, _Transaction);

                _AddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _AddRecordCommand.Parameters.Add("@PaymentDate", SqliteType.Text);
                _AddRecordCommand.Parameters.Add("@DividendAmount", SqliteType.Integer);
                _AddRecordCommand.Parameters.Add("@CompanyTaxRate", SqliteType.Integer);
                _AddRecordCommand.Parameters.Add("@PercentFranked", SqliteType.Integer);
                _AddRecordCommand.Parameters.Add("@DRPPrice", SqliteType.Integer);

                _AddRecordCommand.Prepare();
            }        

            return _AddRecordCommand;
        }

        private SqliteCommand _UpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_UpdateRecordCommand == null)
            {
                _UpdateRecordCommand = new SqliteCommand("UPDATE [Dividends] SET [PaymentDate] = @PaymentDate, [DividendAmount] = @DividendAmount, [CompanyTaxRate] = @CompanyTaxRate, [PercentFranked] = @PercentFranked, [DRPPrice] = @DRPPrice WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _UpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _UpdateRecordCommand.Parameters.Add("@PaymentDate", SqliteType.Text);
                _UpdateRecordCommand.Parameters.Add("@DividendAmount", SqliteType.Integer);
                _UpdateRecordCommand.Parameters.Add("@CompanyTaxRate", SqliteType.Integer);
                _UpdateRecordCommand.Parameters.Add("@PercentFranked", SqliteType.Integer);
                _UpdateRecordCommand.Parameters.Add("@DRPPrice", SqliteType.Integer);

                _UpdateRecordCommand.Prepare();
            }

            return _UpdateRecordCommand;
        }

        protected override void AddParameters(SqliteParameterCollection parameters, CorporateAction entity)
        {
            var dividend = entity as Dividend;

            parameters["@Id"].Value = dividend.Id.ToString();
            parameters["@PaymentDate"].Value = dividend.PaymentDate.ToString("yyyy-MM-dd");
            parameters["@DividendAmount"].Value = SQLiteUtils.DecimalToDB(dividend.DividendAmount);
            parameters["@CompanyTaxRate"].Value = SQLiteUtils.DecimalToDB(dividend.CompanyTaxRate);
            parameters["@PercentFranked"].Value = SQLiteUtils.DecimalToDB(dividend.PercentFranked);
            parameters["@DRPPrice"].Value = SQLiteUtils.DecimalToDB(dividend.DRPPrice);
        }
    }
}
