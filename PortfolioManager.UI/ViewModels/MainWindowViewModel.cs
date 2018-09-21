using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Client;
using PortfolioManager.Service.Interface;

using PortfolioManager.UI.Utilities;
using PortfolioManager.UI.ViewModels.Transactions;

namespace PortfolioManager.UI.ViewModels
{
    class MainWindowViewModel : NotifyClass
    {
        private readonly StockItem _AllCompanies = new StockItem(Guid.Empty, "", "All Companies");

        private RestWebClient _RestWebClient;
        private RestClient _RestClient;

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

#if DEBUG 
            var url = "https://docker.local:8443";
          //  url = "http://localhost";
            var apiKey = new Guid("B34A4C8B-6B17-4E25-A3CC-2E512D5F1B3D");
#else
            var url = "https://portfolio.boothfamily.id.au";
            var apiKey = new Guid("B34A4C8B-6B17-4E25-A3CC-2E512D5F1B3D");
#endif
            _RestWebClient = new RestWebClient(url, apiKey);
            _RestClient = new RestClient(url, apiKey);

            ViewParameter = new ViewParameter();
            ViewParameter.Stock = _AllCompanies;
            ViewParameter.FinancialYear = DateTime.Today.FinancialYear();
            ViewParameter.RestWebClient = _RestWebClient;
            ViewParameter.RestClient = _RestClient;

            EditTransactionWindow = new EditTransactionViewModel(_RestWebClient, _RestClient);
            CreateTransactionsWindow = new CreateMulitpleTransactionsViewModel(_RestWebClient, _RestClient);

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
            var stockDatabasePath = _Settings.StockDatabase;
            if (!Path.IsPathRooted(stockDatabasePath))
                stockDatabasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, stockDatabasePath);

            var portfolioDatabasePath = _Settings.PortfolioDatabase;
            if (!Path.IsPathRooted(portfolioDatabasePath))
                portfolioDatabasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, portfolioDatabasePath);

            var responce = await _RestWebClient.GetPortfolioPropertiesAsync();
          
            PopulateFinancialYearList(responce.StartDate);
            PopulateStockList(responce.StocksHeld);
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
            OwnedStocks.Add(new DescribedObject<StockItem>(_AllCompanies, "All Companies"));

            foreach (var stock in stocks.OrderBy(x => x.FormattedCompanyName()))
            {
                OwnedStocks.Add(new DescribedObject<StockItem>(stock, stock.FormattedCompanyName()));
            }

            ViewParameter.Stock = _AllCompanies;
        }

    }

}
