using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.Memory.Portfolios
{
    public class MemoryPortfolioQuery : IPortfolioQuery
    {
        private MemoryPortfolioDatabase _Database;

        protected internal MemoryPortfolioQuery(MemoryPortfolioDatabase database)
        {
            _Database = database;
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
            var transactionQuery = from transaction in _Database._Transactions
                                   where transaction.TransactionDate >= fromDate && transaction.TransactionDate <= toDate 
                                   select transaction;

            return transactionQuery.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<Transaction> GetTransactions(TransactionType transactionType, DateTime fromDate, DateTime toDate)
        {
            var transactionQuery = from transaction in _Database._Transactions
                                   where transaction.Type == transactionType && transaction.TransactionDate >= fromDate && transaction.TransactionDate <= toDate
                                   select transaction;

            return transactionQuery.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<Transaction> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate)
        {
            var transactionQuery = from transaction in _Database._Transactions
                                   where transaction.ASXCode == asxCode && transaction.TransactionDate >= fromDate && transaction.TransactionDate <= toDate
                                   select transaction;

            return transactionQuery.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<Transaction> GetTransactions(string asxCode, TransactionType transactionType, DateTime fromDate, DateTime toDate)
        {
            var transactionQuery = from transaction in _Database._Transactions
                                   where transaction.ASXCode == asxCode && transaction.Type == transactionType && transaction.TransactionDate >= fromDate && transaction.TransactionDate <= toDate
                                   select transaction;

            return transactionQuery.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<CashAccountTransaction> GetCashAccountTransactions(DateTime fromDate, DateTime toDate)
        {
            return null;
        }
    }
}
