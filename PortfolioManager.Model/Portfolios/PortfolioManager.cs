using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{

    public class PortfolioManager
    {
        private IStockDatabase _StockDatabase;
        private IPortfolioDatabase _PortfolioDatabase;

        public StockManager StockManager { get; private set; }

        public IReadOnlyCollection<Portfolio> Portfolios
        {
            get
            {
                return _PortfolioDatabase.PortfolioQuery.GetAllPortfolios();
            }
        }

        public Portfolio CreatePortfolio(string name)
        {
            Portfolio portfolio = new Portfolio(name, _PortfolioDatabase, _StockDatabase);

            _PortfolioDatabase.CreateUnitOfWork().PortfolioRepository.Add(portfolio);

            return portfolio;
        }

        public void DeletePortfolio(Portfolio portfolio)
        {
            _PortfolioDatabase.CreateUnitOfWork().PortfolioRepository.Delete(portfolio);
        }

        public PortfolioManager(IStockDatabase stockDatabase, IPortfolioDatabase portfolioDatabase)
        {
            _StockDatabase = stockDatabase;
            _PortfolioDatabase = portfolioDatabase;
            StockManager = new StockManager(_StockDatabase); 
        }
    }

}
