using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using PortfolioManager.UI.Utilities;
using PortfolioManager.Service;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;

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

        public UnrealisedGainsViewModel(string label, Portfolio portfolio)
            : base(label, portfolio)
        {
            _Heading = label;
            UnrealisedGains = new ObservableCollection<UnrealisedGainViewItem>();
        }

        public void ShowReport()
        {

        }

        public override void SetData(object data)
        {
            var atDate = DateTime.Now;

            var parcels = Portfolio.ParcelService.GetParcels(atDate);

            UnrealisedGains.Clear();
            foreach (var parcel in parcels)
                UnrealisedGains.Add(new UnrealisedGainViewItem(Portfolio.StockService.Get(parcel.Stock, atDate), parcel));

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


        public UnrealisedGainViewItem(Stock stock, ShareParcel parcel)
        {
            ASXCode = stock.ASXCode;
            CompanyName = string.Format("{0} ({1})", stock.Name, stock.ASXCode);

            AquisitionDate = parcel.AquisitionDate;
            Units = parcel.Units;
            CostBase = parcel.CostBase;
        }
    }
}
