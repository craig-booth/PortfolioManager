using System.Linq;
using System.Collections.ObjectModel;

using PortfolioManager.Common;
using PortfolioManager.Service.Interface;
using PortfolioManager.UI.Utilities;


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

        public async override void RefreshView()
        {
            Heading = string.Format("Taxable Income Report for {0}/{1} Financial Year", _Parameter.FinancialYear - 1, _Parameter.FinancialYear);

            var responce = await _Parameter.RestWebClient.GetIncomeAsync(DateUtils.StartOfFinancialYear(_Parameter.FinancialYear), DateUtils.EndOfFinancialYear(_Parameter.FinancialYear));
            if (responce == null)
                return;

            Income.Clear();
            foreach (var incomeItem in responce.Income.OrderBy(x => x.Stock.Name))
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

        public IncomeItemViewModel(IncomeItem income)
        {
            ASXCode = income.Stock.ASXCode;
            CompanyName = income.Stock.FormattedCompanyName();

            UnfrankedAmount = income.UnfrankedAmount;
            FrankedAmount = income.FrankedAmount;
            FrankingCredits = income.FrankingCredits;
            TotalAmount = income.TotalAmount;
        }
    }

}
