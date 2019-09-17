using System;
using System.Collections.ObjectModel;

using CBControls;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Client;
using PortfolioManager.UI.Utilities;


namespace PortfolioManager.UI.ViewModels.Transactions
{

    class EditTransactionViewModel: PopupWindow
    {
        private RestClient _RestClient;
        private TransactionViewModelFactory _TransactionViewModelFactory;

        private TransactionViewModel _TransactionViewModel;
        public TransactionViewModel TransactionViewModel
        {
            get
            {
                return _TransactionViewModel;
            }
            private set
            {
                _TransactionViewModel = value;
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

        public EditTransactionViewModel(RestClient restClient)
            : base()
        {
            _RestClient = restClient; 
            _TransactionViewModelFactory = new TransactionViewModelFactory(restClient);

            CancelTransactionCommand = new RelayCommand(CancelTransaction);
            SaveTransactionCommand = new RelayCommand(SaveTransaction, CanSaveTransaction);
            DeleteTransactionCommand = new RelayCommand(DeleteTransaction, CanDeleteTransaction);
         }

        public void CreateTransaction(TransactionType transactionType)
        {
            TransactionViewModel = _TransactionViewModelFactory.CreateTransactionViewModel(transactionType);
            TransactionViewModel.BeginEdit();
            NewTransaction = true;

            Commands.Clear();
            Commands.Add(new DialogCommand("Save", SaveTransactionCommand));
            Commands.Add(new DialogCommand("Cancel", CancelTransactionCommand));

            Show();
        }

        public async void EditTransaction(Guid id)
        {
            TransactionViewModel = await _TransactionViewModelFactory.CreateTransactionViewModel(id);
            TransactionViewModel.BeginEdit();
            NewTransaction = false;

            Commands.Clear();
            Commands.Add(new DialogCommand("Save", SaveTransactionCommand));
            Commands.Add(new DialogCommand("Delete", DeleteTransactionCommand));
            Commands.Add(new DialogCommand("Cancel", CancelTransactionCommand));

            Show();
        }

        public RelayCommand CancelTransactionCommand { get; private set; }
        private void CancelTransaction()
        {
            if (TransactionViewModel != null)
                TransactionViewModel.CancelEdit();

            TransactionViewModel = null;
            Close();
        }

        public RelayCommand SaveTransactionCommand { get; private set; }
        private async void SaveTransaction()
        {
            if (_TransactionViewModel != null)
            {
                _TransactionViewModel.EndEdit();

                if (NewTransaction)
                {
                    _TransactionViewModel._Transaction.Id = Guid.NewGuid();
                    await _RestClient.Transactions.Add(_TransactionViewModel._Transaction);
                }
            }

            var eventArgs = new TransactionEventArgs(TransactionViewModel);

            TransactionViewModel = null;
            Close();

            OnTransactionChangedEvent(eventArgs);
        }

        private bool CanSaveTransaction()
        {
            return (TransactionViewModel != null) && (!TransactionViewModel.HasErrors);
        }

        public RelayCommand DeleteTransactionCommand { get; private set; }
        private async void DeleteTransaction()
        {
            if (TransactionViewModel != null)
            {
                _TransactionViewModel.EndEdit();

         //       await _RestWebClient.DeleteTransactionAsync(TransactionViewModel.Transaction.Id);
            }

            TransactionViewModel = null;
            Close();
        }

        private bool CanDeleteTransaction()
        {
            return (TransactionViewModel != null) && (NewTransaction == false);
        }

        public event EventHandler<TransactionEventArgs> TransactionChanged;

        private void OnTransactionChangedEvent(TransactionEventArgs e)
        {
            TransactionChanged?.Invoke(this, e);
        }

    }

    class TransactionEventArgs : EventArgs
    {
        public TransactionViewModel Transaction { get; set; }

        public TransactionEventArgs(TransactionViewModel transaction)
        {
            Transaction = transaction;
        }
    }

}
