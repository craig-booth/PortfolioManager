using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Drawing;
using System.Globalization;

using PortfolioManager.Service;
using PortfolioManager.Model.Utils;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class PortfolioSummaryViewModel : PortfolioViewModel
    {
        private DateTime _PortfolioStartDate;

        public ChangeInValue PortfolioValue { get; private set; }
        public PortfolioReturn Return1Year { get; private set; }
        public PortfolioReturn Return3Year { get; private set; }
        public PortfolioReturn Return5Year { get; private set; }
        public PortfolioReturn ReturnAll { get; private set; }

        public List<HoldingItemViewModel> Holdings { get; private set; }

        public PortfolioSummaryViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Holdings = new List<HoldingItemViewModel>();

            PortfolioValue = new ChangeInValue();
            Return1Year = new PortfolioReturn("1 Year");
            Return3Year = new PortfolioReturn("3 Years");
            Return5Year = new PortfolioReturn("5 Years");
            ReturnAll = new PortfolioReturn("All");
        }

        public override void RefreshView()
        {          
            var holdings = _Parameter.Portfolio.ShareHoldingService.GetHoldings(DateTime.Today);
            PortfolioValue.InitialValue = holdings.Sum(x => x.TotalCostBase); 
            PortfolioValue.Value = holdings.Sum(x => x.MarketValue);

            _PortfolioStartDate = _Parameter.Portfolio.ShareHoldingService.GetPortfolioStartDate();

            CalculateIRR(Return1Year, 1);
            CalculateIRR(Return3Year, 3);
            CalculateIRR(Return5Year, 5);
            CalculateIRR(ReturnAll, 0);   

            Holdings.Clear();
            foreach (var holding in holdings)
                Holdings.Add(new HoldingItemViewModel(holding));

            Holdings.Sort(HoldingItemViewModel.CompareByCompanyName);
        }

        private void CalculateIRR(PortfolioReturn portfolioReturn, int years)
        {
            DateTime startDate;
            if (years == 0)
                startDate = _PortfolioStartDate;
            else
                startDate = DateTime.Today.AddYears(-years);

            if (startDate >= _PortfolioStartDate)
            {
                try
                {
                    portfolioReturn.Value = _Parameter.Portfolio.ShareHoldingService.CalculateIRR(startDate, DateTime.Today);
                    portfolioReturn.NotApplicable = false;
                }
                catch
                {
                    portfolioReturn.Value = 0;
                    portfolioReturn.NotApplicable = true;
                }
            }
            else
            {
                portfolioReturn.Value = 0;
                portfolioReturn.NotApplicable = true;
            }
        }
    }

    class HoldingItemViewModel
    {
        public string ASXCode { get; set; }
        public string CompanyName { get; set; }
        public int Units { get; set; }
        public ChangeInValue ChangeInValue { get; set; }

        public HoldingItemViewModel(ShareHolding holding)
        {
            ASXCode = holding.Stock.ASXCode;
            CompanyName = string.Format("{0} ({1})", holding.Stock.Name, holding.Stock.ASXCode);
            Units = holding.Units;
            ChangeInValue = new ChangeInValue(holding.TotalCostBase, holding.MarketValue);
        }

        public static int CompareByCompanyName(HoldingItemViewModel holding1, HoldingItemViewModel holding2)
        {
            return String.Compare(holding1.CompanyName, holding2.CompanyName);
        }
    }

    enum DirectionChange { Increase, Decrease, Neutral };

    class ChangeInValue
    {
        public decimal InitialValue { get; set; }
        public decimal Value { get; set; }

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
