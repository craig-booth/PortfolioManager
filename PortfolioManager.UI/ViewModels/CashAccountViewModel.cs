using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.UI.Utilities;
using PortfolioManager.Service;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.UI.ViewModels
{
    class CashAccountViewModel : PortfolioViewModel
    {
        private IDateRangeParameter _Parameter;

        public void ParameterChange(object sender, PropertyChangedEventArgs e)
        {
            ShowReport();
        }

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

        public CashAccountViewModel(string label, Portfolio portfolio, IDateRangeParameter parameter)
            : base(label, portfolio)
        {
            Options.AllowStockSelection = false;
            Options.DateSelection = DateSelectionType.Range;

            _Parameter = parameter;

            Transactions = new ObservableCollection<CashAccountItemViewModel>();
        }

        public override void Activate()
        {
            if (_Parameter != null)
                _Parameter.PropertyChanged += ParameterChange;

            ShowReport();
        }

        public override void Deactivate()
        {
            if (_Parameter != null)
                _Parameter.PropertyChanged -= ParameterChange;
        }

        private void ShowReport()
        {
            // Get opening blance
            OpeningBalance = Portfolio.CashAccountService.GetBalance(_Parameter.StartDate);

            decimal balance = OpeningBalance;

            // get transactions
            var transactions = Portfolio.CashAccountService.GetTransactions(_Parameter.StartDate, _Parameter.EndDate);

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
