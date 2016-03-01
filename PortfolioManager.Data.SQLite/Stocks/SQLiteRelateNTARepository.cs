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
    public class SQLiteRelateNTARepository : SQLiteRepository<RelativeNTA>, IRelativeNTARepository
    {
        protected internal SQLiteRelateNTARepository(SQLiteStockDatabase database)
            : base(database, "RelativeNTAs")
        {
        }

        private SQLiteCommand _GetAddRecordCommand;
        protected override SQLiteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SQLiteCommand("INSERT INTO RelativeNTAs ([Id], [Date], [Parent], [Child], [Percentage]) VALUES (@Id, @Date, @Parent, @Child, @Percentage)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SQLiteCommand _GetUpdateRecordCommand;
        protected override SQLiteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SQLiteCommand("UPDATE RelativeNTAs SET [Date] = @Date, [Parent] = @Parent, [Child] = @Child, [Percentage] = @Percentage WHERE [Id] = @Id", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override RelativeNTA CreateEntity(SQLiteDataReader reader)
        {
            return SQLiteStockEntityCreator.CreateRelativeNTA(_Database as SQLiteStockDatabase, reader);
        }

        protected override void AddParameters(SQLiteCommand command, RelativeNTA entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@Date", entity.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Parent", entity.Parent.ToString());
            command.Parameters.AddWithValue("@Child", entity.Child.ToString());
            command.Parameters.AddWithValue("@Percentage", SQLiteUtils.DecimalToDB(entity.Percentage));
        }

        private SQLiteCommand _GetDeleteByDetailsCommand;
        public void Delete(Guid parent, Guid child, DateTime atDate)
        {
            if (_GetDeleteByDetailsCommand == null)
            {
                _GetDeleteByDetailsCommand = new SQLiteCommand("DELETE FROM [RelativeNTAs] WHERE [Parent] = @Parent AND [Child] = @Child AND [Date] = @Date", _Connection);
                _GetDeleteByDetailsCommand.Prepare();
            }

            _GetDeleteByDetailsCommand.Parameters.AddWithValue("@Parent", parent.ToString());
            _GetDeleteByDetailsCommand.Parameters.AddWithValue("@Child", child.ToString());
            _GetDeleteByDetailsCommand.Parameters.AddWithValue("@Date", atDate.ToString("yyyy-MM-dd"));

            _GetDeleteByDetailsCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _GetDeleteAllParentCommand;
        public void DeleteAll(Guid parent)
        {
            if (_GetDeleteAllParentCommand == null)
            {
                _GetDeleteAllParentCommand = new SQLiteCommand("DELETE FROM [RelativeNTAs] WHERE [Parent] = @Parent", _Connection);
                _GetDeleteAllParentCommand.Prepare();
            }

            _GetDeleteAllParentCommand.Parameters.AddWithValue("@Parent", parent.ToString());

            _GetDeleteAllParentCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _GetDeleteAllParentChildCommand;
        public void DeleteAll(Guid parent, Guid child)
        {
            if (_GetDeleteAllParentChildCommand == null)
            {
                _GetDeleteAllParentChildCommand = new SQLiteCommand("DELETE FROM [RelativeNTAs] WHERE [Parent] = @Parent AND [Child] = @Child", _Connection);
                _GetDeleteAllParentChildCommand.Prepare();
            }

            _GetDeleteAllParentChildCommand.Parameters.AddWithValue("@Parent", parent.ToString());
            _GetDeleteAllParentChildCommand.Parameters.AddWithValue("@Child", child.ToString());

            _GetDeleteAllParentChildCommand.ExecuteNonQuery();
        }
    }


}
