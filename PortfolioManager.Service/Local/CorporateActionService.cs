using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

using PortfolioManager.Service.Interface;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.Local
{
    class CorporateActionService : ICorporateActionService
    {
        private readonly Obsolete.CorporateActionService _CorporateActionService;
        private readonly ICorporateActionQuery _CorporateActionQuery;
        private readonly StockService _StockService;

        public CorporateActionService(Obsolete.CorporateActionService corporateActionService, ICorporateActionQuery corporateActionQuery, StockService stockService)
        {
            _CorporateActionService = corporateActionService;
            _CorporateActionQuery = corporateActionQuery;
            _StockService = stockService;
        }

        public Task<UnappliedCorporateActionsResponce> GetUnappliedCorporateActions()
        {
            var responce = new UnappliedCorporateActionsResponce();

            var actions = _CorporateActionService.GetUnappliedCorporateActions();
            foreach (var action in actions)
            {
                var stock = _StockService.Get(action.Stock, action.ActionDate);
                var item = new CorporateActionItem()
                {
                    Id = action.Id,
                    ActionDate = action.ActionDate,
                    ASXCode = stock.ASXCode,
                    CompanyName = stock.Name,
                    Description = action.Description
                };

                responce.CorporateActions.Add(item);
            }

            return Task.FromResult<UnappliedCorporateActionsResponce>(responce);
        }

        public Task<TransactionsForCorparateActionsResponce> TransactionsForCorporateAction(Guid corporateAction)
        {
            var responce = new TransactionsForCorparateActionsResponce();

            var action = _CorporateActionQuery.Get(corporateAction);
            var transactions = _CorporateActionService.CreateTransactionList(action);

            responce.Transactions.AddRange(transactions);

            return Task.FromResult<TransactionsForCorparateActionsResponce>(responce);
        }
    }

}
