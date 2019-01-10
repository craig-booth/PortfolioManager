
using PortfolioManager.Common;
using PortfolioManager.RestApi.Client;
using PortfolioManager.UI.Models;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    class CashTransactionViewModel : TransactionViewModel
    {
        public BankAccountTransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }

        public CashTransactionViewModel(CashTransaction cashTransaction, RestWebClient restWebClient, RestClient restClient)
            : base(cashTransaction, TransactionStockSelection.None, restWebClient, restClient)
        {
            
        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
            {
                TransactionType = ((CashTransaction)Transaction).CashTransactionType;
                Amount = ((CashTransaction)Transaction).Amount;
             }
            else
            {
                TransactionType = BankAccountTransactionType.Deposit;
                Amount = 0.00m;
            }
        }

        protected override void CopyFieldsToTransaction()
        {
            if (Transaction == null)
                Transaction = new CashTransaction();

            base.CopyFieldsToTransaction();

            var cashTransaction = (CashTransaction)Transaction;
            cashTransaction.TransactionDate = RecordDate;
            cashTransaction.CashTransactionType = TransactionType;
            cashTransaction.Amount = Amount;
        }
    }
}
