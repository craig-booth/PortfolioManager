using System;
using Microsoft.Data.Sqlite;

namespace PortfolioManager.Data.SQLite
{
    public abstract class SQLiteEffectiveDatedRepository<T>: SQLiteRepository<T> where T : EffectiveDatedEntity
    {

        protected internal SQLiteEffectiveDatedRepository(SqliteTransaction transaction, string tableName, IEntityCreator entityCreator)
            : base(transaction, tableName, entityCreator)
        {
        }
        
        private SqliteCommand _GetCurrentRecordCommand;
        protected override SqliteCommand GetCurrentRecordCommand()
        {
            if (_GetCurrentRecordCommand == null)
             {
                _GetCurrentRecordCommand = new SqliteCommand("SELECT * FROM " + TableName + " WHERE [Id] = @Id AND [ToDate] = '9999-12-31'", _Transaction.Connection, _Transaction);
                _GetCurrentRecordCommand.Parameters.Add("@Id", SqliteType.Text);       
                _GetCurrentRecordCommand.Prepare();
             }

            return _GetCurrentRecordCommand;
        }

        private SqliteCommand _GetEffectiveRecordCommand;
        protected SqliteCommand GetEffectiveRecordCommand()
        {
            if (_GetEffectiveRecordCommand == null)
            {
                _GetEffectiveRecordCommand = new SqliteCommand("SELECT * FROM " + TableName + " WHERE [Id] = @Id AND @AtDate BETWEEN [FromDate] AND [ToDate]", _Transaction.Connection, _Transaction);
                _GetEffectiveRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetEffectiveRecordCommand.Parameters.Add("@AtDate", SqliteType.Text);
                _GetEffectiveRecordCommand.Prepare();
            }

            return _GetEffectiveRecordCommand;
        }

        private SqliteCommand _GetDeleteRecordCommand;
        protected override SqliteCommand GetDeleteRecordCommand()
        {
            if (_GetDeleteRecordCommand == null)
            {
                _GetDeleteRecordCommand = new SqliteCommand("DELETE FROM " + TableName + " WHERE [Id] = @Id", _Transaction.Connection, _Transaction);
                _GetDeleteRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetDeleteRecordCommand.Prepare();
            }

            return _GetDeleteRecordCommand;
        }

        private SqliteCommand _GetDeleteEffectiveRecordCommand;
        protected SqliteCommand GetDeleteEffectiveRecordCommand()
        {
            if (_GetDeleteEffectiveRecordCommand == null)
            {
                _GetDeleteEffectiveRecordCommand = new SqliteCommand("DELETE FROM " + TableName + " WHERE [Id] = @Id AND [FromDate] = @FromDate", _Transaction.Connection, _Transaction);
                _GetDeleteEffectiveRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetDeleteEffectiveRecordCommand.Parameters.Add("@FromDate", SqliteType.Text);
                _GetDeleteEffectiveRecordCommand.Prepare();
            }

            return _GetDeleteEffectiveRecordCommand;
        }

        public T Get(Guid id, DateTime atDate)
        {
            var command = GetEffectiveRecordCommand();
            command.Parameters["@Id"].Value = id.ToString();
            command.Parameters["@AtDate"].Value = atDate.ToString("yyyy-MM-dd");

            T entity;
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (!reader.Read())
                {
                    throw new RecordNotFoundException(id);
                }

                entity = _EntityCreator.CreateEntity<T>(reader);
            }

            return entity;
        }

        new public void Delete(T entity)
        {
            Delete(entity.Id, entity.FromDate);   
        }

        public void Delete(Guid id, DateTime atDate)
        {
            var command = GetDeleteEffectiveRecordCommand();
            command.Parameters["@Id"].Value = id.ToString();
            command.Parameters["@FromDate"].Value = atDate.ToString("yyyy-MM-dd");

            command.ExecuteNonQuery();
        }

    }
}
