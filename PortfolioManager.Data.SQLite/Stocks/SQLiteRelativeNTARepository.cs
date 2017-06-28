﻿using System;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    public class SQLiteRelativeNTARepository : SQLiteRepository<RelativeNTA>, IRelativeNTARepository
    {
        protected internal SQLiteRelativeNTARepository(SQLiteStockDatabase database, IEntityCreator entityCreator)
            : base(database, "RelativeNTAs", entityCreator)
        {
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO RelativeNTAs ([Id], [Date], [Parent], [Child], [Percentage]) VALUES (@Id, @Date, @Parent, @Child, @Percentage)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE RelativeNTAs SET [Date] = @Date, [Parent] = @Parent, [Child] = @Child, [Percentage] = @Percentage WHERE [Id] = @Id", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SqliteCommand command, RelativeNTA entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@Date", entity.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Parent", entity.Parent.ToString());
            command.Parameters.AddWithValue("@Child", entity.Child.ToString());
            command.Parameters.AddWithValue("@Percentage", SQLiteUtils.DecimalToDB(entity.Percentage));
        }

        private SqliteCommand _GetDeleteByDetailsCommand;
        public void Delete(Guid parent, Guid child, DateTime atDate)
        {
            if (_GetDeleteByDetailsCommand == null)
            {
                _GetDeleteByDetailsCommand = new SqliteCommand("DELETE FROM [RelativeNTAs] WHERE [Parent] = @Parent AND [Child] = @Child AND [Date] = @Date", _Connection);
                _GetDeleteByDetailsCommand.Prepare();
            }

            _GetDeleteByDetailsCommand.Parameters.AddWithValue("@Parent", parent.ToString());
            _GetDeleteByDetailsCommand.Parameters.AddWithValue("@Child", child.ToString());
            _GetDeleteByDetailsCommand.Parameters.AddWithValue("@Date", atDate.ToString("yyyy-MM-dd"));

            _GetDeleteByDetailsCommand.ExecuteNonQuery();
        }

        private SqliteCommand _GetDeleteAllParentCommand;
        public void DeleteAll(Guid parent)
        {
            if (_GetDeleteAllParentCommand == null)
            {
                _GetDeleteAllParentCommand = new SqliteCommand("DELETE FROM [RelativeNTAs] WHERE [Parent] = @Parent", _Connection);
                _GetDeleteAllParentCommand.Prepare();
            }

            _GetDeleteAllParentCommand.Parameters.AddWithValue("@Parent", parent.ToString());

            _GetDeleteAllParentCommand.ExecuteNonQuery();
        }

        private SqliteCommand _GetDeleteAllParentChildCommand;
        public void DeleteAll(Guid parent, Guid child)
        {
            if (_GetDeleteAllParentChildCommand == null)
            {
                _GetDeleteAllParentChildCommand = new SqliteCommand("DELETE FROM [RelativeNTAs] WHERE [Parent] = @Parent AND [Child] = @Child", _Connection);
                _GetDeleteAllParentChildCommand.Prepare();
            }

            _GetDeleteAllParentChildCommand.Parameters.AddWithValue("@Parent", parent.ToString());
            _GetDeleteAllParentChildCommand.Parameters.AddWithValue("@Child", child.ToString());

            _GetDeleteAllParentChildCommand.ExecuteNonQuery();
        }
    }


}
