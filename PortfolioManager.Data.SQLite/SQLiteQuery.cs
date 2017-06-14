using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Data.SQLite;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.SQLite
{
    interface IEntityQuery
    {
        IEntityQuery SQL(string sql);

        IEntityQuery FromTable(string table);
        IEntityQuery Select(string fields);
        IEntityQuery Where(string condition);

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

        SQLiteDataReader Execute();
        SQLiteDataReader ExecuteSingle();

        bool ExecuteScalar(out int value);
        bool ExecuteScalar(out decimal value);
        bool ExecuteScalar(out string value);
        bool ExecuteScalar(out DateTime value);
        bool ExecuteScalar(out Guid value);
        bool ExecuteScalar(out bool value);
    }

    class SQLiteEntityQuery : IEntityQuery
    {
        private SQLiteConnection _Connection;
        private IEntityCreator _EntityCreator;
        private string _TableName;
        private string _Where;
        private string _SQL;
        private string _Fields;
        private string _Orderby;
        private List<SQLiteParameter> _Parameters;

        public SQLiteEntityQuery(SQLiteConnection connection, IEntityCreator entityCreator)
        {
            _Connection = connection;
            _EntityCreator = entityCreator;
            _SQL = "";
            _TableName = "";
            _Where = "";
            _Fields = "*";
            _Orderby = "";
            _Parameters = new List<SQLiteParameter>();
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

        public IEntityQuery WithId(Guid id)
        {
            Where("[Id] = @_Id");
            WithParameter("@_Id", id);

            return this;
        }

        public IEntityQuery EffectiveAt(DateTime date)
        {
            Where("@_Date BETWEEN [FromDate] AND [ToDate]");
            WithParameter("@_Date", date);

            return this;
        }

        public IEntityQuery EffectiveBetween(DateTime fromDate, DateTime toDate)
        {
            Where("(([FromDate] <= @_FromDate) and ([ToDate] >= @_FromDate)) or (([FromDate] <= @_ToDate) and ([ToDate] >= @_FromDate))");
            WithParameter("@_FromDate", fromDate);
            WithParameter("@_ToDate", toDate);

            return this;
        }

        public IEntityQuery WithParameter(string name, string value)
        {
            var parameter = new SQLiteParameter(name, value);
            _Parameters.Add(parameter);

            return this;
        }

        public IEntityQuery WithParameter(string name, DateTime value)
        {
            var parameter = new SQLiteParameter(name, value.ToString("yyyy-MM-dd"));
            _Parameters.Add(parameter);

            return this;
        }

        public IEntityQuery WithParameter(string name, Guid value)
        {
            var parameter = new SQLiteParameter(name, value.ToString());
            _Parameters.Add(parameter);

            return this;
        }

        public IEntityQuery WithParameter(string name, int value)
        {
            var parameter = new SQLiteParameter(name, value);
            _Parameters.Add(parameter);

            return this;
        }

        public IEntityQuery WithParameter(string name, decimal value)
        {
            var parameter = new SQLiteParameter(name, SQLiteUtils.DecimalToDB(value));
            _Parameters.Add(parameter);
           
            return this;
        }

        public IEntityQuery WithParameter(string name, bool value)
        {
            var parameter = new SQLiteParameter(name, SQLiteUtils.BoolToDb(value));
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

            var reader = ExecuteSingle();
            if (reader.Read())
                entity = (T)_EntityCreator.CreateEntity<T>(reader);
            else
                entity = null;

            reader.Close();

            return entity;
        }

        public IEnumerable<T> CreateEntities<T>() where T : Entity
        {
            var list = new List<T>();

            var reader = Execute();
            while (reader.Read())
            {
                list.Add(_EntityCreator.CreateEntity<T>(reader));
            }
            reader.Close();

            return list;
        }

        private SQLiteCommand BuildQuery(bool single)
        {
            if (_SQL == "")
            { 
                _SQL = "SELECT " + _Fields + " FROM [" + _TableName + "]";
                if (_Where != "")
                    _SQL += " WHERE " + _Where;
                if (_Orderby != "")
                    _SQL += " ORDER BY " + _Orderby;
                if (single)
                    _SQL += " LIMIT 1";
            }

            var query = new SQLiteCommand(_SQL, _Connection);
            query.Prepare();

            query.Parameters.AddRange(_Parameters.ToArray());

            return query;
        }

        public SQLiteDataReader Execute()
        {
            var query = BuildQuery(false);

            return query.ExecuteReader();
        }

        public SQLiteDataReader ExecuteSingle()
        {
            var query = BuildQuery(true);

            return query.ExecuteReader();
        }

        public bool ExecuteScalar(out int value)
        {
            var query = BuildQuery(true);

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
            var query = BuildQuery(true);

            var result = query.ExecuteScalar();
            if (result != null)
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
            var query = BuildQuery(true);

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
            var query = BuildQuery(true);

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
            var query = BuildQuery(true);

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
            var query = BuildQuery(true);

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

    abstract class SQLiteQuery
    {
        protected SQLiteConnection _Connection;
        protected IEntityCreator _EntityCreator;

        protected internal SQLiteQuery(SQLiteConnection connection, IEntityCreator entityCreator)
        {
            _Connection = connection;
            _EntityCreator = entityCreator;
        }

        protected IEntityQuery EntityQuery
        {
            get
            {
                return new SQLiteEntityQuery(_Connection, _EntityCreator);
            }
        }

    }
}
