using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data;
using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class CapitalReturnDetailRepository : ICorporateActionDetailRepository
    {
        private SqliteConnection _Connection;

        public CapitalReturnDetailRepository(SqliteConnection connection)
        {
            _Connection = connection;
        }

        private SqliteCommand _AddRecordCommand;
        public void Add(Entity entity)
        {
            var capitalReturn = entity as CapitalReturn;

            if (_AddRecordCommand == null)
            {
                _AddRecordCommand = new SqliteCommand("INSERT INTO [CapitalReturns] ([Id], [PaymentDate], [Amount]) VALUES (@Id, @PaymentDate, @Amount)", _Connection);
                _AddRecordCommand.Prepare();
            }

            AddParameters(_AddRecordCommand, capitalReturn);
            _AddRecordCommand.ExecuteNonQuery();
        }

        private SqliteCommand _UpdateRecordCommand;
        public void Update(Entity entity)
        {
            var capitalReturn = entity as CapitalReturn;

            if (_UpdateRecordCommand == null)
            {
                _UpdateRecordCommand = new SqliteCommand("UPDATE [CapitalReturns] SET [PaymentDate] = @PaymentDate, [Amount] = @Amount WHERE [Id] = @Id", _Connection);
                _UpdateRecordCommand.Prepare();
            }

            AddParameters(_UpdateRecordCommand, capitalReturn);
            _UpdateRecordCommand.ExecuteNonQuery();
        }

        private SqliteCommand _DeleteRecordCommand;
        public void Delete(Guid id)
        {
            if (_DeleteRecordCommand == null)
            {
                _DeleteRecordCommand = new SqliteCommand("DELETE FROM [CapitalReturns] WHERE [Id] = @Id", _Connection);
                _DeleteRecordCommand.Prepare();
            }

            _DeleteRecordCommand.Parameters.AddWithValue("@Id", id.ToString());
            _DeleteRecordCommand.ExecuteNonQuery();
        }

        private void AddParameters(SqliteCommand command, CapitalReturn entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@PaymentDate", entity.PaymentDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Amount", SQLiteUtils.DecimalToDB(entity.Amount));
        }
    }
}
