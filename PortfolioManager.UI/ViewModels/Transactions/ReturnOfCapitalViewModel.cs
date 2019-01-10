using System;

using PortfolioManager.RestApi.Client;
using PortfolioManager.UI.Models;
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

        public ReturnOfCapitalViewModel(ReturnOfCapitalTransaction returnOfCapital, RestWebClient restWebClient, RestClient restClient)
            : base(returnOfCapital, TransactionStockSelection.Holdings, restWebClient, restClient)
        {

        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
            {
                PaymentDate = ((ReturnOfCapitalTransaction)Transaction).TransactionDate;
                Amount = ((ReturnOfCapitalTransaction)Transaction).Amount;
                CreateCashTransaction = ((ReturnOfCapitalTransaction)Transaction).CreateCashTransaction;
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
                Transaction = new ReturnOfCapitalTransaction();

            base.CopyFieldsToTransaction();

            var returnOfCapital = (ReturnOfCapitalTransaction)Transaction;
            returnOfCapital.TransactionDate = PaymentDate;
            returnOfCapital.Amount = Amount;
            returnOfCapital.CreateCashTransaction = CreateCashTransaction;
        }


    }
}
