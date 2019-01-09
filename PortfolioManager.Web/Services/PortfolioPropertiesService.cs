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

            foreach (var holding in Portfolio.Holdings.All())
                response.StocksHeld.Add(holding.Stock.Convert(DateTime.Now));

            response.StartDate = Portfolio.StartDate;
            response.EndDate = Portfolio.EndDate;

            return response;
        }
    }
}
