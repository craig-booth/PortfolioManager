using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Client;
using PortfolioManager.RestApi.CorporateActions;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class AddDividendViewModel : ViewModel, IPageViewModel, IEditableObject
    {
        private bool _BeingEdited;
        private RestClient _RestClient;

        private StockViewItem _Stock;
        public StockViewItem Stock
        {
            get
            {
                return _Stock;
            }
            set
            {
                if (_Stock != value)
                {
                    _Stock = value;

                    OnPropertyChanged();
                }

                ClearErrors();

                if (_Stock == null)
                    AddError("Company is required");
            }
        }

        public DateTime PaymentDate { get; set; }

        private DateTime _RecordDate;
        public DateTime RecordDate
        {
            get
            {
                return _RecordDate;
            }

            set
            {
                if (_RecordDate != value)
                {
                    _RecordDate = value;

                    if (_BeingEdited)
                        PopulateAvailableStocks(_RecordDate);
                }
            }
        }

        public string Description { get; set; }

        public decimal Amount { get; set; }
        public decimal PercentFranked { get; set; }
        public decimal DRPPrice { get; set; }

        public ObservableCollection<StockViewItem> AvailableStocks { get; private set; }

        public AddDividendViewModel(RestClient restClient)
        {
            _RestClient = restClient;

            AvailableStocks = new ObservableCollection<StockViewItem>();

            SaveDividendCommand = new RelayCommand(SaveTransaction, CanSaveTransaction);
        }

        public RelayCommand SaveDividendCommand { get; private set; }

        public string Label => "Add Dividend";

        public string Heading => "Add Dividend";

        public PageViewOptions Options => throw new NotImplementedException();

        private async void SaveTransaction()
        {         
            EndEdit();

            var dividend = new Dividend()
            {
                Id = Guid.NewGuid(),
                Stock = Stock.Id,
                ActionDate = RecordDate,
                Description = Description,
                PaymentDate = PaymentDate,
                DividendAmount = Amount,
                PercentFranked = PercentFranked,
                DRPPrice = DRPPrice
            };

            await _RestClient.CorporateActions.Add(Stock.Id, dividend);
        }

        private bool CanSaveTransaction()
        {
            return !HasErrors;
        }

        public void Activate()
        {
            BeginEdit();
        }

        public void Deactivate()
        {
            EndEdit();
        }

        public void BeginEdit()
        {
            _BeingEdited = true;

            PopulateAvailableStocks(RecordDate);
        }

        public void EndEdit()
        {
            _BeingEdited = false;
        }

        public void CancelEdit()
        {
            _BeingEdited = false;
        }

        private async void PopulateAvailableStocks(DateTime date)
        {
            AvailableStocks.Clear();

            var stocks = await _RestClient.Stocks.Get(date);

            foreach (var stock in stocks)
            {
                var stockItem = new StockViewItem(stock.Id, stock.ASXCode, stock.Name);
                AvailableStocks.Add(stockItem);
                if (!stock.StapledSecurity)
                {
                    foreach (var childSecurity in stock.ChildSecurities)
                    {
                        stockItem = new StockViewItem(stock.Id, childSecurity.ASXCode, childSecurity.Name);
                        AvailableStocks.Add(stockItem);
                    }
                }
            }
        }

    }
}
