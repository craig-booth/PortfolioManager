using System;
using System.Linq;
using System.Collections.ObjectModel;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class UnrealisedGainsViewModel : PortfolioViewModel
    {
 
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

        public ObservableCollection<UnrealisedGainViewItem> UnrealisedGains { get; private set; }

        public UnrealisedGainsViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Options.AllowStockSelection = true;
            Options.DateSelection = DateSelectionType.Single;

            _Heading = label;

            UnrealisedGains = new ObservableCollection<UnrealisedGainViewItem>();
        }


        public async override void RefreshView()
        {
            SimpleUnrealisedGainsResponse response;
            if (_Parameter.Stock.Id == Guid.Empty)
                response = await _Parameter.RestClient.Portfolio.GetCapitalGains(_Parameter.Date);
            else
                response = await _Parameter.RestClient.Holdings.GetCapitalGains(_Parameter.Stock.Id, _Parameter.Date);
            if (response == null)
                return;

            UnrealisedGains.Clear();

            var cgtItems = response.UnrealisedGains.Select(x => new UnrealisedGainViewItem(x));
            foreach (var cgtItem in cgtItems.OrderBy(x => x.Stock.FormattedCompanyName))
                UnrealisedGains.Add(cgtItem);
            
            OnPropertyChanged(""); 
        }
    }


    class UnrealisedGainViewItem
    {
        public StockViewItem Stock { get; private set; }
        public DateTime AquisitionDate { get; private set; }
        public int Units { get; private set; }
        public decimal CostBase { get; private set; }
        public decimal MarketValue { get; private set; }
        public decimal CapitalGain { get; private set; }
        public decimal DiscoutedGain { get; private set; }
        public string DiscountMethod { get; private set; }

        public UnrealisedGainViewItem(SimpleUnrealisedGainsItem item)
        {
            Stock = new StockViewItem(item.Stock);
            AquisitionDate = item.AquisitionDate;
            Units = item.Units;
            CostBase = item.CostBase;
            MarketValue = item.MarketValue;
            CapitalGain = item.CapitalGain;

            if (item.DiscountMethod == CGTMethod.Discount)
            {
                DiscountMethod = "Discount";
                DiscoutedGain = item.DiscoutedGain;
            }
            else if (item.DiscountMethod == CGTMethod.Indexation)
            {
                DiscountMethod = "Indexation";
                DiscoutedGain = 0.00m;
            }
            else
            {
                DiscountMethod = "Other";
                DiscoutedGain = 0.00m;
            }
        }

    }
}
