using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.UI.Utilities;
using PortfolioManager.Service;
using PortfolioManager.Service.Utils;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.UI.ViewModels
{
    class UnrealisedGainsViewModel : PortfolioViewModel
    {
        private ISingleDateParameter _Parameter;

        public void ParameterChange(object sender, PropertyChangedEventArgs e)
        {
            ShowReport();
        }

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

        public UnrealisedGainsViewModel(string label, Portfolio portfolio, ISingleDateParameter parameter)
            : base(label, portfolio)
        {
            Options.AllowStockSelection = true;
            Options.DateSelection = DateSelectionType.Single;

            _Heading = label;
            _Parameter = parameter;

            UnrealisedGains = new ObservableCollection<UnrealisedGainViewItem>();
        }

        public override void Activate()
        {
            if (_Parameter != null)
                _Parameter.PropertyChanged += ParameterChange;

            ShowReport();
        }

        public override void Deactivate()
        {
            if (_Parameter != null)
                _Parameter.PropertyChanged -= ParameterChange;
        }

        private void ShowReport()
        {
            if (_Parameter == null)
            {
                UnrealisedGains.Clear();
                OnPropertyChanged("");

                return;
            }

            var parcels = Portfolio.ParcelService.GetParcels(_Parameter.Date).OrderBy(x => x.Stock);

            Stock currentStock = null;
            Guid previousStock = Guid.Empty;
            decimal unitPrice = 0.00m;
            var unrealisedGainsList = new List<UnrealisedGainViewItem>();
            foreach (var parcel in parcels)
            {
                if (parcel.Stock != previousStock)
                {
                    currentStock = Portfolio.StockService.Get(parcel.Stock, _Parameter.Date);
                    unitPrice = Portfolio.StockPriceService.GetClosingPrice(currentStock, _Parameter.Date);

                    previousStock = parcel.Stock;
                }

                unrealisedGainsList.Add(new UnrealisedGainViewItem(currentStock, parcel, _Parameter.Date, unitPrice));
            }

            UnrealisedGains = new ObservableCollection<UnrealisedGainViewItem>(unrealisedGainsList.OrderBy(x => x.CompanyName).ThenBy(x => x.AquisitionDate));

            OnPropertyChanged("");
        }
    }


    class UnrealisedGainViewItem
    {
        public string ASXCode { get; private set; }
        public string CompanyName { get; private set; }

        public DateTime AquisitionDate { get; private set; }
        public int Units { get; private set; }
        public decimal CostBase { get; private set; }
        public decimal MarketValue { get; private set; }
        public decimal CapitalGain { get; private set; }
        public decimal DiscoutedGain { get; private set; }
        public string DiscountMethod { get; private set; }

        public UnrealisedGainViewItem(Stock stock, ShareParcel parcel, DateTime atDate, decimal unitPrice)
        {
            ASXCode = stock.ASXCode;
            CompanyName = string.Format("{0} ({1})", stock.Name, stock.ASXCode);

            AquisitionDate = parcel.AquisitionDate;
            Units = parcel.Units;
            CostBase = parcel.CostBase;

            MarketValue = Units * unitPrice;
            CapitalGain = MarketValue - CostBase;

            var cgtMethod = CGTCalculator.CGTMethodForParcel(parcel, atDate);
            if (cgtMethod == CGTMethod.Discount)
            {
                DiscountMethod = "Discount";
                DiscoutedGain = CGTCalculator.CGTDiscount(CapitalGain);
            }
            else if (cgtMethod == CGTMethod.Indexation)
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
