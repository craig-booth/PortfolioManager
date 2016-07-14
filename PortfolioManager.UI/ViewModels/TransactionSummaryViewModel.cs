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

        private ViewParameter _Parameter;
        public ViewParameter Parameter
        {
            set
            {
                _Parameter = value;

                if (_Parameter != null)
                    _Parameter.PropertyChanged += ParameterChange;

                ShowReport();
            }

            get
            {
                return _Parameter;
            }
        }

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

        public TransactionSummaryViewModel(string label, Portfolio portfolio)
            : base(label, portfolio)
        {
            Options.AllowStockSelection = false;
            Options.DateSelection = DateSelectionType.Range;

            _Heading = label;
            Transactions = new ObservableCollection<TransactionViewItem>();
        }

        public void ShowReport()
        {
            Transactions.Clear();

            if (_Parameter == null)
            {
                OnPropertyChanged("");
                return;
            }

            var transactions = Portfolio.TransactionService.GetTransactions(_Parameter.StartDate, _Parameter.EndDate);

         //   Stock currentStock = null;
         //   Guid previousStock = Guid.Empty;
         //   decimal unitPrice = 0.00m;
         //   var unrealisedGainsList = new List<UnrealisedGainViewItem>();
            foreach (var transaction in transactions)
            {
                var stock = Portfolio.StockService.Get(transaction.ASXCode, transaction.TransactionDate);

                Transactions.Add(new TransactionViewItem(stock, transaction));
            }

        //    UnrealisedGains = new ObservableCollection<UnrealisedGainViewItem>(unrealisedGainsList.OrderBy(x => x.CompanyName).ThenBy(x => x.AquisitionDate));
           
            OnPropertyChanged("");
        }

        public override void SetData(object data)
        {
            Parameter = data as ViewParameter;
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
