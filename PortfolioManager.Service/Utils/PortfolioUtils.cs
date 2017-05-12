using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.Utils
{
    static class PortfolioUtils
    {

        public static ApportionedCurrencyValue[] ApportionAmountOverParcels(IReadOnlyCollection<ShareParcel> parcels, decimal amount)
        {
            ApportionedCurrencyValue[] result = new ApportionedCurrencyValue[parcels.Count];
            int i = 0;
            foreach (ShareParcel parcel in parcels)
                result[i++].Units = parcel.Units;
            MathUtils.ApportionAmount(amount, result);

            return result;
        }

        public static ApportionedIntegerValue[] ApportionAmountOverParcels(IReadOnlyCollection<ShareParcel> parcels, int amount)
        {
            ApportionedIntegerValue[] result = new ApportionedIntegerValue[parcels.Count];
            int i = 0;
            foreach (ShareParcel parcel in parcels)
                result[i++].Units = parcel.Units;
            MathUtils.ApportionAmount(amount, result);

            return result;
        }

        public static ApportionedCurrencyValue[] ApportionAmountOverChildStocks(IReadOnlyCollection<Stock> childStocks, DateTime atDate, decimal amount, StockService stockService)
        {
            ApportionedCurrencyValue[] result = new ApportionedCurrencyValue[childStocks.Count];
            int i = 0;
            foreach (Stock childStock in childStocks)
            {
                decimal percentageOfParent = stockService.PercentageOfParentCostBase(childStock, atDate);
                int relativeValue = (int)(percentageOfParent * 10000);

                result[i].Units = relativeValue;
                i++;
            }
            MathUtils.ApportionAmount(amount, result);

            return result;
        }

        public static IReadOnlyCollection<ShareParcel> GetStapledSecurityParcels(Stock stock, DateTime date, StockService stockService, IPortfolioQuery portfolioQuery)
        {
            var stapledParcels = new List<ShareParcel>();

            var childStocks = stockService.GetChildStocks(stock, date);

            foreach (var childStock in childStocks)
            {
                var childParcels = portfolioQuery.GetParcelsForStock(childStock.Id, date, date);

                foreach (var childParcel in childParcels)
                {
                    var stapledParcel = stapledParcels.FirstOrDefault(x => x.PurchaseId == childParcel.PurchaseId);
                    if (stapledParcel == null)
                    {
                        stapledParcel = new ShareParcel(childParcel.AquisitionDate, stock.Id, childParcel.Units, childParcel.UnitPrice, childParcel.Amount, childParcel.CostBase, childParcel.PurchaseId);
                        stapledParcels.Add(stapledParcel);
                    }
                    else
                    {
                        stapledParcel.Amount += childParcel.Amount;
                        stapledParcel.CostBase += childParcel.CostBase;
                        stapledParcel.UnitPrice += childParcel.UnitPrice;
                    }

                }
            }

            return stapledParcels;
        }
    }



}
