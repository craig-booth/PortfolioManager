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

        public bool EditingTransaction { get; set; }

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
            EditingTransaction = true;
        }

        public RelayCommand CancelTransactionCommand { get; private set; }
        private void CancelTransaction()
        { 
            CurrentTransactionViewModel.CancelEdit();
            CurrentTransactionViewModel = null;
            EditingTransaction = false;
        }

        public RelayCommand SaveTransactionCommand { get; private set; }
        private void SaveTransaction()
        {
            CurrentTransactionViewModel.EndEdit();
            CurrentTransactionViewModel = null;
            EditingTransaction = false;
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
            EditingTransaction = true;
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

            EditingTransaction = false;
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

}
