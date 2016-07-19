using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

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

        public IReadOnlyCollection<ShareParcel> GetAllParcels(DateTime fromDate, DateTime toDate)
        {
            var parcelsQuery = from parcel in _Database._Parcels
                               where fromDate >= parcel.FromDate && toDate <= parcel.ToDate
                               select parcel;

            return parcelsQuery.ToList().AsReadOnly();
        }


        public IReadOnlyCollection<ShareParcel> GetParcelsForStock(Guid stock, DateTime fromDate, DateTime toDate)
        {
            var parcelsQuery = from parcel in _Database._Parcels
                               where (parcel.Stock == stock) && ((fromDate >= parcel.FromDate && toDate <= parcel.ToDate))
                               select parcel;

            return parcelsQuery.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<CGTEvent> GetCGTEvents(DateTime fromDate, DateTime toDate)
        {
            var cgtQuery = from cgt in _Database._CGTEvents
                           where cgt.EventDate >= fromDate && cgt.EventDate <= toDate
                           orderby cgt.EventDate
                           select cgt;

            return cgtQuery.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<Transaction> GetTransactions(DateTime fromDate, DateTime toDate)
        {
            var list = new List<Transaction>();

            var query = new SQLiteCommand("SELECT * FROM [Transactions] WHERE [TransactionDate] BETWEEN @FromDate AND @ToDate ORDER BY [TransactionDate], [Sequence]", _Connection);
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

            var query = new SQLiteCommand("SELECT * FROM [Transactions] WHERE [Type] = @Type AND [TransactionDate] BETWEEN @FromDate AND @ToDate ORDER BY [TransactionDate], [Sequence]", _Connection);
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

            var query = new SQLiteCommand("SELECT * FROM [Transactions] WHERE [ASXCode] = @ASXCode AND [TransactionDate] BETWEEN @FromDate AND @ToDate ORDER BY [TransactionDate], [Sequence]", _Connection);
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

            var query = new SQLiteCommand("SELECT * FROM [Transactions] WHERE [ASXCode] = @ASXCode AND [Type] = @Type AND [TransactionDate] BETWEEN @FromDate AND @ToDate ORDER BY [TransactionDate], [Sequence]", _Connection);
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

    }
}
