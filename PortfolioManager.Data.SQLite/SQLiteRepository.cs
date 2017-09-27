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
        protected readonly SqliteTransaction _Transaction;
        protected IEntityCreator _EntityCreator;

        public string TableName { get; private set; }

        protected internal SQLiteRepository(SqliteTransaction transaction, string tableName, IEntityCreator entityCreator)
        {
            _Transaction = transaction;
            _EntityCreator = entityCreator;

            TableName = tableName;
        }
        
        private SqliteCommand _GetCurrentRecordCommand;
        protected virtual SqliteCommand GetCurrentRecordCommand()
        {
            if (_GetCurrentRecordCommand == null)
             {
                _GetCurrentRecordCommand = new SqliteCommand("SELECT * FROM " + TableName + " WHERE [Id] = @Id", _Transaction.Connection, _Transaction);
                _GetCurrentRecordCommand.Parameters.Add("@Id", SqliteType.Text);
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
                _GetDeleteRecordCommand = new SqliteCommand("DELETE FROM " + TableName + " WHERE [Id] = @Id", _Transaction.Connection, _Transaction);
                _GetDeleteRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetDeleteRecordCommand.Prepare();
            }

            return _GetDeleteRecordCommand;
        }

        protected abstract void AddParameters(SqliteParameterCollection parameters, T entity);

        public virtual T Get(Guid id)
        {
            T entity;

            var command = GetCurrentRecordCommand();
            command.Parameters["@Id"].Value = id.ToString();

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
            AddParameters(command.Parameters, entity);

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
            AddParameters(command.Parameters, entity);

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
            command.Parameters["@Id"].Value  = id.ToString();

            if (command.ExecuteNonQuery() == 0)
                throw new RecordNotFoundException(id);
        }
    }
}
