using System.Linq;
using System.Collections.ObjectModel;

using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class PortfolioSummaryViewModel : PortfolioViewModel
    {
        public ChangeInValue PortfolioValue { get; private set; }
        public PortfolioReturn Return1Year { get; private set; }
        public PortfolioReturn Return3Year { get; private set; }
        public PortfolioReturn Return5Year { get; private set; }
        public PortfolioReturn ReturnAll { get; private set; }

        public ObservableCollection<HoldingItemViewModel> Holdings { get; private set; }

        public PortfolioSummaryViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Holdings = new ObservableCollection<HoldingItemViewModel>();

            PortfolioValue = new ChangeInValue();
            Return1Year = new PortfolioReturn("1 Year");
            Return3Year = new PortfolioReturn("3 Years");
            Return5Year = new PortfolioReturn("5 Years");
            ReturnAll = new PortfolioReturn("All"); 
        }

        public async override void RefreshView()
        {
            var responce = await _Parameter.RestClient.Portfolio.GetSummary(_Parameter.Date);
            if (responce == null)
                return;

            PortfolioValue.InitialValue = responce.PortfolioCost;
            PortfolioValue.Value = responce.PortfolioValue;

            if (responce.Return1Year != null)
            {
                Return1Year.Value = (decimal)responce.Return1Year;
                Return1Year.NotApplicable = false;
            } 
            else
            {
                Return1Year.Value = 0.00m;
                Return1Year.NotApplicable = true;
            }

            if (responce.Return3Year != null)
            {
                Return3Year.Value = (decimal)responce.Return3Year;
                Return3Year.NotApplicable = false;
            }
            else
            {
                Return3Year.Value = 0.00m;
                Return3Year.NotApplicable = true;
            }

            if (responce.Return5Year != null)
            {
                Return5Year.Value = (decimal)responce.Return5Year;
                Return5Year.NotApplicable = false;
            }
            else
            {
                Return5Year.Value = 0.00m;
                Return5Year.NotApplicable = true;
            }

            if (responce.ReturnAll != null)
            {
                ReturnAll.Value = (decimal)responce.ReturnAll;
                ReturnAll.NotApplicable = false;
            }
            else
            {
                ReturnAll.Value = 0.00m;
                ReturnAll.NotApplicable = true;
            }

            Holdings.Clear();
            foreach (var holding in responce.Holdings.OrderBy(x => x.Stock.Name))
                Holdings.Add(new HoldingItemViewModel(holding));


            Holdings.Add(new HoldingItemViewModel("Cash Account", 0, responce.CashBalance, responce.CashBalance));

            OnPropertyChanged("");
        }

    }

    class HoldingItemViewModel
    {
        public string CompanyName { get; private set; }
        public int Units { get; private set; }
        public ChangeInValue ChangeInValue { get; private set; }

        public HoldingItemViewModel(string companyName, int units, decimal cost, decimal marketValue)
        {           
            CompanyName = companyName;
            Units = units;
            ChangeInValue = new ChangeInValue(cost, marketValue);
        }

        public HoldingItemViewModel(Holding holding)
        {
            CompanyName = holding.Stock.FormattedCompanyName();
            Units = holding.Units;
            ChangeInValue = new ChangeInValue(holding.Cost, holding.Value);
        }

    }

    enum DirectionChange { Increase, Decrease, Neutral };

    class ChangeInValue : NotifyClass
    {
        private decimal _InitialValue;
        public decimal InitialValue
        {
            get
            {
                return _InitialValue;
            }
            set
            {
                if (value != _InitialValue)
                {
                    _InitialValue = value;
                    OnPropertyChanged("");
                }
            }
        }

        private decimal _Value;
        public decimal Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (value != _Value)
                {
                    _Value = value;
                    OnPropertyChanged("");
                }
            }
        }

        public decimal Change
        {
            get
            {
                return (Value - InitialValue);
            }
        }

        public decimal PercentageChange
        {
            get
            {
                if (InitialValue == 0)
                    return 0;
                else
                    return Change / InitialValue;
            }
        }

        public DirectionChange Direction
        {
            get
            {
                if (Change < 0)
                    return DirectionChange.Decrease;
                else if (Change > 0)
                    return DirectionChange.Increase;
                else
                    return DirectionChange.Neutral;
            }
        }

        public ChangeInValue(decimal intialValue, decimal value)
        {
            InitialValue = intialValue;
            Value = value;
        }

        public ChangeInValue(decimal value)
            : this(0, value)
        {
        }

        public ChangeInValue()
            : this(0,0)
        {
        }
    }

    class PortfolioReturn : ChangeInValue
    {
        public string Period { get; private set; }
        public bool NotApplicable { get; set; }

        public string ValueText
        {
            get
            {
                if (NotApplicable)
                    return "--%";
                else
                    return Value.ToString("p");
            }
        }       

        public PortfolioReturn(string period, decimal value)
            : base(value)
        {
            Period = period;
        }

        public PortfolioReturn(string period)
            : this(period, 0)
        {
        }
    } 

}
