using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using PortfolioManager.UI.Utilities;
using PortfolioManager.Service;

namespace PortfolioManager.UI.ViewModels
{

    class TaxableIncomeViewModel : PortfolioViewModel
    {
        private ReportParmeter _Parameter;
        public ReportParmeter Parameter
        {
            get
            {
                return _Parameter;
            }

            set
            {
                if (value != _Parameter)
                {
                    _Parameter = value;
                    OnPropertyChanged();

                    ShowReport();
                }
            }
        }

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

        public TaxableIncomeViewModel(string label, Portfolio portfolio)
            : base(label, portfolio)
        {
            Income = new ObservableCollection<IncomeItemViewModel>();
        }

        public void ShowReport()
        {
            Heading = string.Format("Taxable Income Report for {0}/{1} financial year", _Parameter.FromDate.Year, _Parameter.ToDate.Year);

            // Get  a list of all the income for the year
            var income = Portfolio.IncomeService.GetIncome(_Parameter.FromDate, _Parameter.ToDate);

            Income.Clear();
            foreach (var incomeItem in income)
                Income.Add(new IncomeItemViewModel(incomeItem));

            OnPropertyChanged("");
        }

        public override void SetData(object data)
        {
          //  Parameter = data as ReportParmeter;
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
            CompanyName = string.Format("{0} ({1})", income.Stock.Name, income.Stock.ASXCode);

            UnfrankedAmount = income.UnfrankedAmount;
            FrankedAmount = income.FrankedAmount;
            FrankingCredits = income.FrankingCredits;
            TotalAmount = income.TotalIncome;
        }
    }

}
