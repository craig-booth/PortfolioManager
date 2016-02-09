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
    class SQLiteCorporateActionRepository : SQLiteRepository<ICorporateAction>, ICorporateActionRepository 
    {
        protected internal SQLiteCorporateActionRepository(SQLiteStockDatabase database)
            : base(database, "CorporateActions")
        {
        }

        private SQLiteCommand _GetAddRecordCommand;
        protected override SQLiteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SQLiteCommand("INSERT INTO [CorporateActions] ([Id], [Stock], [ActionDate], [Description], [Type]) VALUES (@Id, @Stock, @ActionDate, @Description, @Type)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SQLiteCommand _GetUpdateRecordCommand;
        protected override SQLiteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SQLiteCommand("UPDATE [CorporateActions] SET [ActionDate] = @ActionDate, [Description] = @Description WHERE [Id] = @Id", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        public override void Add(ICorporateAction entity) 
        {
            /* Add header record */
            base.Add(entity);

            if (entity.Type == CorporateActionType.Dividend)
                AddDividendRecord(entity as Dividend);
            else if (entity.Type == CorporateActionType.CapitalReturn)
                AddCapitalReturnRecord(entity as CapitalReturn);
            else if (entity.Type == CorporateActionType.Transformation)
                AddTransformationRecord(entity as Transformation);
            else if (entity.Type == CorporateActionType.SplitConsolidation)
                AddSplitConsolidationRecord(entity as SplitConsolidation);
            else
                throw new NotSupportedException();
        }      	

        private SQLiteCommand _AddDivendedRecordCommand;
        private void AddDividendRecord(Dividend entity)
        {
            if (_AddDivendedRecordCommand == null)
            {
                _AddDivendedRecordCommand = new SQLiteCommand("INSERT INTO [Dividends] ([Id], [PaymentDate], [DividendAmount], [CompanyTaxRate], [PercentFranked], [DRPPrice]) VALUES (@Id, @PaymentDate, @DividendAmount, @CompanyTaxRate, @PercentFranked, @DRPPrice)", _Connection);
                _AddDivendedRecordCommand.Prepare();
            }

            AddDividendParameters(_AddDivendedRecordCommand, entity);
            _AddDivendedRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _AddCapitalReturnRecordCommand;	
        private void AddCapitalReturnRecord(CapitalReturn entity)
        {
            if (_AddCapitalReturnRecordCommand == null)
            {
                _AddCapitalReturnRecordCommand = new SQLiteCommand("INSERT INTO [CapitalReturns] ([Id], [PaymentDate], [Amount]) VALUES (@Id, @PaymentDate, @Amount)", _Connection);
                _AddCapitalReturnRecordCommand.Prepare();
            }

            AddCapitalReturnParameters(_AddCapitalReturnRecordCommand, entity);
            _AddCapitalReturnRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _AddSplitConsolidationRecordCommand;
        private void AddSplitConsolidationRecord(SplitConsolidation entity)
        {
            if (_AddSplitConsolidationRecordCommand == null)
            {
                _AddSplitConsolidationRecordCommand = new SQLiteCommand("INSERT INTO [SplitConsolidations] ([Id], [OldUnits], [NewUnits]) VALUES (@Id, @OldUnits, @NewUnits)", _Connection);
                _AddSplitConsolidationRecordCommand.Prepare();
            }

            AddSplitConsolidationParameters(_AddSplitConsolidationRecordCommand, entity);
            _AddSplitConsolidationRecordCommand.ExecuteNonQuery();
        }


        private SQLiteCommand _AddTransformationRecordCommand;
        private void AddTransformationRecord(Transformation entity)
        {
            if (_AddTransformationRecordCommand == null)
            {
                _AddTransformationRecordCommand = new SQLiteCommand("INSERT INTO [Transformations] ([Id], [ImplementationDate], [CashComponent], [RolloverRelief]) VALUES (@Id, @ImplementationDate, @CashComponent, @RolloverRelief)", _Connection);
                _AddTransformationRecordCommand.Prepare();
            }

            AddTransformationParameters(_AddTransformationRecordCommand, entity);
            _AddTransformationRecordCommand.ExecuteNonQuery();
        }

        public override void Update(ICorporateAction entity)
        {
            /* Update header record */
            base.Update(entity);

            if (entity.Type == CorporateActionType.Dividend)
                UpdateDividendRecord(entity as Dividend);
            else if (entity.Type == CorporateActionType.CapitalReturn)
                UpdateCapitalReturnRecord(entity as CapitalReturn);
            else if (entity.Type == CorporateActionType.Transformation)
                UpdateTransformationRecord(entity as Transformation);
            else if (entity.Type == CorporateActionType.SplitConsolidation)
                UpdateSplitConsolidationRecord(entity as SplitConsolidation);
            else
                throw new NotSupportedException();
        }

        private SQLiteCommand _UpdateDivendedRecordCommand;
        private void UpdateDividendRecord(Dividend entity)
        {
            if (_UpdateDivendedRecordCommand == null)
            {
                _UpdateDivendedRecordCommand = new SQLiteCommand("UPDATE [Dividends] SET [PaymentDate] = @PaymentDate, [DividendAmount] = @DividendAmount, [CompanyTaxRate] = @CompanyTaxRate, [PercentFranked] = @PercentFranked, [DRPPrice] = @DRPPrice WHERE [Id] = @Id", _Connection);
                _UpdateDivendedRecordCommand.Prepare();
            }

            AddDividendParameters(_UpdateDivendedRecordCommand, entity);
            _UpdateDivendedRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _UpdateCapitalReturnRecordCommand;
        private void UpdateCapitalReturnRecord(CapitalReturn entity)
        {
            if (_UpdateCapitalReturnRecordCommand == null)
            {
                _UpdateCapitalReturnRecordCommand = new SQLiteCommand("UPDATE [CapitalReturns] SET [PaymentDate] = @PaymentDate, [Amount] = @Amount WHERE [Id] = @Id", _Connection);
                _UpdateCapitalReturnRecordCommand.Prepare();
            }

            AddCapitalReturnParameters(_UpdateCapitalReturnRecordCommand, entity);
            _UpdateCapitalReturnRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _UpdateSplitConsolidationRecordCommand;
        private void UpdateSplitConsolidationRecord(SplitConsolidation entity)
        {
            if (_UpdateSplitConsolidationRecordCommand == null)
            {
                _UpdateSplitConsolidationRecordCommand = new SQLiteCommand("UPDATE [SplitConsolidations] SET [OldUnits] = @OldUnits, [NewUnits] = @NewUnits WHERE [Id] = @Id", _Connection);
                _UpdateSplitConsolidationRecordCommand.Prepare();
            }

            AddSplitConsolidationParameters(_UpdateSplitConsolidationRecordCommand, entity);
            _UpdateSplitConsolidationRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _UpdateTransformationRecordCommand;
        private void UpdateTransformationRecord(Transformation entity)
        {
            if (_UpdateTransformationRecordCommand == null)
            {
                _UpdateTransformationRecordCommand = new SQLiteCommand("UPDATE [Transformations] SET [ImplementationDate] = @ImplementationDate, [CashComponent] = @CashComponent, [RolloverRelief] = @RolloverRelief WHERE [Id] = @Id", _Connection);
                _UpdateTransformationRecordCommand.Prepare();
            }

            AddTransformationParameters(_UpdateTransformationRecordCommand, entity);
            _UpdateTransformationRecordCommand.ExecuteNonQuery();

            /* Update result stocks */
            DeleteTransformationResultRecordRecords(entity.Id);
            foreach (ResultingStock resultStock in entity.ResultingStocks)
                AddTransformationResultRecord(entity.Id, resultStock);
        }

        private SQLiteCommand _AddTransformationResultRecordCommand;
        private void AddTransformationResultRecord(Guid transformationId, ResultingStock entity)
        {
            if (_AddTransformationResultRecordCommand == null)
            {
                _AddTransformationResultRecordCommand = new SQLiteCommand("INSERT INTO [TransformationResultingStocks] ([Id], [Stock], [OriginalUnits], [NewUnits], [CostBasePercentage], [AquisitionDate]) VALUES (@Id, @Stock, @OriginalUnits, @NewUnits, @CostBasePercentage, @AquisitionDate)", _Connection);
                _AddTransformationResultRecordCommand.Prepare();
            }
            _AddTransformationResultRecordCommand.Parameters.AddWithValue("@Id", transformationId.ToString());
            _AddTransformationResultRecordCommand.Parameters.AddWithValue("@Stock", entity.Stock.ToString());
            _AddTransformationResultRecordCommand.Parameters.AddWithValue("@OriginalUnits", entity.OriginalUnits);
            _AddTransformationResultRecordCommand.Parameters.AddWithValue("@NewUnits", entity.NewUnits);
            _AddTransformationResultRecordCommand.Parameters.AddWithValue("@CostBasePercentage", DecimalToDB(entity.CostBase));
            _AddTransformationResultRecordCommand.Parameters.AddWithValue("@AquisitionDate", entity.AquisitionDate.ToString("yyyy-MM-dd"));

            _AddTransformationResultRecordCommand.ExecuteNonQuery();
        }

        public override void Delete(ICorporateAction entity)
        {
            /* Delete header record */
            base.Delete(entity);

            if (entity.Type == CorporateActionType.Dividend)
                DeleteDividendRecord(entity.Id);
            else if (entity.Type == CorporateActionType.CapitalReturn)
                DeleteCapitalReturnRecord(entity.Id);
            else if (entity.Type == CorporateActionType.Transformation)
                DeleteTransformationRecord(entity.Id);
            else if (entity.Type == CorporateActionType.SplitConsolidation)
                DeleteSplitConsolidationRecord(entity.Id);
            else
                throw new NotSupportedException();
        }

        public override void Delete(Guid id)
        {
            /* Delete header record */
            base.Delete(id);

            /* We don't know what the type is so just try deleting all types */
            DeleteDividendRecord(id);
            DeleteCapitalReturnRecord(id);
            DeleteTransformationRecord(id);
            DeleteSplitConsolidationRecord(id);
        }

        private SQLiteCommand _DeleteDivendedRecordCommand;
        private void DeleteDividendRecord(Guid id)
        {
            if (_DeleteDivendedRecordCommand == null)
            {
                _DeleteDivendedRecordCommand = new SQLiteCommand("DELETE FROM [Dividends] WHERE [Id] = @Id", _Connection);
                _DeleteDivendedRecordCommand.Prepare();
            }

            _DeleteDivendedRecordCommand.Parameters.AddWithValue("@Id", id.ToString());
            _DeleteDivendedRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _DeleteCapitalReturnRecordCommand;
        private void DeleteCapitalReturnRecord(Guid id)
        {
            if (_DeleteCapitalReturnRecordCommand == null)
            {
                _DeleteCapitalReturnRecordCommand = new SQLiteCommand("DELETE FROM [CapitalReturns] WHERE [Id] = @Id", _Connection);
                _DeleteCapitalReturnRecordCommand.Prepare();
            }

            _DeleteCapitalReturnRecordCommand.Parameters.AddWithValue("@Id", id.ToString());
            _DeleteCapitalReturnRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _DeleteSplitConsolidationRecordCommand;
        private void DeleteSplitConsolidationRecord(Guid id)
        {
            if (_DeleteSplitConsolidationRecordCommand == null)
            {
                _DeleteSplitConsolidationRecordCommand = new SQLiteCommand("DELETE FROM [SplitConsolidations] WHERE [Id] = @Id", _Connection);
                _DeleteSplitConsolidationRecordCommand.Prepare();
            }

            _DeleteSplitConsolidationRecordCommand.Parameters.AddWithValue("@Id", id.ToString());
            _DeleteSplitConsolidationRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _DeleteTransformationRecordCommand;
        private void DeleteTransformationRecord(Guid id)
        {
            /* Delete transformation record */
            if (_DeleteTransformationRecordCommand == null)
            {
                _DeleteTransformationRecordCommand = new SQLiteCommand("DELETE FROM [Transformations] WHERE [Id] = @Id", _Connection);
                _DeleteTransformationRecordCommand.Prepare();
            }

            _DeleteTransformationRecordCommand.Parameters.AddWithValue("@Id", id.ToString());
            _DeleteTransformationRecordCommand.ExecuteNonQuery();

            DeleteTransformationResultRecordRecords(id);
        }

        private SQLiteCommand _DeleteTransformationResultRecordsCommand;
        private void DeleteTransformationResultRecordRecords(Guid id)
        {

            /* Delete result stock record */
            if (_DeleteTransformationResultRecordsCommand == null)
            {
                _DeleteTransformationResultRecordsCommand = new SQLiteCommand("DELETE FROM [TransformationResultingStocks] WHERE [Id] = @Id", _Connection);
                _DeleteTransformationResultRecordsCommand.Prepare();
            }

            _DeleteTransformationResultRecordsCommand.Parameters.AddWithValue("@Id", id.ToString());
            _DeleteTransformationResultRecordsCommand.ExecuteNonQuery();
        }


        protected override void AddParameters(SQLiteCommand command, ICorporateAction entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@Stock", entity.Stock.ToString());
            command.Parameters.AddWithValue("@ActionDate", entity.ActionDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Description", entity.Description);
            command.Parameters.AddWithValue("@Type", entity.Type);
        }

        private void AddDividendParameters(SQLiteCommand command, Dividend entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@PaymentDate", entity.PaymentDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@DividendAmount", DecimalToDB(entity.DividendAmount));
            command.Parameters.AddWithValue("@CompanyTaxRate", DecimalToDB(entity.CompanyTaxRate));
            command.Parameters.AddWithValue("@PercentFranked", DecimalToDB(entity.PercentFranked));
            command.Parameters.AddWithValue("@DRPPrice", DecimalToDB(entity.DRPPrice));
        }

        private void AddCapitalReturnParameters(SQLiteCommand command, CapitalReturn entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@PaymentDate", entity.PaymentDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Amount", DecimalToDB(entity.Amount));
        }

        private void AddSplitConsolidationParameters(SQLiteCommand command, SplitConsolidation entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@OldUnits", entity.OldUnits);
            command.Parameters.AddWithValue("@NewUnits", entity.NewUnits);
        }

        private void AddTransformationParameters(SQLiteCommand command, Transformation entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@ImplementationDate", entity.ImplementationDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@CashComponent", DecimalToDB(entity.CashComponent));
            command.Parameters.AddWithValue("@RolloverRelief", entity.RolloverRefliefApplies ? "Y": "N");
        }

        protected override ICorporateAction CreateEntity(SQLiteDataReader reader)
        {
            return SQLiteStockEntityCreator.CreateCorporateAction(_Database as SQLiteStockDatabase, reader);
        }

    }
}
