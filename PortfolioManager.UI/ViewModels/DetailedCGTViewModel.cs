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

        public ObservableCollection<TransactionViewItem> Transactions { get; private set; }
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

            Transactions = new ObservableCollection<ViewModels.TransactionViewItem>();
            Parcels = new ObservableCollection<ParcelCostBaseViewItem>();
        }

        public override void RefreshView()
        {
            Heading = string.Format("Detailed CGT Report for {0}/{1} Financial Year", _Parameter.FinancialYear - 1, _Parameter.FinancialYear);

            Parcels.Clear();

            IReadOnlyCollection<ShareParcel> parcels;
            
            if (_Parameter.Stock.Id == Guid.Empty)
                parcels = _Parameter.Portfolio.ParcelService.GetParcels(DateUtils.StartOfFinancialYear(_Parameter.FinancialYear), DateUtils.EndOfFinancialYear(_Parameter.FinancialYear));
            else
                parcels = _Parameter.Portfolio.ParcelService.GetParcels(_Parameter.Stock, DateUtils.StartOfFinancialYear(_Parameter.FinancialYear), DateUtils.EndOfFinancialYear(_Parameter.FinancialYear)).ToList();

            var parcelAndStock = new List<Tuple<string, ShareParcel>>();
            foreach (var parcel in parcels)
            {
                // Check if parcel has already been added
                if (parcelAndStock.FirstOrDefault(x => x.Item2.Id == parcel.Id) == null)
                {
                    var stock = _Parameter.Portfolio.StockService.Get(parcel.Stock, parcel.ToDate);
                    parcelAndStock.Add(new Tuple<string, ShareParcel>(string.Format("{0} ({1})", stock.Name, stock.ASXCode), parcel));
                }
            }
            
            foreach (var tuple in parcelAndStock.OrderBy(x => x.Item1).ThenBy(x => x.Item2.AquisitionDate))
            {
                var parcelCostBase = new ParcelCostBaseViewItem(tuple.Item1, tuple.Item2, DateUtils.EndOfFinancialYear(_Parameter.FinancialYear), _Parameter.Portfolio.ParcelService, _Parameter.Portfolio.TransactionService);

                Parcels.Add(parcelCostBase);
            }

            OnPropertyChanged("");
        }
    }

    class TransactionViewItem
    {
        
    }

    class ParcelCostBaseViewItem
    {
        public Guid ParcelId { get; private set; }
        public string CompanyName { get; private set; }
        public DateTime AquisitionDate { get; private set; }
        public int Units { get; private set; }
        public decimal CostBase { get; private set; }

        public ObservableCollection<ParcelCostBaseAuditViewItem> ParcelAudit { get; private set; }

        public ParcelCostBaseViewItem(string companyName, ShareParcel parcel, DateTime toDate, ParcelService parcelService, TransactionService transactionService)
        {           
            ParcelId = parcel.Id;
            CompanyName = companyName;

            AquisitionDate = parcel.AquisitionDate;

            ParcelAudit = new ObservableCollection<ParcelCostBaseAuditViewItem>();

            decimal costBase = 0.00m;
            var parcelAudit = parcelService.GetParcelAudit(parcel.Id, parcel.AquisitionDate, toDate);
            foreach (var auditRecord in parcelAudit)
            {
                costBase += auditRecord.CostBaseChange;

                var transaction = transactionService.GetTransaction(auditRecord.Transaction);

                var parcelAuditItem = new ParcelCostBaseAuditViewItem()
                {
                    TransactionType = transaction.Type.ToString(),
                    Date = auditRecord.Date,
                    Units = auditRecord.UnitCount,
                    Amount = auditRecord.CostBaseChange,
                    CostBase = costBase,
                    Comment = transaction.Description
                };

                ParcelAudit.Add(parcelAuditItem);

                Units = parcelAuditItem.Units;
                CostBase = parcelAuditItem.CostBase;
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
