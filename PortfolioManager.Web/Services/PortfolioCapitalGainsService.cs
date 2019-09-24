using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Utils;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Mappers;
using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.Services
{
    public interface IPortfolioCapitalGainsService
    {
        SimpleUnrealisedGainsResponse GetCapitalGains(Guid portfolioId, DateTime date);
        SimpleUnrealisedGainsResponse GetCapitalGains(Guid portfolioId, Guid stockId, DateTime date);
        DetailedUnrealisedGainsResponse GetDetailedCapitalGains(Guid portfolioId, DateTime date);
        DetailedUnrealisedGainsResponse GetDetailedCapitalGains(Guid portfolioId, Guid stockId, DateTime date);
    }

    public class PortfolioCapitalGainsService: IPortfolioCapitalGainsService
    {
        private readonly IPortfolioCache _PortfolioCache;

        public PortfolioCapitalGainsService(IPortfolioCache portfolioCache)
        {
            _PortfolioCache = portfolioCache;
        }

        public SimpleUnrealisedGainsResponse GetCapitalGains(Guid portfolioId, DateTime date)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            return GetCapitalGains(portfolio.Holdings.All(date), date);
        }

        public SimpleUnrealisedGainsResponse GetCapitalGains(Guid portfolioId, Guid stockId, DateTime date)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var holding = portfolio.Holdings.Get(stockId);
            if (holding == null)
                throw new HoldingNotFoundException(stockId);

            return GetCapitalGains(new[] { holding} , date); 
        }

        public DetailedUnrealisedGainsResponse GetDetailedCapitalGains(Guid portfolioId, DateTime date)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            return GetDetailedCapitalGains(portfolio.Holdings.All(date), date);
        }

        public DetailedUnrealisedGainsResponse GetDetailedCapitalGains(Guid portfolioId, Guid stockId, DateTime date)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var holding = portfolio.Holdings.Get(stockId);
            if (holding == null)
                throw new HoldingNotFoundException(stockId);

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
