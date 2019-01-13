using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.RestApi.Portfolios;


namespace PortfolioManager.Web.Services
{
    public class PortfolioValueService
    {
        public Portfolio Portfolio { get; }
        public ITradingCalander TradingCalander { get; }

        public PortfolioValueService(Portfolio portfolio, ITradingCalander tradingCalander)
        {
            Portfolio = portfolio;
            TradingCalander = tradingCalander;
        }

        public PortfolioValueResponse GetValue(DateRange dateRange, ValueFrequency frequency)
        {
            return GetValue(Portfolio.Holdings.All(dateRange), Portfolio.CashAccount, dateRange, frequency);
        }

        public PortfolioValueResponse GetValue(Domain.Portfolios.Holding holding, DateRange dateRange, ValueFrequency frequency)
        {
            return GetValue(new[] { holding }, null, dateRange, frequency);
        }

        private PortfolioValueResponse GetValue(IEnumerable<Domain.Portfolios.Holding> holdings, ICashAccount cashAccount, DateRange dateRange, ValueFrequency frequency)
        {
            var response = new PortfolioValueResponse();

            IEnumerable<DateTime> dates = null;
            if (frequency == ValueFrequency.Daily)
                dates = TradingCalander.TradingDays(dateRange);
            else if (frequency == ValueFrequency.Weekly)
                dates = DateUtils.WeekEndingDays(dateRange.FromDate, dateRange.ToDate);
            else if (frequency == ValueFrequency.Monthly)
                dates = DateUtils.MonthEndingDays(dateRange.FromDate, dateRange.ToDate);

            IEnumerable<CashAccount.EffectiveBalance> closingBalances;
            if (cashAccount != null)
                closingBalances = Portfolio.CashAccount.EffectiveBalances(dateRange);
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
