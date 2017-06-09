using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.Service.Interface
{
    public interface ITransactionService : IPortfolioService
    {
        Task<ServiceResponce> AddTransaction(TransactionItem transactionItem);
        Task<ServiceResponce> AddTransactions(IEnumerable<TransactionItem> transactionItems);
        Task<ServiceResponce> UpdateTransaction(TransactionItem transactionItem);
        Task<ServiceResponce> DeleteTransaction(TransactionItem transactionItem);

        Task<GetTransactionsResponce> GetTransactions(DateTime fromDate, DateTime toDate);
        Task<GetTransactionsResponce> GetTransactions(Guid stockId, DateTime fromDate, DateTime toDate);
    }

    public class GetTransactionsResponce : ServiceResponce
    {
        public List<TransactionItem> Transactions;

        public GetTransactionsResponce()
            : base()
        {
            Transactions = new List<TransactionItem>();
        }
    }

    public abstract class TransactionItem
    {
        public Guid Id { get; set; }
        public StockItem Stock { get; set; }
        public TransactionType Type { get; set;  }
        public DateTime TransactionDate { get; set; }
        public DateTime RecordDate { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public Guid Attachment { get; set; }
    }

    public class AquisitionTransactionItem : TransactionItem
    {
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public bool CreateCashTransaction { get; set; }
    }

    public class CashTransactionItem : TransactionItem
    {
        public BankAccountTransactionType CashTransactionType { get; set; }
        public decimal Amount { get; set; }
    }

    public class CostBaseAdjustmentTransactionItem : TransactionItem
    {
        public decimal Percentage { get; set; }
    }

    public class DisposalTransactionItem : TransactionItem
    {
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public CGTCalculationMethod CGTMethod { get; set; }
        public bool CreateCashTransaction { get; set; }
    }

    public class IncomeTransactionItem : TransactionItem
    {
        public decimal FrankedAmount { get; set; }
        public decimal UnfrankedAmount { get; set; }
        public decimal FrankingCredits { get; set; }
        public decimal Interest { get; set; }
        public decimal TaxDeferred { get; set; }
        public bool CreateCashTransaction { get; set; }
        public decimal DRPCashBalance { get; set; }
    }

    public class OpeningBalanceTransactionItem : TransactionItem
    {
        public int Units { get; set; }
        public decimal CostBase { get; set; }
        public DateTime AquisitionDate { get; set; }
        public Guid PurchaseId { get; set; }
    }

    public class ReturnOfCapitalTransactionItem : TransactionItem
    {
        public decimal Amount { get; set; }
        public bool CreateCashTransaction { get; set; }
    }

    public class UnitCountAdjustmentTransactionItem : TransactionItem
    {
        public int OriginalUnits { get; set; }
        public int NewUnits { get; set; }
    }
}
