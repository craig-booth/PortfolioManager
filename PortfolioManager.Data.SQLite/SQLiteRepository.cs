using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.SQLite
{
    public class SQLiteRepository<T> where T : IEntity
    {
        protected SQLiteDatabase _Database;
        protected SQLiteConnection _Connection;

        public string TableName { get; private set; }

        protected internal SQLiteRepository(SQLiteDatabase database, string tableName)
        {
            _Database = database;
            _Connection = database._Connection;
            TableName = tableName;
        }
        
        private SQLiteCommand _GetCurrentRecordCommand;
        protected virtual SQLiteCommand GetCurrentRecordCommand()
        {
            if (_GetCurrentRecordCommand == null)
             {
                 _GetCurrentRecordCommand = new SQLiteCommand("SELECT * FROM " + TableName + " WHERE [Id] = @Id", _Connection);
                 _GetCurrentRecordCommand.Prepare();
             }

            return _GetCurrentRecordCommand;
        }

        private SQLiteCommand _GetAddRecordCommand;
        protected virtual SQLiteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SQLiteCommand("", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SQLiteCommand _GetUpdateRecordCommand;
        protected virtual SQLiteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SQLiteCommand("", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        private SQLiteCommand _GetDeleteRecordCommand;
        protected virtual SQLiteCommand GetDeleteRecordCommand()
        {
            if (_GetDeleteRecordCommand == null)
            {
                _GetDeleteRecordCommand = new SQLiteCommand("DELETE FROM " + TableName + " WHERE [Id] = @Id", _Connection);
                _GetDeleteRecordCommand.Prepare();
            }

            return _GetDeleteRecordCommand;
        }

        protected virtual T CreateEntity(SQLiteDataReader reader)
        {
            return default(T);
        }

        protected virtual void AddParameters(SQLiteCommand command, T entity)
        {

        }

        public virtual T Get(Guid id)
        {
            var command = GetCurrentRecordCommand();
            command.Parameters.AddWithValue("@Id", id.ToString());
            SQLiteDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                throw new RecordNotFoundException(id);
            }
                
            T entity = CreateEntity(reader);
            reader.Close();

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
            catch (SQLiteException e)
            {
                if (e.ResultCode == SQLiteErrorCode.Constraint)
                    throw new DuplicateRecordException(entity.Id);
                else
                    throw;
            }
        }

        public virtual void Update(T entity)
        {
            var command = GetUpdateRecordCommand();
            AddParameters(command, entity);

            command.ExecuteNonQuery();
        }

        public virtual void Delete(T entity)
        {
            Delete(entity.Id);
        }

        public virtual void Delete(Guid id)
        {
            var command = GetDeleteRecordCommand();
            command.Parameters.AddWithValue("@Id", id.ToString());

            command.ExecuteNonQuery();
        }
    }
}
