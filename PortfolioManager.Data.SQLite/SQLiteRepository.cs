using System;
using Microsoft.Data.Sqlite;

namespace PortfolioManager.Data.SQLite
{
    public interface IEntityCreator
    {
        T CreateEntity<T>(SqliteDataReader reader) where T : Entity;
    }

    public abstract class SQLiteRepository<T> where T : Entity
    {
        protected SQLiteDatabase _Database;
        protected SqliteConnection _Connection;
        protected IEntityCreator _EntityCreator;

        public string TableName { get; private set; }

        protected internal SQLiteRepository(SQLiteDatabase database, string tableName, IEntityCreator entityCreator)
        {
            _Database = database;
            _Connection = database._Connection;
            _EntityCreator = entityCreator;

            TableName = tableName;
        }
        
        private SqliteCommand _GetCurrentRecordCommand;
        protected virtual SqliteCommand GetCurrentRecordCommand()
        {
            if (_GetCurrentRecordCommand == null)
             {
                 _GetCurrentRecordCommand = new SqliteCommand("SELECT * FROM " + TableName + " WHERE [Id] = @Id", _Connection);
                 _GetCurrentRecordCommand.Prepare();
             }

            return _GetCurrentRecordCommand;
        }

        protected abstract SqliteCommand GetAddRecordCommand();

        protected abstract SqliteCommand GetUpdateRecordCommand();

        private SqliteCommand _GetDeleteRecordCommand;
        protected virtual SqliteCommand GetDeleteRecordCommand()
        {
            if (_GetDeleteRecordCommand == null)
            {
                _GetDeleteRecordCommand = new SqliteCommand("DELETE FROM " + TableName + " WHERE [Id] = @Id", _Connection);
                _GetDeleteRecordCommand.Prepare();
            }

            return _GetDeleteRecordCommand;
        }

        protected abstract void AddParameters(SqliteCommand command, T entity);

        public virtual T Get(Guid id)
        {
            T entity;

            var command = GetCurrentRecordCommand();
            command.Parameters.AddWithValue("@Id", id.ToString());

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

        public virtual void Add(T entity)
        {
            var command = GetAddRecordCommand();
            AddParameters(command, entity);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                if (e.SqliteErrorCode == 19)
                    throw new DuplicateRecordException(entity.Id);
                else
                    throw;
            }
        }

        public virtual void Update(T entity)
        {
            var command = GetUpdateRecordCommand();
            AddParameters(command, entity);

            if (command.ExecuteNonQuery() == 0)
                throw new RecordNotFoundException(entity.Id);
        }

        public virtual void Delete(T entity)
        {
            Delete(entity.Id);
        }

        public virtual void Delete(Guid id)
        {
            var command = GetDeleteRecordCommand();
            command.Parameters.AddWithValue("@Id", id.ToString());

            if (command.ExecuteNonQuery() == 0)
                throw new RecordNotFoundException(id);
        }
    }
}
