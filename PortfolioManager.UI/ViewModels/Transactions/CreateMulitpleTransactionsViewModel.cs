﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using CBControls;

using PortfolioManager.Service.Interface;
using PortfolioManager.Model.Portfolios;

using PortfolioManager.UI.Utilities;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.UI.ViewModels.Transactions
{

    class CreateMulitpleTransactionsViewModel : PopupWindow
    {
        private ITransactionService _TransactionService;
        private TransactionViewModelFactory _TransactionViewModelFactory;

        private IPortfolioManagerServiceFactory _PortfolioManagerService;
        public IPortfolioManagerServiceFactory PortfolioManagerService
        {
            set
            {
                _PortfolioManagerService = value;
                _TransactionService = _PortfolioManagerService.GetService<ITransactionService>();

                var holdingService = _PortfolioManagerService.GetService<IHoldingService>();
                _TransactionViewModelFactory = new TransactionViewModelFactory(_Portfolio.StockService, _Portfolio.ShareHoldingService, holdingService);
            }
        }

        private Portfolio _Portfolio;
        public Portfolio Portfolio
        {
            set
            {
                _Portfolio = value;              
            }
        }

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

        public CreateMulitpleTransactionsViewModel()
            : base()
        {
            Transactions = new ObservableCollection<TransactionViewModel>();

            CancelCommand = new RelayCommand(Cancel);
            SaveTransactionsCommand = new RelayCommand(SaveTransactions, CanSaveTransactions);

            Commands.Add(new DialogCommand("Create", SaveTransactionsCommand));
            Commands.Add(new DialogCommand("Cancel", CancelCommand));
        }

        public void AddTransactions(IEnumerable<Transaction> transactions)
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
