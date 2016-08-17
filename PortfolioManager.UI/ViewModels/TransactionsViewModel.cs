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

        private TransactionViewModel _CurrentTransactionViewModel;
        public TransactionViewModel CurrentTransactionViewModel
        {
            get
            {
                return _CurrentTransactionViewModel;
            }
            private set
            {
                _CurrentTransactionViewModel = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand<TransactionViewModel> EditTransactionCommand { get; private set; }
        private void EditTransaction(TransactionViewModel transactionViewModel)
        {
            CurrentTransactionViewModel = transactionViewModel;
            CurrentTransactionViewModel.BeginEdit();
        }

        public RelayCommand CancelTransactionCommand { get; private set; }
        private void CancelTransaction()
        { 
            CurrentTransactionViewModel.CancelEdit();
            CurrentTransactionViewModel = null;
        }

        public RelayCommand SaveTransactionCommand { get; private set; }
        private void SaveTransaction()
        {
            CurrentTransactionViewModel.EndEdit();
        }

        public RelayCommand DeleteTransactionCommand { get; private set; }
        private void DeleteTransaction()
        {

        }

        public RelayCommand<TransactionType> AddTransactionCommand { get; private set; }
        private void AddTransaction(TransactionType transactionType)
        {
            CurrentTransactionViewModel = _TransactionViewModelFactory.CreateTransactionViewModel(transactionType);
            CurrentTransactionViewModel.BeginEdit();  
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

            EditTransactionCommand = new RelayCommand<TransactionViewModel>(EditTransaction);
            CancelTransactionCommand = new RelayCommand(CancelTransaction);
            SaveTransactionCommand = new RelayCommand(SaveTransaction);
            DeleteTransactionCommand = new RelayCommand(DeleteTransaction);
            AddTransactionCommand = new RelayCommand<TransactionType>(AddTransaction);

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

    class TransactionViewModel : NotifyClass, IEditableObject
    {
        private StockService _StockService;
        protected bool _BeingEdited;

        public Transaction Transaction { get; private set; }
        public string Description { get; private set; }

        public string ASXCode { get; set; }
        public Stock Stock { get; set; }
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

                if (_BeingEdited)
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

        public List<Stock> AvailableStocks { get; private set; }

        public TransactionViewModel(Transaction transaction, StockService stockService)
        {
            AvailableStocks = new List<Stock>();

            _StockService = stockService;

            Transaction = transaction;
            if (transaction != null)
                Stock = _StockService.Get(transaction.ASXCode, transaction.RecordDate);

            CopyTransactionToFields();
        }

        public void BeginEdit()
        {
            _BeingEdited = true;

            PopulateAvailableStocks(RecordDate);
            Stock = AvailableStocks.Find(x => x.ASXCode == ASXCode);
        }

        public void EndEdit()
        {
            _BeingEdited = false;
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

        public TransactionViewModel CreateTransactionViewModel(TransactionType type)
        {
            if (type == TransactionType.Aquisition)
                return new AquisitionViewModel(null, _StockService);
            else if (type == TransactionType.CashTransaction)
                return new CashTransactionViewModel(null, _StockService);
            else if (type == TransactionType.CostBaseAdjustment)
                return new CostBaseAdjustmentViewModel(null, _StockService);
            else if (type == TransactionType.Disposal)
                return new DisposalViewModel(null, _StockService);
            else if (type == TransactionType.Income)
                return new IncomeReceivedViewModel(null, _StockService);
            else if (type == TransactionType.OpeningBalance)
                return new OpeningBalanceViewModel(null, _StockService);
            else if (type == TransactionType.ReturnOfCapital)
                return new ReturnOfCapitalViewModel(null, _StockService);
            else if (type == TransactionType.UnitCountAdjustment)
                return new UnitCountAdjustmentViewModel(null, _StockService);
            else
                throw new NotSupportedException();
        }

        public TransactionViewModel CreateTransactionViewModel(Transaction transaction)
        {
            if (transaction.Type == TransactionType.Aquisition)
                return new AquisitionViewModel(transaction as Aquisition, _StockService);
            else if (transaction.Type == TransactionType.CashTransaction)
                return new CashTransactionViewModel(transaction as CashTransaction, _StockService);
            else if (transaction.Type == TransactionType.CostBaseAdjustment)
                return new CostBaseAdjustmentViewModel(transaction as CostBaseAdjustment, _StockService);
            else if (transaction.Type == TransactionType.Disposal)
                return new DisposalViewModel(transaction as Disposal, _StockService);
            else if (transaction.Type == TransactionType.Income)
                return new IncomeReceivedViewModel(transaction as IncomeReceived, _StockService);
            else if (transaction.Type == TransactionType.OpeningBalance)
                return new OpeningBalanceViewModel(transaction as OpeningBalance, _StockService);
            else if (transaction.Type == TransactionType.ReturnOfCapital)
                return new ReturnOfCapitalViewModel(transaction as ReturnOfCapital, _StockService);
            else if (transaction.Type == TransactionType.UnitCountAdjustment)
                return new UnitCountAdjustmentViewModel(transaction as UnitCountAdjustment, _StockService);
            else
                throw new NotSupportedException();
        }
    }

    class AquisitionViewModel : TransactionViewModel
    {
        public Aquisition Aquisition { get; private set; }
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public bool CreateCashTransaction { get; set; }

        public AquisitionViewModel(Aquisition aquisition, StockService stockService)
            : base(aquisition, stockService)
        {
            Aquisition = aquisition;
        }

        protected override void CopyTransactionToFields()
        {
            if (Transaction != null)
            {
                Units = Aquisition.Units;
                AveragePrice = Aquisition.AveragePrice;
                TransactionCosts = Aquisition.TransactionCosts;
                CreateCashTransaction = Aquisition.CreateCashTransaction;
            }
            else
            {
                Units = 0;
                AveragePrice = 0.00m;
                TransactionCosts = 0.00m;
                CreateCashTransaction = false;
            }
        }

        protected override void CopyFieldsToTransaction()
        {

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
        public OpeningBalance OpeningBalance { get; private set; }
        public int Units { get; set; }
        public decimal CostBase { get; set; }
        public DateTime AquisitionDate { get; set; }

        public OpeningBalanceViewModel(OpeningBalance openingBalance, StockService stockService)
            : base(openingBalance, stockService)
        {
            OpeningBalance = openingBalance;
        }

        protected override void CopyTransactionToFields()
        {
            if (Transaction != null)
            {
                Units = OpeningBalance.Units;
                CostBase = OpeningBalance.CostBase;
                AquisitionDate = OpeningBalance.AquisitionDate;
            }
            else
            {
                Units = 0;
                CostBase = 0.00m;
                AquisitionDate = DateTime.Today;
            }
        }

        protected override void CopyFieldsToTransaction()
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
