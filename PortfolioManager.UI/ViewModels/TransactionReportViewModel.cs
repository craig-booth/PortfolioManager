using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;

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

        public override void RefreshView()
        {
            Transactions.Clear();

           
            IReadOnlyCollection<Transaction> transactions;
            if (_Parameter.Stock.Id == Guid.Empty)
                transactions = _Parameter.Portfolio.TransactionService.GetTransactions(_Parameter.StartDate, _Parameter.EndDate);
            else
                transactions = _Parameter.Portfolio.TransactionService.GetTransactions(_Parameter.Stock.ASXCode, _Parameter.StartDate, _Parameter.EndDate);
            foreach (var transaction in transactions)
            {
                if (transaction.Type != TransactionType.CashTransaction)
                {
                    var stock = _Parameter.Portfolio.StockService.Get(transaction.ASXCode, transaction.RecordDate);
                    Transactions.Add(new TransactionReportViewItem(stock, transaction));
                }
            }

         
            OnPropertyChanged("");
        }

    }

    class TransactionReportViewItem
    {
        public string ASXCode { get; private set; }
        public string CompanyName { get; private set; }

        public DateTime TransactionDate { get; private set; }
        public string Description { get; private set; }

        public TransactionReportViewItem(Stock stock, Transaction transaction)
        {
            ASXCode = stock.ASXCode;
            CompanyName = string.Format("{0} ({1})", stock.Name, stock.ASXCode);
            
            TransactionDate = transaction.TransactionDate;

            Description = transaction.Description;
        }
    }
}
