using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using PortfolioManager.Model.Data;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Service;

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

        public SingleDateParameter SingleDateParameter { get; private set; }
        public DateRangeParameter DateRangeParameter { get; private set; }
        public FinancialYearParameter FinancialYearParameter { get; set; }

        public MainWindowViewModel()
        {
            IStockDatabase stockDatabase = new SQLiteStockDatabase(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Stocks.db"));
            IPortfolioDatabase portfolioDatabase = new SQLitePortfolioDatabase(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "portfolio.db"));

            _Portfolio = new Portfolio(portfolioDatabase, stockDatabase.StockQuery, stockDatabase.CorporateActionQuery);

            SingleDateParameter = new SingleDateParameter();
            DateRangeParameter = new DateRangeParameter();
            FinancialYearParameter = new FinancialYearParameter(2010);             

            _Modules = new List<Module>();

            var homeModule = new Module("Home", "HomeIcon");
            homeModule.Views.Add(new PortfolioSummaryViewModel("Summary", _Portfolio));
            _Modules.Add(homeModule);

            var reportsModule = new Module("Reports", "ReportsIcon");
            reportsModule.ViewSelectionAreaVisible = Visibility.Visible;
            reportsModule.ViewParameterAreaVisible = Visibility.Visible;

            reportsModule.Views.Add(new UnrealisedGainsViewModel("Unrealised Gains", _Portfolio, SingleDateParameter));
            reportsModule.Views.Add(new TransactionSummaryViewModel("Transactions", _Portfolio, DateRangeParameter));
           
            _Modules.Add(reportsModule);

            var taxModule = new Module("Tax", "TaxIcon");
            taxModule.ViewSelectionAreaVisible = Visibility.Visible;
            taxModule.ViewParameterAreaVisible = Visibility.Visible;
            taxModule.Views.Add(new TaxableIncomeViewModel("Taxable Income", _Portfolio, FinancialYearParameter));
            taxModule.Views.Add(new CGTViewModel("CGT", _Portfolio, FinancialYearParameter));
            _Modules.Add(taxModule);

            SelectedModule = homeModule;
        }

    }   

}
