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
            _Modules = new List<Module>();
            _FinancialYears = new Dictionary<int, string>();
            _Stocks = new Dictionary<Stock, string>();

            StockParameter = new StockParameter();
            SingleDateParameter = new SingleDateParameter();
            DateRangeParameter = new DateRangeParameter();
            FinancialYearParameter = new FinancialYearParameter();

            _Settings = new ApplicationSettings();
            _Settings.DatabaseChanged += LoadPortfolio;

            LoadPortfolio(_Settings, EventArgs.Empty);
        }
        
        private void LoadPortfolio(object sender, EventArgs e)
        {
            var stockDataBasePath = _Settings.StockDatabase;
            if (!Path.IsPathRooted(stockDataBasePath))
                stockDataBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, stockDataBasePath);
            IStockDatabase stockDatabase = new SQLiteStockDatabase(stockDataBasePath);

            var portfolioDatabasePath = _Settings.PortfolioDatabase;
            if (!Path.IsPathRooted(portfolioDatabasePath))
                portfolioDatabasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, portfolioDatabasePath);
            IPortfolioDatabase portfolioDatabase = new SQLitePortfolioDatabase(portfolioDatabasePath);

            _Portfolio = new Portfolio(portfolioDatabase, stockDatabase.StockQuery, stockDatabase.CorporateActionQuery);
            _PortfolioStartDate = _Portfolio.ShareHoldingService.GetPortfolioStartDate();

            PopulateFinancialYearList();
            PopulateStockList();

            StockParameter.Stock = _Stocks.First().Key;

            _Modules.Clear();
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
