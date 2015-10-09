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


        public IReadOnlyCollection<ShareParcel> GetChildParcels(Guid parcel, DateTime atDate)
        {
            var parcelsQuery = from childParcel in _Database._Parcels
                               where (childParcel.Parent == parcel) && (atDate >= childParcel.FromDate && atDate <= childParcel.ToDate)
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

        public IReadOnlyCollection<IncomeReceived> GetIncome(Guid portfolio, DateTime fromDate, DateTime toDate)
        {
            var transactionQuery = from transaction in _Database._Transactions
                                   where transaction.Type == TransactionType.Income
                                   select transaction as IncomeReceived;



            var incomeQuery = from income in transactionQuery
                              where income.PaymentDate >= fromDate && income.PaymentDate <= toDate
                              orderby income.PaymentDate
                              select income;


            return incomeQuery.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<ITransaction> GetTransactions(Guid portfolio, DateTime fromDate, DateTime toDate)
        {
            var transactionQuery = from transaction in _Database._Transactions
                                   where transaction.TransactionDate >= fromDate && transaction.TransactionDate <= toDate
                                   orderby transaction.TransactionDate
                                   select transaction;

            return transactionQuery.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<ITransaction> GetTransactions(Guid portfolio, TransactionType transactionType, DateTime fromDate, DateTime toDate)
        {
            var transactionQuery = from transaction in _Database._Transactions
                                   where transaction.Type == transactionType && transaction.TransactionDate >= fromDate && transaction.TransactionDate <= toDate
                                   orderby transaction.TransactionDate
                                   select transaction;

            return transactionQuery.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<ITransaction> GetTransactions(Guid portfolio, string asxCode, DateTime fromDate, DateTime toDate)
        {
            var transactionQuery = from transaction in _Database._Transactions
                                   where transaction.ASXCode == asxCode && transaction.TransactionDate >= fromDate && transaction.TransactionDate <= toDate
                                   orderby transaction.TransactionDate
                                   select transaction;

            return transactionQuery.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<ITransaction> GetTransactions(Guid portfolio, string asxCode, TransactionType transactionType, DateTime fromDate, DateTime toDate)
        {
            var transactionQuery = from transaction in _Database._Transactions
                                   where transaction.ASXCode == asxCode && transaction.Type == transactionType && transaction.TransactionDate >= fromDate && transaction.TransactionDate <= toDate
                                   orderby transaction.TransactionDate
                                   select transaction;

            return transactionQuery.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<OwnedStock> GetStocksInPortfolio(Guid portfolio)
        {
            List<OwnedStock> ownedStocks = new List<OwnedStock>();

            var parcelsQuery = from parcel in _Database._Parcels
                               orderby parcel.Stock, parcel.FromDate 
                               select parcel;

            OwnedStock currentStock = null;
            foreach (ShareParcel shareParcel in parcelsQuery)
            {
                if ((currentStock != null) && (shareParcel.Id == currentStock.Id) && (shareParcel.FromDate < currentStock.ToDate))
                {
                    if (shareParcel.ToDate > currentStock.ToDate)
                        currentStock.ToDate = shareParcel.ToDate;
                }
                else
                {
                    currentStock = new OwnedStock()
                    {
                        Id = shareParcel.Id,
                        FromDate = shareParcel.FromDate,
                        ToDate = shareParcel.ToDate
                    };
                    ownedStocks.Add(currentStock);
                }
            }

            return ownedStocks.AsReadOnly();
        }   
    }
}
