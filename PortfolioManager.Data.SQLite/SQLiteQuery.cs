using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;

namespace PortfolioManager.Data.SQLite
{
    interface IEntityQuery
    {
        IEntityQuery SQL(string sql);

        IEntityQuery FromTable(string table);
        IEntityQuery Select(string fields);
        IEntityQuery Where(string condition);
        IEntityQuery Join(string table, string on);

        IEntityQuery WithId(Guid id);
        IEntityQuery EffectiveAt(DateTime date);
        IEntityQuery EffectiveBetween(DateTime fromDate, DateTime toDate);

        IEntityQuery WithParameter(string name, string value);
        IEntityQuery WithParameter(string name, DateTime value);
        IEntityQuery WithParameter(string name, Guid value);
        IEntityQuery WithParameter(string name, int value);
        IEntityQuery WithParameter(string name, decimal value);
        IEntityQuery WithParameter(string name, bool value);

        IEntityQuery OrderBy(string fields);

        T CreateEntity<T>() where T : Entity;
        IEnumerable<T> CreateEntities<T>() where T : Entity;

        SqliteDataReader Execute();
        SqliteDataReader ExecuteSingle();

        bool ExecuteScalar(out int value);
        bool ExecuteScalar(out decimal value);
        bool ExecuteScalar(out string value);
        bool ExecuteScalar(out DateTime value);
        bool ExecuteScalar(out Guid value);
        bool ExecuteScalar(out bool value);
    }

    class SQLiteEntityQuery : IEntityQuery
    {
        private SqliteTransaction _Transaction;
        private IEntityCreator _EntityCreator;

        private string _TableName;
        private List<string> _Join;
        private string _Where;
        private string _SQL;
        private string _Fields;
        private string _Orderby;

        private List<SqliteParameter> _Parameters;

        public SQLiteEntityQuery(SqliteTransaction transaction, IEntityCreator entityCreator)
        {
            _Transaction = transaction;
            _EntityCreator = entityCreator;
            _SQL = "";
            _TableName = "";
            _Join = new List<string>();
            _Where = "";
            _Fields = "*";
            _Orderby = "";
            _Parameters = new List<SqliteParameter>();
        }

        public IEntityQuery SQL(string sql)
        {
            _SQL = sql;

            return this;
        }

        public IEntityQuery FromTable(string tableName)
        {
            _TableName = tableName;

            return this;
        }

        public IEntityQuery Select(string fields)
        {
            _Fields = fields;

            return this;
        }

        public IEntityQuery Where(string condition)
        {
            if (_Where == "")
                _Where = condition;
            else
                _Where += " AND (" + condition + ")";

            return this;
        }

        public IEntityQuery Join(string table, string on)
        {
            _Join.Add(" JOIN [" + table + "] ON (" + on + ")");

            return this;
        }

        public IEntityQuery WithId(Guid id)
        {
            Where("[" + _TableName + "].[Id] = @_Id");
            WithParameter("@_Id", id);

            return this;
        }

        public IEntityQuery EffectiveAt(DateTime date)
        {
            Where("@_Date BETWEEN [" + _TableName + "].[FromDate] AND [" + _TableName + "].[ToDate]");
            WithParameter("@_Date", date);

            return this;
        }

        public IEntityQuery EffectiveBetween(DateTime fromDate, DateTime toDate)
        {
            Where("(([" + _TableName + "].[FromDate] <= @_FromDate) and ([" + _TableName + "].[ToDate] >= @_FromDate)) or (([" + _TableName + "].[FromDate] <= @_ToDate) and ([" + _TableName + "].[ToDate] >= @_FromDate))");
            WithParameter("@_FromDate", fromDate);
            WithParameter("@_ToDate", toDate);

            return this;
        }

        public IEntityQuery WithParameter(string name, string value)
        {
            var parameter = new SqliteParameter(name, value);
            _Parameters.Add(parameter);

            return this;
        }

        public IEntityQuery WithParameter(string name, DateTime value)
        {
            var parameter = new SqliteParameter(name, value.ToString("yyyy-MM-dd"));
            _Parameters.Add(parameter);

            return this;
        }

        public IEntityQuery WithParameter(string name, Guid value)
        {
            var parameter = new SqliteParameter(name, value.ToString());
            _Parameters.Add(parameter);

            return this;
        }

        public IEntityQuery WithParameter(string name, int value)
        {
            var parameter = new SqliteParameter(name, value);
            _Parameters.Add(parameter);

            return this;
        }

