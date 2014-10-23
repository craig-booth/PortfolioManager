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

        public Portfolio Get(Guid id)
        {
            return null;
        }

        public IReadOnlyCollection<Portfolio> GetAllPortfolios()
        {
            return _Database._Portfolios.AsReadOnly();
        }

        public ShareParcel GetParcel(Guid id, DateTime atDate)
        {
            var parcelsQuery = from parcel in _Database._Parcels
                               where (parcel.Id == id) && ((atDate >= parcel.FromDate && atDate <= parcel.ToDate))
                               select parcel;

            return parcelsQuery.First();
        }

        public IReadOnlyCollection<ShareParcel> GetAllParcels(Guid portfolio, DateTime atDate)
        {
            var parcelsQuery = from parcel in _Database._Parcels
                               where atDate >= parcel.FromDate && atDate <= parcel.ToDate
                               select parcel;

            return parcelsQuery.ToList().AsReadOnly();
        }


        public IReadOnlyCollection<ShareParcel> GetChildParcels(Guid parcel)
        {
            var parcelsQuery = from childParcel in _Database._Parcels
                               where childParcel.Parent == parcel
                               select childParcel;

            return parcelsQuery.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<ShareParcel> GetParcelsForStock(Guid portfolio, Guid stock, DateTime atDate)
        {
            var parcelsQuery = from parcel in _Database._Parcels
                               where (parcel.Stock == stock) && ((atDate >= parcel.FromDate && atDate <= parcel.ToDate))
                               select parcel;

            return parcelsQuery.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<CGTEvent> GetCGTEvents(Guid portfolio, DateTime fromDate, DateTime toDate)
        {
            var cgtQuery = from cgt in _Database._CGTEvents
                           where cgt.EventDate >= fromDate && cgt.EventDate <= toDate
                           orderby cgt.EventDate
                           select cgt;

            return cgtQuery.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<ITransaction> GetTransactions(Guid portfolio, DateTime fromDate, DateTime toDate)
        {
            var list = new List<ITransaction>();

            var query = new SQLiteCommand("SELECT * FROM [Transactions] WHERE [TransactionDate] BETWEEN @FromDate AND @ToDate", _Connection);
            query.Prepare();

            query.Parameters.AddWithValue("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            query.Parameters.AddWithValue("@ToDate", toDate.ToString("yyyy-MM-dd"));

            SQLiteDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                ITransaction transaction = SQLitePortfolioEntityCreator.CreateTransaction(_Database as SQLitePortfolioDatabase, reader);
                list.Add(transaction);
            }
            reader.Close();

            return list;
        }

        public IReadOnlyCollection<ITransaction> GetTransactions(Guid portfolio, TransactionType transactionType, DateTime fromDate, DateTime toDate)
        {
            var list = new List<ITransaction>();

            var query = new SQLiteCommand("SELECT * FROM [Transactions] WHERE [TransactionType] = @TransactionType AND [TransactionDate] BETWEEN @FromDate AND @ToDate", _Connection);
            query.Prepare();

            query.Parameters.AddWithValue("@TransactionType", transactionType);
            query.Parameters.AddWithValue("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            query.Parameters.AddWithValue("@ToDate", toDate.ToString("yyyy-MM-dd"));

            SQLiteDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                ITransaction transaction = SQLitePortfolioEntityCreator.CreateTransaction(_Database as SQLitePortfolioDatabase, reader);
                list.Add(transaction);
            }
            reader.Close();

            return list;
        }

        public IReadOnlyCollection<ITransaction> GetTransactions(Guid portfolio, string asxCode, DateTime fromDate, DateTime toDate)
        {
            var list = new List<ITransaction>();

            var query = new SQLiteCommand("SELECT * FROM [Transactions] WHERE [ASXCode] = @ASXCode AND [TransactionDate] BETWEEN @FromDate AND @ToDate", _Connection);
            query.Prepare();

            query.Parameters.AddWithValue("@ASXCode", asxCode);
            query.Parameters.AddWithValue("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            query.Parameters.AddWithValue("@ToDate", toDate.ToString("yyyy-MM-dd"));

            SQLiteDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                ITransaction transaction = SQLitePortfolioEntityCreator.CreateTransaction(_Database as SQLitePortfolioDatabase, reader);
                list.Add(transaction);
            }
            reader.Close();

            return list;
        }

        public IReadOnlyCollection<ITransaction> GetTransactions(Guid portfolio, string asxCode, TransactionType transactionType, DateTime fromDate, DateTime toDate)
        {
            var list = new List<ITransaction>();

            var query = new SQLiteCommand("SELECT * FROM [Transactions] WHERE [ASXCode] = @ASXCode AND [TransactionType] = @TransactionType AND [TransactionDate] BETWEEN @FromDate AND @ToDate", _Connection);
            query.Prepare();

            query.Parameters.AddWithValue("@ASXCode", asxCode);
            query.Parameters.AddWithValue("@TransactionType", transactionType);
            query.Parameters.AddWithValue("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            query.Parameters.AddWithValue("@ToDate", toDate.ToString("yyyy-MM-dd"));

            SQLiteDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                ITransaction transaction = SQLitePortfolioEntityCreator.CreateTransaction(_Database as SQLitePortfolioDatabase, reader);
                list.Add(transaction);
            }
            reader.Close();

            return list;
        }
    }
}
