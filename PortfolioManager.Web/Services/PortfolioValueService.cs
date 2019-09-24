using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.TradingCalanders;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.Services
{

    public interface IPortfolioValueService
    {
        PortfolioValueResponse GetValue(Guid portfolioId, DateRange dateRange, ValueFrequency frequency);
        PortfolioValueResponse GetValue(Guid portfolioId, Guid stockId, DateRange dateRange, ValueFrequency frequency);
    }

    public class PortfolioValueService : IPortfolioValueService
    {
        private readonly IPortfolioCache _PortfolioCache;
        private readonly ITradingCalander _TradingCalander;

        public PortfolioValueService(IPortfolioCache portfolioCache, ITradingCalander tradingCalander)
        {
            _PortfolioCache = portfolioCache;
            _TradingCalander = tradingCalander;
        }

        public PortfolioValueResponse GetValue(Guid portfolioId, DateRange dateRange, ValueFrequency frequency)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            return GetValue(portfolio.Holdings.All(dateRange), portfolio.CashAccount, dateRange, frequency);
        }

        public PortfolioValueResponse GetValue(Guid portfolioId, Guid stockId, DateRange dateRange, ValueFrequency frequency)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var holding = portfolio.Holdings.Get(stockId);
            if (holding == null)
                throw new HoldingNotFoundException(stockId);

            return GetValue(new[] { holding }, null, dateRange, frequency);
        }

        private PortfolioValueResponse GetValue(IEnumerable<Domain.Portfolios.Holding> holdings, ICashAccount cashAccount, DateRange dateRange, ValueFrequency frequency)
        {
            var response = new PortfolioValueResponse();

            IEnumerable<DateTime> dates = null;
            if (frequency == ValueFrequency.Daily)
                dates = _TradingCalander.TradingDays(dateRange);
            else if (frequency == ValueFrequency.Weekly)
                dates = DateUtils.WeekEndingDays(dateRange.FromDate, dateRange.ToDate);
            else if (frequency == ValueFrequency.Monthly)
                dates = DateUtils.MonthEndingDays(dateRange.FromDate, dateRange.ToDate);

            IEnumerable<CashAccount.EffectiveBalance> closingBalances;
            if (cashAccount != null)
                closingBalances = cashAccount.EffectiveBalances(dateRange);
            else
                closingBalances = new[] { new CashAccount.EffectiveBalance(dateRange.FromDate, dateRange.ToDate, 0.00m) };
            var closingBalanceEnumerator = closingBalances.GetEnumerator();
            closingBalanceEnumerator.MoveNext();

            var h = holdings.ToArray();

            foreach (var date in dates)
            {
                var amount = 0.00m;

                // Add holding values
                foreach (var holding in h)
                    amount += holding.Value(date);

                // Add cash account balances
                if (date > closingBalanceEnumerator.Current.EffectivePeriod.ToDate)
                    closingBalanceEnumerator.MoveNext();
                amount += closingBalanceEnumerator.Current.Balance;

                var value = new PortfolioValueResponse.ValueItem()
                {
                    Date = date,
                    Amount = amount
                };

                response.Values.Add(value);
            }

            return response;
        }


    }

}
