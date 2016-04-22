using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{

    class TaxableIncomeViewModel : PortfolioViewModel, IViewModelWithData
    {
        private ReportParmeter _Parameter;

        public List<IncomeItemViewModel> Income { get; private set; }
        public decimal UnfrankedAmount { get; private set; }
        public decimal FrankedAmount { get; private set; }
        public decimal FrankingCredits { get; private set; }
        public decimal TotalAmount { get; private set; }

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

        public TaxableIncomeViewModel()
        {
            Income = new List<IncomeItemViewModel>();
        }

        public void SetData(object data)
        {
            _Parameter = data as ReportParmeter;

            Heading = string.Format("Taxable Income Repoort for {0}/{1} financial year", _Parameter.FromDate.Year, _Parameter.ToDate.Year);

            // Get  a list of all the income for the year
            var income = _Portfolio.IncomeService.GetIncome(_Parameter.FromDate, _Parameter.ToDate);

            Income.Clear();
            UnfrankedAmount = 0.00m;
            FrankedAmount = 0.00m;
            FrankingCredits = 0.00m;
            TotalAmount = 0.00m;
            foreach (var incomeItem in income)
            {
                Income.Add(new IncomeItemViewModel(incomeItem));

                UnfrankedAmount += incomeItem.UnfrankedAmount;
                FrankedAmount += incomeItem.FrankedAmount;
                FrankingCredits += incomeItem.FrankingCredits;
                TotalAmount += incomeItem.TotalIncome;
            }

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

        public IncomeItemViewModel(Service.Income income)
        {
            ASXCode = income.Stock.ASXCode;
            CompanyName = string.Format("{0} ({1})", income.Stock.ASXCode, income.Stock.Name);

            UnfrankedAmount = income.UnfrankedAmount;
            FrankedAmount = income.FrankedAmount;
            FrankingCredits = income.FrankingCredits;
            TotalAmount = income.TotalIncome;
        }
    }

}
