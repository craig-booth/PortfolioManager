using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.CorporateActions;
using PortfolioManager.Service.Utils;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Service.Services
{
    public class CorporateActionService : ICorporateActionService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly IStockDatabase _StockDatabase;
        private readonly StockExchange _StockExchange;

        public CorporateActionService(IPortfolioDatabase portfolioDatabase, IStockDatabase stockDatabase, StockExchange stockExchange)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockDatabase = stockDatabase;
            _StockExchange = stockExchange;
        }

        public Task<UnappliedCorporateActionsResponce> GetUnappliedCorporateActions()
        {
            var responce = new UnappliedCorporateActionsResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                using (var stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
                {
                    var corporateActionHandlerFactory = new CorporateActionHandlerFactory(portfolioUnitOfWork.PortfolioQuery, stockUnitOfWork.StockQuery);

                    // Get a list of all stocks held
                    var allOwnedStocks = portfolioUnitOfWork.PortfolioQuery.GetStocksOwned(DateTime.Today, DateTime.Today);

                    var actions = new List<CorporateAction>();
                    foreach (var ownedStock in allOwnedStocks)
                    {
                        var corporateActions = stockUnitOfWork.CorporateActionQuery.Find(ownedStock, DateUtils.NoStartDate, DateUtils.NoEndDate);
                        foreach (var corporateAction in corporateActions)
                        {
                            if (portfolioUnitOfWork.PortfolioQuery.StockOwned(corporateAction.Stock, corporateAction.ActionDate))
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

                    foreach (var action in actions.OrderBy(x => x.ActionDate))
                    {
                        var item = new CorporateActionItem()
                        {
                            Id = action.Id,
                            ActionDate = action.ActionDate,
                            Stock = StockUtils.Get(action.Stock, action.ActionDate, stockUnitOfWork.StockQuery),
                            Description = action.Description
                        };

                        responce.CorporateActions.Add(item);
                    }

                    responce.SetStatusToSuccessfull();
                }
            }

            return Task.FromResult<UnappliedCorporateActionsResponce>(responce); 
        }

        public Task<TransactionsForCorparateActionsResponce> TransactionsForCorporateAction(Guid corporateAction)
        {
            var responce = new TransactionsForCorparateActionsResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                using (var stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
                {
                    var corporateActionHandlerFactory = new CorporateActionHandlerFactory(portfolioUnitOfWork.PortfolioQuery, stockUnitOfWork.StockQuery);

                    var action = stockUnitOfWork.CorporateActionQuery.Get(corporateAction);

                    var handler = corporateActionHandlerFactory.GetHandler(action);
                    var transactions = handler.CreateTransactionList(action);

                    responce.Transactions.AddRange(Mapper.Map<IEnumerable<TransactionItem>>(transactions));

                    responce.SetStatusToSuccessfull();
                }
            }

            return Task.FromResult<TransactionsForCorparateActionsResponce>(responce); 
        }
    }

}
