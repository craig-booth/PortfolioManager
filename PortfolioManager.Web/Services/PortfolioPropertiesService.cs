using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Domain.Portfolios;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Mappers;

namespace PortfolioManager.Web.Services
{
    public class PortfolioPropertiesService
    {
        public Portfolio Portfolio { get; }

        public PortfolioPropertiesService(Portfolio portfolio)
        {
            Portfolio = portfolio;
        }

        public PortfolioPropertiesResponse GetProperties()
        {
            var response = new PortfolioPropertiesResponse();

            response.Id = Portfolio.Id;
            response.Name = Portfolio.Name;
            response.StartDate = Portfolio.StartDate;
            response.EndDate = Portfolio.EndDate;

            foreach (var holding in Portfolio.Holdings.All())
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
