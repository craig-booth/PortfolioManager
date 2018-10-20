using System;
using System.Collections.ObjectModel;

using PortfolioManager.Service.Interface;
using PortfolioManager.UI.Utilities;


namespace PortfolioManager.UI.ViewModels
{
    class CashAccountViewModel : PortfolioViewModel
    {
     
        public decimal OpeningBalance { get; private set; }
        public decimal ClosingBalance { get; private set; }
        public ObservableCollection<CashAccountItemViewModel> Transactions { get; private set; }

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

        public CashAccountViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Options.AllowStockSelection = false;
            Options.DateSelection = DateSelectionType.Range;
            
            Transactions = new ObservableCollection<CashAccountItemViewModel>();
        }

        public async override void RefreshView()
        {
            var responce = await _Parameter.RestWebClient.GetCashAccountTransactionsAsync(_Parameter.DateRange.FromDate, _Parameter.DateRange.ToDate);
            if (responce == null)
                return;

            OpeningBalance = responce.OpeningBalance;
            ClosingBalance = responce.ClosingBalance;

            Transactions.Clear();
            foreach (var transaction in responce.Transactions)
                Transactions.Add(new CashAccountItemViewModel(transaction));


            OnPropertyChanged("");
        }

    }

    class CashAccountItemViewModel
    {
        public DateTime Date { get; private set; }
        public string Description { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Balance { get; private set; }

        public CashAccountItemViewModel(CashAccountTransactionItem transaction)
        {
            Date = transaction.Date;
            Description = transaction.Description;
            Amount = transaction.Amount;
            Balance = transaction.Balance;
        }
    }
}
