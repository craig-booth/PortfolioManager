using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks.CorporateActions
{
    class SQLiteTransformationRepository : SQLiteRepository<CorporateAction>
    {

        public SQLiteTransformationRepository(SqliteTransaction transaction, IEntityCreator entityCreator)
            : base(transaction, "Transformations", entityCreator)
        {

        }

        private SqliteCommand _AddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_AddRecordCommand == null)
            {
                _AddRecordCommand = new SqliteCommand("INSERT INTO [Transformations] ([Id], [ImplementationDate], [CashComponent], [RolloverRelief]) VALUES (@Id, @ImplementationDate, @CashComponent, @RolloverRelief)", _Transaction.Connection, _Transaction);

                _AddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _AddRecordCommand.Parameters.Add("@ImplementationDate", SqliteType.Text);
                _AddRecordCommand.Parameters.Add("@CashComponent", SqliteType.Integer);
                _AddRecordCommand.Parameters.Add("@RolloverRelief", SqliteType.Text);

                _AddRecordCommand.Prepare();
            }

            return _AddRecordCommand;
        }

        private SqliteCommand _UpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_UpdateRecordCommand == null)
            {
                _UpdateRecordCommand = new SqliteCommand("UPDATE [Transformations] SET [ImplementationDate] = @ImplementationDate, [CashComponent] = @CashComponent, [RolloverRelief] = @RolloverRelief WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _UpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _UpdateRecordCommand.Parameters.Add("@ImplementationDate", SqliteType.Text);
                _UpdateRecordCommand.Parameters.Add("@CashComponent", SqliteType.Integer);
                _UpdateRecordCommand.Parameters.Add("@RolloverRelief", SqliteType.Text);

                _UpdateRecordCommand.Prepare();
            }

            return _UpdateRecordCommand;
        }

        public override void Add(CorporateAction entity)
        {
            base.Add(entity);

            /* Add result stocks */
            var transformation = entity as Transformation;
            foreach (ResultingStock resultStock in transformation.ResultingStocks)
                AddResultRecord(transformation.Id, resultStock);
        }

        public override void Update(CorporateAction entity)
        {
            base.Update(entity);

            /* Remove and Add result stocks */
            var transformation = entity as Transformation;
            DeleteResultRecords(transformation.Id);
            foreach (ResultingStock resultStock in transformation.ResultingStocks)
                AddResultRecord(transformation.Id, resultStock);
        }

        public override void Delete(CorporateAction entity)
        {
            base.Delete(entity);

            /* Remove result stocks */
            var transformation = entity as Transformation;
            DeleteResultRecords(transformation.Id);
        }

        public override void Delete(Guid id)
        {
            var corporateAction = Get(id);

            if (corporateAction != null)
                Delete(corporateAction);
        }

        protected override void AddParameters(SqliteParameterCollection parameters, CorporateAction entity)
        {
            var transformation = entity as Transformation;

            parameters["@Id"].Value = transformation.Id.ToString();
            parameters["@ImplementationDate"].Value = transformation.ImplementationDate.ToString("yyyy-MM-dd");
            parameters["@CashComponent"].Value = SQLiteUtils.DecimalToDB(transformation.CashComponent);
            parameters["@RolloverRelief"].Value = SQLiteUtils.BoolToDb(transformation.RolloverRefliefApplies);
        }

        private SqliteCommand _AddResultRecordCommand;
        private void AddResultRecord(Guid transformationId, ResultingStock entity)
        {
            if (_AddResultRecordCommand == null)
            {
                _AddResultRecordCommand = new SqliteCommand("INSERT INTO [TransformationResultingStocks] ([Id], [Stock], [OriginalUnits], [NewUnits], [CostBasePercentage], [AquisitionDate]) VALUES (@Id, @Stock, @OriginalUnits, @NewUnits, @CostBasePercentage, @AquisitionDate)", _Transaction.Connection, _Transaction);

                _AddResultRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _AddResultRecordCommand.Parameters.Add("@Stock", SqliteType.Text);
                _AddResultRecordCommand.Parameters.Add("@OriginalUnits", SqliteType.Integer);
                _AddResultRecordCommand.Parameters.Add("@NewUnits", SqliteType.Integer);
                _AddResultRecordCommand.Parameters.Add("@CostBasePercentage", SqliteType.Integer);
                _AddResultRecordCommand.Parameters.Add("@AquisitionDate", SqliteType.Text);

                _AddResultRecordCommand.Prepare();
            }

            _AddResultRecordCommand.Parameters["@Id"].Value = transformationId.ToString();
            _AddResultRecordCommand.Parameters["@Stock"].Value = entity.Stock.ToString();
            _AddResultRecordCommand.Parameters["@OriginalUnits"].Value = entity.OriginalUnits;
            _AddResultRecordCommand.Parameters["@NewUnits"].Value = entity.NewUnits;
            _AddResultRecordCommand.Parameters["@CostBasePercentage"].Value = SQLiteUtils.DecimalToDB(entity.CostBase);
            _AddResultRecordCommand.Parameters["@AquisitionDate"].Value = entity.AquisitionDate.ToString("yyyy-MM-dd");

            _AddResultRecordCommand.ExecuteNonQuery();
        }

        private SqliteCommand _DeleteResultRecordsCommand;
        private void DeleteResultRecords(Guid id)
        {
            if (_DeleteResultRecordsCommand == null)
            {
                _DeleteResultRecordsCommand = new SqliteCommand("DELETE FROM [TransformationResultingStocks] WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _DeleteResultRecordsCommand.Parameters.Add("@Id", SqliteType.Text);

                _DeleteResultRecordsCommand.Prepare();
            }

            _DeleteResultRecordsCommand.Parameters["@Id"].Value = id.ToString();
            _DeleteResultRecordsCommand.ExecuteNonQuery();
        }
    }
}
