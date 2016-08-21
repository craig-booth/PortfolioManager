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
    enum EditMode { None, Create, View, Update, Delete };

    class TransactionsViewModel : PortfolioViewModel
    {
        private IStockParameter _StockParameter;
        private IDateRangeParameter _DateParameter;

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

        public TransactionViewModelFactory TransactionViewModelFactory { get; private set; }

        public ObservableCollection<TransactionViewModel> Transactions { get; private set; }

        private EditMode _TransactionEditMode;
        public EditMode TransactionEditMode
        {
            get
            {
                return _TransactionEditMode;
            }

            set
            {
                _TransactionEditMode = value;
                OnPropertyChanged();
            }
        }

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
            TransactionEditMode = EditMode.Update;
        }

        public RelayCommand CancelTransactionCommand { get; private set; }
        private void CancelTransaction()
        { 
            if (CurrentTransactionViewModel != null)
                CurrentTransactionViewModel.CancelEdit();
            CurrentTransactionViewModel = null;
            TransactionEditMode = EditMode.None;
        }

        public RelayCommand SaveTransactionCommand { get; private set; }
        private void SaveTransaction()
        {
            if (CurrentTransactionViewModel != null)
                CurrentTransactionViewModel.EndEdit();
            CurrentTransactionViewModel = null;
            TransactionEditMode = EditMode.None;
        }

        public RelayCommand DeleteTransactionCommand { get; private set; }
        private void DeleteTransaction()
        {

        }

        public RelayCommand<TransactionType> AddTransactionCommand { get; private set; }
        private void AddTransaction(TransactionType transactionType)
        {
            CurrentTransactionViewModel = TransactionViewModelFactory.CreateTransactionViewModel(transactionType);
            CurrentTransactionViewModel.BeginEdit();
            TransactionEditMode = EditMode.Create;
        }

        public TransactionsViewModel(string label, Portfolio portfolio, IStockParameter stockParameter, IDateRangeParameter dateParameter)
            : base(label, portfolio)
        {
            Options.AllowStockSelection = true;
            Options.DateSelection = DateSelectionType.Range;

            _Heading = label;
            _StockParameter = stockParameter;
            _DateParameter = dateParameter;

            Transactions = new ObservableCollection<ViewModels.TransactionViewModel>();
            TransactionViewModelFactory = new ViewModels.TransactionViewModelFactory(Portfolio.StockService);

            TransactionEditMode = EditMode.None;
            EditTransactionCommand = new RelayCommand<TransactionViewModel>(EditTransaction);
            CancelTransactionCommand = new RelayCommand(CancelTransaction);
            SaveTransactionCommand = new RelayCommand(SaveTransaction, CanSaveTransaction);
            DeleteTransactionCommand = new RelayCommand(DeleteTransaction, CanDeleteTransaction);
            AddTransactionCommand = new RelayCommand<TransactionType>(AddTransaction);
        }

        private bool CanSaveTransaction()
        {
            return (TransactionEditMode == EditMode.Create) || (TransactionEditMode == EditMode.Update);
        }

        private bool CanDeleteTransaction()
        {
            return (TransactionEditMode == EditMode.Delete);
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
                    Transactions.Add(TransactionViewModelFactory.CreateTransactionViewModel(transaction));
                }
            }

            OnPropertyChanged("");            
        }

    }

}
