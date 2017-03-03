using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.Service.Interface
{
    public interface ITransactionService : IPortfolioManagerService
    {
        Task<AddTransactionsResponce> AddTransaction(TransactionItem transaction);
        Task<AddTransactionsResponce> AddTransactions(IEnumerable<TransactionItem> transactions);
        Task<UpdateTransactionsResponce> UpdateTransaction(TransactionItem transaction);
        Task<DeleteTransactionsResponce> DeleteTransaction(TransactionItem transaction);

        Task<GetTransactionsResponce> GetTransactions(DateTime fromDate, DateTime toDate);
        Task<GetTransactionsResponce> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate);
    }

    public class AddTransactionsResponce : ServiceResponce
    {
        public AddTransactionsResponce()
            : base()
        {
        }
    }

    public class UpdateTransactionsResponce : ServiceResponce
    {
        public UpdateTransactionsResponce()
            : base()
        {
        }
    }

    public class DeleteTransactionsResponce : ServiceResponce
    {
        public DeleteTransactionsResponce()
            : base()
        {
        }
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

    public class TransactionItem
    {
        public Guid Id { get; set; }
        public TransactionType Type { get; set;  }
        public DateTime TransactionDate { get; set; }
        public string ASXCode { get; set; }
        public DateTime RecordDate { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
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
