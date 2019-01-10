using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Client;
using PortfolioManager.UI.Models;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    enum TransactionStockSelection { None, Holdings, TradeableHoldings, Stocks, TradeableStocks }

    class TransactionViewModel : ViewModel, IEditableObject
    {
        protected bool _BeingEdited;
        protected TransactionStockSelection _StockSelection;
        protected RestWebClient _RestWebClient;
        protected RestClient _RestClient;

        public Transaction Transaction { get; protected set; }
        public string Description { get; private set; }    

        private Stock _Stock;
        public Stock Stock
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

        public string CompanyName
        {
            get
            {
                return Stock.FormattedCompanyName;
            }
        }
        public string Comment { get; set; }

        public ObservableCollection<Stock> AvailableStocks { get; private set; }

        public bool IsStockEditable
        {
            get
            {
                return (Transaction == null);
            }
        }

        public TransactionViewModel(Transaction transaction, TransactionStockSelection stockSeletion, RestWebClient restWebClient, RestClient restClient)
        {
            _StockSelection = stockSeletion;
            _RestWebClient = restWebClient;
            _RestClient = restClient;

            Transaction = transaction;

            if (_StockSelection != TransactionStockSelection.None)
                AvailableStocks = new ObservableCollection<Stock>();                   

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

            CopyFieldsToTransaction();

            OnPropertyChanged("");
        }

        public void CancelEdit()
        {
            _BeingEdited = false;

            CopyTransactionToFields();
        }

        protected virtual void CopyTransactionToFields()
        {
            if (Transaction != null)
            {
                Stock = Transaction.Stock;
                Description = Transaction.Description;
                TransactionDate = Transaction.TransactionDate;
                RecordDate = Transaction.RecordDate;
                Comment = Transaction.Comment;
            }
            else
            {
                Stock = new Stock(Guid.Empty, "", "");
                Description = "";
                TransactionDate = DateTime.Today;
                RecordDate = DateTime.Today;
                Comment = "";
            }
        }

        protected virtual void CopyFieldsToTransaction()
        {
            if (Transaction != null)
            {
                Transaction.Stock = Stock;
                Transaction.TransactionDate = TransactionDate;
                Transaction.RecordDate = RecordDate;
                Transaction.Comment = Comment;
            }
        }

        private async void PopulateAvailableStocks(DateTime date)
        {
            if (_StockSelection == TransactionStockSelection.None)
                return;

            AvailableStocks.Clear();

            if (_StockSelection == TransactionStockSelection.Holdings)
            {
                var responce = await _RestWebClient.GetPortfolioHoldingsAsync(date);

                foreach (var holding in responce.Holdings.OrderBy(x => x.Stock.FormattedCompanyName()))
                    AvailableStocks.Add(new Stock(holding.Stock));
            }
            else if (_StockSelection == TransactionStockSelection.TradeableHoldings)
            {
                var responce = await _RestWebClient.GetPortfolioTradeableHoldingsAsync(date);
        
                foreach (var holding in responce.Holdings.OrderBy(x => x.Stock.FormattedCompanyName()))
                    AvailableStocks.Add(new Stock(holding.Stock));
            }
            else if (_StockSelection == TransactionStockSelection.Stocks)
            {
                var stocks = await _RestClient.Stocks.Get(date);
                
                foreach (var stock in stocks)
                {
                    var stockItem = new Stock(stock.Id, stock.ASXCode, stock.Name);
                    AvailableStocks.Add(stockItem);
                    if (!stock.StapledSecurity)
                    {
                        foreach (var childSecurity in stock.ChildSecurities)
                        {
                            stockItem = new Stock(stock.Id, childSecurity.ASXCode, childSecurity.Name);
                            AvailableStocks.Add(stockItem);
                        }
                    }
                }
            }
            else if (_StockSelection == TransactionStockSelection.TradeableStocks)
            {
                var stocks = await _RestClient.Stocks.Get(date);

                foreach (var stock in stocks)
                {
                    var stockItem = new Stock(stock.Id, stock.ASXCode, stock.Name);
                    AvailableStocks.Add(stockItem);
                }
            }
        }
    }

    class TransactionViewModelFactory
    {
        private RestWebClient _RestWebClient;
        private RestClient _RestClient;

        public Dictionary<string, TransactionType> TransactionTypes { get; private set; }

        public TransactionViewModelFactory(RestWebClient restWebClient, RestClient restClient)
        {
            _RestWebClient = restWebClient;
            _RestClient = restClient;

            TransactionTypes = new Dictionary<string, TransactionType>();
            TransactionTypes.Add("Buy", TransactionType.Aquisition);
            TransactionTypes.Add("Cash Transaction", TransactionType.CashTransaction);
            TransactionTypes.Add("Adjust Cost Base", TransactionType.CostBaseAdjustment);
            TransactionTypes.Add("Sell", TransactionType.Disposal);
            TransactionTypes.Add("Income Received", TransactionType.Income);
            TransactionTypes.Add("Opening Balance", TransactionType.OpeningBalance);
            TransactionTypes.Add("Return Of Capital", TransactionType.ReturnOfCapital);
            TransactionTypes.Add("Adjust Unit Count", TransactionType.UnitCountAdjustment);
        }

        public TransactionViewModel CreateTransactionViewModel(TransactionType type)
        {
            if (type == TransactionType.Aquisition)
                return new AquisitionViewModel(null, _RestWebClient, _RestClient);
            else if (type == TransactionType.CashTransaction)
                return new CashTransactionViewModel(null, _RestWebClient, _RestClient);
            else if (type == TransactionType.CostBaseAdjustment)
                return new CostBaseAdjustmentViewModel(null, _RestWebClient, _RestClient);
            else if (type == TransactionType.Disposal)
                return new DisposalViewModel(null, _RestWebClient, _RestClient);
            else if (type == TransactionType.Income)
                return new IncomeReceivedViewModel(null, _RestWebClient, _RestClient);
            else if (type == TransactionType.OpeningBalance)
                return new OpeningBalanceViewModel(null, _RestWebClient, _RestClient);
            else if (type == TransactionType.ReturnOfCapital)
                return new ReturnOfCapitalViewModel(null, _RestWebClient, _RestClient);
            else if (type == TransactionType.UnitCountAdjustment)
                return new UnitCountAdjustmentViewModel(null, _RestWebClient, _RestClient);
            else
                throw new NotSupportedException();
        }

        public TransactionViewModel CreateTransactionViewModel(Transaction transaction)
        {
            if (transaction.Type == TransactionType.Aquisition)
                return new AquisitionViewModel(transaction as AquisitionTransaction, _RestWebClient, _RestClient);
            else if (transaction.Type == TransactionType.CashTransaction)
                return new CashTransactionViewModel(transaction as CashTransaction, _RestWebClient, _RestClient);
            else if (transaction.Type == TransactionType.CostBaseAdjustment)
                return new CostBaseAdjustmentViewModel(transaction as CostBaseAdjustmentTransaction, _RestWebClient, _RestClient);
            else if (transaction.Type == TransactionType.Disposal)
                return new DisposalViewModel(transaction as DisposalTransaction, _RestWebClient, _RestClient);
            else if (transaction.Type == TransactionType.Income)
                return new IncomeReceivedViewModel(transaction as IncomeTransaction, _RestWebClient, _RestClient);
            else if (transaction.Type == TransactionType.OpeningBalance)
                return new OpeningBalanceViewModel(transaction as OpeningBalanceTransaction, _RestWebClient, _RestClient);
            else if (transaction.Type == TransactionType.ReturnOfCapital)
                return new ReturnOfCapitalViewModel(transaction as ReturnOfCapitalTransaction, _RestWebClient, _RestClient);
            else if (transaction.Type == TransactionType.UnitCountAdjustment)
                return new UnitCountAdjustmentViewModel(transaction as UnitCountAdjustmentTransaction, _RestWebClient, _RestClient);
            else
                throw new NotSupportedException();
        }
    }
}
