using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Utils;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Mappers;

namespace PortfolioManager.Web.Services
{
    public class PortfolioCapitalGainsService
    {
        public Portfolio Portfolio { get; }

        public PortfolioCapitalGainsService(Portfolio portfolio)
        {
            Portfolio = portfolio;
        }

        public SimpleUnrealisedGainsResponse GetCapitalGains(DateTime date)
        {
            return GetCapitalGains(Portfolio.Holdings.All(date), date);
        }

        public SimpleUnrealisedGainsResponse GetCapitalGains(Domain.Portfolios.Holding holding, DateTime date)
        {
            return GetCapitalGains(new[] { holding} , date); 
        }

        public DetailedUnrealisedGainsResponse GetDetailedCapitalGains(DateTime date)
        {
            return GetDetailedCapitalGains(Portfolio.Holdings.All(date), date);
        }

        public DetailedUnrealisedGainsResponse GetDetailedCapitalGains(Domain.Portfolios.Holding holding, DateTime date)
        {
            return GetDetailedCapitalGains(new[] { holding }, date);
        }

        private SimpleUnrealisedGainsResponse GetCapitalGains(IEnumerable<Domain.Portfolios.Holding> holdings, DateTime date)
        {
            var response = new SimpleUnrealisedGainsResponse();

            foreach (var holding in holdings)
            {
                foreach (var parcel in holding.Parcels(date))
                {
                    var properties = parcel.Properties[date];

                    var value = properties.Units * holding.Stock.GetPrice(date);
                    var capitalGain = value - properties.CostBase;
                    var discountMethod = CgtCalculator.CgtMethodForParcel(parcel.AquisitionDate, date);
                    var discoutedGain = (discountMethod == CGTMethod.Discount) ? CgtCalculator.CgtDiscount(capitalGain) : capitalGain;

                    var unrealisedGain = new SimpleUnrealisedGainsItem()
                    {
                        Stock = holding.Stock.Convert(date),
                        AquisitionDate = parcel.AquisitionDate,
                        Units = properties.Units,
                        CostBase = properties.CostBase,
                        MarketValue = value,
                        CapitalGain = capitalGain,
                        DiscoutedGain = discoutedGain,
                        DiscountMethod = discountMethod
                    };

                    response.UnrealisedGains.Add(unrealisedGain);
                }
            }

            return response;
        }

        private DetailedUnrealisedGainsResponse GetDetailedCapitalGains(IEnumerable<Domain.Portfolios.Holding> holdings, DateTime date)
        {
            var response = new DetailedUnrealisedGainsResponse();

            foreach (var holding in holdings)
            {
                foreach (var parcel in holding.Parcels(date))
                {
                    var properties = parcel.Properties[date];

                    var value = properties.Units * holding.Stock.GetPrice(date);
                    var capitalGain = value - properties.CostBase;
                    var discountMethod = CgtCalculator.CgtMethodForParcel(parcel.AquisitionDate, date);
                    var discoutedGain = (discountMethod == CGTMethod.Discount) ? CgtCalculator.CgtDiscount(capitalGain) : capitalGain;

                    var unrealisedGain = new DetailedUnrealisedGainsItem()
                    {
                        Stock = holding.Stock.Convert(date),
                        AquisitionDate = parcel.AquisitionDate,
                        Units = properties.Units,
                        CostBase = properties.CostBase,
                        MarketValue = value,
                        CapitalGain = capitalGain,
                        DiscoutedGain = discoutedGain,
                        DiscountMethod = discountMethod
                    };

                    int units = 0;
                    decimal costBase = 0.00m;
                    foreach (var auditRecord in parcel.Audit.TakeWhile(x => x.Date <= date))
                    {
                        units += auditRecord.UnitCountChange;
                        costBase += auditRecord.CostBaseChange;

                        var cgtEvent = new DetailedUnrealisedGainsItem.CGTEventItem()
                        {
                            Date = auditRecord.Date,
                            Description = auditRecord.Transaction.Description,
                            Units = units,
                            CostBaseChange = auditRecord.CostBaseChange,
                            CostBase = costBase,
                        };

                        unrealisedGain.CGTEvents.Add(cgtEvent);
                    }

                    response.UnrealisedGains.Add(unrealisedGain);

                }
            }

            return response;
        }
    }
}
