using System;

using PortfolioManager.Service.Interface;
using PortfolioManager.RestApi.Client;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    class ReturnOfCapitalViewModel : TransactionViewModel
    {
        private decimal _Amount;
        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                _Amount = value;

                ClearErrors();

                if (_Amount < 0.00m)
                    AddError("Amount must not be less than 0");
            }
        }

        private DateTime _PaymentDate;
        public DateTime PaymentDate
        {
            get
            {
                return _PaymentDate;
            }
            set
            {
                _PaymentDate = value;

                ClearErrors();

                if (_PaymentDate < RecordDate)
                    AddError("Payment Date must be after the Record date");
            }
        }

        public bool CreateCashTransaction { get; set; }

        public ReturnOfCapitalViewModel(ReturnOfCapitalTransactionItem returnOfCapital, RestClient restClient)
            : base(returnOfCapital, TransactionStockSelection.Holdings, restClient)
        {

        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
            {
                PaymentDate = ((ReturnOfCapitalTransactionItem)Transaction).TransactionDate;
                Amount = ((ReturnOfCapitalTransactionItem)Transaction).Amount;
                CreateCashTransaction = ((ReturnOfCapitalTransactionItem)Transaction).CreateCashTransaction;
            }
            else
            {
                PaymentDate = RecordDate;
                Amount = 0.00m;
                CreateCashTransaction = true;
            }
        }

        protected override void CopyFieldsToTransaction()
        {
            if (Transaction == null)
                Transaction = new ReturnOfCapitalTransactionItem();

            base.CopyFieldsToTransaction();

            var returnOfCapital = (ReturnOfCapitalTransactionItem)Transaction;
            returnOfCapital.TransactionDate = PaymentDate;
            returnOfCapital.Amount = Amount;
            returnOfCapital.CreateCashTransaction = CreateCashTransaction;
        }


    }
}
