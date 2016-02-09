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
    class TransformationDetailRepository : ICorporateActionDetailRepository
    {
        private SQLiteConnection _Connection;

        public TransformationDetailRepository(SQLiteConnection connection)
        {
            _Connection = connection;
        }

        private SQLiteCommand _AddRecordCommand;
        public void Add(IEntity entity)
        {
            var transformation = entity as Transformation;

            if (_AddRecordCommand == null)
            {
                _AddRecordCommand = new SQLiteCommand("INSERT INTO [Transformations] ([Id], [ImplementationDate], [CashComponent]) VALUES (@Id, @ImplementationDate, @CashComponent)", _Connection);
                _AddRecordCommand.Prepare();
            }

            AddParameters(_AddRecordCommand, transformation);
            _AddRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _UpdateRecordCommand;
        public void Update(IEntity entity)
        {
            var transformation = entity as Transformation;

            if (_UpdateRecordCommand == null)
            {
                _UpdateRecordCommand = new SQLiteCommand("UPDATE [Transformations] SET [ImplementationDate] = @ImplementationDate, [CashComponent] = @CashComponent WHERE [Id] = @Id", _Connection);
                _UpdateRecordCommand.Prepare();
            }

            AddParameters(_UpdateRecordCommand, entity as Transformation);
            _UpdateRecordCommand.ExecuteNonQuery();

            /* Update result stocks */
            DeleteTransformationResultRecordRecords(entity.Id);
            foreach (ResultingStock resultStock in transformation.ResultingStocks)
                AddResultRecord(entity.Id, resultStock);
        }

        private SQLiteCommand _DeleteRecordCommand;
        public void Delete(Guid id)
        {
            /* Delete transformation record */
            if (_DeleteRecordCommand == null)
            {
                _DeleteRecordCommand = new SQLiteCommand("DELETE FROM [Transformations] WHERE [Id] = @Id", _Connection);
                _DeleteRecordCommand.Prepare();
            }

            _DeleteRecordCommand.Parameters.AddWithValue("@Id", id.ToString());
            _DeleteRecordCommand.ExecuteNonQuery();
        }

        private void AddParameters(SQLiteCommand command, Transformation entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@ImplementationDate", entity.ImplementationDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@CashComponent", SQLiteUtils.DecimalToDB(entity.CashComponent));
        }

        private SQLiteCommand _AddResultRecordCommand;
        private void AddResultRecord(Guid transformationId, ResultingStock entity)
        {
            if (_AddResultRecordCommand == null)
            {
                _AddResultRecordCommand = new SQLiteCommand("INSERT INTO [TransformationResultingStocks] ([Id], [Stock], [OriginalUnits], [NewUnits], [CostBasePercentage]) VALUES (@Id, @Stock, @OriginalUnits, @NewUnits, @CostBasePercentage)", _Connection);
                _AddResultRecordCommand.Prepare();
            }
            _AddResultRecordCommand.Parameters.AddWithValue("@Id", transformationId.ToString());
            _AddResultRecordCommand.Parameters.AddWithValue("@Stock", entity.Stock.ToString());
            _AddResultRecordCommand.Parameters.AddWithValue("@OriginalUnits", entity.OriginalUnits);
            _AddResultRecordCommand.Parameters.AddWithValue("@NewUnits", entity.NewUnits);
            _AddResultRecordCommand.Parameters.AddWithValue("@CostBasePercentage", SQLiteUtils.DecimalToDB(entity.CostBasePercentage));

            _AddResultRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _DeleteResultRecordsCommand;
        private void DeleteTransformationResultRecordRecords(Guid id)
        {
            if (_DeleteResultRecordsCommand == null)
            {
                _DeleteResultRecordsCommand = new SQLiteCommand("DELETE FROM [TransformationResultingStocks] WHERE [Id] = @Id", _Connection);
                _DeleteResultRecordsCommand.Prepare();
            }

            _DeleteResultRecordsCommand.Parameters.AddWithValue("@Id", id.ToString());
            _DeleteResultRecordsCommand.ExecuteNonQuery();
        }
    }
}
