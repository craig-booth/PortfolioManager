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
                _SelectedView = value;
                if (_SelectedView != null)
                {
                    if (_SelectedView.Options.DateSelection == DateSelectionType.Single)
                        _SelectedView.SetData(ReportViewParameter);
                    else if (_SelectedView.Options.DateSelection == DateSelectionType.Range)
                        _SelectedView.SetData(ReportViewParameter);
                    else if (_SelectedView.Options.DateSelection == DateSelectionType.FinancialYear)
                        _SelectedView.SetData(FinancialYearParameter);
                    else
                        _SelectedView.SetData(null);
                }
                OnPropertyChanged();
            }
        }
        
        private List<Module> _Modules;
        public IReadOnlyList<Module> Modules
        {
            get { return _Modules; }
        }

        public ViewParameter ReportViewParameter { get; set; }
        public FinancialYearParameter FinancialYearParameter { get; set; }

        public MainWindowViewModel()
        {
            IStockDatabase stockDatabase = new SQLiteStockDatabase(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Stocks.db"));
            IPortfolioDatabase portfolioDatabase = new SQLitePortfolioDatabase(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "portfolio.db"));

            _Portfolio = new Portfolio(portfolioDatabase, stockDatabase.StockQuery, stockDatabase.CorporateActionQuery);

            ReportViewParameter = new ViewParameter();
            FinancialYearParameter = new FinancialYearParameter(2010);
             

            _Modules = new List<Module>();

            var homeModule = new Module("Home", "HomeIcon");
            homeModule.Views.Add(new PortfolioSummaryViewModel("Summary", _Portfolio));
            _Modules.Add(homeModule);

            var reportsModule = new Module("Reports", "ReportsIcon");
            reportsModule.ViewSelectionAreaVisible = Visibility.Visible;
            reportsModule.Views.Add(new UnrealisedGainsViewModel("Unrealised Gains", _Portfolio));
            reportsModule.ViewParameterAreaVisible = Visibility.Visible;
            _Modules.Add(reportsModule);

            var taxModule = new Module("Tax", "TaxIcon");
            taxModule.ViewSelectionAreaVisible = Visibility.Visible;
            taxModule.Views.Add(new TaxableIncomeViewModel("Taxable Income", _Portfolio));
            taxModule.Views.Add(new CGTViewModel("CGT", _Portfolio));
            taxModule.ViewParameterAreaVisible = Visibility.Visible;
            _Modules.Add(taxModule);

            SelectedModule = homeModule;
        }

    }   

}
