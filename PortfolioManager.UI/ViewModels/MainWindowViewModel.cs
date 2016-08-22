using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Service;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Utils;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class MainWindowViewModel : NotifyClass
    {
        private Portfolio _Portfolio;

        private Module _SelectedModule;
        public Module SelectedModule
        {
            get
            {
                return _SelectedModule;
            }
            set
            {
                _SelectedModule = value;

                if (_SelectedModule != null)
                {
                    if (_SelectedModule.Views.Count > 0)
                        SelectedView = _SelectedModule.Views[0];
                }

                OnPropertyChanged();
            }
        }


        private IViewModel _SelectedView;
        public IViewModel SelectedView
        {
            get
            {
                return _SelectedView;
            }
            set
            {
                if (_SelectedView != null)
                    _SelectedView.Deactivate();

                _SelectedView = value;

                if (_SelectedView != null)
                    _SelectedView.Activate();

                OnPropertyChanged();
            }
        }
        
        private List<Module> _Modules;
        public IReadOnlyList<Module> Modules
        {
            get { return _Modules; }
        }

        public StockParameter StockParameter { get; private set; }
        public SingleDateParameter SingleDateParameter { get; private set; }
        public DateRangeParameter DateRangeParameter { get; private set; }
        public FinancialYearParameter FinancialYearParameter { get; set; }

        private Dictionary<int, string> _FinancialYears;
        public IReadOnlyDictionary<int, string> FinancialYears
        {
            get
            {
                return _FinancialYears;
            }
        }

        private Dictionary<Stock, string> _Stocks;
        public IReadOnlyDictionary<Stock, string> Stocks
        {
            get
            {
                return _Stocks;
            }
        }

        private DateTime _PortfolioStartDate;

        private ApplicationSettings _Settings;
        public ApplicationSettings Settings
        {
            get
            {
                return _Settings;
            }
            set
            {
                _Settings = value;
            }
        }

        public MainWindowViewModel()
        {
            _Settings = new ApplicationSettings();

            var stockDataBasePath = _Settings.StockDatabase;
            if (!Path.IsPathRooted(stockDataBasePath))
                stockDataBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, stockDataBasePath);
            IStockDatabase stockDatabase = new SQLiteStockDatabase(stockDataBasePath);

            var portfolioDatabasePath = _Settings.PortfolioDatabase;
            if (!Path.IsPathRooted(portfolioDatabasePath))
                portfolioDatabasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, portfolioDatabasePath);
            _Settings.PortfolioDatabase = portfolioDatabasePath;
            IPortfolioDatabase portfolioDatabase = new SQLitePortfolioDatabase(portfolioDatabasePath);

            _Portfolio = new Portfolio(portfolioDatabase, stockDatabase.StockQuery, stockDatabase.CorporateActionQuery);
            _PortfolioStartDate = _Portfolio.ShareHoldingService.GetPortfolioStartDate();

            _Modules = new List<Module>();

            _FinancialYears = new Dictionary<int, string>();
            PopulateFinancialYearList();

            _Stocks = new Dictionary<Stock, string>();
            PopulateStockList();

            StockParameter = new StockParameter();
            StockParameter.Stock = _Stocks.First().Key;
            SingleDateParameter = new SingleDateParameter();
            DateRangeParameter = new DateRangeParameter();
            FinancialYearParameter = new FinancialYearParameter();


            var homeModule = new Module("Home", "HomeIcon");
            _Modules.Add(homeModule);
            homeModule.Views.Add(new PortfolioSummaryViewModel("Summary", _Portfolio));
            

            var transactionsModule = new Module("Transactions", "SettingsIcon")
            {
                ViewSelectionAreaVisible = Visibility.Hidden,
                ViewParameterAreaVisible = Visibility.Visible
            };
            _Modules.Add(transactionsModule);
            transactionsModule.Views.Add(new TransactionsViewModel("Transactions", _Portfolio, StockParameter, DateRangeParameter));


            var reportsModule = new Module("Reports", "ReportsIcon")
            {
                ViewSelectionAreaVisible = Visibility.Visible,
                ViewParameterAreaVisible = Visibility.Visible
            };
            _Modules.Add(reportsModule);
            reportsModule.Views.Add(new UnrealisedGainsViewModel("Unrealised Gains", _Portfolio, StockParameter, SingleDateParameter));
            reportsModule.Views.Add(new TransactionReportViewModel("Transactions", _Portfolio, StockParameter, DateRangeParameter));
            reportsModule.Views.Add(new CashAccountViewModel("Cash Summary", _Portfolio, DateRangeParameter));
           
           
            var taxModule = new Module("Tax", "TaxIcon")
            {
                ViewSelectionAreaVisible = Visibility.Visible,
                ViewParameterAreaVisible = Visibility.Visible

            };   
            _Modules.Add(taxModule);
            taxModule.Views.Add(new TaxableIncomeViewModel("Taxable Income", _Portfolio, FinancialYearParameter));
            taxModule.Views.Add(new CGTViewModel("CGT", _Portfolio, FinancialYearParameter));

            var settingsModule = new Module("Settings", "SettingsIcon")
            {
                ViewSelectionAreaVisible = Visibility.Hidden,
                ViewParameterAreaVisible = Visibility.Hidden
            };
            _Modules.Add(settingsModule);
            settingsModule.Views.Add(new SettingsViewModel("Settings", _Settings));

            SelectedModule = homeModule;
        }

        private void AddCashTransactions()
        {
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2009, 06, 18), "Opening Balance", 12086.04m + 1310.40m + 176.53m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2009, 08, 31), "NAB share purchase plan", 2902.50m);
            AddCashTransaction(CashAccountTransactionType.Interest, new DateTime(2009, 10, 01), "Interest", 1.05m);
            AddCashTransaction(CashAccountTransactionType.Interest, new DateTime(2012, 01, 01), "Interest", 3.73m);
            AddCashTransaction(CashAccountTransactionType.Interest, new DateTime(2013, 10, 01), "Interest", 1.21m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2014, 09, 19), "Regular Deposit", 2000.00m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2014, 09, 22), "Regular Deposit", 1000.00m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2014, 10, 23), "Regular Deposit", 1000.00m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2014, 11, 24), "Regular Deposit", 1000.00m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2014, 12, 22), "Regular Deposit", 1000.00m);
            AddCashTransaction(CashAccountTransactionType.Interest, new DateTime(2015, 01, 01), "Interest", 1.41m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2015, 01, 22), "Regular Deposit", 1000.00m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2015, 02, 23), "Regular Deposit", 1000.00m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2015, 03, 23), "Regular Deposit", 1000.00m);
            AddCashTransaction(CashAccountTransactionType.Interest, new DateTime(2015, 04, 01), "Interest", 0.27m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2015, 04, 22), "Regular Deposit", 1000.00m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2015, 05, 22), "Regular Deposit", 1000.00m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2015, 06, 22), "Regular Deposit", 1000.00m);
            AddCashTransaction(CashAccountTransactionType.Interest, new DateTime(2015, 07, 01), "Interest", 1.63m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2015, 06, 22), "Regular Deposit", 1000.00m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2015, 07, 22), "Regular Deposit", 1000.00m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2015, 08, 24), "Regular Deposit", 1000.00m);
            AddCashTransaction(CashAccountTransactionType.Interest, new DateTime(2015, 10, 01), "Interest", 0.89m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2015, 10, 22), "Regular Deposit", 1000.00m);
            AddCashTransaction(CashAccountTransactionType.Deposit, new DateTime(2015, 11, 23), "Regular Deposit", 1000.00m);
        }

        private void AddCashTransaction(CashAccountTransactionType type, DateTime date, string description, decimal amount)
        {
            var transaction = new CashTransaction()
            {
                CashTransactionType = type,
                TransactionDate = date,
                Comment = description,
                Amount = amount
            };
            _Portfolio.TransactionService.ProcessTransaction(transaction);

        }

        private void PopulateFinancialYearList()
        {
            _FinancialYears.Clear();

            // Determine current financial year
            int currentFinancialYear;
            if (DateTime.Today.Month <= 6)
                currentFinancialYear = DateTime.Today.Year;
            else
                currentFinancialYear = DateTime.Today.Year + 1;

            int year = currentFinancialYear;
            while (year >= 2010)
            {
                if (year == currentFinancialYear)
                    _FinancialYears.Add(year, "Current");
                else if (year == currentFinancialYear - 1)
                    _FinancialYears.Add(year, "Previous");
                else
                    _FinancialYears.Add(year, string.Format("{0} - {1}", year - 1, year));

                year--;
            }
        }

        private void PopulateStockList()
        {
            _Stocks.Clear();

            // Add entry to entire portfolio
            var dummyStock = new Stock(Guid.Empty, DateTimeConstants.NoStartDate, DateTimeConstants.NoEndDate, "", "All Stocks", StockType.Ordinary, Guid.Empty);
            _Stocks.Add(dummyStock, " ");

            var stocks = _Portfolio.ShareHoldingService.GetOwnedStocks(DateTimeConstants.NoStartDate, DateTimeConstants.NoEndDate).OrderBy(x => x.Name);
            foreach (var stock in stocks)
            {
                _Stocks.Add(stock, string.Format("{0} ({1})", stock.Name, stock.ASXCode));
            }

        }



    }

}
