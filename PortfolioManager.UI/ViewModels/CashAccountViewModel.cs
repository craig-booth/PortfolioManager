using System;
using System.Linq;
using System.Collections.ObjectModel;

using PortfolioManager.RestApi.Portfolios;
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
            var response = await _Parameter.RestClient.Portfolio.GetCashAccount(_Parameter.DateRange);
            if (response == null)
                return;

            OpeningBalance = response.OpeningBalance;
            ClosingBalance = response.ClosingBalance;

            Transactions.Clear();
            for (var i = response.Transactions.Count - 1; i >= 0; i--)
                Transactions.Add(new CashAccountItemViewModel(response.Transactions[i]));

            OnPropertyChanged("");
        }

    }

    class CashAccountItemViewModel
    {
        public DateTime Date { get; private set; }
        public string Description { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Balance { get; private set; }

        public CashAccountItemViewModel(CashAccountTransactionsResponse.CashTransactionItem transaction)
        {
            Date = transaction.Date;
            Description = transaction.Description;
            Amount = transaction.Amount;
            Balance = transaction.Balance;
        }
    }
}
