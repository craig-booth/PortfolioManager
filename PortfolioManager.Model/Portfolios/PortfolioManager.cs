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
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly IStockQuery _StockQuery;
        private readonly ICorporateActionQuery _CorporateActionQuery;

        public StockService StockService { get; private set; }

        public IReadOnlyCollection<Portfolio> Portfolios
        {
            get
            {
                return _PortfolioDatabase.PortfolioQuery.GetAllPortfolios();
            }
        }

        public Portfolio CreatePortfolio(string name)
        {
            Portfolio portfolio = new Portfolio(name, _PortfolioDatabase, _StockQuery, _CorporateActionQuery);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.PortfolioRepository.Add(portfolio);
                unitOfWork.Save();
            }

            return portfolio;
        }

        public void DeletePortfolio(Portfolio portfolio)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.PortfolioRepository.Delete(portfolio);
                unitOfWork.Save();
            }
        }

        public PortfolioManager(IPortfolioDatabase portfolioDatabase, IStockQuery stockQuery, ICorporateActionQuery corporateActionQuery)
        {
             _PortfolioDatabase = portfolioDatabase;
            _StockQuery = stockQuery;
            _CorporateActionQuery = corporateActionQuery;

            StockService = new StockService(stockQuery);
        }
    }

}
