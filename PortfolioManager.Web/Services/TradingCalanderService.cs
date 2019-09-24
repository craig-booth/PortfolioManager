using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Domain.TradingCalanders;
using PortfolioManager.RestApi.TradingCalanders;
using PortfolioManager.Domain;
using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.Services
{
    public interface ITradingCalanderService
    {
        TradingCalanderResponse Get(int year);
        void Update(int year, IEnumerable<Domain.TradingCalanders.NonTradingDay> nonTradingDays);
    }

    public class TradingCalanderService : ITradingCalanderService
    {
        private readonly TradingCalander _TradingCalander;
        private readonly IRepository<TradingCalander> _Repository;

        public TradingCalanderService(IEntityCache<TradingCalander> cache, IRepository<TradingCalander> repository)
        {
            _TradingCalander = cache.Get(TradingCalanderIds.ASX);
            _Repository = repository;
        }

        public TradingCalanderResponse Get(int year)
        {
            var nonTradingDays = _TradingCalander.NonTradingDays(year);

            var response = new TradingCalanderResponse();
            response.Year = year;
            response.NonTradingDays.AddRange(nonTradingDays.Select(x => new RestApi.TradingCalanders.NonTradingDay(x.Date, x.Desciption)));

            return response;
        }

        public void Update(int year, IEnumerable<Domain.TradingCalanders.NonTradingDay> nonTradingDays)
        {
            _TradingCalander.SetNonTradingDays(year, nonTradingDays.Select(x => new Domain.TradingCalanders.NonTradingDay(x.Date, x.Desciption)));

            _Repository.Update(_TradingCalander);

        }
    }
}
