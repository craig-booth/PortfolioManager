using System;
using System.Collections.ObjectModel;

using PortfolioManager.Common;
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

            Transactions = new ObservableCollection<TransactionViewItem>();

            _EditTransactionViewModel = editTransactionViewModel;

            EditTransactionCommand = new RelayCommand<Guid>(EditTransaction);
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

        public ObservableCollection<TransactionViewItem> Transactions { get; private set; }

        private EditTransactionViewModel _EditTransactionViewModel;

        public override void Activate()
        {
            if (_Parameter != null)
            {
                TransactionViewModelFactory = new TransactionViewModelFactory(_Parameter.RestClient);
            }

            base.Activate();
        }

        public async override void RefreshView()
        {
            RestApi.Portfolios.TransactionsResponse responce;
            if (_Parameter.Stock.Id == Guid.Empty)
                responce = await _Parameter.RestClient.Portfolio.GetTransactions(_Parameter.DateRange);
            else
                responce = await _Parameter.RestClient.Holdings.GetTransactions(_Parameter.Stock.Id, _Parameter.DateRange);
            if (responce == null)
                return;

            Transactions.Clear();

            foreach (var transaction in responce.Transactions)
                Transactions.Add(new TransactionViewItem(transaction));

            OnPropertyChanged("");            
        }

        public RelayCommand<TransactionType> CreateTransactionCommand { get; private set; }
        private void CreateTransaction(TransactionType transactionType)
        {
            _EditTransactionViewModel.CreateTransaction(transactionType);
        }

        public RelayCommand<Guid> EditTransactionCommand { get; private set; }
        private void EditTransaction(Guid id)
        {
            _EditTransactionViewModel.EditTransaction(id);
        }

    }

    class TransactionViewItem
    {
        public Guid Id { get; private set; }
        public StockViewItem Stock;
        public DateTime TransactionDate { get; private set; }
        public string Description { get; private set; }

        public TransactionViewItem(RestApi.Portfolios.TransactionsResponse.TransactionItem transaction)
        {
            Id = transaction.Id;
            Stock = new StockViewItem(transaction.Stock);
            TransactionDate = transaction.TransactionDate;
            Description = transaction.Description;
        }
    }

}
