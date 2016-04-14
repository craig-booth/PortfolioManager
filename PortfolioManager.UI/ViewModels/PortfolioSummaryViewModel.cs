using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class PortfolioSummaryViewModel: PortfolioViewModel
    {

        public decimal MarketValue { get; private set; }

        public List<HoldingItemViewModel> Holdings { get; private set; }

        public PortfolioSummaryViewModel()
        {
            Heading = "Portfolio";
            Holdings = new List<HoldingItemViewModel>();
        }

        public override void SetPortfolio(Portfolio portfolio)
        {
            base.SetPortfolio(portfolio);

            MarketValue = 125.00m; //portfolio.MarketValue;

            Holdings.Clear();
            foreach (var holding in portfolio.ShareHoldingService.GetHoldings(DateTime.Today))
                Holdings.Add(new HoldingItemViewModel(holding));
        }
    }

    class HoldingItemViewModel
    {
        public string ASXCode { get; private set; }
        public decimal CurrentValue { get; private set;  }
        public decimal CapitalGain { get;  private set; }
        public decimal CapitalGainPercentage { get; private set; }

        public ViewWithData HoldingSummaryView { get; private set; }

        public HoldingItemViewModel(ShareHolding holding)
        {
            ASXCode = holding.Stock.ASXCode;
            CurrentValue = holding.MarketValue;
            CapitalGain = holding.MarketValue - holding.TotalCostBase;
            if (holding.TotalCostBase != 0)
                CapitalGainPercentage = CapitalGain / holding.TotalCostBase;
            else
                CapitalGainPercentage = 0.00m;


            HoldingSummaryView = new ViewWithData("HoldingSummary", holding);
        }

    }
}
