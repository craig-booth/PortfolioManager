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
    class TransactionsViewModel : PortfolioViewModel
    {
        private IStockParameter _StockParameter;
        private IDateRangeParameter _DateParameter;

        public void ParameterChange(object sender, PropertyChangedEventArgs e)
        {
            ShowTransactions();
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

        private TransactionViewItem _SelectedTransactionViewItem;
        public TransactionViewItem SelectedTransactionViewItem
        {
            get
            {
                return _SelectedTransactionViewItem;
            }
            set
            {
                _SelectedTransactionViewItem = value;
                
                OnPropertyChanged();
            }
        }

        public TransactionViewModel SelectedTransaction { get; private set; }        

        public List<string> TransactionTypes { get; private set; }

        public TransactionsViewModel(string label, Portfolio portfolio, IStockParameter stockParameter, IDateRangeParameter dateParameter)
            : base(label, portfolio)
        {
            Options.AllowStockSelection = true;
            Options.DateSelection = DateSelectionType.Range;

            _Heading = label;
            _StockParameter = stockParameter;
            _DateParameter = dateParameter;

            Transactions = new ObservableCollection<TransactionViewItem>();
            TransactionTypes = new List<string>();

            TransactionTypes.Add("Aquisition");
            TransactionTypes.Add("Disposal");
            TransactionTypes.Add("Income");
        }

        public override void Activate()
        {
            if (_DateParameter != null)
                _DateParameter.PropertyChanged += ParameterChange;

            if (_StockParameter != null)
                _StockParameter.PropertyChanged += ParameterChange;

            ShowTransactions();
        }

        public override void Deactivate()
        {
            if (_DateParameter != null)
                _DateParameter.PropertyChanged -= ParameterChange;

            if (_StockParameter != null)
                _StockParameter.PropertyChanged -= ParameterChange;
        }

        private void ShowTransactions()
        {
            Transactions.Clear();

            if ((_StockParameter == null) || (_DateParameter == null))
            {
                OnPropertyChanged("");
                return;
            }

            IReadOnlyCollection<Transaction> transactions;
            if (_StockParameter.Stock.Id == Guid.Empty)
                transactions = Portfolio.TransactionService.GetTransactions(_DateParameter.StartDate, _DateParameter.EndDate);
            else
                transactions = Portfolio.TransactionService.GetTransactions(_StockParameter.Stock.ASXCode, _DateParameter.StartDate, _DateParameter.EndDate);
            foreach (var transaction in transactions)
            {
                if (transaction.Type != TransactionType.CashTransaction)
                {
                    var stock = Portfolio.StockService.Get(transaction.ASXCode, transaction.RecordDate);
                    Transactions.Add(new TransactionViewItem(stock, transaction));
                }
            }


            OnPropertyChanged("");
        }

    }

    class TransactionViewItem
    {
        public Transaction Transaction { get; private set; } 
        public string ASXCode { get; private set; }
        public string CompanyName { get; private set; }

        public DateTime TransactionDate { get; private set; }
        public string Description { get; private set; }

        public TransactionViewItem(Stock stock, Transaction transaction)
        {
            Transaction = transaction;
            ASXCode = stock.ASXCode;
            CompanyName = string.Format("{0} ({1})", stock.Name, stock.ASXCode);

            TransactionDate = transaction.TransactionDate;

            Description = transaction.Description;
        }
    }

    abstract class TransactionViewModel
    {
        public DateTime TransactionDate { get; set; }
        public string ASXCode { get; set; }
        public DateTime RecordDate { get; set; }
        public string Comment { get; set; }

        public TransactionViewModel(Transaction transaction)
        {
            TransactionDate = transaction.TransactionDate;
            ASXCode = transaction.ASXCode;
            RecordDate = transaction.RecordDate;
            Comment = transaction.Comment;
        }
    }

    class AquisitionViewModel : TransactionViewModel
    {
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public bool CreateCashTransaction { get; set; }

        public AquisitionViewModel(Aquisition aquisition)
            : base(aquisition)
        {
            Units = aquisition.Units;
            AveragePrice = aquisition.AveragePrice;
            TransactionCosts = aquisition.TransactionCosts;
            CreateCashTransaction = aquisition.CreateCashTransaction;
        }
    }
}
