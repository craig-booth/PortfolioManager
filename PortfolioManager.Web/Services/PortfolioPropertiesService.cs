using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Domain.Portfolios;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Mappers;
using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.Services
{

    public interface IPortfolioPropertiesService
    { 
        PortfolioPropertiesResponse GetProperties(Guid portfolioId);
    }

    public class PortfolioPropertiesService : IPortfolioPropertiesService
    {
        private readonly IPortfolioCache _PortfolioCache;

        public PortfolioPropertiesService(IPortfolioCache portfolioCache)
        {
            _PortfolioCache = portfolioCache;
        }

        public PortfolioPropertiesResponse GetProperties(Guid portfolioId)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var response = new PortfolioPropertiesResponse();
            response.Id = portfolio.Id;
            response.Name = portfolio.Name;
            response.StartDate = portfolio.StartDate;
            response.EndDate = portfolio.EndDate;

            foreach (var holding in portfolio.Holdings.All())
            {
                var holdingProperty = new RestApi.Portfolios.HoldingProperties()
                {
                    Stock = holding.Stock.Convert(DateTime.Now),
                    StartDate = holding.EffectivePeriod.FromDate,
                    EndDate = holding.EffectivePeriod.ToDate,
                    ParticipatingInDrp = holding.Settings.ParticipateInDrp
                };
                response.Holdings.Add(holdingProperty);
            }

            return response;
        }
    }
}
