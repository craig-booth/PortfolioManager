using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Common;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Service.Interface;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    class CashTransactionViewModel : TransactionViewModel
    {
        public BankAccountTransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }

        public CashTransactionViewModel(CashTransactionItem cashTransaction, StockService stockService, ShareHoldingService obsoleteHoldingService, IHoldingService holdingService)
            : base(cashTransaction, null, stockService, obsoleteHoldingService, holdingService)
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
            cashTransaction.ASXCode = "";
            cashTransaction.TransactionDate = RecordDate;
            cashTransaction.CashTransactionType = TransactionType;
            cashTransaction.Amount = Amount;
        }
    }
}
