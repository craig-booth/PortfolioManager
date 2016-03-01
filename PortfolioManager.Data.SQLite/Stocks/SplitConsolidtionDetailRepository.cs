using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class SplitConsolidtionDetailRepository : ICorporateActionDetailRepository
    {
        private SQLiteConnection _Connection;

        public SplitConsolidtionDetailRepository(SQLiteConnection connection)
        {
            _Connection = connection;
        }

        private SQLiteCommand _AddRecordCommand;
        public void Add(IEntity entity)
        {
            var splitConsolidation = entity as SplitConsolidation;

            if (_AddRecordCommand == null)
            {
                _AddRecordCommand = new SQLiteCommand("INSERT INTO [SplitConsolidations] ([Id], [OldUnits], [NewUnits]) VALUES (@Id, @OldUnits, @NewUnits)", _Connection);
                _AddRecordCommand.Prepare();
            }

            AddParameters(_AddRecordCommand, splitConsolidation);
            _AddRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _UpdateRecordCommand;
        public void Update(IEntity entity)
        {
            var splitConsolidation = entity as SplitConsolidation;

            if (_UpdateRecordCommand == null)
            {
                _UpdateRecordCommand = new SQLiteCommand("UPDATE [SplitConsolidations] SET [OldUnits] = @OldUnits, [NewUnits] = @NewUnits WHERE [Id] = @Id", _Connection);

                _UpdateRecordCommand.Prepare();
            }

            AddParameters(_UpdateRecordCommand, splitConsolidation);
            _UpdateRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _DeleteRecordCommand;
        public void Delete(Guid id)
        {
            if (_DeleteRecordCommand == null)
            {
                _DeleteRecordCommand = new SQLiteCommand("DELETE FROM [SplitConsolidations] WHERE [Id] = @Id", _Connection);
                _DeleteRecordCommand.Prepare();
            }

            _DeleteRecordCommand.Parameters.AddWithValue("@Id", id.ToString());
            _DeleteRecordCommand.ExecuteNonQuery();
        }

        private void AddParameters(SQLiteCommand command, SplitConsolidation entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@OldUnits", entity.OldUnits);
            command.Parameters.AddWithValue("@NewUnits", entity.NewUnits);
        }

    }
}
