using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

using CBControls;

using PortfolioManager.Service.Interface;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{

    class CreateMulitpleTransactionsViewModel : PopupWindow
    {
        private ITransactionService _TransactionService = null;
        private TransactionViewModelFactory _TransactionViewModelFactory;

        public ObservableCollection<TransactionViewModel> Transactions { get; private set; }

        private TransactionViewModel _SelectedTransaction;
        public TransactionViewModel SelectedTransaction
        {
            get
            {
                return _SelectedTransaction;
            }
            set
            {
                if (_SelectedTransaction != value)
                {
                    _SelectedTransaction = value;
                    OnPropertyChanged();
                }
            }
        }

        public CreateMulitpleTransactionsViewModel(RestWebClient restWebClient)
            : base()
        {
            _TransactionViewModelFactory = new TransactionViewModelFactory(restWebClient);
            Transactions = new ObservableCollection<TransactionViewModel>();

            CancelCommand = new RelayCommand(Cancel);
            SaveTransactionsCommand = new RelayCommand(SaveTransactions, CanSaveTransactions);

            Commands.Add(new DialogCommand("Create", SaveTransactionsCommand));
            Commands.Add(new DialogCommand("Cancel", CancelCommand));
        }

        public void AddTransactions(IEnumerable<TransactionItem> transactions)
        {
            Transactions.Clear();

            foreach (var transaction in transactions)
            {
                var transactionViewModel = _TransactionViewModelFactory.CreateTransactionViewModel(transaction);
                transactionViewModel.BeginEdit();

                Transactions.Add(transactionViewModel);
            }

            SelectedTransaction = Transactions.First();

            Show();
        }
        
        public RelayCommand CancelCommand { get; private set; }
        private void Cancel()
        {
            foreach (var transactionviewModel in Transactions)
                transactionviewModel.CancelEdit();

            IsOpen = false;
        }

        public RelayCommand SaveTransactionsCommand { get; private set; }
        private void SaveTransactions()
        {
            foreach (var transactionviewModel in Transactions)
                transactionviewModel.EndEdit();

            _TransactionService.AddTransactions(Transactions.Select(x => x.Transaction));

            IsOpen = false;
        }
        
        private bool CanSaveTransactions()
        {
            foreach (var transactionviewModel in Transactions)
            {
                if (transactionviewModel.HasErrors)
                    return false;
            }

            return true;
        }


    }
}
