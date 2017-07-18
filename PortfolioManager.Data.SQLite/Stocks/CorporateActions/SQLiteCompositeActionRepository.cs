using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks.CorporateActions
{
    class SQLiteCompositeActionRepository : SQLiteRepository<CorporateAction>
    {
        private Dictionary<CorporateActionType, SQLiteRepository<CorporateAction>> _DetailRepositories;

        public SQLiteCompositeActionRepository(SqliteTransaction transaction, Dictionary<CorporateActionType, SQLiteRepository<CorporateAction>> detailRepositories, IEntityCreator entityCreator)
            : base(transaction, "CompositeActions", entityCreator)
        {
            _DetailRepositories = detailRepositories;
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
              if (_GetAddRecordCommand == null)
              {
                  _GetAddRecordCommand = new SqliteCommand("INSERT INTO [CompositeActions] ([Id], [Sequence], [ChildAction], [ChildType]) VALUES (@Id, @Sequence, @ChildAction, @ChildType)", _Transaction.Connection, _Transaction);

                  _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                  _GetAddRecordCommand.Parameters.Add("@Sequence", SqliteType.Integer);
                  _GetAddRecordCommand.Parameters.Add("@ChildAction", SqliteType.Text);
                  _GetAddRecordCommand.Parameters.Add("@ChildType", SqliteType.Integer);

                  _GetAddRecordCommand.Prepare();
              }
              
            return _GetAddRecordCommand;
        }

        protected override SqliteCommand GetUpdateRecordCommand()
        {
            throw new NotImplementedException();
        }

        public override void Add(CorporateAction entity)
        {
            var compositeAction = entity as CompositeAction;

            var command = GetAddRecordCommand();

            int sequence = 0;
            foreach (var childAction in compositeAction.Children)
            {
                sequence += 10;
                command.Parameters["@Sequence"].Value = sequence;

                AddParameters(command.Parameters, childAction);
                command.ExecuteNonQuery();

                _DetailRepositories[childAction.Type].Add(childAction);
            }
        }

        public override void Update(CorporateAction entity)
        {
            /* Update header record */
            base.Update(entity);

            _DetailRepositories[entity.Type].Update(entity);
        }

        public override void Delete(CorporateAction entity)
        {
            // Delete the child actions and re-add
            Delete(entity.Id);
            Add(entity);
        }

        private SqliteCommand _GetChildrenCommand;
        public override void Delete(Guid id)
        {
            if (_GetChildrenCommand == null)
            {
                _GetChildrenCommand = new SqliteCommand("SELECT [ChildAction], [ChildType] FROM [CompositeActions] WHERE [Id] = @Id", _Transaction.Connection, _Transaction);
                _GetChildrenCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetChildrenCommand.Prepare();
            }

            _GetChildrenCommand.Parameters["@Id"].Value = id.ToString();
            using (SqliteDataReader reader = _GetChildrenCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var childId = new Guid(reader.GetString(0));
                    var childType = (CorporateActionType)reader.GetInt32(1);

                    _DetailRepositories[childType].Delete(childId);
                }
            }

            base.Delete(id);
        }

        protected override void AddParameters(SqliteParameterCollection parameters, CorporateAction entity)
        {
            parameters["@Id"].Value = entity.Id.ToString();
            parameters["@ChildAction"].Value = entity.Id.ToString();
            parameters["@Type"].Value = entity.Type;
        }

    }
}
