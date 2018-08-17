using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Data;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.CorporateActions;
using PortfolioManager.Service.Utils;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.CorporateActions;

namespace PortfolioManager.Service.Services
{
    public class CorporateActionService : ICorporateActionService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly StockExchange _StockExchange;
        private readonly IMapper _Mapper;

        public CorporateActionService(IPortfolioDatabase portfolioDatabase, StockExchange stockExchange, IMapper mapper)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockExchange = stockExchange;
            _Mapper = mapper;
        }

        public Task<UnappliedCorporateActionsResponce> GetUnappliedCorporateActions()
        {
            var responce = new UnappliedCorporateActionsResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                var corporateActionHandlerFactory = new CorporateActionHandlerFactory(portfolioUnitOfWork.PortfolioQuery);

                // Get a list of all stocks held
                var allOwnedStocks = portfolioUnitOfWork.PortfolioQuery.GetStocksOwned(DateTime.Today, DateTime.Today);

                var actions = new List<CorporateAction>();
                foreach (var ownedStock in allOwnedStocks)
                {
                    var stock = _StockExchange.Stocks.Get(ownedStock);

                    foreach (var corporateAction in stock.CorporateActions)
                    {
                        if (portfolioUnitOfWork.PortfolioQuery.StockOwned(stock.Id, corporateAction.ActionDate))
                        {
                            var handler = corporateActionHandlerFactory.GetHandler(corporateAction);
                            if (handler != null)
                            {
                                if (!handler.HasBeenApplied(corporateAction))
                                    actions.Add(corporateAction);
                            }
                        }
                    }
                }

                responce.CorporateActions.AddRange(actions.OrderBy(x => x.ActionDate).Select(x => x.ToCorporateActionItem()));
                
                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<UnappliedCorporateActionsResponce>(responce); 
        }

        public Task<TransactionsForCorparateActionsResponce> TransactionsForCorporateAction(Guid stockId, Guid actionId)
        {
            var responce = new TransactionsForCorparateActionsResponce();

            var stock = _StockExchange.Stocks.Get(stockId);
            var corporateAction = stock.CorporateActions[actionId];

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                var corporateActionHandlerFactory = new CorporateActionHandlerFactory(portfolioUnitOfWork.PortfolioQuery);


                var handler = corporateActionHandlerFactory.GetHandler(corporateAction);
                var transactions = handler.CreateTransactionList(corporateAction);

                responce.Transactions.AddRange(_Mapper.Map<IEnumerable<TransactionItem>>(transactions));

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<TransactionsForCorparateActionsResponce>(responce); 
        }
    }

}
