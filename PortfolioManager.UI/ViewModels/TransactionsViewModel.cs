﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Model.Portfolios;

using PortfolioManager.UI.Utilities;
using PortfolioManager.UI.ViewModels.Transactions;

namespace PortfolioManager.UI.ViewModels
{
    class TransactionsViewModel : PortfolioViewModel
    {
        public TransactionsViewModel(string label, ViewParameter parameter, EditTransactionViewModel editTransactionViewModel)
            : base(label, parameter)
        {
            Options.AllowStockSelection = true;
            Options.DateSelection = DateSelectionType.Range;

            _Heading = label;

            Transactions = new ObservableCollection<TransactionViewModel>();

            _EditTransactionViewModel = editTransactionViewModel;

            EditTransactionCommand = new RelayCommand<TransactionViewModel>(EditTransaction);
            CreateTransactionCommand = new RelayCommand<TransactionType>(CreateTransaction);
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

        private EditTransactionViewModel _EditTransactionViewModel;

        public override void Activate()
        {
            if (_Parameter != null)
                TransactionViewModelFactory = new TransactionViewModelFactory(_Parameter.Portfolio.StockService, _Parameter.Portfolio.ShareHoldingService);

            base.Activate();
        }

        public override void RefreshView()
        {
            Transactions.Clear();

            IReadOnlyCollection<Transaction> transactions;
            if (_Parameter.Stock.Id == Guid.Empty)
                transactions = _Parameter.Portfolio.TransactionService.GetTransactions(_Parameter.StartDate, _Parameter.EndDate);
            else
                transactions = _Parameter.Portfolio.TransactionService.GetTransactions(_Parameter.Stock.ASXCode, _Parameter.StartDate, _Parameter.EndDate);
            foreach (var transaction in transactions)
                   Transactions.Add(TransactionViewModelFactory.CreateTransactionViewModel(transaction));

            OnPropertyChanged("");            
        }

        public RelayCommand<TransactionType> CreateTransactionCommand { get; private set; }
        private void CreateTransaction(TransactionType transactionType)
        {
            _EditTransactionViewModel.CreateTransaction(transactionType);
        }

        public RelayCommand<TransactionViewModel> EditTransactionCommand { get; private set; }
        private void EditTransaction(TransactionViewModel transactionViewModel)
        {
            _EditTransactionViewModel.EditTransaction(transactionViewModel);
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
