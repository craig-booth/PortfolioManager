using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using CBControls;

using PortfolioManager.Common;
using PortfolioManager.Service.Interface;
using PortfolioManager.UI.Utilities;


namespace PortfolioManager.UI.ViewModels.Transactions
{
    class PopupWindow : NotifyClass
    {
        private bool _IsOpen;
        public bool IsOpen
        {
            get
            {
                return _IsOpen;
            }
            set
            {
                if (_IsOpen != value)
                {
                    _IsOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<DialogCommand> Commands { get; private set; }

        public PopupWindow()
        {
            Commands = new ObservableCollection<DialogCommand>();
        }

        public void Show()
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
        }

    }

    class EditTransactionViewModel: PopupWindow
    { 
        private ITransactionService _TransactionService = null;
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

        public EditTransactionViewModel(RestWebClient restWebClient)
            : base()
        {
            _TransactionViewModelFactory = new TransactionViewModelFactory(restWebClient);

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

        public void EditTransaction(TransactionViewModel transactionViewModel)
        {
            if (transactionViewModel != null)
            {
                TransactionViewModel = transactionViewModel;
                TransactionViewModel.BeginEdit();
                NewTransaction = false;

                Commands.Clear();
                Commands.Add(new DialogCommand("Save", SaveTransactionCommand));
                Commands.Add(new DialogCommand("Delete", DeleteTransactionCommand));
                Commands.Add(new DialogCommand("Cancel", CancelTransactionCommand));

                Show();
            }
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
        private void SaveTransaction()
        {
            if (TransactionViewModel != null)
            {
                TransactionViewModel.EndEdit();

                if (NewTransaction)
                    _TransactionService.AddTransaction(TransactionViewModel.Transaction);
                else
                    _TransactionService.UpdateTransaction(TransactionViewModel.Transaction);
            }

            TransactionViewModel = null;
            Close();
        }

        private bool CanSaveTransaction()
        {
            return (TransactionViewModel != null) && (!TransactionViewModel.HasErrors);
        }

        public RelayCommand DeleteTransactionCommand { get; private set; }
        private void DeleteTransaction()
        {
            if (TransactionViewModel != null)
            {
                _TransactionViewModel.EndEdit();

                _TransactionService.DeleteTransaction(TransactionViewModel.Transaction.Id);
            }

            TransactionViewModel = null;
            Close();
        }

        private bool CanDeleteTransaction()
        {
            return (TransactionViewModel != null) && (NewTransaction == false);
        }


    }
}
