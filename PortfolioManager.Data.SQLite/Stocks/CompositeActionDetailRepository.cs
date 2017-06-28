using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class CompositeActionDetailRepository : ICorporateActionDetailRepository
    {
        private SqliteConnection _Connection;
        private Dictionary<CorporateActionType, ICorporateActionDetailRepository> _DetailRepositories;

        public CompositeActionDetailRepository(SqliteConnection connection, Dictionary<CorporateActionType, ICorporateActionDetailRepository> detailRepositories)
        {
            _Connection = connection;
            _DetailRepositories = detailRepositories;
        }

        private SqliteCommand _AddRecordCommand;
        public void Add(Entity entity)
        {
            var compositeAction = entity as CompositeAction;

            if (_AddRecordCommand == null)
            {
                _AddRecordCommand = new SqliteCommand("INSERT INTO [CompositeActions] ([Id], [Sequence], [ChildAction], [ChildType]) VALUES (@Id, @Sequence, @ChildAction, @ChildType)", _Connection);
                _AddRecordCommand.Prepare();
            }

            int sequence = 0;
            foreach (var childAction in compositeAction.Children)
            {
                sequence += 10;
                _AddRecordCommand.Parameters.AddWithValue("@Id", compositeAction.Id.ToString());
                _AddRecordCommand.Parameters.AddWithValue("@Sequence", sequence);
                _AddRecordCommand.Parameters.AddWithValue("@ChildAction", childAction.Id.ToString());
                _AddRecordCommand.Parameters.AddWithValue("@ChildType", childAction.Type);
                _AddRecordCommand.ExecuteNonQuery();

                _DetailRepositories[childAction.Type].Add(childAction);
            }
        }

        public void Update(Entity entity)
        {
            // Delete the child actions and re-add
            Delete(entity.Id);
            Add(entity);
        }

        private SqliteCommand _DeleteRecordCommand;
        private SqliteCommand _GetChildrenCommand;
        public void Delete(Guid id)
        {
            if (_GetChildrenCommand == null)
            {
                _GetChildrenCommand = new SqliteCommand("SELECT [ChildAction], [ChildType] FROM [CompositeActions] WHERE [Id] = @Id", _Connection);
                _GetChildrenCommand.Prepare();
            }

            _GetChildrenCommand.Parameters.AddWithValue("@Id", id.ToString());

            using (SqliteDataReader compositeActionReader = _GetChildrenCommand.ExecuteReader())
            {
                while (compositeActionReader.Read())
                {
                    var childId = new Guid(compositeActionReader.GetString(0));
                    var childType = (CorporateActionType)compositeActionReader.GetInt32(1);

                    _DetailRepositories[childType].Delete(childId);
                }
            }

            if (_DeleteRecordCommand == null)
            {
                _DeleteRecordCommand = new SqliteCommand("DELETE FROM [CompositeActions] WHERE [Id] = @Id", _Connection);
                _DeleteRecordCommand.Prepare();
            }

            _DeleteRecordCommand.Parameters.AddWithValue("@Id", id.ToString());
            _DeleteRecordCommand.ExecuteNonQuery();
        }

    }
}
