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
    interface ICorporateActionDetailRepository
    {
        void Add(IEntity entity);
        void Update(IEntity entity);
        void Delete(Guid id);
    }

    class SQLiteCorporateActionRepository : SQLiteRepository<ICorporateAction>, ICorporateActionRepository 
    {
        private Dictionary<CorporateActionType, ICorporateActionDetailRepository> _DetailRepositories;

        protected internal SQLiteCorporateActionRepository(SQLiteStockDatabase database)
            : base(database, "CorporateActions")
        {
            _DetailRepositories = new Dictionary<CorporateActionType, ICorporateActionDetailRepository>();
            _DetailRepositories.Add(CorporateActionType.Dividend, new DividendDetailRepository(_Connection));
            _DetailRepositories.Add(CorporateActionType.CapitalReturn, new CapitalReturnDetailRepository(_Connection));
            _DetailRepositories.Add(CorporateActionType.Transformation, new TransformationDetailRepository(_Connection));
            _DetailRepositories.Add(CorporateActionType.Composite, new CompositeActionDetailRepository(_Connection, _DetailRepositories));
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
                _AddTransformationRecordCommand = new SQLiteCommand("INSERT INTO [Transformations] ([Id], [ImplementationDate], [CashComponent], [RolloverRelief]) VALUES (@Id, @ImplementationDate, @CashComponent, @RolloverRelief)", _Connection);
                _AddTransformationRecordCommand.Prepare();
            }

        public override void Update(ICorporateAction entity)
        {
            /* Update header record */
            base.Update(entity);

        
        _DetailRepositories[entity.Type].Update(entity);
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
        public override void Delete(ICorporateAction entity)
        {
            /* Delete header record */
            base.Delete(entity);

            _DetailRepositories[entity.Type].Delete(entity.Id);
        }

        public override void Delete(Guid id)
        {
            /* Delete header record */
            base.Delete(id);

            /* We don't know what the type is so just try deleting all types */
            foreach (var detailRepository in _DetailRepositories.Values)
                detailRepository.Delete(id);
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

        private void AddSplitConsolidationParameters(SQLiteCommand command, SplitConsolidation entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@OldUnits", entity.OldUnits);
            command.Parameters.AddWithValue("@NewUnits", entity.NewUnits);
        }
        protected override void AddParameters(SQLiteCommand command, ICorporateAction entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@Stock", entity.Stock.ToString());
            command.Parameters.AddWithValue("@ActionDate", entity.ActionDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Description", entity.Description);
            command.Parameters.AddWithValue("@Type", entity.Type);
        }

        protected override ICorporateAction CreateEntity(SQLiteDataReader reader)
        {
            return SQLiteStockEntityCreator.CreateCorporateAction(_Database as SQLiteStockDatabase, reader);
        }

    }
}
