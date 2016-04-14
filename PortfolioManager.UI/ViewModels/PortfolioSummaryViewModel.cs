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
        public string CompanyName { get; private set; }
        public decimal CurrentValue { get; private set;  }
        public decimal CapitalGain { get;  private set; }
        public decimal CapitalGainPercentage { get; private set; }

        public ViewWithData HoldingSummaryView { get; private set; }

        public HoldingItemViewModel(ShareHolding holding)
        {
            ASXCode = holding.Stock.ASXCode;
            CompanyName = holding.Stock.Name;
            CurrentValue = holding.MarketValue;
            CapitalGain = holding.MarketValue - holding.TotalCostBase;
            if (holding.TotalCostBase != 0)
                CapitalGainPercentage = CapitalGain / holding.TotalCostBase;
            else
                CapitalGainPercentage = 0.00m;


            HoldingSummaryView = new ViewWithData("HoldingSummary", holding);
        }

    }

    [ValueConversion(typeof(decimal), typeof(Brush))]
    class DecimalToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((decimal)value >= 0)
                return Brushes.Green;
            else
                return Brushes.Red;

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(decimal), typeof(bool))]
    class AmountIncreased : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((decimal)value >= 0);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
