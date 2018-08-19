using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Stocks;
using PortfolioManager.RestApi.Client;
using PortfolioManager.Service.Interface;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    enum TransactionStockSelection { None, Holdings, TradeableHoldings, Stocks, TradeableStocks }

    class TransactionViewModel : ViewModel, IEditableObject
    {
        protected bool _BeingEdited;
        protected TransactionStockSelection _StockSelection;
        protected RestClient _RestClient;

        public TransactionItem Transaction { get; protected set; }
        public string Description { get; private set; }    

        private TransactionStock _Stock;
        public TransactionStock Stock
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
                return Stock.FormattedCompanyName();
            }
        }
        public string Comment { get; set; }

        public ObservableCollection<TransactionStock> AvailableStocks { get; private set; }

        public bool IsStockEditable
        {
            get
            {
                return (Transaction == null);
            }
        }

        public TransactionViewModel(TransactionItem transaction, TransactionStockSelection stockSeletion, RestClient restClient)
        {
            _StockSelection = stockSeletion;
            _RestClient = restClient;

            Transaction = transaction;

            if (_StockSelection != TransactionStockSelection.None)
                AvailableStocks = new ObservableCollection<TransactionStock>();                   

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
                Stock = new StockItem(Guid.Empty, "", "");
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
                var responce = await _RestClient.Holdings.Get(date);

                foreach (var stock in responce.Select(x => x.Stock).ToTransactionStock(true).OrderBy(x => x.ToString()))
                    AvailableStocks.Add(stock);
            }
            else if (_StockSelection == TransactionStockSelection.TradeableHoldings)
            {
                var responce = await _RestClient.Holdings.Get(date);

                foreach (var stock in responce.Select(x => x.Stock).ToTransactionStock(false).OrderBy(x => x.ToString()))
                    AvailableStocks.Add(stock);
            }
            else if (_StockSelection == TransactionStockSelection.Stocks)
            {
                var response = await _RestClient.Stocks.Get(date);

                foreach (var stock in response.ToTransactionStock(true).OrderBy(x => x.ToString()))
                    AvailableStocks.Add(stock);
            }
            else if (_StockSelection == TransactionStockSelection.TradeableStocks)
            {
                var response = await _RestClient.Stocks.Get(date);

                foreach (var stock in response.ToTransactionStock(false).OrderBy(x => x.ToString()))
                    AvailableStocks.Add(stock);
            }
        }
    }

    class TransactionViewModelFactory
    {
        private RestClient _RestClient;

        public Dictionary<string, TransactionType> TransactionTypes { get; private set; }

        public TransactionViewModelFactory(RestClient restClient)
        {
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
                return new AquisitionViewModel(null, _RestClient);
            else if (type == TransactionType.CashTransaction)
                return new CashTransactionViewModel(null, _RestClient);
            else if (type == TransactionType.CostBaseAdjustment)
                return new CostBaseAdjustmentViewModel(null, _RestClient);
            else if (type == TransactionType.Disposal)
                return new DisposalViewModel(null, _RestClient);
            else if (type == TransactionType.Income)
                return new IncomeReceivedViewModel(null, _RestClient);
            else if (type == TransactionType.OpeningBalance)
                return new OpeningBalanceViewModel(null, _RestClient);
            else if (type == TransactionType.ReturnOfCapital)
                return new ReturnOfCapitalViewModel(null, _RestClient);
            else if (type == TransactionType.UnitCountAdjustment)
                return new UnitCountAdjustmentViewModel(null, _RestClient);
            else
                throw new NotSupportedException();
        }

        public TransactionViewModel CreateTransactionViewModel(TransactionItem transaction)
        {
            if (transaction.Type == TransactionType.Aquisition)
                return new AquisitionViewModel(transaction as AquisitionTransactionItem, _RestClient);
            else if (transaction.Type == TransactionType.CashTransaction)
                return new CashTransactionViewModel(transaction as CashTransactionItem, _RestClient);
            else if (transaction.Type == TransactionType.CostBaseAdjustment)
                return new CostBaseAdjustmentViewModel(transaction as CostBaseAdjustmentTransactionItem, _RestClient);
            else if (transaction.Type == TransactionType.Disposal)
                return new DisposalViewModel(transaction as DisposalTransactionItem, _RestClient);
            else if (transaction.Type == TransactionType.Income)
                return new IncomeReceivedViewModel(transaction as IncomeTransactionItem, _RestClient);
            else if (transaction.Type == TransactionType.OpeningBalance)
                return new OpeningBalanceViewModel(transaction as OpeningBalanceTransactionItem, _RestClient);
            else if (transaction.Type == TransactionType.ReturnOfCapital)
                return new ReturnOfCapitalViewModel(transaction as ReturnOfCapitalTransactionItem, _RestClient);
            else if (transaction.Type == TransactionType.UnitCountAdjustment)
                return new UnitCountAdjustmentViewModel(transaction as UnitCountAdjustmentTransactionItem, _RestClient);
            else
                throw new NotSupportedException();
        }
    }

    class TransactionStock
    {
        public Guid Id;
        public string AsxCode;
        public string Name;

        public string ChildCode;
        public string ChildName;

        public override string ToString()
        {
            if (ChildCode != "")
                return string.Format("{0} {1}", Name, AsxCode);
            else
                return string.Format("{0} {1} - {2} {3}", Name, AsxCode, ChildCode, ChildName);
        }
    }

    static class StockResponseExtensions
    {
        public static IEnumerable<TransactionStock> ToTransactionStock(this IEnumerable<StockResponse> stockResponses, bool includeChildSecurities)
        {
            var transactionStocks = new List<TransactionStock>();

            foreach (var stockResponse in stockResponses)
            {
                if ((! stockResponse.StapledSecurity) || ! includeChildSecurities)
                {
                    transactionStocks.Add(new TransactionStock()
                    {
                        Id = stockResponse.Id,
                        AsxCode = stockResponse.ASXCode,
                        Name = stockResponse.Name,
                        ChildCode = "",
                        ChildName = ""
                    });
                }
                else
                {
                    foreach (var childSecurity in stockResponse.ChildSecurities)
                    {
                        transactionStocks.Add(new TransactionStock()
                        {
                            Id = stockResponse.Id,
                            AsxCode = stockResponse.ASXCode,
                            Name = stockResponse.Name,
                            ChildCode = childSecurity.ASXCode,
                            ChildName = childSecurity.Name
                        });
                    }
                }
            }

            return transactionStocks;
        }
    }
}
