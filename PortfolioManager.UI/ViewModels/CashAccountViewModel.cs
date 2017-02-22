using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.UI.Utilities;
using PortfolioManager.Model.Portfolios;

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

        public override void RefreshView()
        {
            // Get opening blance
            OpeningBalance = _Parameter.Portfolio.CashAccountService.GetBalance(_Parameter.StartDate.AddDays(-1));

            decimal balance = OpeningBalance;

            // get transactions
            var transactions = _Parameter.Portfolio.CashAccountService.GetTransactions(_Parameter.StartDate, _Parameter.EndDate);

            Transactions.Clear();
            foreach (var transaction in transactions)
            {
                balance += transaction.Amount;
                var newItem = new CashAccountItemViewModel(transaction, balance);

                Transactions.Add(newItem);
            }

            ClosingBalance = balance;

            OnPropertyChanged("");
        }

    }

    class CashAccountItemViewModel
    {
        public DateTime Date { get; private set; }
        public string Description { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Balance { get; private set; }

        public CashAccountItemViewModel(CashAccountTransaction transaction, decimal balance)
        {
            Date = transaction.Date;
            Description = transaction.Description;
            Amount = transaction.Amount;
            Balance = balance;
        }
    }
}
