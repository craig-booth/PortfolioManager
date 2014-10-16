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

            if (entity is Dividend)
                AddDividendRecord(entity as Dividend);
            else if (entity is CapitalReturn)
                AddCapitalReturnRecord(entity as CapitalReturn);
            else if (entity is Transformation)
                AddTransformationRecord(entity as Transformation);
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

        private SQLiteCommand _AddTransformationRecordCommand;
        private void AddTransformationRecord(Transformation entity)
        {
            if (_AddTransformationRecordCommand == null)
            {
                _AddTransformationRecordCommand = new SQLiteCommand("INSERT INTO [Transformations] ([Id], [ImplementationDate], [CashComponent]) VALUES (@Id, @ImplementationDate, @CashComponent)", _Connection);
                _AddTransformationRecordCommand.Prepare();
            }

            AddTransformationParameters(_AddTransformationRecordCommand, entity);
            _AddTransformationRecordCommand.ExecuteNonQuery();
        }

        public override void Update(ICorporateAction entity)
        {
            /* Update header record */
            base.Update(entity);

            if (entity is Dividend)
                UpdateDividendRecord(entity as Dividend);
            else if (entity is CapitalReturn)
                UpdateCapitalReturnRecord(entity as CapitalReturn);
            else if (entity is Transformation)
                UpdateTransformationRecord(entity as Transformation);
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

        private SQLiteCommand _UpdateTransformationRecordCommand;
        private void UpdateTransformationRecord(Transformation entity)
        {
            if (_UpdateTransformationRecordCommand == null)
            {
                _UpdateTransformationRecordCommand = new SQLiteCommand("UPDATE [Transformations] SET [ImplementationDate] = @ImplementationDate, [CashComponent] = @CashComponent WHERE [Id] = @Id", _Connection);
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
                _AddTransformationResultRecordCommand = new SQLiteCommand("INSERT INTO [TransformationResultingStocks] ([Id], [Stock], [OriginalUnits], [NewUnits], [CostBasePercentage]) VALUES (@Id, @Stock, @OriginalUnits, @NewUnits, @CostBasePercentage)", _Connection);
                _AddTransformationResultRecordCommand.Prepare();
            }
            _AddTransformationResultRecordCommand.Parameters.AddWithValue("@Id", transformationId.ToString());
            _AddTransformationResultRecordCommand.Parameters.AddWithValue("@Stock", entity.Stock.ToString());
            _AddTransformationResultRecordCommand.Parameters.AddWithValue("@OriginalUnits", entity.OriginalUnits);
            _AddTransformationResultRecordCommand.Parameters.AddWithValue("@NewUnits", entity.NewUnits);
            _AddTransformationResultRecordCommand.Parameters.AddWithValue("@CostBasePercentage", DecimalToDB(entity.CostBasePercentage));

            _AddTransformationResultRecordCommand.ExecuteNonQuery();
        }

        public override void Delete(ICorporateAction entity)
        {
            /* Delete header record */
            base.Delete(entity);

            if (entity is Dividend)
                DeleteDividendRecord(entity.Id);
            else if (entity is CapitalReturn)
                DeleteCapitalReturnRecord(entity.Id);
            else if (entity is Transformation)
                DeleteTransformationRecord(entity.Id);
        }

        public override void Delete(Guid id)
        {
            /* Delete header record */
            base.Delete(id);

            /* We don't know what the type is so just try deleting all types */
            DeleteDividendRecord(id);
            DeleteCapitalReturnRecord(id);
            DeleteTransformationRecord(id);
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
            command.Parameters.AddWithValue("@Type", ActionType(entity));
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

        private void AddTransformationParameters(SQLiteCommand command, Transformation entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@ImplementationDate", entity.ImplementationDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@CashComponent", DecimalToDB(entity.CashComponent));
        }

        protected override ICorporateAction CreateEntity(SQLiteDataReader reader)
        {
            return SQLiteStockEntityCreator.CreateCorporateAction(_Database as SQLiteStockDatabase, reader);
        }

        private int ActionType(ICorporateAction entity)
        {
            if (entity is Dividend)
                return 1;
            else if (entity is CapitalReturn)
                return 2;
            else if (entity is Transformation)
                return 3;  
            else 
                return 0;
        } 
    }
}
