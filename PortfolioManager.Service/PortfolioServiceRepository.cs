using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Service
{

    public class PortfolioServiceRepository
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly IStockQuery _StockQuery;
        private readonly ICorporateActionQuery _CorporateActionQuery;

        public StockService StockService { get; private set; }
        public StockPriceService StockPriceService { get; private set; }
      
        public PortfolioServiceRepository(IPortfolioDatabase portfolioDatabase, IStockQuery stockQuery, ICorporateActionQuery corporateActionQuery)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockQuery = stockQuery;
            _CorporateActionQuery = corporateActionQuery;

            StockService = new StockService(stockQuery);
            StockPriceService = new StockPriceService(stockQuery);                       
        }

        public Portfolio CreatePortfolio(string name)
        {
            Portfolio portfolio = new Portfolio(name, _PortfolioDatabase, StockService, StockPriceService, _CorporateActionQuery);

            /* Load transactions */
            var allTransactions = portfolio.TransactionService.GetTransactions(DateTime.MinValue, DateTime.MaxValue);
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                foreach (var transaction in allTransactions)
                    portfolio.TransactionService.ApplyTransaction(unitOfWork, transaction);
                unitOfWork.Save();
            }

            return portfolio;
        }
        
    }

}
