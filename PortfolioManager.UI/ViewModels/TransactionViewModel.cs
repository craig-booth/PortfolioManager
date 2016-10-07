using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Service;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class TransactionStockSelection
    {
        public bool OwnedStocksOnly;
        public bool IncludeStapledSecurites;
        public bool IncludeChildStocks;    
        
        public TransactionStockSelection(bool ownedStocksOnly, bool includeStapledSecurities, bool includeChildStocks)
        {
            OwnedStocksOnly = ownedStocksOnly;
            IncludeStapledSecurites = includeStapledSecurities;
            IncludeChildStocks = includeChildStocks;
        }

        public static TransactionStockSelection AllStocks()
        {
            return new TransactionStockSelection(false, true, true);
        }

        public static TransactionStockSelection TradeableStocks(bool owned)
        {
            return new TransactionStockSelection(owned, true, false);
        }

        public static TransactionStockSelection NonStapledStocks(bool owned)
        {
            return new TransactionStockSelection(owned, false, true);
        }
    }
    

    class TransactionViewModel : ViewModel, IEditableObject
    {
        protected StockService _StockService;
        protected ShareHoldingService _HoldingService;

        protected bool _BeingEdited;
        protected TransactionStockSelection _StockSelection;

        public Transaction Transaction { get; protected set; }
        public string Description { get; private set; }

        public string ASXCode { get; set; }

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

                if (_StockSelection != null)
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
                _RecordDate = value;

                if (_BeingEdited && (_StockSelection != null))
                    PopulateAvailableStocks(_RecordDate);
            }
        }

        public string CompanyName
        {
            get
            {
                if (Stock != null)
                    return string.Format("{0} ({1})", Stock.Name, Stock.ASXCode);
                else
                    return ASXCode;
            }
        }
        public string Comment { get; set; }

        public ObservableCollection<Stock> AvailableStocks { get; private set; }

        public bool StockReadOnly
        {
            get
            {
                return (Transaction == null);
            }
        }

        public TransactionViewModel(Transaction transaction, TransactionStockSelection stockSeletion, StockService stockService, ShareHoldingService holdingService)
        {
            _StockSelection = stockSeletion;
            _StockService = stockService;
            _HoldingService = holdingService;
            Transaction = transaction;

            if (_StockSelection != null)
            {
                AvailableStocks = new ObservableCollection<Stock>();

                if (transaction != null)
                    Stock = _StockService.Get(transaction.ASXCode, transaction.RecordDate);
            }

            CopyTransactionToFields();
        }

        public void BeginEdit()
        {
            _BeingEdited = true;

            if (_StockSelection != null)
            {
                PopulateAvailableStocks(RecordDate);
                Stock = AvailableStocks.FirstOrDefault(x => x.ASXCode == ASXCode);
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
                ASXCode = Transaction.ASXCode;
                Description = Transaction.Description;
                TransactionDate = Transaction.TransactionDate;
                RecordDate = Transaction.RecordDate;
                Comment = Transaction.Comment;
            }
            else
            {
                ASXCode = "";
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
                if (Stock != null)
                    Transaction.ASXCode = Stock.ASXCode;
                else
                    Transaction.ASXCode = ASXCode;
                Transaction.TransactionDate = TransactionDate;
                Transaction.RecordDate = RecordDate;
                Transaction.Comment = Comment;
            }
        }

        private void PopulateAvailableStocks(DateTime date)
        {
            if (_StockSelection == null)
                return;

            AvailableStocks.Clear();

            IEnumerable<Stock> stocks = null;
            if (_StockSelection.OwnedStocksOnly)
            {
                stocks = _HoldingService.GetOwnedStocks(date, _StockSelection.IncludeChildStocks).OrderBy(x => x.Name);
            }
            else
            {
                stocks = _StockService.GetAll(date).OrderBy(x => x.Name);

                if (!_StockSelection.IncludeStapledSecurites)
                    stocks = stocks.Where(x => x.Type != StockType.StapledSecurity);

                if (!_StockSelection.IncludeChildStocks)
                    stocks = stocks.Where(x => x.ParentId == Guid.Empty);
            }

            if (stocks != null)
            {
                foreach (var stock in stocks)
                    AvailableStocks.Add(stock);
            }
        }


    }

    class TransactionViewModelFactory
    {
        private StockService _StockService;
        private ShareHoldingService _HoldingService;

        public Dictionary<string, TransactionType> TransactionTypes { get; private set; }

        public TransactionViewModelFactory(StockService stockService, ShareHoldingService holdingService)
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

        public TransactionViewModel CreateTransactionViewModel(Transaction transaction)
        {
            if (transaction.Type == TransactionType.Aquisition)
                return new AquisitionViewModel(transaction as Aquisition, _StockService, _HoldingService);
            else if (transaction.Type == TransactionType.CashTransaction)
                return new CashTransactionViewModel(transaction as CashTransaction, _StockService, _HoldingService);
            else if (transaction.Type == TransactionType.CostBaseAdjustment)
                return new CostBaseAdjustmentViewModel(transaction as CostBaseAdjustment, _StockService, _HoldingService);
            else if (transaction.Type == TransactionType.Disposal)
                return new DisposalViewModel(transaction as Disposal, _StockService, _HoldingService);
            else if (transaction.Type == TransactionType.Income)
                return new IncomeReceivedViewModel(transaction as IncomeReceived, _StockService, _HoldingService);
            else if (transaction.Type == TransactionType.OpeningBalance)
                return new OpeningBalanceViewModel(transaction as OpeningBalance, _StockService, _HoldingService);
            else if (transaction.Type == TransactionType.ReturnOfCapital)
                return new ReturnOfCapitalViewModel(transaction as ReturnOfCapital, _StockService, _HoldingService);
            else if (transaction.Type == TransactionType.UnitCountAdjustment)
                return new UnitCountAdjustmentViewModel(transaction as UnitCountAdjustment, _StockService, _HoldingService);
            else
                throw new NotSupportedException();
        }
    }
}
