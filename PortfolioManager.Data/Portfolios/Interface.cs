using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.Data.Portfolios
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
        IDRPCashBalanceRepository DRPCashBalanceRepository { get; }
        IStockSettingRepository StockSettingRepository { get; }

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

    public interface IStockSettingRepository : IEffectiveDatedRepository<StockSetting>
    {

    }

    public interface IDRPCashBalanceRepository : IEffectiveDatedRepository<DRPCashBalance>
    {

    }

    public interface IPortfolioQuery
    {
        ShareParcel GetParcel(Guid id, DateTime atDate);
        IEnumerable<ShareParcel> GetParcels(Guid id, DateTime fromDate, DateTime toDate);
        IEnumerable<ShareParcel> GetAllParcels(DateTime fromDate, DateTime toDate);
        IEnumerable<ShareParcel> GetParcelsForStock(Guid stock, DateTime fromDate, DateTime toDate);

        bool StockOwned(Guid id, DateTime atDate);
        IEnumerable<Guid> GetStocksOwned(DateTime fromDate, DateTime toDate);

        IEnumerable<CGTEvent> GetCGTEvents(DateTime fromDate, DateTime toDate);

        Transaction GetTransaction(Guid id);
        IEnumerable<Transaction> GetTransactions(DateTime fromDate, DateTime toDate);
        IEnumerable<Transaction> GetTransactions(TransactionType transactionType, DateTime fromDate, DateTime toDate);
        IEnumerable<Transaction> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate);
        IEnumerable<Transaction> GetTransactions(string asxCode, TransactionType transactionType, DateTime fromDate, DateTime toDate);

        decimal GetCashBalance(DateTime atDate);
        IEnumerable<CashAccountTransaction> GetCashAccountTransactions(DateTime fromDate, DateTime toDate);

        IEnumerable<ShareParcelAudit> GetParcelAudit(Guid id, DateTime fromDate, DateTime toDate);

        StockSetting GetStockSetting(Guid stock, DateTime atDate);

        DRPCashBalance GetDRPCashBalance(Guid stock, DateTime atDate);
        decimal GetDRPBalance(Guid stock, DateTime atDate);
    }
}
