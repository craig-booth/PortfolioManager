using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.Services
{

    public interface IPortfolioService
    {
        IEnumerable<RestApi.Portfolios.Holding> GetHoldings(Guid portfolioId, DateTime date);
        IEnumerable<RestApi.Portfolios.Holding> GetHoldings(Guid portfolioId, DateRange dateRange);
        RestApi.Portfolios.Holding GetHolding(Guid portfolioId, Guid stockId, DateTime date);
        void ChangeDrpParticipation(Guid portfolioId, Guid stockId, bool participation);
    }

    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioCache _PortfolioCache;
        private readonly IRepository<Portfolio> _PortfolioRepository;
        private readonly IMapper _Mapper;

        public PortfolioService(IPortfolioCache portfolioCache, IRepository<Portfolio> portfolioRepository, IMapper mapper)
        {
            _PortfolioCache = portfolioCache;
            _PortfolioRepository = portfolioRepository;
            _Mapper = mapper;
        }


        public IEnumerable<RestApi.Portfolios.Holding> GetHoldings(Guid portfolioId, DateTime date)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var holdings = portfolio.Holdings.All(date);

            return _Mapper.Map<List<RestApi.Portfolios.Holding>>(holdings, opts => opts.Items["date"] = date);
        }

        public IEnumerable<RestApi.Portfolios.Holding> GetHoldings(Guid portfolioId, DateRange dateRange)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var holdings = portfolio.Holdings.All(dateRange);

            return _Mapper.Map<List<RestApi.Portfolios.Holding>>(holdings, opts => opts.Items["date"] = dateRange.ToDate);
        }

        public RestApi.Portfolios.Holding GetHolding(Guid portfolioId, Guid id, DateTime date)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var holding = portfolio.Holdings.Get(id);
            if (holding == null)
                throw new HoldingNotFoundException(id);

            return _Mapper.Map<RestApi.Portfolios.Holding>(holding, opts => opts.Items["date"] = date);
        }

        public void ChangeDrpParticipation(Guid portfolioId, Guid holding, bool participation)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            portfolio.ChangeDrpParticipation(holding, participation);

            _PortfolioRepository.Update(portfolio);
        }
    }
}
