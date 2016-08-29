using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;


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

        public ViewParameter ViewParameter { get; set; }

        public ObservableCollection<DescribedObject<int>> FinancialYears { get; set; }
        public ObservableCollection<DescribedObject<Stock>> OwnedStocks { get; set; }

        private DateTime _PortfolioStartDate;

        private ApplicationSettings _Settings;
        public ApplicationSettings Settings
        {
            get
            {
                return _Settings;
            }
            private set
            {
                _Settings = value;
            }
        }

        public MainWindowViewModel()
        {
            _Modules = new List<Module>();

            FinancialYears = new ObservableCollection<DescribedObject<int>>();
            OwnedStocks = new ObservableCollection<Utilities.DescribedObject<Stock>>();

            ViewParameter = new ViewParameter();

            _Settings = new ApplicationSettings();
            _Settings.DatabaseChanged += LoadPortfolio;


            _Modules.Clear();
            var homeModule = new Module("Home", "HomeIcon");
            _Modules.Add(homeModule);
            homeModule.Views.Add(new PortfolioSummaryViewModel("Summary", ViewParameter));

            var transactionsModule = new Module("Transactions", "SettingsIcon")
            {
                ViewSelectionAreaVisible = Visibility.Hidden,
                ViewParameterAreaVisible = Visibility.Visible
            };
            _Modules.Add(transactionsModule);
            transactionsModule.Views.Add(new TransactionsViewModel("Transactions", ViewParameter));

            var reportsModule = new Module("Reports", "ReportsIcon")
            {
                ViewSelectionAreaVisible = Visibility.Visible,
                ViewParameterAreaVisible = Visibility.Visible
            };
            _Modules.Add(reportsModule);
            reportsModule.Views.Add(new UnrealisedGainsViewModel("Unrealised Gains", ViewParameter));
            reportsModule.Views.Add(new TransactionReportViewModel("Transactions", ViewParameter));
            reportsModule.Views.Add(new CashAccountViewModel("Cash Summary", ViewParameter));


            var taxModule = new Module("Tax", "TaxIcon")
            {
                ViewSelectionAreaVisible = Visibility.Visible,
                ViewParameterAreaVisible = Visibility.Visible

            };
            _Modules.Add(taxModule);
            taxModule.Views.Add(new TaxableIncomeViewModel("Taxable Income", ViewParameter));
            taxModule.Views.Add(new CGTViewModel("CGT", ViewParameter));

            var settingsModule = new Module("Settings", "SettingsIcon")
            {
                ViewSelectionAreaVisible = Visibility.Hidden,
                ViewParameterAreaVisible = Visibility.Hidden
            };
            _Modules.Add(settingsModule);
            settingsModule.Views.Add(new SettingsViewModel("Settings", _Settings));

            LoadPortfolio(_Settings, EventArgs.Empty);

            SelectedModule = homeModule;
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
            ViewParameter.Portfolio = _Portfolio;

            _PortfolioStartDate = _Portfolio.ShareHoldingService.GetPortfolioStartDate();

            PopulateFinancialYearList();
            PopulateStockList();
        }

        private void PopulateFinancialYearList()
        {
            FinancialYears.Clear();

            // Determinefirst and last financial years
            int currentFinancialYear = DateTime.Today.FinancialYear();
            int oldestFinancialYear = _PortfolioStartDate.FinancialYear();

            int year = currentFinancialYear;
            while (year >= oldestFinancialYear)
            {
                if (year == currentFinancialYear)
                    FinancialYears.Add(new DescribedObject<int>(year, "Current"));
                else if (year == currentFinancialYear - 1)
                    FinancialYears.Add(new DescribedObject<int>(year, "Previous"));
                else
                    FinancialYears.Add(new DescribedObject<int>(year, String.Format("{0} - {1}", year - 1, year)));

                year--;
            }

            ViewParameter.FinancialYear = currentFinancialYear;
        }

        private void PopulateStockList()
        {
            OwnedStocks.Clear();

            // Add entry to entire portfolio
            var allCompanies = new Stock(Guid.Empty, DateUtils.NoStartDate, DateUtils.NoEndDate, "", "All Companies", StockType.Ordinary, Guid.Empty);
            OwnedStocks.Add(new DescribedObject<Stock>(allCompanies, "All Companies"));

            var stocks = _Portfolio.ShareHoldingService.GetOwnedStocks(DateUtils.NoStartDate, DateUtils.NoEndDate).OrderBy(x => x.Name);
            foreach (var stock in stocks)
            {
                OwnedStocks.Add(new DescribedObject<Stock>(stock, string.Format("{0} ({1})", stock.Name, stock.ASXCode)));
            }

            ViewParameter.Stock = allCompanies;
        }



    }

}
