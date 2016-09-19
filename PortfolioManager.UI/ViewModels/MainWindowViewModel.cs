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

        public EditTransactionWindow EditTransactionWindow { get; private set; }
    
        public MainWindowViewModel()
        {
            _Modules = new List<Module>();

            FinancialYears = new ObservableCollection<DescribedObject<int>>();
            OwnedStocks = new ObservableCollection<Utilities.DescribedObject<Stock>>();

            ViewParameter = new ViewParameter();

            EditTransactionWindow = new EditTransactionWindow();

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
            transactionsModule.Pages.Add(new CorporateActionsViewModel("CorporateActions", ViewParameter));

            var reportsModule = new Module("Reports", "ReportsIcon")
            {
                PageSelectionAreaVisible = Visibility.Visible,
                PageParameterAreaVisible = Visibility.Visible
            };
            _Modules.Add(reportsModule);
            reportsModule.Pages.Add(new UnrealisedGainsViewModel("Unrealised Gains", ViewParameter));
            reportsModule.Pages.Add(new TransactionReportViewModel("Transactions", ViewParameter));
            reportsModule.Pages.Add(new CashAccountViewModel("Cash Summary", ViewParameter));


            var taxModule = new Module("Tax", "TaxIcon")
            {
                PageSelectionAreaVisible = Visibility.Visible,
                PageParameterAreaVisible = Visibility.Visible

            };
            _Modules.Add(taxModule);
            taxModule.Pages.Add(new TaxableIncomeViewModel("Taxable Income", ViewParameter));
            taxModule.Pages.Add(new CGTViewModel("CGT", ViewParameter));

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

            EditTransactionWindow.Portfolio = _Portfolio;
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

            var stocks = _Portfolio.ShareHoldingService.GetOwnedStocks(DateUtils.NoStartDate, DateUtils.NoEndDate, false).OrderBy(x => x.Name);
            foreach (var stock in stocks)
            {
                OwnedStocks.Add(new DescribedObject<Stock>(stock, string.Format("{0} ({1})", stock.Name, stock.ASXCode)));
            }

            ViewParameter.Stock = allCompanies;
        }

    }

    class PopupWindow: NotifyClass
    {
        private bool _IsOpen;
        public bool IsOpen
        {
            get
            {
                return _IsOpen;
            }
            set
            {
                _IsOpen = value;
                OnPropertyChanged();
            }
        }
    }

    class EditTransactionWindow : PopupWindow
    {
        private TransactionService _TransactionService;
        private TransactionViewModelFactory _TransactionViewModelFactory;

        private Portfolio _Portfolio;
        public Portfolio Portfolio
        {
            set
            {
                _Portfolio = value;

                _TransactionService = _Portfolio.TransactionService;
                _TransactionViewModelFactory = new TransactionViewModelFactory(_Portfolio.StockService, _Portfolio.ShareHoldingService);
            }
        }

        private TransactionViewModel _TransactionViewModel;
        public TransactionViewModel TransactionViewModel
        {
            get
            {
                return _TransactionViewModel;
            }
            private set
            {
                _TransactionViewModel = value;
                OnPropertyChanged();
            }
        }


        private bool _NewTransaction;
        public bool NewTransaction
        {
            get
            {
                return _NewTransaction;
            }

            set
            {
                _NewTransaction = value;
                OnPropertyChanged();
            }
        }

        public EditTransactionWindow()
            : base()
        {
            EditTransactionCommand = new RelayCommand<TransactionViewModel>(EditTransaction);
            CancelTransactionCommand = new RelayCommand(CancelTransaction);
            SaveTransactionCommand = new RelayCommand(SaveTransaction, CanSaveTransaction);
            DeleteTransactionCommand = new RelayCommand(DeleteTransaction, CanDeleteTransaction);
            AddTransactionCommand = new RelayCommand<TransactionType>(AddTransaction);
        }

        public RelayCommand<TransactionViewModel> EditTransactionCommand { get; private set; }
        private void EditTransaction(TransactionViewModel transactionViewModel)
        {
            if (transactionViewModel != null)
            {
                TransactionViewModel = transactionViewModel;
                TransactionViewModel.BeginEdit();
                NewTransaction = false;

                IsOpen = true;
            }
        }

        public RelayCommand CancelTransactionCommand { get; private set; }
        private void CancelTransaction()
        {
            if (TransactionViewModel != null)
                TransactionViewModel.CancelEdit();

            TransactionViewModel = null;
            IsOpen = false;
        }

        public RelayCommand SaveTransactionCommand { get; private set; }
        private void SaveTransaction()
        {
            if (TransactionViewModel != null)
            {
                TransactionViewModel.EndEdit();

                var transaction = TransactionViewModel.Transaction;

                if (NewTransaction)
                    _TransactionService.ProcessTransaction(transaction);
                else
                    _TransactionService.UpdateTransaction(transaction);
            }

            TransactionViewModel = null;
            IsOpen = false;
        }

        public RelayCommand DeleteTransactionCommand { get; private set; }
        private void DeleteTransaction()
        {
            if (TransactionViewModel != null)
            {
                _TransactionViewModel.EndEdit();

                _TransactionService.DeleteTransaction(TransactionViewModel.Transaction);
            }

            TransactionViewModel = null;
            IsOpen = false;
        }

        public RelayCommand<TransactionType> AddTransactionCommand { get; private set; }
        private void AddTransaction(TransactionType transactionType)
        {
            TransactionViewModel = _TransactionViewModelFactory.CreateTransactionViewModel(transactionType);
            TransactionViewModel.BeginEdit();
            NewTransaction = true;

            IsOpen = true;
        }

        private bool CanDeleteTransaction()
        {
            return (TransactionViewModel != null) && (NewTransaction == false);
        }

        private bool CanSaveTransaction()
        {
            return (TransactionViewModel != null) && (!TransactionViewModel.HasErrors);
        }


    }

}
