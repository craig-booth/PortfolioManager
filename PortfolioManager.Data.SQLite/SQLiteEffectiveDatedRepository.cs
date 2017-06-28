using System;
using Microsoft.Data.Sqlite;

namespace PortfolioManager.Data.SQLite
{
    public abstract class SQLiteEffectiveDatedRepository<T>: SQLiteRepository<T> where T : EffectiveDatedEntity
    {

        protected internal SQLiteEffectiveDatedRepository(SQLiteDatabase database, string tableName, IEntityCreator entityCreator) : base(database, tableName, entityCreator)
        {
        }
        
        private SqliteCommand _GetCurrentRecordCommand;
        protected override SqliteCommand GetCurrentRecordCommand()
        {
            if (_GetCurrentRecordCommand == null)
             {
                 _GetCurrentRecordCommand = new SqliteCommand("SELECT * FROM " + TableName + " WHERE [Id] = @Id AND [ToDate] = '9999-12-31'", _Connection);
                 _GetCurrentRecordCommand.Prepare();
             }

            return _GetCurrentRecordCommand;
        }

        private SqliteCommand _GetEffectiveRecordCommand;
        protected SqliteCommand GetEffectiveRecordCommand()
        {
            if (_GetEffectiveRecordCommand == null)
            {
                _GetEffectiveRecordCommand = new SqliteCommand("SELECT * FROM " + TableName + " WHERE [Id] = @Id AND @AtDate BETWEEN [FromDate] AND [ToDate]", _Connection);
                _GetEffectiveRecordCommand.Prepare();
            }

            return _GetEffectiveRecordCommand;
        }

        private SqliteCommand _GetDeleteRecordCommand;
        protected override SqliteCommand GetDeleteRecordCommand()
        {
            if (_GetDeleteRecordCommand == null)
            {
                _GetDeleteRecordCommand = new SqliteCommand("DELETE FROM " + TableName + " WHERE [Id] = @Id", _Connection);
                _GetDeleteRecordCommand.Prepare();
            }

            return _GetDeleteRecordCommand;
        }

        private SqliteCommand _GetDeleteEffectiveRecordCommand;
        protected SqliteCommand GetDeleteEffectiveRecordCommand()
        {
            if (_GetDeleteEffectiveRecordCommand == null)
            {
                _GetDeleteEffectiveRecordCommand = new SqliteCommand("DELETE FROM " + TableName + " WHERE [Id] = @Id AND [FromDate] = @FromDate", _Connection);
                _GetDeleteEffectiveRecordCommand.Prepare();
            }

            return _GetDeleteEffectiveRecordCommand;
        }

        public T Get(Guid id, DateTime atDate)
        {
            var command = GetEffectiveRecordCommand();
            command.Parameters.AddWithValue("@Id", id.ToString());
            command.Parameters.AddWithValue("@AtDate", atDate.ToString("yyyy-MM-dd"));

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
            command.Parameters.AddWithValue("@Id", id.ToString());
            command.Parameters.AddWithValue("@FromDate", atDate.ToString("yyyy-MM-dd"));

            command.ExecuteNonQuery();
        }

    }
}
