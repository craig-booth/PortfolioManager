using System;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    public class SQLiteRelativeNTARepository : SQLiteRepository<RelativeNTA>, IRelativeNTARepository
    {
        protected internal SQLiteRelativeNTARepository(SqliteTransaction transaction, IEntityCreator entityCreator)
            : base(transaction, "RelativeNTAs", entityCreator)
        {
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO RelativeNTAs ([Id], [Date], [Parent], [Child], [Percentage]) VALUES (@Id, @Date, @Parent, @Child, @Percentage)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Date", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Parent", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Child", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Percentage", SqliteType.Integer);

                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE RelativeNTAs SET [Date] = @Date, [Parent] = @Parent, [Child] = @Child, [Percentage] = @Percentage WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Date", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Parent", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Child", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Percentage", SqliteType.Integer);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SqliteParameterCollection parameters, RelativeNTA entity)
        {
            parameters["@Id"].Value = entity.Id.ToString();
            parameters["@Date"].Value = entity.Date.ToString("yyyy-MM-dd");
            parameters["@Parent"].Value = entity.Parent.ToString();
            parameters["@Child"].Value = entity.Child.ToString();
            parameters["@Percentage"].Value = SQLiteUtils.DecimalToDB(entity.Percentage);
        }

        private SqliteCommand _GetDeleteByDetailsCommand;
        public void Delete(Guid parent, Guid child, DateTime atDate)
        {
            if (_GetDeleteByDetailsCommand == null)
            {
                _GetDeleteByDetailsCommand = new SqliteCommand("DELETE FROM [RelativeNTAs] WHERE [Parent] = @Parent AND [Child] = @Child AND [Date] = @Date", _Transaction.Connection, _Transaction);
       
                _GetDeleteByDetailsCommand.Parameters.Add("@Parent", SqliteType.Text);
                _GetDeleteByDetailsCommand.Parameters.Add("@Child", SqliteType.Text);
                _GetDeleteByDetailsCommand.Parameters.Add("@Date", SqliteType.Text);

                _GetDeleteByDetailsCommand.Prepare();
            }

            _GetDeleteByDetailsCommand.Parameters["@Parent"].Value = parent.ToString();
            _GetDeleteByDetailsCommand.Parameters["@Child"].Value = child.ToString();
            _GetDeleteByDetailsCommand.Parameters["@Date"].Value = atDate.ToString("yyyy-MM-dd");

            _GetDeleteByDetailsCommand.ExecuteNonQuery();
        }

        private SqliteCommand _GetDeleteAllParentCommand;
        public void DeleteAll(Guid parent)
        {
            if (_GetDeleteAllParentCommand == null)
            {
                _GetDeleteAllParentCommand = new SqliteCommand("DELETE FROM [RelativeNTAs] WHERE [Parent] = @Parent", _Transaction.Connection, _Transaction);

                _GetDeleteAllParentCommand.Parameters.Add("@Parent", SqliteType.Text);

                _GetDeleteAllParentCommand.Prepare();
            }

            _GetDeleteAllParentCommand.Parameters["@Parent"].Value = parent.ToString();

            _GetDeleteAllParentCommand.ExecuteNonQuery();
        }

        private SqliteCommand _GetDeleteAllParentChildCommand;
        public void DeleteAll(Guid parent, Guid child)
        {
            if (_GetDeleteAllParentChildCommand == null)
            {
                _GetDeleteAllParentChildCommand = new SqliteCommand("DELETE FROM [RelativeNTAs] WHERE [Parent] = @Parent AND [Child] = @Child", _Transaction.Connection, _Transaction);

                _GetDeleteAllParentChildCommand.Parameters.Add("@Parent", SqliteType.Text);
                _GetDeleteAllParentChildCommand.Parameters.Add("@Child", SqliteType.Text);

                _GetDeleteAllParentChildCommand.Prepare();
            }

            _GetDeleteAllParentChildCommand.Parameters["@Parent"].Value = parent.ToString();
            _GetDeleteAllParentChildCommand.Parameters["@Child"].Value = child.ToString();

            _GetDeleteAllParentChildCommand.ExecuteNonQuery();
        }
    }


}
