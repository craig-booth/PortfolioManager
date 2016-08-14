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

        public ObservableCollection<TransactionViewModel> Transactions { get; private set; }

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

            Transactions = new ObservableCollection<TransactionViewModel>();
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
                    Transactions.Add(_TransactionViewModelFactory.CreateTransactionViewModel(transaction));
                }
            }


            OnPropertyChanged("");
        }

    }

    abstract class TransactionViewModel
    {
        private StockService _StockService;
        private Stock _Stock;

        public Transaction Transaction { get; private set; }
        public string CompanyName { get; private set; }
        public string Description { get; private set; }

        public DateTime TransactionDate { get; set; }
        public string ASXCode { get; set; }
        public DateTime RecordDate { get; set; }
        public string Comment { get; set; }

        public TransactionViewModel(Transaction transaction, StockService stockService)
        {
            _StockService = stockService;

            Transaction = transaction;

            Description = transaction.Description;
            TransactionDate = transaction.TransactionDate;
            ASXCode = transaction.ASXCode;
            RecordDate = transaction.RecordDate;
            Comment = transaction.Comment;

            _Stock = _StockService.Get(ASXCode, RecordDate);
            CompanyName = string.Format("{0} ({1})", _Stock.Name, ASXCode);
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
        public OpeningBalanceViewModel(OpeningBalance openingBalance, StockService stockService)
            : base(openingBalance, stockService)
        {

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
