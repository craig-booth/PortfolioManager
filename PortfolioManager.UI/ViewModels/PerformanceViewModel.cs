using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Collections.ObjectModel;

using PortfolioManager.UI.Utilities;
//using PortfolioManager.Service;
//using PortfolioManager.Service.Utils;
using PortfolioManager.Model.Portfolios;
//using PortfolioManager.Model.Stocks;

namespace PortfolioManager.UI.ViewModels
{
    class PerformanceViewModel : PortfolioViewModel
    {

        public decimal OpeningBalance { get; private set; }
        public decimal Deposits { get; private set; }
        public decimal Withdrawls { get; private set; }
        public decimal Interest { get; private set; }
        public decimal Dividends { get; private set; }
        public decimal Fees { get; private set; }
        public decimal CapitalGains { get; private set; }
        public decimal ClosingBalance { get; private set; }

        public PerformanceViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Options.AllowStockSelection = true;
            Options.DateSelection = DateSelectionType.Range;
        }


        public override void RefreshView()
        {
            var openingHoldings = _Parameter.Portfolio.ShareHoldingService.GetHoldings(_Parameter.StartDate);
            var openingCashBalance = _Parameter.Portfolio.CashAccountService.GetBalance(_Parameter.StartDate);
            OpeningBalance = openingHoldings.Sum(x => x.MarketValue) + openingCashBalance;

            var closingHoldings = _Parameter.Portfolio.ShareHoldingService.GetHoldings(_Parameter.EndDate);
            var closingCashBalance = _Parameter.Portfolio.CashAccountService.GetBalance(_Parameter.EndDate);
            ClosingBalance = closingHoldings.Sum(x => x.MarketValue) + closingCashBalance;

            var cashTransactions = _Parameter.Portfolio.CashAccountService.GetTransactions(_Parameter.StartDate, _Parameter.EndDate);
            Deposits = cashTransactions.Where(x => x.Type == CashAccountTransactionType.Deposit).Sum(x => x.Amount);
            Withdrawls = cashTransactions.Where(x => x.Type == CashAccountTransactionType.Withdrawl).Sum(x => x.Amount);
            Interest = cashTransactions.Where(x => x.Type == CashAccountTransactionType.Interest).Sum(x => x.Amount);
            Fees = cashTransactions.Where(x => x.Type == CashAccountTransactionType.Fee).Sum(x => x.Amount);

            var income = _Parameter.Portfolio.IncomeService.GetIncome(_Parameter.StartDate, _Parameter.EndDate);
            Dividends = income.Sum(x => x.CashIncome);

            CapitalGains = ClosingBalance - (Dividends + Interest + Fees) - (Deposits + Withdrawls) - OpeningBalance;

            OnPropertyChanged(""); 
        }

    }
}
