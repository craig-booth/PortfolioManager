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
            var transactionService = _Parameter.PortfolioManagerService.GetService<ITransactionService>();

            GetTransactionsResponce responce;
            if (_Parameter.Stock.Id == Guid.Empty)
                responce = await transactionService.GetTransactions(_Parameter.StartDate, _Parameter.EndDate);
            else
                responce = await transactionService.GetTransactions(_Parameter.Stock.ASXCode, _Parameter.StartDate, _Parameter.EndDate);

            Transactions.Clear();
            foreach (var transaction in responce.Transactions)
            {
                if (transaction.Type != TransactionType.CashTransaction)
                    Transactions.Add(new TransactionReportViewItem(transaction));
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

        public TransactionReportViewItem(TransactionItem transaction)
        {
            ASXCode = transaction.Stock.ASXCode;
            CompanyName = transaction.Stock.FormattedCompanyName();
            
            TransactionDate = transaction.TransactionDate;

            Description = transaction.Description;
        }
    }
}
