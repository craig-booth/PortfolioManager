using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Service;
using PortfolioManager.Service.Utils;
using PortfolioManager.Model.Utils;

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

            var cgtService = _Parameter.PortfolioService.GetService<CGTService>();

            DetailedCGTResponce responce;
            if (_Parameter.Stock.Id == Guid.Empty)
                responce = await cgtService.GetDetailedCGT(_Parameter.Date);
            else
                responce = await cgtService.GetDetailedCGT(_Parameter.Stock.Id, _Parameter.Date);

            Parcels.Clear();
            foreach (var item in responce.CGTItems.OrderBy(x => x.CompanyName).ThenBy(x => x.AquisitionDate))
                 Parcels.Add(new ParcelCostBaseViewItem(item));
 
            OnPropertyChanged("");
        }
    }

    class ParcelCostBaseViewItem
    {
        public Guid ParcelId { get; private set; }
        public string CompanyName { get; private set; }
        public DateTime AquisitionDate { get; private set; }
        public int Units { get; private set; }
        public decimal CostBase { get; private set; }

        public ObservableCollection<ParcelCostBaseAuditViewItem> ParcelAudit { get; private set; }

        public ParcelCostBaseViewItem(DetailedCGTResponceItem item)
        {           
            ParcelId = item.Id;
            CompanyName = PortfolioViewModel.FormattedCompanyName(item.ASXCode, item.CompanyName);
            AquisitionDate = item.AquisitionDate;
            Units = item.Units;
            CostBase = item.CostBase;

            ParcelAudit = new ObservableCollection<ParcelCostBaseAuditViewItem>();

            foreach (var historyItem in item.History)
            {
                var parcelAuditItem = new ParcelCostBaseAuditViewItem()
                {
                    TransactionType = historyItem.TransactionType.ToString(),
                    Date = historyItem.Date,
                    Units = historyItem.Units,
                    Amount = historyItem.Amount,
                    CostBase = historyItem.CostBase,
                    Comment = historyItem.Comment
                };

                ParcelAudit.Add(parcelAuditItem);
            }
        }
    }

    class ParcelCostBaseAuditViewItem
    {
        public string TransactionType { get; set; }
        public DateTime Date { get; set; }
        public int Units { get; set; }
        public decimal Amount { get; set; }
        public decimal CostBase { get; set; }
        public string Comment { get; set; }
    }
}
