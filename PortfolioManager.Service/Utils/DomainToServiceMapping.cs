using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Service.Interface;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.CorporateActions;

namespace PortfolioManager.Service.Utils
{
    static class DomainToServiceMapping
    {
        public static StockItem ToStockItem(this Stock stock, DateTime date)
        {
            var stockProperties = stock.Properties[date];
            return new StockItem(stock.Id, stockProperties.ASXCode, stockProperties.Name);
        }

        public static CorporateActionItem ToCorporateActionItem(this ICorporateAction corporateAction)
        {
            return new CorporateActionItem()
            {
                Id = corporateAction.Id,
                ActionDate = corporateAction.ActionDate,
                Stock = corporateAction.Stock.ToStockItem(corporateAction.ActionDate),
                Description = corporateAction.Description
            };
        }
    }
}
