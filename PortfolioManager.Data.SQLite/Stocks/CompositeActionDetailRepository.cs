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
    class CompositeActionDetailRepository : ICorporateActionDetailRepository
    {
        private SQLiteConnection _Connection;
        private Dictionary<CorporateActionType, ICorporateActionDetailRepository> _DetailRepositories;

        public CompositeActionDetailRepository(SQLiteConnection connection, Dictionary<CorporateActionType, ICorporateActionDetailRepository> detailRepositories)
        {
            _Connection = connection;
            _DetailRepositories = detailRepositories;
        }

        private SQLiteCommand _AddRecordCommand;
        public void Add(IEntity entity)
        {
            var compositeAction = entity as CompositeAction;

            if (_AddRecordCommand == null)
            {
                _AddRecordCommand = new SQLiteCommand("INSERT INTO [CompositeActions] ([Id], [Sequence], [ChildAction]) VALUES (@Id, @Sequence, @ChildAction)", _Connection);
                _AddRecordCommand.Prepare();
            }

            int sequence = 0;
            foreach (var childAction in compositeAction.Children)
            {
                sequence += 10;
                _AddRecordCommand.Parameters.AddWithValue("@Id", compositeAction.Id.ToString());
                _AddRecordCommand.Parameters.AddWithValue("@Sequence", sequence);
                _AddRecordCommand.Parameters.AddWithValue("@ChildAction", childAction.Id);
                _AddRecordCommand.ExecuteNonQuery();

                _DetailRepositories[childAction.Type].Add(childAction);
            }
        }

        public void Update(IEntity entity)
        {
            // Delete the child actions and re-add

            Delete(entity.Id);
            Add(entity);
        }

        private SQLiteCommand _DeleteRecordCommand;
        public void Delete(Guid id)
        {
      /*      if (_DeleteRecordCommand == null)
            {
                _DeleteRecordCommand = new SQLiteCommand("DELETE FROM [CompositeActions] WHERE [Id] = @Id", _Connection);
                _DeleteRecordCommand.Prepare();
            }

            _DeleteRecordCommand.Parameters.AddWithValue("@Id", id.ToString());
            _DeleteRecordCommand.ExecuteNonQuery();

            foreach (var childAction in compositeAction.Children)
            {
                _AddRecordCommand.Parameters.AddWithValue("@Id", compositeAction.Id.ToString());
                _AddRecordCommand.Parameters.AddWithValue("@Sequence", sequence);
                _AddRecordCommand.Parameters.AddWithValue("@ChildAction", childAction.Id);
                _AddRecordCommand.ExecuteNonQuery();

                _DetailRepositories[childAction.Type].Add(childAction);
            } */
        }
    }
}
