
using PortfolioManager.Common;
using PortfolioManager.RestApi.Client;
using PortfolioManager.Service.Interface;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    class CashTransactionViewModel : TransactionViewModel
    {
        public BankAccountTransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }

        public CashTransactionViewModel(CashTransactionItem cashTransaction, RestClient restClient)
            : base(cashTransaction, TransactionStockSelection.None, restClient)
        {
            
        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
            {
                TransactionType = ((CashTransactionItem)Transaction).CashTransactionType;
                Amount = ((CashTransactionItem)Transaction).Amount;
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
                Transaction = new CashTransactionItem();

            base.CopyFieldsToTransaction();

            var cashTransaction = (CashTransactionItem)Transaction;
            cashTransaction.TransactionDate = RecordDate;
            cashTransaction.CashTransactionType = TransactionType;
            cashTransaction.Amount = Amount;
        }
    }
}
