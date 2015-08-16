using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class SQLiteCorporateActionQuery: ICorporateActionQuery
    {
        protected SQLiteDatabase _Database;
        protected SQLiteConnection _Connection;

        protected internal SQLiteCorporateActionQuery(SQLiteDatabase database)
        {
            _Database = database;
            _Connection = database._Connection;
        }

        private SQLiteCommand _GetCorporateActionById;
        public ICorporateAction Get(Guid id)
        {
            if (_GetCorporateActionById == null)
            {
                _GetCorporateActionById = new SQLiteCommand("SELECT * FROM [CorporateActions] WHERE [Id] = @Id", _Connection);
                _GetCorporateActionById.Prepare();
            }

            _GetCorporateActionById.Parameters.AddWithValue("@Id", id.ToString());

            SQLiteDataReader reader = _GetCorporateActionById.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                throw new RecordNotFoundException("");
            }

            ICorporateAction corpoarateAction = SQLiteStockEntityCreator.CreateCorporateAction(_Database as SQLiteStockDatabase, reader);
            reader.Close();

            return corpoarateAction;
        }

        private SQLiteCommand _FindCorporateAction;
        public IReadOnlyCollection<ICorporateAction> Find(Guid stock, DateTime fromDate, DateTime toDate)
        {
            var list = new List<ICorporateAction>();

            if (_FindCorporateAction == null)
            {
                _FindCorporateAction = new SQLiteCommand("SELECT * FROM [CorporateActions] WHERE [Stock] = @Stock AND [ActionDate] BETWEEN @FromDate AND @ToDate", _Connection);
                _FindCorporateAction.Prepare();
            }

            _FindCorporateAction.Parameters.AddWithValue("@Stock", stock.ToString());
            _FindCorporateAction.Parameters.AddWithValue("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            _FindCorporateAction.Parameters.AddWithValue("@ToDate", toDate.ToString("yyyy-MM-dd"));

            SQLiteDataReader reader = _FindCorporateAction.ExecuteReader();

            while (reader.Read())
            {
                ICorporateAction corporateAction = SQLiteStockEntityCreator.CreateCorporateAction(_Database as SQLiteStockDatabase, reader);
                list.Add(corporateAction);
            }    
            reader.Close();

            return list;
        }
    }
}
