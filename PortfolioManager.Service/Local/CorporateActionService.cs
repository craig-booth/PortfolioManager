using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.CorporateActions;
using PortfolioManager.Service.Utils;

namespace PortfolioManager.Service.Local
{
    class CorporateActionService : ICorporateActionService
    {
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly ICorporateActionQuery _CorporateActionQuery;
        private readonly ICorporateActionHandlerFactory _CorporateActionHandlerFactory;
        private readonly StockUtils _StockUtils;

        public CorporateActionService(IPortfolioQuery portfolioQuery, ICorporateActionQuery corporateActionQuery, IStockQuery stockQuery, IStockDatabase stockDatabase, ICorporateActionHandlerFactory corporateActionHandlerFactory)
        {
            _PortfolioQuery = portfolioQuery;
            _CorporateActionQuery = corporateActionQuery;
            _CorporateActionHandlerFactory = corporateActionHandlerFactory;
            _StockUtils = new StockUtils(stockQuery, stockDatabase);
        }

        public Task<UnappliedCorporateActionsResponce> GetUnappliedCorporateActions()
        {
            var responce = new UnappliedCorporateActionsResponce();

            // Get a list of all stocks held
            var allOwnedStocks = _PortfolioQuery.GetStocksOwned(DateTime.Today, DateTime.Today);

            var actions = new List<CorporateAction>();
            foreach (var ownedStock in allOwnedStocks)
            {
                var corporateActions = _CorporateActionQuery.Find(ownedStock, DateUtils.NoStartDate, DateUtils.NoEndDate);
                foreach (var corporateAction in corporateActions)
                {
                    if (_PortfolioQuery.StockOwned(corporateAction.Stock, corporateAction.ActionDate))
                    {
                        var handler = _CorporateActionHandlerFactory.GetHandler(corporateAction);
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
                    Stock = _StockUtils.Get(action.Stock, action.ActionDate),
                    Description = action.Description
                };

                responce.CorporateActions.Add(item);
            }

            responce.SetStatusToSuccessfull();

            return Task.FromResult<UnappliedCorporateActionsResponce>(responce);
        }

        public Task<TransactionsForCorparateActionsResponce> TransactionsForCorporateAction(Guid corporateAction)
        {
            var responce = new TransactionsForCorparateActionsResponce();

            var action = _CorporateActionQuery.Get(corporateAction);

            var handler = _CorporateActionHandlerFactory.GetHandler(action);
            var transactions = handler.CreateTransactionList(action);

            responce.Transactions.AddRange(Mapper.Map<IEnumerable<TransactionItem>>(transactions));

            responce.SetStatusToSuccessfull();

            return Task.FromResult<TransactionsForCorparateActionsResponce>(responce); 
        }
    }

}
