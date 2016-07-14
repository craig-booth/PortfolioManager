using System;
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
    class TransactionSummaryViewModel : PortfolioViewModel
    {
        private IDateRangeParameter _Parameter;

        public void ParameterChange(object sender, PropertyChangedEventArgs e)
        {
            ShowReport();
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

        public ObservableCollection<TransactionViewItem> Transactions { get; private set; }

        public TransactionSummaryViewModel(string label, Portfolio portfolio, IDateRangeParameter parameter)
            : base(label, portfolio)
        {
            Options.AllowStockSelection = false;
            Options.DateSelection = DateSelectionType.Range;

            _Heading = label;
            _Parameter = parameter;

            Transactions = new ObservableCollection<TransactionViewItem>();
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
            Transactions.Clear();

            if (_Parameter == null)
            {
                OnPropertyChanged("");
                return;
            }

            var transactions = Portfolio.TransactionService.GetTransactions(_Parameter.StartDate, _Parameter.EndDate);
            foreach (var transaction in transactions)
            {
                var stock = Portfolio.StockService.Get(transaction.ASXCode, transaction.TransactionDate);

                Transactions.Add(new TransactionViewItem(stock, transaction));
            }

         
            OnPropertyChanged("");
        }

    }

    class TransactionViewItem
    {
        public string ASXCode { get; private set; }
        public string CompanyName { get; private set; }

        public DateTime TransactionDate { get; private set; }
        public string Description { get; private set; }

        public TransactionViewItem(Stock stock, Transaction transaction)
        {
            ASXCode = stock.ASXCode;
            CompanyName = string.Format("{0} ({1})", stock.Name, stock.ASXCode);

            TransactionDate = transaction.TransactionDate;

            Description = transaction.Description;
        }
    }
}
