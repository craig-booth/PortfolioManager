using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Mappers;

namespace PortfolioManager.Web.Services
{
    public class PortfolioCgtLiabilityService
    {
        public Portfolio Portfolio { get; }

        public PortfolioCgtLiabilityService(Portfolio portfolio)
        {
            Portfolio = portfolio;
        }

        public CgtLiabilityResponse GetCGTLiability(DateRange dateRange)
        {
            var response = new CgtLiabilityResponse();

            // Get a list of all the cgt events for the year
            var cgtEvents = Portfolio.CgtEvents.InDateRange(dateRange);
            foreach (var cgtEvent in cgtEvents)
            {
                var item = new CgtLiabilityResponse.CgtLiabilityEvent()
                {
                    Stock = cgtEvent.Stock.Convert(cgtEvent.Date),
                    EventDate = cgtEvent.Date,
                    CostBase = cgtEvent.CostBase,
                    AmountReceived = cgtEvent.AmountReceived,
                    CapitalGain = cgtEvent.CapitalGain,
                    Method = cgtEvent.CgtMethod
                };

                response.Events.Add(item);

                // Apportion capital gains
                if (cgtEvent.CapitalGain < 0)
                    response.CurrentYearCapitalLossesTotal += -cgtEvent.CapitalGain;
                else if (cgtEvent.CgtMethod == CGTMethod.Discount)
                    response.CurrentYearCapitalGainsDiscounted += cgtEvent.CapitalGain;
                else
                    response.CurrentYearCapitalGainsOther += cgtEvent.CapitalGain;
            }

            response.CurrentYearCapitalGainsTotal = response.CurrentYearCapitalGainsOther + response.CurrentYearCapitalGainsDiscounted;

            if (response.CurrentYearCapitalGainsOther > response.CurrentYearCapitalLossesTotal)
                response.CurrentYearCapitalLossesOther = response.CurrentYearCapitalLossesTotal;
            else
                response.CurrentYearCapitalLossesOther = response.CurrentYearCapitalGainsOther;

            if (response.CurrentYearCapitalGainsOther > response.CurrentYearCapitalLossesTotal)
                response.CurrentYearCapitalLossesDiscounted = 0.00m;
            else
                response.CurrentYearCapitalLossesDiscounted = response.CurrentYearCapitalLossesTotal - response.CurrentYearCapitalGainsOther;

            response.GrossCapitalGainOther = response.CurrentYearCapitalGainsOther - response.CurrentYearCapitalLossesOther;
            response.GrossCapitalGainDiscounted = response.CurrentYearCapitalGainsDiscounted - response.CurrentYearCapitalLossesDiscounted;
            response.GrossCapitalGainTotal = response.GrossCapitalGainOther + response.GrossCapitalGainDiscounted;
            if (response.GrossCapitalGainDiscounted > 0)
                response.Discount = (response.GrossCapitalGainDiscounted / 2).ToCurrency(RoundingRule.Round);
            else
                response.Discount = 0.00m;
            response.NetCapitalGainOther = response.GrossCapitalGainOther;
            response.NetCapitalGainDiscounted = response.GrossCapitalGainDiscounted - response.Discount;
            response.NetCapitalGainTotal = response.NetCapitalGainOther + response.NetCapitalGainDiscounted;

            return response;
        }
    }
}
