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

    class SQLitePortfolioQuery : IPortfolioQuery
    {
        private SQLitePortfolioDatabase _Database;
        protected SQLiteConnection _Connection;

        protected internal SQLitePortfolioQuery(SQLitePortfolioDatabase database)
        {
            _Database = database;
            _Connection = database._Connection;
        }

        public ShareParcel GetParcel(Guid id, DateTime atDate)
        {
            var parcelsQuery = from parcel in _Database._Parcels
                               where (parcel.Id == id) && ((atDate >= parcel.FromDate && atDate <= parcel.ToDate))
                               select parcel;

            return parcelsQuery.First();
        }

        public IReadOnlyCollection<ShareParcel> GetParcels(Guid id, DateTime fromDate, DateTime toDate)
        {
            var parcelsQuery = from parcel in _Database._Parcels
                               where (parcel.Id == id) &&
                                   (((parcel.FromDate <= fromDate) && (parcel.ToDate >= fromDate)) || ((parcel.FromDate <= toDate) && (parcel.ToDate >= fromDate)))
                               select parcel;

            return parcelsQuery.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<ShareParcel> GetAllParcels(DateTime fromDate, DateTime toDate)
        {
            var parcelsQuery = from parcel in _Database._Parcels
                               where (parcel.FromDate <= fromDate && parcel.ToDate >= fromDate) || (parcel.FromDate >= fromDate && parcel.FromDate <= toDate)
                               select parcel;

            return parcelsQuery.ToList().AsReadOnly();
        }


        public IReadOnlyCollection<ShareParcel> GetParcelsForStock(Guid stock, DateTime fromDate, DateTime toDate)
        {
            var parcelsQuery = from parcel in _Database._Parcels
                               where (parcel.Stock == stock) && ((parcel.FromDate <= fromDate && parcel.ToDate >= fromDate) || (parcel.FromDate >= fromDate && parcel.FromDate <= toDate))
                               select parcel;

            return parcelsQuery.ToList().AsReadOnly();
        }

        public bool StockOwned(Guid id, DateTime atDate)
        {
            var parcelsQuery = from parcel in _Database._Parcels
                               where (parcel.Stock == id) && (parcel.FromDate <= atDate && parcel.ToDate >= atDate)
                               select parcel;

            return parcelsQuery.Any();
        }

        public IReadOnlyCollection<Guid> GetStocksOwned(DateTime atDate)
        {
            var parcelsQuery = from parcel in _Database._Parcels
                               where (parcel.FromDate <= atDate && parcel.ToDate >= atDate)
                               group parcel by parcel.Stock into stockGroup
                               select stockGroup.Key;

            return parcelsQuery.Distinct().ToList().AsReadOnly();
        }

        public IReadOnlyCollection<CGTEvent> GetCGTEvents(DateTime fromDate, DateTime toDate)
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
                transaction = SQLitePortfolioEntityCreator.CreateTransaction(_Database as SQLitePortfolioDatabase, reader);
            else
                transaction = null;

            reader.Close();

            return transaction;
        }

        public IReadOnlyCollection<Transaction> GetTransactions(DateTime fromDate, DateTime toDate)
        {
            var list = new List<Transaction>();

            var query = new SQLiteCommand("SELECT * FROM [Transactions] WHERE [TransactionDate] BETWEEN @FromDate AND @ToDate ORDER BY [RecordDate], [Sequence]", _Connection);
            query.Prepare();

            query.Parameters.AddWithValue("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            query.Parameters.AddWithValue("@ToDate", toDate.ToString("yyyy-MM-dd"));

            SQLiteDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                var transaction = SQLitePortfolioEntityCreator.CreateTransaction(_Database as SQLitePortfolioDatabase, reader);
                list.Add(transaction);
            }
            reader.Close();

            return list;
        }

        public IReadOnlyCollection<Transaction> GetTransactions(TransactionType transactionType, DateTime fromDate, DateTime toDate)
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
                var transaction = SQLitePortfolioEntityCreator.CreateTransaction(_Database as SQLitePortfolioDatabase, reader);
                list.Add(transaction);
            }
            reader.Close();

            return list;
        }

        public IReadOnlyCollection<Transaction> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate)
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
                var transaction = SQLitePortfolioEntityCreator.CreateTransaction(_Database as SQLitePortfolioDatabase, reader);
                list.Add(transaction);
            }
            reader.Close();

            return list;
        }

        public IReadOnlyCollection<Transaction> GetTransactions(string asxCode, TransactionType transactionType, DateTime fromDate, DateTime toDate)
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
                var transaction = SQLitePortfolioEntityCreator.CreateTransaction(_Database as SQLitePortfolioDatabase, reader);
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

        public IReadOnlyCollection<CashAccountTransaction> GetCashAccountTransactions(DateTime fromDate, DateTime toDate)
        {
            return _Database._CashAccountTransactions.Where(t => (t.Date >= fromDate) && (t.Date <= toDate)).OrderBy(x => x.Date).ThenByDescending(x => x.Amount).ToList();
        }

        public IReadOnlyCollection<ShareParcelAudit> GetParcelAudit(Guid id, DateTime fromDate, DateTime toDate)
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
