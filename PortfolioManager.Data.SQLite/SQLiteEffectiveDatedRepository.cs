using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.SQLite
{
    public class SQLiteEffectiveDatedRepository<T>: SQLiteRepository<T> where T : IEffectiveDatedEntity
    {

        protected internal SQLiteEffectiveDatedRepository(SQLiteDatabase database, string tableName) : base(database, tableName)
        {
        }
        
        private SQLiteCommand _GetCurrentRecordCommand;
        protected override SQLiteCommand GetCurrentRecordCommand()
        {
            if (_GetCurrentRecordCommand == null)
             {
                 _GetCurrentRecordCommand = new SQLiteCommand("SELECT * FROM " + TableName + " WHERE [Id] = @Id AND [ToDate] = '9999-12-31'", _Connection);
                 _GetCurrentRecordCommand.Prepare();
             }

            return _GetCurrentRecordCommand;
        }

        private SQLiteCommand _GetEffectiveRecordCommand;
        protected SQLiteCommand GetEffectiveRecordCommand()
        {
            if (_GetEffectiveRecordCommand == null)
            {
                _GetEffectiveRecordCommand = new SQLiteCommand("SELECT * FROM " + TableName + " WHERE [Id] = @Id AND @AtDate BETWEEN [FromDate] AND [ToDate]", _Connection);
                _GetEffectiveRecordCommand.Prepare();
            }

            return _GetEffectiveRecordCommand;
        }

        private SQLiteCommand _GetDeleteRecordCommand;
        protected override SQLiteCommand GetDeleteRecordCommand()
        {
            if (_GetDeleteRecordCommand == null)
            {
                _GetDeleteRecordCommand = new SQLiteCommand("DELETE FROM " + TableName + " WHERE [Id] = @Id", _Connection);
                _GetDeleteRecordCommand.Prepare();
            }

            return _GetDeleteRecordCommand;
        }

        private SQLiteCommand _GetDeleteEffectiveRecordCommand;
        protected SQLiteCommand GetDeleteEffectiveRecordCommand()
        {
            if (_GetDeleteEffectiveRecordCommand == null)
            {
                _GetDeleteEffectiveRecordCommand = new SQLiteCommand("DELETE FROM " + TableName + " WHERE [Id] = @Id AND [FromDate] = @FromDate", _Connection);
                _GetDeleteEffectiveRecordCommand.Prepare();
            }

            return _GetDeleteEffectiveRecordCommand;
        }

        public T Get(Guid id, DateTime atDate)
        {
            var command = GetEffectiveRecordCommand();
            command.Parameters.AddWithValue("@Id", id.ToString());
            command.Parameters.AddWithValue("@AtDate", atDate.ToString("yyyy-MM-dd"));
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
