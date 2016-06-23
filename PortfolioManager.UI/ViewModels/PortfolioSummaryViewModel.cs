using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Drawing;
using System.Globalization;

using PortfolioManager.Service;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class PortfolioSummaryViewModel: PortfolioViewModel
    {

        public decimal MarketValue { get; private set; }
        public ChangeInValue ChangeInValue { get; private set; }

        public List<HoldingItemViewModel> Holdings { get; private set; }

        public PortfolioSummaryViewModel(string label, Portfolio portfolio)
            : base(label, portfolio)
        {
            Holdings = new List<HoldingItemViewModel>();
        }

        public override void SetData(object data)
        {
            var holdings = Portfolio.ShareHoldingService.GetHoldings(DateTime.Today);

            MarketValue = holdings.Sum(x => x.MarketValue);
            ChangeInValue = new ChangeInValue(holdings.Sum(x => x.TotalCostBase), MarketValue);

            Holdings.Clear();
            foreach (var holding in holdings)
                Holdings.Add(new HoldingItemViewModel(holding));
        }
    }

    class HoldingItemViewModel
    {
        public string ASXCode { get; private set; }
        public string CompanyName { get; private set; }
        public decimal CurrentValue { get; private set;  }
        public ChangeInValue ChangeInValue { get; private set; }

     //   public ViewWithData HoldingSummaryView { get; private set; }

        public HoldingItemViewModel(ShareHolding holding)
        {
            ASXCode = holding.Stock.ASXCode;
            CompanyName = string.Format("{0} ({1})", holding.Stock.Name, holding.Stock.ASXCode);
            CurrentValue = holding.MarketValue;
            ChangeInValue = new ChangeInValue(holding.TotalCostBase, holding.MarketValue);

       //     HoldingSummaryView = new ViewWithData("HoldingSummary", holding);
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
