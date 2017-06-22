using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Common;
using PortfolioManager.UI.Utilities;
using PortfolioManager.Service.Interface;

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
            SimpleUnrealisedGainsResponce responce;
            if (_Parameter.Stock.Id != Guid.Empty)
                responce = await _Parameter.RestWebClient.GetSimpleUnrealisedGainsAsync(_Parameter.Date);
            else
                responce = await _Parameter.RestWebClient.GetSimpleUnrealisedGainsAsync(_Parameter.Stock.Id, _Parameter.Date);
            if (responce == null)
                return;

            UnrealisedGains.Clear();
            foreach (var cgtItem in responce.CGTItems.OrderBy(x => x.Stock.FormattedCompanyName()))
            {
                var viewItem = new UnrealisedGainViewItem(cgtItem);
                UnrealisedGains.Add(viewItem);
            }
            
            OnPropertyChanged(""); 
        }
    }


    class UnrealisedGainViewItem
    {
        public string CompanyName { get; private set; }

        public DateTime AquisitionDate { get; private set; }
        public int Units { get; private set; }
        public decimal CostBase { get; private set; }
        public decimal MarketValue { get; private set; }
        public decimal CapitalGain { get; private set; }
        public decimal DiscoutedGain { get; private set; }
        public string DiscountMethod { get; private set; }

        public UnrealisedGainViewItem(SimpleUnrealisedGainsItem item)
        {
            CompanyName = item.Stock.FormattedCompanyName();
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
