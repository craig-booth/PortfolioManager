using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Common;
using PortfolioManager.Service.Interface;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    enum TransactionStockSelection { None, Holdings, TradeableHoldings, Stocks, TradeableStocks }

    class TransactionViewModel : ViewModel, IEditableObject
    {
        protected IStockService _StockService;
        protected IHoldingService _HoldingService;

        protected bool _BeingEdited;
        protected TransactionStockSelection _StockSelection;

        public TransactionItem Transaction { get; protected set; }
        public string Description { get; private set; }

        private StockItem _Stock;
        public StockItem Stock
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

        public ObservableCollection<StockItem> AvailableStocks { get; private set; }

        public bool IsStockEditable
        {
            get
            {
                return (Transaction == null);
            }
        }

        public TransactionViewModel(TransactionItem transaction, TransactionStockSelection stockSeletion, IStockService stockService, IHoldingService holdingService)
        {
            _StockSelection = stockSeletion;
            _StockService = stockService;
            _HoldingService = holdingService;
            Transaction = transaction;

            if (_StockSelection != TransactionStockSelection.None)
                AvailableStocks = new ObservableCollection<StockItem>();                   

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

            AvailableStocks.Clear();

            if (_StockSelection == TransactionStockSelection.Holdings)
            {
                var responce = await _HoldingService.GetHoldings(date);

                foreach (var holding in responce.Holdings.OrderBy(x => x.Stock.FormattedCompanyName()))
                    AvailableStocks.Add(holding.Stock);
            }
            else if (_StockSelection == TransactionStockSelection.TradeableHoldings)
            {
                var responce = await _HoldingService.GetTradeableHoldings(date);

                foreach (var holding in responce.Holdings.OrderBy(x => x.Stock.FormattedCompanyName()))
                    AvailableStocks.Add(holding.Stock);
            }
            else if (_StockSelection == TransactionStockSelection.Stocks)
            {
                var responce = await _StockService.GetStocks(date, true, true);

                foreach (var stock in responce.Stocks.OrderBy(x => x.FormattedCompanyName()))
                    AvailableStocks.Add(stock);
            }
            else if (_StockSelection == TransactionStockSelection.TradeableStocks)
            {
                var responce = await _StockService.GetStocks(date, true, false);

                foreach (var stock in responce.Stocks.OrderBy(x => x.FormattedCompanyName()))
                    AvailableStocks.Add(stock);
            }
        }
    }

    class TransactionViewModelFactory
    {
        private IStockService _StockService;
        private IHoldingService _HoldingService;

        public Dictionary<string, TransactionType> TransactionTypes { get; private set; }

        public TransactionViewModelFactory(IStockService stockService, IHoldingService holdingService)
        {
            _StockService = stockService;
            _HoldingService = holdingService;

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
                return new AquisitionViewModel(null, _StockService, _HoldingService);
            else if (type == TransactionType.CashTransaction)
                return new CashTransactionViewModel(null, _StockService, _HoldingService);
            else if (type == TransactionType.CostBaseAdjustment)
                return new CostBaseAdjustmentViewModel(null, _StockService, _HoldingService);
            else if (type == TransactionType.Disposal)
                return new DisposalViewModel(null, _StockService, _HoldingService);
            else if (type == TransactionType.Income)
                return new IncomeReceivedViewModel(null, _StockService, _HoldingService);
            else if (type == TransactionType.OpeningBalance)
                return new OpeningBalanceViewModel(null, _StockService, _HoldingService);
            else if (type == TransactionType.ReturnOfCapital)
                return new ReturnOfCapitalViewModel(null, _StockService, _HoldingService);
            else if (type == TransactionType.UnitCountAdjustment)
                return new UnitCountAdjustmentViewModel(null, _StockService, _HoldingService);
            else
                throw new NotSupportedException();
        }

        public TransactionViewModel CreateTransactionViewModel(TransactionItem transaction)
        {
            if (transaction.Type == TransactionType.Aquisition)
                return new AquisitionViewModel(transaction as AquisitionTransactionItem, _StockService, _HoldingService);
            else if (transaction.Type == TransactionType.CashTransaction)
                return new CashTransactionViewModel(transaction as CashTransactionItem, _StockService, _HoldingService);
            else if (transaction.Type == TransactionType.CostBaseAdjustment)
                return new CostBaseAdjustmentViewModel(transaction as CostBaseAdjustmentTransactionItem, _StockService, _HoldingService);
            else if (transaction.Type == TransactionType.Disposal)
                return new DisposalViewModel(transaction as DisposalTransactionItem, _StockService, _HoldingService);
            else if (transaction.Type == TransactionType.Income)
                return new IncomeReceivedViewModel(transaction as IncomeTransactionItem, _StockService, _HoldingService);
            else if (transaction.Type == TransactionType.OpeningBalance)
                return new OpeningBalanceViewModel(transaction as OpeningBalanceTransactionItem, _StockService, _HoldingService);
            else if (transaction.Type == TransactionType.ReturnOfCapital)
                return new ReturnOfCapitalViewModel(transaction as ReturnOfCapitalTransactionItem, _StockService, _HoldingService);
            else if (transaction.Type == TransactionType.UnitCountAdjustment)
                return new UnitCountAdjustmentViewModel(transaction as UnitCountAdjustmentTransactionItem, _StockService, _HoldingService);
            else
                throw new NotSupportedException();
        }
    }
}
