using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Client;

using PortfolioManager.UI.Utilities;
using PortfolioManager.UI.ViewModels.Transactions;

namespace PortfolioManager.UI.ViewModels
{
    class MainWindowViewModel : NotifyClass
    {
        private readonly StockViewItem _AllCompanies = new StockViewItem(Guid.Empty, "", "All Companies");

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

        public DateRange PortfolioDateRange { get; private set; }

        public ViewParameter ViewParameter { get; set; }

        public ObservableCollection<DescribedObject<int>> FinancialYears { get; set; }
        public ObservableCollection<DescribedObject<StockViewItem>> OwnedStocks { get; set; }

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
            OwnedStocks = new ObservableCollection<DescribedObject<StockViewItem>>();

#if DEBUG 
           // var url = "https://docker.local:8443";
            // var url = "http://localhost";
            var url = "https://portfolio.boothfamily.id.au";
            var apiKey = new Guid("B34A4C8B-6B17-4E25-A3CC-2E512D5F1B3D");
#else
            var url = "https://portfolio.boothfamily.id.au";
            var apiKey = new Guid("B34A4C8B-6B17-4E25-A3CC-2E512D5F1B3D");
#endif
            var portfolioId = new Guid("5D5DE669-726C-4C5D-BB2E-6520C924DB90");
            _RestClient = new RestClient(url, apiKey, portfolioId);

            ViewParameter = new ViewParameter
            {
                Stock = _AllCompanies,
                FinancialYear = DateTime.Today.FinancialYear(),
                RestClient = _RestClient
            };

            EditTransactionWindow = new EditTransactionViewModel(_RestClient);
            EditTransactionWindow.TransactionChanged += HandleTransactionChanged;
            CreateTransactionsWindow = new CreateMulitpleTransactionsViewModel(_RestClient);

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
                PageSelectionAreaVisible = Visibility.Visible,
                PageParameterAreaVisible = Visibility.Hidden
            };
            _Modules.Add(settingsModule);
            settingsModule.Pages.Add(new PortfolioSettingsViewModel("Portfolio", ViewParameter));
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

            await UpdatePortfolioProperties();
        }
   
        private async Task UpdatePortfolioProperties()
        {
            var response = await _RestClient.Portfolio.GetProperties();

            PortfolioDateRange = new DateRange(response.StartDate, response.EndDate);

            PopulateFinancialYearList();
            PopulateStockList(response.Holdings.Select(x => x.Stock));
        }
            
        private void PopulateFinancialYearList()
        {
            // Determinefirst and last financial years
            var currentFinancialYear = DateTime.Today.FinancialYear();
            var oldestFinancialYear = PortfolioDateRange.FromDate.FinancialYear();

            if (FinancialYears.Count == 0)
            {
                FinancialYears.Add(new DescribedObject<int>(currentFinancialYear, "Current"));
                FinancialYears.Add(new DescribedObject<int>(currentFinancialYear - 1, "Previous"));

                ViewParameter.FinancialYear = currentFinancialYear;

                return;
            }

            if (PortfolioDateRange.FromDate == DateUtils.NoStartDate)
                return;

            var lastYearInList = FinancialYears.Last().Value;
            if (lastYearInList > oldestFinancialYear)
            {
                var year = lastYearInList - 1;
                while (year > oldestFinancialYear)
                {
                    FinancialYears.Add(new DescribedObject<int>(year, String.Format("{0} - {1}", year - 1, year)));
                    year--;
                }
            }
            else if (lastYearInList < oldestFinancialYear)
            {
                var year = lastYearInList;
                while (year > oldestFinancialYear)
                {
                    FinancialYears.RemoveAt(FinancialYears.Count - 1);
                    year++;
                }
            }            
        }

        private void PopulateStockList(IEnumerable<RestApi.Portfolios.Stock> stocks)
        {
            if (OwnedStocks.Count == 0)
            {
                // Add entry to entire portfolio
                OwnedStocks.Add(new DescribedObject<StockViewItem>(_AllCompanies, "All Companies"));

                ViewParameter.Stock = _AllCompanies;
            }

            foreach (var stock in stocks.Select(x => new StockViewItem(x)))
            {
                var index = 1;
                for (var i = OwnedStocks.Count - 1; i >= 1; i--)
                {
                    if (stock.FormattedCompanyName.CompareTo(OwnedStocks[i].Value.FormattedCompanyName) > 0)
                    {
                        index = i + 1; 
                        break;
                    }
                }

                OwnedStocks.Insert(index, new DescribedObject<StockViewItem>(stock, stock.FormattedCompanyName));
            }            
        }

        private async void HandleTransactionChanged(object sender, TransactionEventArgs e)
        {
            var transaction = e.Transaction;

            if ((PortfolioDateRange.FromDate == DateUtils.NoStartDate) || 
                !PortfolioDateRange.Contains(transaction.TransactionDate) ||
                ! OwnedStocks.Any(x => x.Value.Id == transaction.Stock.Id))
            {
                await UpdatePortfolioProperties();
                SelectedPage.Activate();
            }
        }

    }

}
