using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Service.Utils
{
    public static class PortfolioUtils
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

        public static CGTEvent CreateCGTEvent(ShareParcel parcel, DateTime eventDate, decimal amount)
        {
            return CreateCGTEvent(parcel, eventDate, parcel.Units, amount);
        }

        public static CGTEvent CreateCGTEvent(ShareParcel parcel, DateTime eventDate, int units, decimal amount)
        {
            var costBase = parcel.CostBase * ((decimal)units / parcel.Units);

            return new CGTEvent(parcel.Stock, eventDate, units, costBase, amount, amount - costBase, CGTCalculator.CGTMethodForParcel(parcel, eventDate));
        }
    }



}
