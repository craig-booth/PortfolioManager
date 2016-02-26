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
        IParcelRepository ParcelRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        ICGTEventRepository CGTEventRepository { get; }

        void Save();
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
        ShareParcel GetParcel(Guid id, DateTime atDate);
        IReadOnlyCollection<ShareParcel> GetAllParcels(DateTime atDate);
        IReadOnlyCollection<ShareParcel> GetParcelsForStock(Guid stock, DateTime atDate);
        IReadOnlyCollection<CGTEvent> GetCGTEvents(DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<ITransaction> GetTransactions(DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<ITransaction> GetTransactions(TransactionType transactionType, DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<ITransaction> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<ITransaction> GetTransactions(string asxCode, TransactionType transactionType, DateTime fromDate, DateTime toDate);
    }
}
