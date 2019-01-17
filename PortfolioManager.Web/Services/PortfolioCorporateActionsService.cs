﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Mappers;

namespace PortfolioManager.Web.Services
{
    public class PortfolioCorporateActionsService
    {
        public Portfolio Portfolio { get; }

        public PortfolioCorporateActionsService(Portfolio portfolio)
        {
            Portfolio = portfolio;
        }

        public CorporateActionsResponse GetCorporateActions()
        {
            return GetCorporateActions(Portfolio.Holdings.All(DateTime.Today));
        }

        public CorporateActionsResponse GetCorporateActions(Domain.Portfolios.Holding holding)
        {
            return GetCorporateActions(new[] { holding });
        }

        private CorporateActionsResponse GetCorporateActions(IEnumerable<Domain.Portfolios.Holding> holdings)
        {
            var response = new CorporateActionsResponse();

            foreach (var holding in holdings)
            {
                foreach (var corporateAction in holding.Stock.CorporateActions.InDateRange(holding.EffectivePeriod))
                {
                    if (! corporateAction.HasBeenApplied(Portfolio.Transactions))
                    {
                        response.CorporateActions.Add(new CorporateActionsResponse.CorporateActionItem()
                        {
                            Id = corporateAction.Id,
                            ActionDate = corporateAction.Date,
                            Stock = corporateAction.Stock.Convert(corporateAction.Date),
                            Description = corporateAction.Description
                        });
                    }
                }
            }

            return response;
        }
        
    }
}
