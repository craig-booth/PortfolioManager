using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.CorporateActions;
using PortfolioManager.RestApi.Stocks;
using PortfolioManager.RestApi.CorporateActions;

namespace PortfolioManager.Web
{
    public static class CommandResponseMapping
    {

        public static StockResponse ToResponse(this Stock stock, DateTime date)
        {
            var properties = stock.Properties.ClosestTo(date);

            var response = new StockResponse()
            {
                Id = stock.Id,
                ASXCode = properties.ASXCode,
                Name = properties.Name,
                Category = properties.Category,
                Trust = stock.Trust,
                LastPrice = stock.GetPrice(DateTime.Now)
            };

            if (stock is StapledSecurity)
            {
                response.StapledSecurity = true;
                response.ChildSecurities.AddRange(((StapledSecurity)stock).ChildSecurities.Select(x => new StockResponse.StapledSecurityChild() { ASXCode = x.ASXCode, Name = x.Name, Trust = x.Trust }));
            }
            else
                response.StapledSecurity = false;

            var drp = stock.DividendReinvestmentPlan[date];
            response.DRP.Active = drp.DRPActive;
            response.DRP.DividendRoundingRule = drp.DividendRoundingRule;
            response.DRP.DRPMethod = drp.DRPMethod;

            return response;
        }

        public static StockHistoryResponse ToHistoryResponse(this Stock stock)
        {
            var properties = stock.Properties.ClosestTo(DateTime.Today);

            var response = new StockHistoryResponse()
            {
                Id = stock.Id,
                ASXCode = properties.ASXCode,
                Name = properties.Name,
            };

            response.History.AddRange(
                stock.Properties.Values.Select(x => 
                    new StockHistoryResponse.HistoricProperties()
                    {
                        FromDate = x.EffectivePeriod.FromDate,
                        ToDate = x.EffectivePeriod.ToDate,
                        ASXCode = x.Properties.ASXCode,
                        Name = x.Properties.Name,
                        Category = x.Properties.Category
                    })
            );

            response.DRP.AddRange(
                stock.DividendReinvestmentPlan.Values.Select(x =>
                    new StockHistoryResponse.HistoricDRP()
                    {
                        FromDate = x.EffectivePeriod.FromDate,
                        ToDate = x.EffectivePeriod.ToDate,
                        Active = x.Properties.DRPActive,
                        DRPMethod = x.Properties.DRPMethod,
                        DividendRoundingRule = x.Properties.DividendRoundingRule
                    })
            );

            return response;
        }

        public static StockPriceResponse ToPriceResponse(this Stock stock, DateRange dateRange)
        {
            var properties = stock.Properties.ClosestTo(dateRange.ToDate);

            var response = new StockPriceResponse()
            {
                Id = stock.Id,
                ASXCode = properties.ASXCode,
                Name = properties.Name,
            };

            response.ClosingPrices.AddRange(stock.GetPrices(dateRange).Select(x => new StockPriceResponse.ClosingPrice(x.Key, x.Value)));

            return response;
        }

        public static RelativeNTAResponse ToRelativeNTAResponse(this StapledSecurity stock, DateRange dateRange)
        {
            var properties = stock.Properties.ClosestTo(dateRange.ToDate);

            var response = new RelativeNTAResponse()
            {
                Id = stock.Id,
                ASXCode = properties.ASXCode,
                Name = properties.Name,
            };

            var ntas = stock.RelativeNTAs.Values.Where(x => x.EffectivePeriod.Overlaps(dateRange));

            var childNTAs = stock.ChildSecurities.Select(x => new RelativeNTAResponse.ChildSecurityNTA(x.ASXCode, 0.00m)).ToArray();

            foreach (var nta in ntas)
            {
                //if (nta.EffectivePeriod.FromDate < dateRange.FromDate)
                for (var i = 0; i < childNTAs.Length; i ++)
                    childNTAs[i].Percentage = nta.Properties.Percentages[i];

                response.RelativeNTAs.Add(new RelativeNTAResponse.RelativeNTA(nta.EffectivePeriod.FromDate, nta.EffectivePeriod.ToDate, childNTAs));
            }
       
            return response;
        }

        public static CorporateActionResponse ToCorporateActionResponse(this CorporateAction corporateAction)
        {
            return new CorporateActionResponse()
            {
                Id = corporateAction.Id,
                Stock = corporateAction.Stock.Id,
                Type = corporateAction.Type,
                ActionDate = corporateAction.ActionDate,
                Description = corporateAction.Description
            };
        }

        public static DividendResponse ToDividendResponse(this Dividend dividend)
        {
            return new DividendResponse()
            {
                Id = dividend.Id,
                Stock = dividend.Stock.Id,
                Type = dividend.Type,
                ActionDate = dividend.ActionDate,
                Description = dividend.Description,
                PaymentDate = dividend.PaymentDate,
                DividendAmount = dividend.DividendAmount,
                CompanyTaxRate = dividend.CompanyTaxRate,
                PercentFranked = dividend.PercentFranked,
                DRPPrice = dividend.DRPPrice
            };
        }
    }
}
