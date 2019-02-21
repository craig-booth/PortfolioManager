using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Client;
using PortfolioManager.RestApi.Transactions;
using PortfolioManager.UI.ViewModels;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    enum TransactionStockSelection { None, Holdings, TradeableHoldings, Stocks, TradeableStocks }

    abstract class TransactionViewModel : ViewModel, IEditableObject
    {
        protected bool _BeingEdited;
        protected TransactionStockSelection _StockSelection;
        protected RestClient _RestClient;
        protected Transaction _Transaction { get; set; }

        public string Description { get; private set; }    

        private StockViewItem _Stock;
        public StockViewItem Stock
        {
            get
            {
                return _Stock;
            }
            set
            {
                _Stock = value;

                if (_StockSelection != TransactionStockSelection.None)
                {
                    ClearErrors();

                    if (_Stock == null)
                        AddError("Company is required");
                }
            }
        }

        public DateTime TransactionDate { get; set; }

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

        public string Comment { get; set; }

        public ObservableCollection<StockViewItem> AvailableStocks { get; private set; }

        public bool IsStockEditable
        {
            get
            {
                return (_Transaction == null);
            }
        }

        public TransactionViewModel(Transaction transaction, TransactionStockSelection stockSeletion, RestClient restClient)
        {
            _StockSelection = stockSeletion;
            _RestClient = restClient;

            _Transaction = transaction;

            if (_StockSelection != TransactionStockSelection.None)
                AvailableStocks = new ObservableCollection<StockViewItem>();                   

            CopyTransactionToFields();
        }

        public void BeginEdit()
        {
            _BeingEdited = true;

            if (_StockSelection != TransactionStockSelection.None)
            {
                PopulateAvailableStocks(RecordDate);
            }
        }

        public void EndEdit()
        {
            _BeingEdited = false;

            if (_Transaction != null)
                CopyFieldsToTransaction();
        }

        public void CancelEdit()
        {
            _BeingEdited = false;
        }

        public async void Save()
        {
            EndEdit();

            if (_Transaction != null)
            {

            }
            else
            {
                CopyFieldsToTransaction();
                _Transaction.Id = Guid.NewGuid();

                await _RestClient.Transactions.Add(_Transaction);
            }                
        }

        protected virtual void CopyTransactionToFields()
        {
            if (_Transaction != null)
            {
                Stock = AvailableStocks.FirstOrDefault(x => x.Id == _Transaction.Id);
                Description = _Transaction.Description;
                TransactionDate = _Transaction.TransactionDate;
                Comment = _Transaction.Comment;
            }
            else
            {
                Stock = new StockViewItem(Guid.Empty, "", "");
                Description = "";
                TransactionDate = DateTime.Today;
                RecordDate = DateTime.Today;
                Comment = "";
            }
        }

        protected virtual void CopyFieldsToTransaction()
        {
            if (_Transaction != null)
            { 
                _Transaction.Stock = Stock.Id;
                _Transaction.TransactionDate = TransactionDate;
                _Transaction.Comment = Comment;
            }
        }

        private async void PopulateAvailableStocks(DateTime date)
        {
            if (_StockSelection == TransactionStockSelection.None)
                return;

            AvailableStocks.Clear();

            if (_StockSelection == TransactionStockSelection.Holdings)
            {
                var response = await _RestClient.Holdings.Get(date);

                var stocks = response.Select(x => new StockViewItem(x.Stock));
                foreach (var stock in stocks.OrderBy(x => x.FormattedCompanyName))
                    AvailableStocks.Add(stock);
            }
            else if (_StockSelection == TransactionStockSelection.TradeableHoldings)
            {
                var response = await _RestClient.Holdings.Get(date);

                var stocks = response.Select(x => new StockViewItem(x.Stock));
                foreach (var stock in stocks.OrderBy(x => x.FormattedCompanyName))
                    AvailableStocks.Add(stock);
            }
            else if (_StockSelection == TransactionStockSelection.Stocks)
            {
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
            else if (_StockSelection == TransactionStockSelection.TradeableStocks)
            {
                var response = await _RestClient.Stocks.Get(date);

                var stocks = response.Select(x => new StockViewItem(x.Id, x.ASXCode, x.Name));
                foreach (var stock in stocks.OrderBy(x => x.FormattedCompanyName))
                    AvailableStocks.Add(stock);
            }
        }
    }


}
