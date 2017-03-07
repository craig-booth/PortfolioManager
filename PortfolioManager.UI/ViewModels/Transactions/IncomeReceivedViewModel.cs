using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Service.Interface;

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

        public IncomeReceivedViewModel(IncomeTransactionItem incomeReceived, IStockService stockService, IHoldingService holdingService)
            : base(incomeReceived, TransactionStockSelection.NonStapledStocks(true), stockService, holdingService)
        {

        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
            {
                PaymentDate = ((IncomeTransactionItem)Transaction).TransactionDate;
                FrankedAmount = ((IncomeTransactionItem)Transaction).FrankedAmount;
                UnfrankedAmount = ((IncomeTransactionItem)Transaction).UnfrankedAmount;
                FrankingCredits = ((IncomeTransactionItem)Transaction).FrankingCredits;
                TaxDeferred = ((IncomeTransactionItem)Transaction).TaxDeferred;
                Interest = ((IncomeTransactionItem)Transaction).Interest;
                CreateCashTransaction = ((IncomeTransactionItem)Transaction).CreateCashTransaction;
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
            if (Transaction == null)
                Transaction = new IncomeTransactionItem();

            base.CopyFieldsToTransaction();

            var income = (IncomeTransactionItem)Transaction;
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
