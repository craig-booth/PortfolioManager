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
    class SplitConsolidtionDetailRepository : ICorporateActionDetailRepository
    {
        private SqliteConnection _Connection;

        public SplitConsolidtionDetailRepository(SqliteConnection connection)
        {
            _Connection = connection;
        }

        private SqliteCommand _AddRecordCommand;
        public void Add(Entity entity)
        {
            var splitConsolidation = entity as SplitConsolidation;

            if (_AddRecordCommand == null)
            {
                _AddRecordCommand = new SqliteCommand("INSERT INTO [SplitConsolidations] ([Id], [OldUnits], [NewUnits]) VALUES (@Id, @OldUnits, @NewUnits)", _Connection);
                _AddRecordCommand.Prepare();
            }

            AddParameters(_AddRecordCommand, splitConsolidation);
            _AddRecordCommand.ExecuteNonQuery();
        }

        private SqliteCommand _UpdateRecordCommand;
        public void Update(Entity entity)
        {
            var splitConsolidation = entity as SplitConsolidation;

            if (_UpdateRecordCommand == null)
            {
                _UpdateRecordCommand = new SqliteCommand("UPDATE [SplitConsolidations] SET [OldUnits] = @OldUnits, [NewUnits] = @NewUnits WHERE [Id] = @Id", _Connection);

                _UpdateRecordCommand.Prepare();
            }

            AddParameters(_UpdateRecordCommand, splitConsolidation);
            _UpdateRecordCommand.ExecuteNonQuery();
        }

        private SqliteCommand _DeleteRecordCommand;
        public void Delete(Guid id)
        {
            if (_DeleteRecordCommand == null)
            {
                _DeleteRecordCommand = new SqliteCommand("DELETE FROM [SplitConsolidations] WHERE [Id] = @Id", _Connection);
                _DeleteRecordCommand.Prepare();
            }

            _DeleteRecordCommand.Parameters.AddWithValue("@Id", id.ToString());
            _DeleteRecordCommand.ExecuteNonQuery();
        }

        private void AddParameters(SqliteCommand command, SplitConsolidation entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@OldUnits", entity.OldUnits);
            command.Parameters.AddWithValue("@NewUnits", entity.NewUnits);
        }

    }
}
