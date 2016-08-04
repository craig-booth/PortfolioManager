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
    class PortfolioSummaryViewModel: PortfolioViewModel
    {

        public decimal MarketValue { get; private set; }
        public decimal DollarChangeInValue { get; private set; }
        public decimal PercentChangeInValue { get; private set; }

        public decimal? Return1Year { get; private set; }
        public decimal? Return3Year { get; private set; }
        public decimal? Return5Year { get; private set; }
        public decimal ReturnAll { get; private set; }

        public List<HoldingItemViewModel> Holdings { get; private set; }

        public PortfolioSummaryViewModel(string label, Portfolio portfolio)
            : base(label, portfolio)
        {
            Holdings = new List<HoldingItemViewModel>();
        }

        public override void Activate()
        {
            ShowReport();
        }

        private void ShowReport()
        {
            var holdings = Portfolio.ShareHoldingService.GetHoldings(DateTime.Today);

            MarketValue = holdings.Sum(x => x.MarketValue);
            var totalCost = holdings.Sum(x => x.TotalCostBase);
            DollarChangeInValue = MarketValue - totalCost;
            if (totalCost == 0)
                PercentChangeInValue = 0;
            else
                PercentChangeInValue = DollarChangeInValue / totalCost;

            Return1Year = Portfolio.ShareHoldingService.CalculateIRR(DateTime.Today.AddYears(-1), DateTime.Today);
            Return3Year = Portfolio.ShareHoldingService.CalculateIRR(DateTime.Today.AddYears(-3), DateTime.Today);
            Return5Year = Portfolio.ShareHoldingService.CalculateIRR(DateTime.Today.AddYears(-5), DateTime.Today);
            ReturnAll = Portfolio.ShareHoldingService.CalculateIRR(DateTimeConstants.NoDate, DateTime.Today);

            Holdings.Clear();
            foreach (var holding in holdings)
                Holdings.Add(new HoldingItemViewModel(holding));
        }    
    }

    class HoldingItemViewModel
    {
        public string ASXCode { get; set; }
        public string CompanyName { get; set; }
        public int Units { get; set; }
        public decimal CurrentValue { get; set;  }
        public decimal CostBase { get; set; }
        public ChangeInValue ChangeInValue { get; set; }

        public HoldingItemViewModel(ShareHolding holding)
        {
            ASXCode = holding.Stock.ASXCode;
            CompanyName = string.Format("{0} ({1})", holding.Stock.Name, holding.Stock.ASXCode);
            Units = holding.Units;
            CostBase = holding.TotalCostBase;
            CurrentValue = holding.MarketValue;
            ChangeInValue = new ChangeInValue(CostBase, CurrentValue);
        }

    }

    enum ChangeDirection { Increase, Decrease, Nuetral };
    struct ChangeInValue
    {
        public decimal Value { get; private set; }
        public decimal Percentage { get; private set; }
        public ChangeDirection Direction { get; private set; }

        public ChangeInValue(decimal originalValue, decimal currentValue)
        {
            Value = currentValue - originalValue;
            if (originalValue == 0)
                Percentage = 0;
            else
                Percentage = Value / originalValue;

            if (Value < 0)
                Direction = ChangeDirection.Decrease;
            else if (Value > 0)
                Direction = ChangeDirection.Increase;
            else
                Direction = ChangeDirection.Nuetral;
        }
    }

}
