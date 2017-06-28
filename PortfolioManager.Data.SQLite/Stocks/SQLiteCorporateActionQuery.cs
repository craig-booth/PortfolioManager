using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class SQLiteCorporateActionQuery: ICorporateActionQuery
    {
        protected SQLiteStockDatabase _Database;
        protected SqliteConnection _Connection;
        private SQLiteStockEntityCreator _EntityCreator;

        protected internal SQLiteCorporateActionQuery(SQLiteStockDatabase database)
        {
            _Database = database;
            _Connection = database._Connection;
            _EntityCreator = new Stocks.SQLiteStockEntityCreator(database);
        }

        private SqliteCommand _GetCorporateActionById;
        public CorporateAction Get(Guid id)
        {
            if (_GetCorporateActionById == null)
            {
                _GetCorporateActionById = new SqliteCommand("SELECT * FROM [CorporateActions] WHERE [Id] = @Id", _Connection);
                _GetCorporateActionById.Prepare();
            }

            _GetCorporateActionById.Parameters.AddWithValue("@Id", id.ToString());

            CorporateAction corporateAction;
            using (SqliteDataReader reader = _GetCorporateActionById.ExecuteReader())
            {
                if (!reader.Read())
                {
                    throw new RecordNotFoundException("");
                }

                corporateAction = _EntityCreator.CreateEntity<CorporateAction>(reader);
            }

            return corporateAction;
        }

        private SqliteCommand _FindCorporateAction;
        public IEnumerable<CorporateAction> Find(Guid stock, DateTime fromDate, DateTime toDate)
        {
            var list = new List<CorporateAction>();

            if (_FindCorporateAction == null)
            {
                _FindCorporateAction = new SqliteCommand("SELECT * FROM [CorporateActions] WHERE [Stock] = @Stock AND [ActionDate] BETWEEN @FromDate AND @ToDate", _Connection);
                _FindCorporateAction.Prepare();
            }

            _FindCorporateAction.Parameters.AddWithValue("@Stock", stock.ToString());
            _FindCorporateAction.Parameters.AddWithValue("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            _FindCorporateAction.Parameters.AddWithValue("@ToDate", toDate.ToString("yyyy-MM-dd"));

            using (SqliteDataReader reader = _FindCorporateAction.ExecuteReader())
            {
                while (reader.Read())
                {
                    var corporateAction = _EntityCreator.CreateEntity<CorporateAction>(reader);
                    list.Add(corporateAction);
                }
            }

            return list;
        }
    }
}
