﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;
using PortfolioManager.Data.Stocks;

using PortfolioManager.Common;

namespace PortfolioManager.Data.SQLite.Stocks.CorporateActions
{
    public class SQLiteCorporateActionRepository : SQLiteRepository<CorporateAction>, ICorporateActionRepository
    {

        private Dictionary<CorporateActionType, SQLiteRepository<CorporateAction>> _DetailRepositories;

        protected internal SQLiteCorporateActionRepository(SqliteTransaction transaction, IEntityCreator entityCreator)
            : base(transaction, "CorporateActions", entityCreator)
        {
            _DetailRepositories = new Dictionary<CorporateActionType, SQLiteRepository<CorporateAction>>();

            _DetailRepositories.Add(CorporateActionType.Dividend, new SQLiteDividendRepository(transaction, entityCreator));
            _DetailRepositories.Add(CorporateActionType.CapitalReturn, new SQLiteCapitalReturnRepository(transaction, entityCreator));
            _DetailRepositories.Add(CorporateActionType.Transformation, new SQLiteTransformationRepository(transaction, entityCreator));
            _DetailRepositories.Add(CorporateActionType.SplitConsolidation, new SQLiteSplitConsolidationRepository(transaction, entityCreator));
            _DetailRepositories.Add(CorporateActionType.Composite, new SQLiteCompositeActionRepository(transaction, _DetailRepositories, entityCreator));
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO [CorporateActions] ([Id], [Stock], [ActionDate], [Description], [Type]) VALUES (@Id, @Stock, @ActionDate, @Description, @Type)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Stock", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@ActionDate", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Description", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Type", SqliteType.Integer);

                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE [CorporateActions] SET [ActionDate] = @ActionDate, [Description] = @Description WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Stock", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@ActionDate", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Description", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Type", SqliteType.Integer);

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

        protected override void AddParameters(SqliteParameterCollection parameters, CorporateAction entity)
        {
            parameters["@Id"].Value = entity.Id.ToString();
            parameters["@Stock"].Value = entity.Stock.ToString();
            parameters["@ActionDate"].Value = entity.ActionDate.ToString("yyyy-MM-dd");
            parameters["@Description"].Value = entity.Description;
            parameters["@Type"].Value = entity.Type;
        }

    }
}
