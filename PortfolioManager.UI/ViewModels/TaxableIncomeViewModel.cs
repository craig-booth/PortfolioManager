using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Common;
using PortfolioManager.UI.Utilities;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.UI.ViewModels
{

    class TaxableIncomeViewModel : PortfolioViewModel
    {

        public ObservableCollection<IncomeItemViewModel> Income { get; private set; }

        private string _Heading;
        new public string Heading
        {
            get
            {
                return _Heading;
            }
            private set
            {
                _Heading = value;
                OnPropertyChanged();
            }
        }

        public TaxableIncomeViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Options.AllowStockSelection = false;
            Options.DateSelection = DateSelectionType.FinancialYear;

            Income = new ObservableCollection<IncomeItemViewModel>();
        }

        public override void RefreshView()
        {
            Heading = string.Format("Taxable Income Report for {0}/{1} Financial Year", _Parameter.FinancialYear - 1, _Parameter.FinancialYear);

            // Get  a list of all the income for the year
            var income = _Parameter.Portfolio.IncomeService.GetIncome(DateUtils.StartOfFinancialYear(_Parameter.FinancialYear), DateUtils.EndOfFinancialYear(_Parameter.FinancialYear));

            Income.Clear();
            foreach (var incomeItem in income)
                Income.Add(new IncomeItemViewModel(incomeItem));

            OnPropertyChanged("");
        }

    }

    class IncomeItemViewModel
    {
        public string ASXCode { get; private set; }
        public string CompanyName { get; private set; } 

        public decimal UnfrankedAmount { get; private set; }
        public decimal FrankedAmount { get; private set; }
        public decimal FrankingCredits { get; private set; }
        public decimal TotalAmount { get; private set; }

        public IncomeItemViewModel(Income income)
        {
            ASXCode = income.Stock.ASXCode;
            CompanyName = string.Format("{0} ({1})", income.Stock.Name, income.Stock.ASXCode);

            UnfrankedAmount = income.UnfrankedAmount;
            FrankedAmount = income.FrankedAmount;
            FrankingCredits = income.FrankingCredits;
            TotalAmount = income.TotalIncome;
        }
    }

}
