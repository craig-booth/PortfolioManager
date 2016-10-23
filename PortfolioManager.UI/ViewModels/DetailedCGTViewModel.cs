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
            Options.AllowStockSelection = false;
            Options.DateSelection = DateSelectionType.FinancialYear;

            Transactions = new ObservableCollection<ViewModels.TransactionViewItem>();
            Parcels = new ObservableCollection<ParcelCostBaseViewItem>();
        }

        public override void RefreshView()
        {
            Heading = string.Format("Detailed CGT Report for {0}/{1} Financial Year", _Parameter.FinancialYear - 1, _Parameter.FinancialYear);

            var parcels = _Parameter.Portfolio.ParcelService.GetParcels(DateUtils.StartOfFinancialYear(_Parameter.FinancialYear), DateUtils.EndOfFinancialYear(_Parameter.FinancialYear));
            foreach (var parcel in parcels)
            {
                var stock = _Parameter.Portfolio.StockService.Get(parcel.Stock, parcel.ToDate);
                var parcelCostBase = new ParcelCostBaseViewItem(stock, parcel.Id, _Parameter.Portfolio.ParcelService);

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

        public ObservableCollection<ParcelCostBaseTransactionsViewItem> ParcelTransactions { get; private set; }

        public ParcelCostBaseViewItem(Stock stock, Guid id, ParcelService parcelService)
        {           
            ParcelId = id;
            CompanyName = string.Format("{0} ({1})", stock.Name, stock.ASXCode);

            ParcelTransactions = new ObservableCollection<ViewModels.ParcelCostBaseTransactionsViewItem>();

            var parcels = parcelService.GetParcels(id);
            foreach (var parcel in parcels)
            {
                AquisitionDate = parcel.AquisitionDate;
                Units = parcel.Units;
                CostBase = parcel.CostBase;

                var parcelTransaction = new ParcelCostBaseTransactionsViewItem(parcel);


                ParcelTransactions.Add(parcelTransaction);
            }

        }
    }

    class ParcelCostBaseTransactionsViewItem
    {
        public string TransactionType { get; private set; }
        public DateTime Date { get; private set; }
        public int Units { get; private set; }
        public decimal Amount { get; private set; }
        public decimal TransactionCosts { get; private set; }
        public decimal CostBase { get; private set; }
        public string Comment { get; private set; }

        public ParcelCostBaseTransactionsViewItem(ShareParcel parcel)
        {
            TransactionType = "";
            Date = parcel.FromDate;
            Units = parcel.Units;
            Amount = 0.00m;
            TransactionCosts = 0.00m;
            CostBase = parcel.CostBase;
            Comment = "";
        }
    }
}
