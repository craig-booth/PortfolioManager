using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Domain;
using PortfolioManager.Domain.Portfolios;

namespace PortfolioManager.Web.Services
{
    public class PortfolioService
    {
        public Portfolio Portfolio { get; }

        private IRepository<Portfolio> _PortfolioRepository;

        public PortfolioService(Portfolio portfolio, IRepository<Portfolio> portfolioRepository)
        {
            Portfolio = portfolio;
            _PortfolioRepository = portfolioRepository;
        }

        public void ChangeDrpParticipation(Guid holding, bool participation)
        {
            Portfolio.ChangeDrpParticipation(holding, participation);

            _PortfolioRepository.Update(Portfolio);
        }

    }
}
