using System;

using PortfolioManager.RestApi.Client;
using PortfolioManager.RestApi.Transactions;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    class IncomeReceivedViewModel : TransactionViewModel
    {

        private decimal _FrankedAmount;
        public decimal FrankedAmount
        {
            get
            {
                return _FrankedAmount;
            }
            set
            {
                _FrankedAmount = value;

                ClearErrors();

                if (_FrankedAmount < 0.00m)
                    AddError("Franked amounr must not be less than 0");
            }
        }

        private decimal _UnfrankedAmount;
        public decimal UnfrankedAmount
        {
            get
            {
                return _UnfrankedAmount;
            }
            set
            {
                _UnfrankedAmount = value;

                ClearErrors();

                if (_UnfrankedAmount < 0.00m)
                    AddError("Unfranked amount must not be less than 0");
            }
        }

        private decimal _FrankingCredits;
        public decimal FrankingCredits
        {
            get
            {
                return _FrankingCredits;
            }
            set
            {
                _FrankingCredits = value;

                ClearErrors();

                if (_FrankingCredits < 0.00m)
                    AddError("Franking credits must not be less than 0");
            }
        }

        private decimal _Interest;
        public decimal Interest
        {
            get
            {
                return _Interest;
            }
            set
            {
                _Interest = value;

                ClearErrors();

                if (_Interest < 0.00m)
                    AddError("Interest must not be less than 0");
            }
        }

        private decimal _TaxDeferred;
        public decimal TaxDeferred
        {
            get
            {
                return _TaxDeferred;
            }
            set
            {
                _TaxDeferred = value;

                ClearErrors();

                if (_TaxDeferred < 0.00m)
                    AddError("Tax Deferred must not be less than 0");
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

        public IncomeReceivedViewModel(IncomeReceived incomeReceived, RestClient restClient)
            : base(incomeReceived, TransactionStockSelection.Holdings, restClient)
        {

        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (_Transaction != null)
            {
                PaymentDate = ((IncomeReceived)_Transaction).TransactionDate;
                FrankedAmount = ((IncomeReceived)_Transaction).FrankedAmount;
                UnfrankedAmount = ((IncomeReceived)_Transaction).UnfrankedAmount;
                FrankingCredits = ((IncomeReceived)_Transaction).FrankingCredits;
                TaxDeferred = ((IncomeReceived)_Transaction).TaxDeferred;
                Interest = ((IncomeReceived)_Transaction).Interest;
                CreateCashTransaction = ((IncomeReceived)_Transaction).CreateCashTransaction;
            }
            else
            {
                PaymentDate = RecordDate;
                FrankedAmount = 0.00m;
                UnfrankedAmount = 0.00m;
                FrankingCredits = 0.00m;
                TaxDeferred = 0.00m;
                Interest = 0.00m;
                CreateCashTransaction = true;
            }
        }

        protected override void CopyFieldsToTransaction()
        {
            if (_Transaction == null)
                _Transaction = new IncomeReceived();

            base.CopyFieldsToTransaction();

            var income = (IncomeReceived)_Transaction;
            income.TransactionDate = PaymentDate;
            income.FrankedAmount = FrankedAmount;
            income.UnfrankedAmount = UnfrankedAmount;
            income.FrankingCredits = FrankingCredits;
            income.TaxDeferred = TaxDeferred;
            income.Interest = Interest;
            income.CreateCashTransaction = CreateCashTransaction;
        }

    }
}
