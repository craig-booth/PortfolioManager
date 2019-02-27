using System;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Client;
using PortfolioManager.RestApi.Transactions;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    class CashTransactionViewModel : TransactionViewModel
    {
        public BankAccountTransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }

        public CashTransactionViewModel(CashTransaction cashTransaction, RestClient restClient)
            : base(cashTransaction, "Cash Transaction", TransactionStockSelection.None, restClient)
        {
            
        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (_Transaction != null)
            {
                TransactionType = RestApiNameMapping.ToBankAccountTransactionType(((CashTransaction)_Transaction).CashTransactionType);
                Amount = ((CashTransaction)_Transaction).Amount;
             }
            else
            {
                TransactionType = BankAccountTransactionType.Deposit;
                Amount = 0.00m;
            }

            Stock = new StockViewItem(Guid.Empty, "", "");
        }

        protected override void CopyFieldsToTransaction()
        {
            if (_Transaction == null)
                _Transaction = new CashTransaction();

            base.CopyFieldsToTransaction();

            var cashTransaction = (CashTransaction)_Transaction;
            cashTransaction.TransactionDate = RecordDate;
            cashTransaction.CashTransactionType = TransactionType.ToRestName();
            cashTransaction.Amount = Amount;
        }
    }
}
