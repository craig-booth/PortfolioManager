using System;
using System.Linq;
using System.Collections.ObjectModel;

using PortfolioManager.RestApi.Portfolios;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class DetailedCGTViewModel : PortfolioViewModel
    {

        public ObservableCollection<ParcelCostBaseViewItem> Parcels { get; private set; }

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

        public DetailedCGTViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Options.AllowStockSelection = true;
            Options.DateSelection = DateSelectionType.FinancialYear;

            Parcels = new ObservableCollection<ParcelCostBaseViewItem>();
        }

        public async override void RefreshView()
        {
            Heading = string.Format("Detailed CGT Report for {0}/{1} Financial Year", _Parameter.FinancialYear - 1, _Parameter.FinancialYear);

            DetailedUnrealisedGainsResponse responce;
            if (_Parameter.Stock.Id == Guid.Empty)
                responce = await _Parameter.RestClient.Portfolio.GetDetailedCapitalGains(_Parameter.Date);
            else
                responce = await _Parameter.RestClient.Holdings.GetDetailedCapitalGains(_Parameter.Stock.Id, _Parameter.Date);
            if (responce == null)
                return;

            Parcels.Clear();
            foreach (var item in responce.UnrealisedGains.OrderBy(x => x.Stock.Name).ThenBy(x => x.AquisitionDate))
                 Parcels.Add(new ParcelCostBaseViewItem(item));
 
            OnPropertyChanged("");
        }
    }

    class ParcelCostBaseViewItem
    {
        public StockViewItem Stock { get; private set; }
        public DateTime AquisitionDate { get; private set; }
        public int Units { get; private set; }
        public decimal CostBase { get; private set; }

        public ObservableCollection<ParcelCostBaseAuditViewItem> ParcelAudit { get; private set; }

        public ParcelCostBaseViewItem(DetailedUnrealisedGainsItem item)
        {
            Stock = new StockViewItem(item.Stock);
            AquisitionDate = item.AquisitionDate;
            Units = item.Units;
            CostBase = item.CostBase;

            ParcelAudit = new ObservableCollection<ParcelCostBaseAuditViewItem>();

            foreach (var cgtEvent in item.CGTEvents)
            {
                var parcelAuditItem = new ParcelCostBaseAuditViewItem()
                {
                    Date = cgtEvent.Date,
                    Description = cgtEvent.Description,
                    Units = cgtEvent.Units,
                    Amount = cgtEvent.CostBaseChange,
                    CostBase = cgtEvent.CostBase
                };

                ParcelAudit.Add(parcelAuditItem);
            }
        }
    }

    class ParcelCostBaseAuditViewItem
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int Units { get; set; }
        public decimal Amount { get; set; }
        public decimal CostBase { get; set; }
        
    }
}
