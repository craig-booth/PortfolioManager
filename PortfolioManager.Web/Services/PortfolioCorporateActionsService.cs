using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Mappers;
using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.Services
{
    public interface IPortfolioCorporateActionsService
    {
        CorporateActionsResponse GetCorporateActions(Guid portfolioId);
        CorporateActionsResponse GetCorporateActions(Guid portfolioId, Guid stockId);
        IEnumerable<RestApi.Transactions.Transaction> GetTransactionsForCorporateAction(Guid portfolioId, Guid stockId, Guid actionId);
    }

    public class PortfolioCorporateActionsService : IPortfolioCorporateActionsService
    {
        private readonly IPortfolioCache _PortfolioCache;
        private IMapper _Mapper;

        public PortfolioCorporateActionsService(IPortfolioCache portfolioCache, IMapper mapper)
        {
            _PortfolioCache = portfolioCache;
            _Mapper = mapper;
        }

        public CorporateActionsResponse GetCorporateActions(Guid portfolioId)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            return GetCorporateActions(portfolio, portfolio.Holdings.All(DateTime.Today));
        }

        public CorporateActionsResponse GetCorporateActions(Guid portfolioId, Guid stockId)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var holding = portfolio.Holdings.Get(stockId);
            if (holding == null)
                throw new HoldingNotFoundException(stockId);

            return GetCorporateActions(portfolio, new[] { holding });
        }

        private CorporateActionsResponse GetCorporateActions(Portfolio portfolio, IEnumerable<Domain.Portfolios.Holding> holdings)
        {
            var response = new CorporateActionsResponse();

            foreach (var holding in holdings)
            {
                foreach (var corporateAction in holding.Stock.CorporateActions.InDateRange(holding.EffectivePeriod))
                {
                    if (! corporateAction.HasBeenApplied(portfolio.Transactions))
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

        public IEnumerable<RestApi.Transactions.Transaction> GetTransactionsForCorporateAction(Guid portfolioId, Guid stockId, Guid actionId)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var holding = portfolio.Holdings.Get(stockId);
            if (holding == null)
                throw new HoldingNotFoundException(stockId);

            var corporateAction = holding.Stock.CorporateActions[actionId];
            if (corporateAction == null)
                throw new CorporateActionNotFoundException(actionId);

            var transactions = corporateAction.GetTransactionList(holding);

            return _Mapper.Map<IEnumerable<RestApi.Transactions.Transaction>>(transactions);
        }

    }
}
