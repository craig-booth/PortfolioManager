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
using PortfolioManager.Service.Utils;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class TransactionsViewModel : PortfolioViewModel
    {
        private IStockParameter _StockParameter;
        private IDateRangeParameter _DateParameter;
        private TransactionViewModelFactory _TransactionViewModelFactory;

        public void ParameterChange(object sender, PropertyChangedEventArgs e)
        {
            ShowTransactions();
        }

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

        public ObservableCollection<TransactionViewItem> Transactions { get; private set; }

        private TransactionViewModel _SelectedTransactionViewModel;
        public TransactionViewModel SelectedTransactionViewModel
        {
            get
            {
                return _SelectedTransactionViewModel;
            }
            set
            {
                _SelectedTransactionViewModel = value;
                
                OnPropertyChanged();
            }
        }


        public RelayCommand<Transaction> EditTransactionCommand { get; private set; }
        private void EditTransaction(Transaction transaction)
        {
            SelectedTransactionViewModel = _TransactionViewModelFactory.CreateTransactionViewModel(transaction);
        }

        public List<string> TransactionTypes { get; private set; }

        public TransactionsViewModel(string label, Portfolio portfolio, IStockParameter stockParameter, IDateRangeParameter dateParameter)
            : base(label, portfolio)
        {
            Options.AllowStockSelection = true;
            Options.DateSelection = DateSelectionType.Range;

            _Heading = label;
            _StockParameter = stockParameter;
            _DateParameter = dateParameter;
            _TransactionViewModelFactory = new ViewModels.TransactionViewModelFactory(Portfolio.StockService);

            EditTransactionCommand = new RelayCommand<Transaction>(EditTransaction);

            Transactions = new ObservableCollection<TransactionViewItem>();
            TransactionTypes = new List<string>();

            TransactionTypes.Add("Aquisition");
            TransactionTypes.Add("Disposal");
            TransactionTypes.Add("Income");
        }

        public override void Activate()
        {
            if (_DateParameter != null)
                _DateParameter.PropertyChanged += ParameterChange;

            if (_StockParameter != null)
                _StockParameter.PropertyChanged += ParameterChange;

            ShowTransactions();
        }

        public override void Deactivate()
        {
            if (_DateParameter != null)
                _DateParameter.PropertyChanged -= ParameterChange;

            if (_StockParameter != null)
                _StockParameter.PropertyChanged -= ParameterChange;
        }

        private void ShowTransactions()
        {
            Transactions.Clear();

            if ((_StockParameter == null) || (_DateParameter == null))
            {
                OnPropertyChanged("");
                return;
            }

            IReadOnlyCollection<Transaction> transactions;
            if (_StockParameter.Stock.Id == Guid.Empty)
                transactions = Portfolio.TransactionService.GetTransactions(_DateParameter.StartDate, _DateParameter.EndDate);
            else
                transactions = Portfolio.TransactionService.GetTransactions(_StockParameter.Stock.ASXCode, _DateParameter.StartDate, _DateParameter.EndDate);
            foreach (var transaction in transactions)
            {
                if (transaction.Type != TransactionType.CashTransaction)
                {
                    var stock = Portfolio.StockService.Get(transaction.ASXCode, transaction.RecordDate);
                    Transactions.Add(new TransactionViewItem(transaction, Portfolio.StockService));
                }
            }

            _SelectedTransactionViewModel = _TransactionViewModelFactory.CreateTransactionViewModel(Transactions.First().Transaction);

            OnPropertyChanged("");            
        }

    }

    class TransactionViewItem
    {
        public Transaction Transaction { get; private set; }

        public DateTime TransactionDate { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }

        public TransactionViewItem(Transaction transaction, StockService stockService)
        {
            Transaction = transaction;

            TransactionDate = transaction.TransactionDate;
            Description = transaction.Description;

            if (transaction.Type != TransactionType.CashTransaction)
            {
                var stock = stockService.Get(transaction.ASXCode, transaction.RecordDate);
                CompanyName = string.Format("{0} ({1})", stock.Name, stock.ASXCode);
            }
            else
                CompanyName = "";
        }
    }

    abstract class TransactionViewModel : NotifyClass
    {
        private StockService _StockService;

        public Transaction Transaction { get; private set; }
        public string Description { get; private set; }

        public string ASXCode { get; set; }
        public Stock Stock { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime RecordDate { get; set; }
        public string Comment { get; set; }

        public List<Stock> AvailableStocks { get; private set; }

        public TransactionViewModel(Transaction transaction, StockService stockService)
        {
            AvailableStocks = new List<Stock>();

            _StockService = stockService;
            Transaction = transaction;

            ASXCode = transaction.ASXCode;
            Description = transaction.Description;
            TransactionDate = transaction.TransactionDate;
            RecordDate = transaction.RecordDate;
            Comment = transaction.Comment;

            PopulateAvailableStocks(RecordDate);
            Stock = AvailableStocks.Find(x => x.ASXCode == transaction.ASXCode);
        }

        private void PopulateAvailableStocks(DateTime date)
        {
            var stocks = _StockService.GetAll(date).Where(x => x.ParentId == Guid.Empty).OrderBy(x => x.Name);

            AvailableStocks.Clear();
            AvailableStocks.AddRange(stocks);

            OnPropertyChanged("AvailableStocks");
        }


    }

    class TransactionViewModelFactory
    {
        private StockService _StockService;

        public TransactionViewModelFactory(StockService stockService)
        {
            _StockService = stockService;
        }

        public TransactionViewModel CreateTransactionViewModel(Transaction transaction)
        {
            if (transaction is Aquisition)
                return new AquisitionViewModel(transaction as Aquisition, _StockService);
            else if (transaction is CashTransaction)
                return new CashTransactionViewModel(transaction as CashTransaction, _StockService);
            else if (transaction is CostBaseAdjustment)
                return new CostBaseAdjustmentViewModel(transaction as CostBaseAdjustment, _StockService);
            else if (transaction is Disposal)
                return new DisposalViewModel(transaction as Disposal, _StockService);
            else if (transaction is IncomeReceived)
                return new IncomeReceivedViewModel(transaction as IncomeReceived, _StockService);
            else if (transaction is OpeningBalance)
                return new OpeningBalanceViewModel(transaction as OpeningBalance, _StockService);
            else if (transaction is ReturnOfCapital)
                return new ReturnOfCapitalViewModel(transaction as ReturnOfCapital, _StockService);
            else if (transaction is UnitCountAdjustment)
                return new UnitCountAdjustmentViewModel(transaction as UnitCountAdjustment, _StockService);
            else
                throw new NotSupportedException();
        }
    }

    class AquisitionViewModel : TransactionViewModel
    {
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public bool CreateCashTransaction { get; set; }

        public AquisitionViewModel(Aquisition aquisition, StockService stockService)
            : base(aquisition, stockService)
        {
            Units = aquisition.Units;
            AveragePrice = aquisition.AveragePrice;
            TransactionCosts = aquisition.TransactionCosts;
            CreateCashTransaction = aquisition.CreateCashTransaction;
        }
    }

    class CashTransactionViewModel : TransactionViewModel
    {
        public CashTransactionViewModel(CashTransaction cashTransaction, StockService stockService)
            : base(cashTransaction, stockService)
        {

        }
    }

    class CostBaseAdjustmentViewModel : TransactionViewModel
    {
        public CostBaseAdjustmentViewModel(CostBaseAdjustment costBaseAdjustment, StockService stockService)
            : base(costBaseAdjustment, stockService)
        {

        }
    }

    class DisposalViewModel : TransactionViewModel
    {
        public DisposalViewModel(Disposal disposal, StockService stockService)
            : base(disposal, stockService)
        {

        }
    }

    class IncomeReceivedViewModel : TransactionViewModel
    {
        public IncomeReceivedViewModel(IncomeReceived incomeReceived, StockService stockService)
            : base(incomeReceived, stockService)
        {

        }
    }

    class OpeningBalanceViewModel : TransactionViewModel
    {
        public int Units { get; set; }
        public decimal CostBase { get; set; }
        public DateTime AquisitionDate { get; set; }

        public OpeningBalanceViewModel(OpeningBalance openingBalance, StockService stockService)
            : base(openingBalance, stockService)
        {
            Units = openingBalance.Units;
            CostBase = openingBalance.CostBase;
            AquisitionDate = openingBalance.AquisitionDate;
        }

    }

    class ReturnOfCapitalViewModel : TransactionViewModel
    {
        public ReturnOfCapitalViewModel(ReturnOfCapital returnOfCapital, StockService stockService)
            : base(returnOfCapital, stockService)
        {

        }
    }

    class UnitCountAdjustmentViewModel : TransactionViewModel
    {
        public UnitCountAdjustmentViewModel(UnitCountAdjustment unitCostAdjustment, StockService stockService)
            : base(unitCostAdjustment, stockService)
        {

        }
    }
}
