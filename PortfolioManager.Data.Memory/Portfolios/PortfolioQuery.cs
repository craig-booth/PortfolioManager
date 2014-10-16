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

        public IReadOnlyCollection<IncomeReceived> GetIncomeReceived(Guid portfolio, DateTime fromDate, DateTime toDate)
        {
            var incomeQuery = from income in _Database._Transactions 
                              where income.Type == TransactionType.Income && income.TransactionDate >= fromDate && income.TransactionDate <= toDate
                              orderby income.TransactionDate
                              select income as IncomeReceived;

            return incomeQuery.ToList().AsReadOnly();
        }
    }
}
