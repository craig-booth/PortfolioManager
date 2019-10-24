using System;
using System.Collections.ObjectModel;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class TransactionReportViewModel : PortfolioViewModel
    {

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

        public ObservableCollection<TransactionReportViewItem> Transactions { get; private set; }

        public TransactionReportViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Options.AllowStockSelection = true;
            Options.DateSelection = DateSelectionType.Range;

            _Heading = label;
          
            Transactions = new ObservableCollection<TransactionReportViewItem>();
        }

        public async override void RefreshView()
        {
            RestApi.Portfolios.TransactionsResponse response;
            if (_Parameter.Stock.Id == Guid.Empty)
                response = await _Parameter.RestClient.Portfolio.GetTransactions(_Parameter.DateRange);
            else
                response = await _Parameter.RestClient.Holdings.GetTransactions(_Parameter.Stock.Id, _Parameter.DateRange);
            if (response == null)
                return;

            Transactions.Clear();
            for (var i = response.Transactions.Count - 1; i >= 0; i--)
                Transactions.Add(new TransactionReportViewItem(response.Transactions[i]));
         
            OnPropertyChanged("");
        }

    }

    class TransactionReportViewItem
    {
        public StockViewItem Stock { get; private set; }

        public DateTime TransactionDate { get; private set; }
        public string Description { get; private set; }

        public TransactionReportViewItem(RestApi.Portfolios.TransactionsResponse.TransactionItem transaction)
        {
            Stock = new StockViewItem(transaction.Stock);
            TransactionDate = transaction.TransactionDate;
            Description = transaction.Description;
        }
    }
}
