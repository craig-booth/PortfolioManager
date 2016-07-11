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

                OnPropertyChanged();

                if (_SelectedModule != null)
                {
                    if (_SelectedModule.Views.Count > 0)
                        _SelectedModule.SelectedView = _SelectedModule.Views[0];
                }

            }
        }

        private List<Module> _Modules;
        public IReadOnlyList<Module> Modules
        {
            get { return _Modules; }
        }

        public MainWindowViewModel()
        {
            IStockDatabase stockDatabase = new SQLiteStockDatabase(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Stocks.db"));
            IPortfolioDatabase portfolioDatabase = new SQLitePortfolioDatabase(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "portfolio.db"));

            _Portfolio = new Portfolio(portfolioDatabase, stockDatabase.StockQuery, stockDatabase.CorporateActionQuery);

            _Modules = new List<Module>();

            var homeModule = new Module("Home", "HomeIcon");
            homeModule.SelectedView = new PortfolioSummaryViewModel("Summary", _Portfolio);
            _Modules.Add(homeModule);

            var reportsModule = new Module("Reports", "ReportsIcon");
            reportsModule.ViewSelectionAreaVisible = Visibility.Visible;
            reportsModule.Views.Add(new UnrealisedGainsViewModel("Unrealised Gains", _Portfolio));
            reportsModule.ViewParameterAreaVisible = Visibility.Visible;
            reportsModule.ViewParameter = new DateRangeParameter();
            _Modules.Add(reportsModule);

            var taxModule = new Module("Tax", "TaxIcon");
            taxModule.ViewSelectionAreaVisible = Visibility.Visible;
            taxModule.Views.Add(new TaxableIncomeViewModel("Taxable Income", _Portfolio));
            taxModule.Views.Add(new CGTViewModel("CGT", _Portfolio));
            taxModule.ViewParameterAreaVisible = Visibility.Visible;
            taxModule.ViewParameter = new FinancialYearParameter();
            _Modules.Add(taxModule);

            SelectedModule = homeModule;
        }

    }   

}