        public IEntityQuery WithParameter(string name, decimal value)
        {
            var parameter = new SqliteParameter(name, SQLiteUtils.DecimalToDB(value));
            _Parameters.Add(parameter);
           
            return this;
        }

        public IEntityQuery WithParameter(string name, bool value)
        {
            var parameter = new SqliteParameter(name, SQLiteUtils.BoolToDb(value));
            _Parameters.Add(parameter);

            return this;
        }

        public IEntityQuery OrderBy(string fields)
        {
            _Orderby = fields;

            return this;
        }

        public T CreateEntity<T>() where T : Entity
        {
            T entity;

            using (var reader = ExecuteSingle())
            {
                if (reader.Read())
                    entity = (T)_EntityCreator.CreateEntity<T>(reader);
                else
                    entity = null;
            }

            return entity;
        }

        public IEnumerable<T> CreateEntities<T>() where T : Entity
        {
            var list = new List<T>();

            using (var reader = Execute())
            {
                while (reader.Read())
                {
                    list.Add(_EntityCreator.CreateEntity<T>(reader));
                }
            }

            return list;
        }

        private SqliteCommand BuildQuery(bool single)
        {
            if (_SQL == "")
            { 
                _SQL = "SELECT " + _Fields + " FROM [" + _TableName + "]";
                foreach (var join in _Join)
                {
                    _SQL += join;
                }
                if (_Where != "")
                    _SQL += " WHERE " + _Where;
                if (_Orderby != "")
                    _SQL += " ORDER BY " + _Orderby;
                if (single)
                    _SQL += " LIMIT 1";
            }

            var query = new SqliteCommand(_SQL, _Transaction.Connection, _Transaction);
            query.Prepare();

            query.Parameters.AddRange(_Parameters.ToArray());

            return query;
        }

        public SqliteDataReader Execute()
        {
            var query = BuildQuery(false);

            return query.ExecuteReader();
        }

        public SqliteDataReader ExecuteSingle()
        {
            var query = BuildQuery(true);

            return query.ExecuteReader();
        }

        public bool ExecuteScalar(out int value)
        {
            var query = BuildQuery(false);

            var result = query.ExecuteScalar();
            if (result != null)
            {
                value = Convert.ToInt32((long)result);
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public bool ExecuteScalar(out decimal value)
        {
            var query = BuildQuery(false);

            var result = query.ExecuteScalar();
            if ((result != null) && (result != DBNull.Value))
            {
                value = SQLiteUtils.DBToDecimal((long)result);
                return true;
            }
            else
            {
                value = 0.00m;
                return false;
            }
        }

        public bool ExecuteScalar(out string value)
        {
            var query = BuildQuery(false);

            var result = query.ExecuteScalar();
            if (result != null)
            {
                value = (string)result;
                return true;
            }
            else
            {
                value = "";
                return false;
            }
        }

        public bool ExecuteScalar(out DateTime value)
        {
            var query = BuildQuery(false);

            var result = query.ExecuteScalar();
            if (result != null)
            {
                value = DateTime.ParseExact((string)result, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                return true;
            }
            else
            {
                value = DateUtils.NoDate;
                return false;
            }
        }

        public bool ExecuteScalar(out Guid value)
        {
            var query = BuildQuery(false);

            var result = query.ExecuteScalar();
            if (result != null)
            {
                value = new Guid((string)result);
                return true;
            }
            else
            {
                value = Guid.Empty;
                return false;
            }
        }

        public bool ExecuteScalar(out bool value)
        {
            var query = BuildQuery(false);

            var result = query.ExecuteScalar();
            if (result != null)
            {
                value = SQLiteUtils.DBToBool((string)result);
                return true;
            }
            else
            {
                value = false;
                return false;
            }
        }
    }

    abstract class SQLiteQuery : IDisposable
    {
        protected SqliteTransaction _Transaction;
        protected IEntityCreator _EntityCreator;

        protected internal SQLiteQuery(SqliteTransaction transaction, IEntityCreator entityCreator)
        {
            _Transaction = transaction;
            _EntityCreator = entityCreator;
        }

        protected IEntityQuery EntityQuery
        {
            get
            {
                return new SQLiteEntityQuery(_Transaction, _EntityCreator);
            }
        }

        public void Dispose()
        {
            _Transaction.Dispose();
        }
    }
}
