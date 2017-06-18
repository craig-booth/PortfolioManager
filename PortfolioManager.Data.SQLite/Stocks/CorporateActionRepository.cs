using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;
using PortfolioManager.Data;
using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    interface ICorporateActionDetailRepository
    {
        void Add(Entity entity);
        void Update(Entity entity);
        void Delete(Guid id);
    }

    class SQLiteCorporateActionRepository : SQLiteRepository<CorporateAction>, ICorporateActionRepository 
    {
        private Dictionary<CorporateActionType, ICorporateActionDetailRepository> _DetailRepositories;

        protected internal SQLiteCorporateActionRepository(SQLiteStockDatabase database, IEntityCreator entityCreator)
            : base(database, "CorporateActions", entityCreator)
        {
            _DetailRepositories = new Dictionary<CorporateActionType, ICorporateActionDetailRepository>();
            _DetailRepositories.Add(CorporateActionType.Dividend, new DividendDetailRepository(_Connection));
            _DetailRepositories.Add(CorporateActionType.CapitalReturn, new CapitalReturnDetailRepository(_Connection));
            _DetailRepositories.Add(CorporateActionType.Transformation, new TransformationDetailRepository(_Connection));
            _DetailRepositories.Add(CorporateActionType.SplitConsolidation, new SplitConsolidtionDetailRepository(_Connection));
            _DetailRepositories.Add(CorporateActionType.Composite, new CompositeActionDetailRepository(_Connection, _DetailRepositories));
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO [CorporateActions] ([Id], [Stock], [ActionDate], [Description], [Type]) VALUES (@Id, @Stock, @ActionDate, @Description, @Type)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE [CorporateActions] SET [ActionDate] = @ActionDate, [Description] = @Description WHERE [Id] = @Id", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        public override void Add(CorporateAction entity) 
        {
            /* Add header record */
            base.Add(entity);

            _DetailRepositories[entity.Type].Add(entity);
       }      	

        public override void Update(CorporateAction entity)
        {
            /* Update header record */
            base.Update(entity);

            _DetailRepositories[entity.Type].Update(entity);
        }

        public override void Delete(CorporateAction entity)
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

        protected override void AddParameters(SqliteCommand command, CorporateAction entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@Stock", entity.Stock.ToString());
            command.Parameters.AddWithValue("@ActionDate", entity.ActionDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Description", entity.Description);
            command.Parameters.AddWithValue("@Type", entity.Type);
        }
    }
}
