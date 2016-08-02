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
        private IStockParameter _StockParameter;
        private ISingleDateParameter _DateParameter;

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

        public UnrealisedGainsViewModel(string label, Portfolio portfolio, IStockParameter stockParameter, ISingleDateParameter dateParameter)
            : base(label, portfolio)
        {
            Options.AllowStockSelection = true;
            Options.DateSelection = DateSelectionType.Single;

            _Heading = label;
            _StockParameter = stockParameter;
            _DateParameter = dateParameter;

            UnrealisedGains = new ObservableCollection<UnrealisedGainViewItem>();
        }

        public override void Activate()
        {
            if (_DateParameter != null)
                _DateParameter.PropertyChanged += ParameterChange;

            if (_StockParameter != null)
                _StockParameter.PropertyChanged += ParameterChange;

            ShowReport();
        }

        public override void Deactivate()
        {
            if (_DateParameter != null)
                _DateParameter.PropertyChanged -= ParameterChange;

            if (_StockParameter != null)
                _StockParameter.PropertyChanged -= ParameterChange;
        }


        private void ShowReport()
        {
            if (_DateParameter == null)
            {
                UnrealisedGains.Clear();
                OnPropertyChanged("");

                return;
            }

            IEnumerable<ShareParcel> parcels;
            if (_StockParameter.Stock.Id == Guid.Empty)
                parcels = Portfolio.ParcelService.GetParcels(_DateParameter.Date).OrderBy(x => x.Stock);
            else
                parcels = Portfolio.ParcelService.GetParcels(_StockParameter.Stock, _DateParameter.Date);

            Stock currentStock = null;
            Guid previousStock = Guid.Empty;
            decimal unitPrice = 0.00m;
            var unrealisedGainsList = new List<UnrealisedGainViewItem>();
            foreach (var parcel in parcels)
            {
                if (parcel.Stock != previousStock)
                {
                    currentStock = Portfolio.StockService.Get(parcel.Stock, _DateParameter.Date);
                    unitPrice = Portfolio.StockPriceService.GetPrice(currentStock, _DateParameter.Date);

                    previousStock = parcel.Stock;
                }

                unrealisedGainsList.Add(new UnrealisedGainViewItem(currentStock, parcel, _DateParameter.Date, unitPrice));
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
