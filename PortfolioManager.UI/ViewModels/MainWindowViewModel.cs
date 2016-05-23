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

        private List<MenuItem> _Menu;
        public IReadOnlyList<MenuItem> Menu
        {
            get { return _Menu; }
        }

        public MainWindowViewModel()
        {
            IStockDatabase stockDatabase = new SQLiteStockDatabase(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Stocks.db"));
            IPortfolioDatabase portfolioDatabase = new SQLitePortfolioDatabase(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "portfolio.db"));

            _Portfolio = new Portfolio(portfolioDatabase, stockDatabase.StockQuery, stockDatabase.CorporateActionQuery);


            _Menu = new List<MenuItem>();
            _Menu.Add(new MenuItem("Portfolio Summary", "PortfolioSummary", _Portfolio));
            _Menu.Add(new MenuItem("Transactions", "TransactionSummary", _Portfolio));
            _Menu.Add(new MenuItem("Taxable Income", "TaxableIncome", ReportParmeter.FinancialYear(2016)));
            _Menu.Add(new MenuItem("CGT", "CGT", ReportParmeter.FinancialYear(2016)));

            var navigator = Application.Current.FindResource("ViewNavigator") as ViewNavigator;
            navigator.SetPortfolio(_Portfolio);
            navigator.NavigateTo(new ViewWithData("PortfolioSummary", _Portfolio));
        }

    }   

    class MenuItem
    {
        public string Heading { get; private set; }
        public ViewWithData View { get; private set; }

        public MenuItem(string heading, string viewName, object data)
        {
            Heading = heading;
            View = new ViewWithData(viewName, data);
        }
    }

}
