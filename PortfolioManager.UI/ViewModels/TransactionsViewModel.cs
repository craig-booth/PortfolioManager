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
        public TransactionsViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Options.AllowStockSelection = true;
            Options.DateSelection = DateSelectionType.Range;

            _Heading = label;

            Transactions = new ObservableCollection<TransactionViewModel>();

            EditTransactionCommand = new RelayCommand<TransactionViewModel>(EditTransaction);
            CancelTransactionCommand = new RelayCommand(CancelTransaction);
            SaveTransactionCommand = new RelayCommand(SaveTransaction, CanSaveTransaction);
            DeleteTransactionCommand = new RelayCommand(DeleteTransaction, CanDeleteTransaction);
            AddTransactionCommand = new RelayCommand<TransactionType>(AddTransaction);
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

        private bool _NewTransaction;
        public bool NewTransaction
        {
            get
            {
                return _NewTransaction;
            }

            set
            {
                _NewTransaction = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand<TransactionViewModel> EditTransactionCommand { get; private set; }
        private void EditTransaction(TransactionViewModel transactionViewModel)
        {
            if (transactionViewModel != null)
            {
                CurrentTransactionViewModel = transactionViewModel;
                CurrentTransactionViewModel.BeginEdit();
                NewTransaction = false;
            }
        }

        public RelayCommand CancelTransactionCommand { get; private set; }
        private void CancelTransaction()
        { 
            if (CurrentTransactionViewModel != null)
                CurrentTransactionViewModel.CancelEdit();
            CurrentTransactionViewModel = null;;
        }

        public RelayCommand SaveTransactionCommand { get; private set; }
        private void SaveTransaction()
        {
            if (CurrentTransactionViewModel != null)
            {
                CurrentTransactionViewModel.EndEdit();

                var transaction = CurrentTransactionViewModel.Transaction;

                if (NewTransaction)
                {
                    _Parameter.Portfolio.TransactionService.ProcessTransaction(transaction);

                    if (MeetsSelectionCriteria(transaction))
                        Transactions.Add(CurrentTransactionViewModel);
                }
                else
                {
                    _Parameter.Portfolio.TransactionService.UpdateTransaction(transaction);

                    if (!MeetsSelectionCriteria(transaction))
                        Transactions.Remove(CurrentTransactionViewModel);
                }
            }

            CurrentTransactionViewModel = null;
        }

        public RelayCommand DeleteTransactionCommand { get; private set; }
        private void DeleteTransaction()
        {
            if (CurrentTransactionViewModel != null)
            {
                CurrentTransactionViewModel.EndEdit();

                _Parameter.Portfolio.TransactionService.DeleteTransaction(CurrentTransactionViewModel.Transaction);

                Transactions.Remove(CurrentTransactionViewModel);
            }

            CurrentTransactionViewModel = null;
        }

        public RelayCommand<TransactionType> AddTransactionCommand { get; private set; }
        private void AddTransaction(TransactionType transactionType)
        {
            CurrentTransactionViewModel = TransactionViewModelFactory.CreateTransactionViewModel(transactionType);
            CurrentTransactionViewModel.BeginEdit();
            NewTransaction = true;
        }

        private bool CanDeleteTransaction()
        {
            return (CurrentTransactionViewModel != null) && (NewTransaction == false);
        }

        private bool CanSaveTransaction()
        {
            return (CurrentTransactionViewModel != null) && (! CurrentTransactionViewModel.HasErrors);
        }

        public override void Activate()
        {
            if (_Parameter != null)
                TransactionViewModelFactory = new ViewModels.TransactionViewModelFactory(_Parameter.Portfolio.StockService);

            base.Activate();
        }

        public override void RefreshView()
        {
            CurrentTransactionViewModel = null; 

            Transactions.Clear();

            IReadOnlyCollection<Transaction> transactions;
            if (_Parameter.Stock.Id == Guid.Empty)
                transactions = _Parameter.Portfolio.TransactionService.GetTransactions(_Parameter.StartDate, _Parameter.EndDate);
            else
                transactions = _Parameter.Portfolio.TransactionService.GetTransactions(_Parameter.Stock.ASXCode, _Parameter.StartDate, _Parameter.EndDate);
            foreach (var transaction in transactions)
            {
                if (transaction.Type != TransactionType.CashTransaction)
                {
                    Transactions.Add(TransactionViewModelFactory.CreateTransactionViewModel(transaction));
                }
            }

            OnPropertyChanged("");            
        }

        private bool MeetsSelectionCriteria(Transaction transaction)
        {
            if ((transaction.TransactionDate < _Parameter.StartDate) || (transaction.TransactionDate > _Parameter.EndDate))
                return false;

            if (_Parameter.Stock.Id != Guid.Empty)
                return (transaction.ASXCode == _Parameter.Stock.ASXCode);

            return true;
        }

    }

}
