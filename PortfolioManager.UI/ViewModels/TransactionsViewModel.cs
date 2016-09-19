﻿using System;
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
        public TransactionsViewModel(string label, ViewParameter parameter, EditTransactionWindow editTransactionWindow)
            : base(label, parameter)
        {
            Options.AllowStockSelection = true;
            Options.DateSelection = DateSelectionType.Range;

            _Heading = label;

            Transactions = new ObservableCollection<TransactionViewModel>();

            EditTransactionWindow = editTransactionWindow;
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
        
        public EditTransactionWindow EditTransactionWindow { get; private set; }

        public override void Activate()
        {
            if (_Parameter != null)
                TransactionViewModelFactory = new ViewModels.TransactionViewModelFactory(_Parameter.Portfolio.StockService, _Parameter.Portfolio.ShareHoldingService);

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
