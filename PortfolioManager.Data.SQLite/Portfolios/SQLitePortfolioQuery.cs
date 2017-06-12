using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLitePortfolioQuery : SQLiteQuery, IPortfolioQuery
    {
        private SQLitePortfolioDatabase _Database;

        protected internal SQLitePortfolioQuery(SQLitePortfolioDatabase database)
            : base(database._Connection, new SQLitePortfolioEntityCreator(database))
        {
            _Database = database;
        }

        public ShareParcel GetParcel(Guid id, DateTime atDate)
        {
            var query = EntityQuery.FromTable("Parcels")
                .WithId(id)
                .EffectiveAt(atDate);

            return query.CreateEntity<ShareParcel>();
        }

        public IEnumerable<ShareParcel> GetParcels(Guid id, DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("Parcels")
                            .WithId(id)
                            .EffectiveBetween(fromDate, toDate);

            return query.CreateEntities<ShareParcel>();
        }

        public IEnumerable<ShareParcel> GetAllParcels(DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("Parcels")
                            .EffectiveBetween(fromDate, toDate);

            return query.CreateEntities<ShareParcel>();
        }


        public IEnumerable<ShareParcel> GetParcelsForStock(Guid stock, DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("Parcels")
                .Where("[Stock] = @Stock")
                .WithParameter("@Stock", stock)
                .EffectiveBetween(fromDate, toDate);

            return query.CreateEntities<ShareParcel>();
        }

        public bool StockOwned(Guid id, DateTime atDate)
        {
            bool result;

            var query = EntityQuery.FromTable("Parcels")
                    .Where("[Stock] = @Stock")
                    .WithParameter("@Stock", id)
                    .EffectiveAt(atDate);

            var reader = query.GetFields("[Id]");
            result = reader.HasRows;

            reader.Close();

            return result;
        }

        public IEnumerable<Guid> GetStocksOwned(DateTime fromDate, DateTime toDate)
        {
            List<Guid> stocks = new List<Guid>();

            var query = EntityQuery.FromTable("Parcels")
                    .EffectiveBetween(fromDate, toDate);

            var reader = query.GetFields("DISTINCT [Stock]");
            while (reader.Read())
            {
                stocks.Add(new Guid(reader.GetString(0)));
            }

            reader.Close();

            return stocks;
        }

        public IEnumerable<CGTEvent> GetCGTEvents(DateTime fromDate, DateTime toDate)
        {
            var cgtQuery = from cgt in _Database._CGTEvents
                           where cgt.EventDate >= fromDate && cgt.EventDate <= toDate
                           orderby cgt.EventDate
                           select cgt;

            return cgtQuery.ToList().AsReadOnly();
        }

        public Transaction GetTransaction(Guid id)
        {
            Transaction transaction;

            var query = new SQLiteCommand("SELECT * FROM [Transactions] WHERE [Id] = @Id", _Connection);
            query.Prepare();

            query.Parameters.AddWithValue("@Id", id.ToString());

            SQLiteDataReader reader = query.ExecuteReader();

            if (reader.Read())
                transaction = _EntityCreator.CreateEntity<Transaction>(reader);
            else
                transaction = null;

            reader.Close();

            return transaction;
        }

        public IEnumerable<Transaction> GetTransactions(DateTime fromDate, DateTime toDate)
        {
            var list = new List<Transaction>();

            var query = new SQLiteCommand("SELECT * FROM [Transactions] WHERE [TransactionDate] BETWEEN @FromDate AND @ToDate ORDER BY [RecordDate], [Sequence]", _Connection);
            query.Prepare();

            query.Parameters.AddWithValue("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            query.Parameters.AddWithValue("@ToDate", toDate.ToString("yyyy-MM-dd"));

            SQLiteDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                var transaction = _EntityCreator.CreateEntity<Transaction>(reader);
                list.Add(transaction);
            }
            reader.Close();

            return list;
        }

        public IEnumerable<Transaction> GetTransactions(TransactionType transactionType, DateTime fromDate, DateTime toDate)
        {
            var list = new List<Transaction>();

            var query = new SQLiteCommand("SELECT * FROM [Transactions] WHERE [Type] = @Type AND [TransactionDate] BETWEEN @FromDate AND @ToDate ORDER BY [RecordDate], [Sequence]", _Connection);
            query.Prepare();

            query.Parameters.AddWithValue("@Type", transactionType);
            query.Parameters.AddWithValue("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            query.Parameters.AddWithValue("@ToDate", toDate.ToString("yyyy-MM-dd"));

            SQLiteDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                var transaction = _EntityCreator.CreateEntity<Transaction>(reader);
                list.Add(transaction);
            }
            reader.Close();

            return list;
        }

        public IEnumerable<Transaction> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate)
        {
            var list = new List<Transaction>();

            var query = new SQLiteCommand("SELECT * FROM [Transactions] WHERE [ASXCode] = @ASXCode AND [TransactionDate] BETWEEN @FromDate AND @ToDate ORDER BY [RecordDate], [Sequence]", _Connection);
            query.Prepare();

            query.Parameters.AddWithValue("@ASXCode", asxCode);
            query.Parameters.AddWithValue("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            query.Parameters.AddWithValue("@ToDate", toDate.ToString("yyyy-MM-dd"));

            SQLiteDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                var transaction = _EntityCreator.CreateEntity<Transaction>(reader);
                list.Add(transaction);
            }
            reader.Close();

            return list;
        }

        public IEnumerable<Transaction> GetTransactions(string asxCode, TransactionType transactionType, DateTime fromDate, DateTime toDate)
        {
            var list = new List<Transaction>();

            var query = new SQLiteCommand("SELECT * FROM [Transactions] WHERE [ASXCode] = @ASXCode AND [Type] = @Type AND [TransactionDate] BETWEEN @FromDate AND @ToDate ORDER BY [RecordDate], [Sequence]", _Connection);
            query.Prepare();

            query.Parameters.AddWithValue("@ASXCode", asxCode);
            query.Parameters.AddWithValue("@Type", transactionType);
            query.Parameters.AddWithValue("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            query.Parameters.AddWithValue("@ToDate", toDate.ToString("yyyy-MM-dd"));

            SQLiteDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                var transaction = _EntityCreator.CreateEntity<Transaction>(reader);
                list.Add(transaction);
            }
            reader.Close();

            return list;
        }

        public decimal GetCashBalance(DateTime atDate)
        {
            // Sum up transactions prior to the request date
            var transactions = GetCashAccountTransactions(DateUtils.NoStartDate, atDate);

            return transactions.Sum(x => x.Amount);
        }

        public IEnumerable<CashAccountTransaction> GetCashAccountTransactions(DateTime fromDate, DateTime toDate)
        {
            return _Database._CashAccountTransactions.Where(t => (t.Date >= fromDate) && (t.Date <= toDate)).OrderBy(x => x.Date).ThenByDescending(x => x.Amount).ToList();
        }

        public IEnumerable<ShareParcelAudit> GetParcelAudit(Guid id, DateTime fromDate, DateTime toDate)
        {
            return _Database._ParcelAudit.Where(x => (x.Parcel == id) && (x.Date >= fromDate) && (x.Date <= toDate)).ToList();
        }

        public StockSetting GetStockSetting(Guid stock, DateTime atDate)
        {
            var stockSettingQuery = from stockSetting in _Database._StockSettings
                                  where (stockSetting.Id == stock) && ((atDate >= stockSetting.FromDate && atDate <= stockSetting.ToDate))
                                  select stockSetting;

            return stockSettingQuery.FirstOrDefault();
        }

        public decimal GetDRPCashBalance(Guid stock, DateTime atDate)
        {
            var drpBalanceQuery = from drpBalance in _Database._DRPCashBalances
                               where (drpBalance.Id == stock) && ((atDate >= drpBalance.FromDate && atDate <= drpBalance.ToDate))
                               select drpBalance.Balance;

            return drpBalanceQuery.FirstOrDefault();
        }

    }
}
