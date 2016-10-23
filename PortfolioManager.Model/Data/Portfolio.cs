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
        IAttachmentRepository AttachmentRepository { get; }
        ICashAccountRepository CashAccountRepository { get; }
        IParcelAuditRepository ParcelAuditRepository { get; }

        void Save();
    }

    public interface IParcelRepository : IEffectiveDatedRepository<ShareParcel>
    {

    }

    public interface ITransactionRepository : IRepository<Transaction>
    {

    }

    public interface ICGTEventRepository : IRepository<CGTEvent>
    {

    }

    public interface IAttachmentRepository : IRepository<Attachment>
    {

    }

    public interface ICashAccountRepository : IRepository<CashAccountTransaction>
    {

    }

    public interface IParcelAuditRepository : IRepository<ShareParcelAudit>
    {

    }

    public interface IPortfolioQuery
    {
        ShareParcel GetParcel(Guid id, DateTime atDate);
        IReadOnlyCollection<ShareParcel> GetParcels(Guid id, DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<ShareParcel> GetAllParcels(DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<ShareParcel> GetParcelsForStock(Guid stock, DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<CGTEvent> GetCGTEvents(DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<Transaction> GetTransactions(DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<Transaction> GetTransactions(TransactionType transactionType, DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<Transaction> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<Transaction> GetTransactions(string asxCode, TransactionType transactionType, DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<CashAccountTransaction> GetCashAccountTransactions(DateTime fromDate, DateTime toDate);
        IReadOnlyCollection<ShareParcelAudit> GetParcelAudit(Guid id, DateTime fromDate, DateTime toDate);
    }
}
