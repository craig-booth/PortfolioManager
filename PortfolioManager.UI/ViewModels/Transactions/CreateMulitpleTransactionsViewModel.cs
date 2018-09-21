using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

using CBControls;

using PortfolioManager.RestApi.Client;
using PortfolioManager.Service.Interface;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{

    class CreateMulitpleTransactionsViewModel : PopupWindow
    {
        protected RestWebClient _RestWebClient;
        protected RestClient _RestClient;
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

        public CreateMulitpleTransactionsViewModel(RestWebClient restWebClient, RestClient restClient)
            : base()
        {
            _RestWebClient = restWebClient;
            _TransactionViewModelFactory = new TransactionViewModelFactory(restWebClient, restClient);
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
        private async void SaveTransactions()
        {
            foreach (var transactionviewModel in Transactions)
            {
                transactionviewModel.EndEdit();
                await _RestWebClient.AddTransactionAsync(transactionviewModel.Transaction);
            }      

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
