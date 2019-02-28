using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.RestApi.Stocks;

namespace PortfolioManager.Web.Mappers
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
                ListingDate = stock.EffectivePeriod.FromDate,
                DelistedDate = stock.EffectivePeriod.ToDate,
                LastPrice = stock.GetPrice(DateTime.Now)
            };

            if (stock is StapledSecurity)
            {
                response.StapledSecurity = true;
                response.ChildSecurities.AddRange(((StapledSecurity)stock).ChildSecurities.Select(x => new StockResponse.StapledSecurityChild() { ASXCode = x.ASXCode, Name = x.Name, Trust = x.Trust }));
            }
            else
                response.StapledSecurity = false;

            if (stock.IsEffectiveAt(date))
            {
                var drp = stock.DividendRules[date];

                response.CompanyTaxRate = drp.CompanyTaxRate;
                response.DividendRoundingRule = drp.DividendRoundingRule;
                response.DRPActive = drp.DRPActive;
                response.DRPMethod = drp.DRPMethod;
            }

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
                ListingDate = stock.EffectivePeriod.FromDate,
                DelistedDate = stock.EffectivePeriod.ToDate
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

            response.DividendRules.AddRange(
                stock.DividendRules.Values.Select(x =>
                    new StockHistoryResponse.HistoricDividendRules()
                    {
                        FromDate = x.EffectivePeriod.FromDate,
                        ToDate = x.EffectivePeriod.ToDate,
                        CompanyTaxRate = x.Properties.CompanyTaxRate,
                        DividendRoundingRule = x.Properties.DividendRoundingRule,
                        DRPActive = x.Properties.DRPActive,
                        DRPMethod = x.Properties.DRPMethod
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
                for (var i = 0; i < childNTAs.Length; i++)
                    childNTAs[i].Percentage = nta.Properties.Percentages[i];

                response.RelativeNTAs.Add(new RelativeNTAResponse.RelativeNTA(nta.EffectivePeriod.FromDate, nta.EffectivePeriod.ToDate, childNTAs));
            }

            return response;
        }

        public static RestApi.CorporateActions.Dividend ToResponse(this Domain.CorporateActions.Dividend dividend)
        {
            return new RestApi.CorporateActions.Dividend()
            {
                Id = dividend.Id,
                Stock = dividend.Stock.Id,
                ActionDate = dividend.Date,
                Description = dividend.Description,
                PaymentDate = dividend.PaymentDate,
                DividendAmount = dividend.DividendAmount,
                PercentFranked = dividend.PercentFranked,
                DRPPrice = dividend.DRPPrice
            };
        } 

        public static RestApi.CorporateActions.CapitalReturn ToResponse(this Domain.CorporateActions.CapitalReturn capitalReturn)
        {
            return new RestApi.CorporateActions.CapitalReturn()
            {
                Id = capitalReturn.Id,
                Stock = capitalReturn.Stock.Id,
                ActionDate = capitalReturn.Date,
                Description = capitalReturn.Description,
                PaymentDate = capitalReturn.PaymentDate,
                Amount = capitalReturn.Amount
            };
        }

        public static RestApi.CorporateActions.Transformation ToResponse(this Domain.CorporateActions.Transformation transformation)
        {
            var response = new RestApi.CorporateActions.Transformation()
            {
                Id = transformation.Id,
                Stock = transformation.Stock.Id,
                ActionDate = transformation.Date,
                Description = transformation.Description,
                ImplementationDate = transformation.ImplementationDate,
                CashComponent = transformation.CashComponent,
                RolloverRefliefApplies = transformation.RolloverRefliefApplies,
            };

            response.ResultingStocks.AddRange(transformation.ResultingStocks.Select(x => new RestApi.CorporateActions.Transformation.ResultingStock(x.Stock, x.OriginalUnits, x.NewUnits, x.CostBase, x.AquisitionDate)));

            return response;
        } 

    }
}
