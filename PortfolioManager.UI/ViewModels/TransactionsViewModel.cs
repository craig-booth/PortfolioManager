using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Common;
using PortfolioManager.Service.Interface;

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
            {
                var holdingService = _Parameter.PortfolioManagerService.GetService<IHoldingService>();
                var stockService = _Parameter.PortfolioManagerService.GetService<IStockService>();
                TransactionViewModelFactory = new TransactionViewModelFactory(stockService, holdingService);
            }

            base.Activate();
        }

        public async override void RefreshView()
        {
            var transactionService = _Parameter.PortfolioManagerService.GetService<ITransactionService>();

            GetTransactionsResponce responce;
            if (_Parameter.Stock.Id == Guid.Empty)
                responce = await transactionService.GetTransactions(_Parameter.StartDate, _Parameter.EndDate);
            else
                responce = await transactionService.GetTransactions(_Parameter.Stock.Id, _Parameter.StartDate, _Parameter.EndDate);

            Transactions.Clear();

            foreach (var transaction in responce.Transactions)
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

    }

 
}
