using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Data
{

    public interface IPortfolioDatabase
    {
        IPortfolioUnitOfWork CreateUnitOfWork();
        IPortfolioQuery PortfolioQuery { get; }
    }

    public interface IPortfolioUnitOfWork : IDisposable
    {
        IPortfolioRepository PortfolioRepository { get; }
        IParcelRepository ParcelRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        ICGTEventRepository CGTEventRepository { get; }

        void Save();
    }

    public interface IPortfolioRepository : IRepository<Portfolio>
    {
        
    }

    public interface IParcelRepository : IEffectiveDatedRepository<ShareParcel>
    {

    }

    public interface ITransactionRepository : IRepository<ITransaction>
    {

    }

    public interface ICGTEventRepository : IRepository<CGTEvent>
    {

    }

    public interface IPortfolioQuery
    {
        Portfolio Get(Guid id);

        IReadOnlyCollection<Portfolio> GetAllPortfolios();

        ShareParcel GetParcel(Guid id, DateTime atDate);
        IReadOnlyCollection<ShareParcel> GetChildParcels(Guid parcel);

        IReadOnlyCollection<ShareParcel> GetAllParcels(Guid portfolio, DateTime atDate);
        IReadOnlyCollection<ShareParcel> GetParcelsForStock(Guid portfolio, Guid stock, DateTime atDate);  
        IReadOnlyCollection<CGTEvent> GetCGTEvents(Guid portfolio, DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<IncomeReceived> GetIncome(Guid portfolio, DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<ITransaction> GetTransactions(Guid portfolio, DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<ITransaction> GetTransactions(Guid portfolio, TransactionType transactionType, DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<ITransaction> GetTransactions(Guid portfolio, string asxCode, DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<ITransaction> GetTransactions(Guid portfolio, string asxCode, TransactionType transactionType, DateTime fromDate, DateTime toDate);

        IReadOnlyCollection<OwnedStock> GetStocksInPortfolio(Guid portfolio);
    }

    public class OwnedStock
    {
        public Guid Id;
        public DateTime FromDate;
        public DateTime ToDate;
    }

}
