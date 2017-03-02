using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;

using StockManager.Service;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Local;

using PortfolioManager.UI.Utilities;
using PortfolioManager.UI.ViewModels.Transactions;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.UI.ViewModels
{
    class MainWindowViewModel : NotifyClass
    {      
        private Portfolio _Portfolio;
        private StockServiceRepository _StockServiceRepository;

        private IPortfolioManagerServiceFactory _PortfolioManagerService;

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
                    if (_SelectedModule.Pages.Count > 0)
                        SelectedPage = _SelectedModule.Pages[0];
                }

                OnPropertyChanged();
            }
        }


        private IPageViewModel _SelectedPage;
        public IPageViewModel SelectedPage
        {
            get
            {
                return _SelectedPage;
            }
            set
            {
                if (_SelectedPage != null)
                    _SelectedPage.Deactivate();

                _SelectedPage = value;

                if (_SelectedPage != null)
                    _SelectedPage.Activate();

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
        public ObservableCollection<DescribedObject<StockItem>> OwnedStocks { get; set; }

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

        public EditTransactionViewModel EditTransactionWindow { get; private set; }
        public CreateMulitpleTransactionsViewModel CreateTransactionsWindow { get; private set; }
    
        public MainWindowViewModel()
        {
            _Modules = new List<Module>();

            FinancialYears = new ObservableCollection<DescribedObject<int>>();
            OwnedStocks = new ObservableCollection<DescribedObject<StockItem>>();

            ViewParameter = new ViewParameter();
            EditTransactionWindow = new EditTransactionViewModel();
            CreateTransactionsWindow = new CreateMulitpleTransactionsViewModel();

            _Settings = new ApplicationSettings();
            _Settings.DatabaseChanged += LoadPortfolio;

            _Modules.Clear();
            var homeModule = new Module("Home", "HomeIcon");
            _Modules.Add(homeModule);
            homeModule.Pages.Add(new PortfolioSummaryViewModel("Summary", ViewParameter));

            var transactionsModule = new Module("Transactions", "SettingsIcon")
            {
                PageSelectionAreaVisible = Visibility.Visible,
                PageParameterAreaVisible = Visibility.Visible
            };
            _Modules.Add(transactionsModule);
            transactionsModule.Pages.Add(new TransactionsViewModel("Transactions", ViewParameter, EditTransactionWindow));
            transactionsModule.Pages.Add(new CorporateActionsViewModel("CorporateActions", ViewParameter, CreateTransactionsWindow));

            var reportsModule = new Module("Reports", "ReportsIcon")
            {
                PageSelectionAreaVisible = Visibility.Visible,
                PageParameterAreaVisible = Visibility.Visible
            };
            _Modules.Add(reportsModule);
            reportsModule.Pages.Add(new UnrealisedGainsViewModel("Unrealised Gains", ViewParameter));
            reportsModule.Pages.Add(new TransactionReportViewModel("Transactions", ViewParameter));
            reportsModule.Pages.Add(new CashAccountViewModel("Cash Summary", ViewParameter));
            reportsModule.Pages.Add(new PerformanceViewModel("Performance", ViewParameter));
            reportsModule.Pages.Add(new AssetAllocationViewModel("Asset Allocation", ViewParameter));
            reportsModule.Pages.Add(new PortfolioValueViewModel("Valuation", ViewParameter));

            var taxModule = new Module("Tax", "TaxIcon")
            {
                PageSelectionAreaVisible = Visibility.Visible,
                PageParameterAreaVisible = Visibility.Visible

            };
            _Modules.Add(taxModule);
            taxModule.Pages.Add(new TaxableIncomeViewModel("Taxable Income", ViewParameter));
            taxModule.Pages.Add(new CGTViewModel("CGT", ViewParameter));
            taxModule.Pages.Add(new DetailedCGTViewModel("Detailed CGT", ViewParameter));

            var settingsModule = new Module("Settings", "SettingsIcon")
            {
                PageSelectionAreaVisible = Visibility.Hidden,
                PageParameterAreaVisible = Visibility.Hidden
            };
            _Modules.Add(settingsModule);
            settingsModule.Pages.Add(new SettingsViewModel("Settings", _Settings));

            LoadPortfolio(_Settings, EventArgs.Empty);

            SelectedModule = homeModule;
        }

        private async void LoadPortfolio(object sender, EventArgs e)
        {
            var stockDataBasePath = _Settings.StockDatabase;
            if (!Path.IsPathRooted(stockDataBasePath))
                stockDataBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, stockDataBasePath);
            IStockDatabase stockDatabase = new SQLiteStockDatabase(stockDataBasePath);

            var portfolioDatabasePath = _Settings.PortfolioDatabase;
            if (!Path.IsPathRooted(portfolioDatabasePath))
                portfolioDatabasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, portfolioDatabasePath);
            IPortfolioDatabase portfolioDatabase = new SQLitePortfolioDatabase(portfolioDatabasePath);

            _StockServiceRepository = new StockServiceRepository(stockDatabase);
            _Portfolio = new Portfolio(portfolioDatabase, _StockServiceRepository, stockDatabase.StockQuery, stockDatabase.CorporateActionQuery);
            ViewParameter.Portfolio = _Portfolio;

            _PortfolioManagerService = new LocalPortfolioManagerService(portfolioDatabase, stockDatabase);
            ViewParameter.PortfolioManagerService = _PortfolioManagerService;

            var summaryService = _PortfolioManagerService.GetService<IPortfolioSummaryService>();
            var responce = await summaryService.GetProperties();

            PopulateFinancialYearList(responce.StartDate);
            PopulateStockList(responce.StocksHeld);

            EditTransactionWindow.Portfolio = _Portfolio;
            EditTransactionWindow.PortfolioManagerService = _PortfolioManagerService;
            CreateTransactionsWindow.Portfolio = _Portfolio;
            CreateTransactionsWindow.PortfolioManagerService = _PortfolioManagerService;


            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Run(() => { _StockServiceRepository.DownloadUpdatedData(); }).ContinueWith(t => { (SelectedPage as PortfolioViewModel)?.RefreshView(); }, ui);
        }
   
        private void PopulateFinancialYearList(DateTime startDate)
        {
            FinancialYears.Clear();

            // Determinefirst and last financial years
            int currentFinancialYear = DateTime.Today.FinancialYear();
            int oldestFinancialYear = startDate.FinancialYear();

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

        private void PopulateStockList(IEnumerable<StockItem> stocks)
        {
            OwnedStocks.Clear();

            // Add entry to entire portfolio
            var allCompanies = new StockItem(Guid.Empty, "", "All Companies");
            OwnedStocks.Add(new DescribedObject<StockItem>(allCompanies, "All Companies"));

            foreach (var stock in stocks)
            {
                OwnedStocks.Add(new DescribedObject<StockItem>(stock, stock.FormattedCompanyName()));
            }

            ViewParameter.Stock = allCompanies;
        }

    }

}
