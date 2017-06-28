using System;
using Microsoft.Data.Sqlite;
using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class DividendDetailRepository : ICorporateActionDetailRepository
    {
        private SqliteConnection _Connection;

        public DividendDetailRepository(SqliteConnection connection)
        {
            _Connection = connection;
        }

        private SqliteCommand _AddRecordCommand;
        public void Add(Entity entity)
        {
            var dividend = entity as Dividend;

            if (_AddRecordCommand == null)
            {
                _AddRecordCommand = new SqliteCommand("INSERT INTO [Dividends] ([Id], [PaymentDate], [DividendAmount], [CompanyTaxRate], [PercentFranked], [DRPPrice]) VALUES (@Id, @PaymentDate, @DividendAmount, @CompanyTaxRate, @PercentFranked, @DRPPrice)", _Connection);
                _AddRecordCommand.Prepare();
            }

            AddParameters(_AddRecordCommand, dividend);
            _AddRecordCommand.ExecuteNonQuery();
        }

        private SqliteCommand _UpdateRecordCommand;
        public void Update(Entity entity)
        {
            var dividend = entity as Dividend;

            if (_UpdateRecordCommand == null)
            {
                _UpdateRecordCommand = new SqliteCommand("UPDATE [Dividends] SET [PaymentDate] = @PaymentDate, [DividendAmount] = @DividendAmount, [CompanyTaxRate] = @CompanyTaxRate, [PercentFranked] = @PercentFranked, [DRPPrice] = @DRPPrice WHERE [Id] = @Id", _Connection);
                _UpdateRecordCommand.Prepare();
            }

            AddParameters(_UpdateRecordCommand, dividend);
            _UpdateRecordCommand.ExecuteNonQuery();
        }     

        private SqliteCommand _DeleteRecordCommand;
        public void Delete(Guid id)
        {
            if (_DeleteRecordCommand == null)
            {
                _DeleteRecordCommand = new SqliteCommand("DELETE FROM [Dividends] WHERE [Id] = @Id", _Connection);
                _DeleteRecordCommand.Prepare();
            }

            _DeleteRecordCommand.Parameters.AddWithValue("@Id", id.ToString());
            _DeleteRecordCommand.ExecuteNonQuery();
        }

        private void AddParameters(SqliteCommand command, Dividend entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@PaymentDate", entity.PaymentDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@DividendAmount", SQLiteUtils.DecimalToDB(entity.DividendAmount));
            command.Parameters.AddWithValue("@CompanyTaxRate", SQLiteUtils.DecimalToDB(entity.CompanyTaxRate));
            command.Parameters.AddWithValue("@PercentFranked", SQLiteUtils.DecimalToDB(entity.PercentFranked));
            command.Parameters.AddWithValue("@DRPPrice", SQLiteUtils.DecimalToDB(entity.DRPPrice));
        }

    }
}
