using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Utils
{
    public static class PortfolioUtils
    {

        public static ApportionedValue[] ApportionAmountOverParcels(IReadOnlyCollection<ShareParcel> parcels, decimal amount)
        {
            ApportionedValue[] result = new ApportionedValue[parcels.Count];
            int i = 0;
            foreach (ShareParcel parcel in parcels)
                result[i++].Units = parcel.Units;
            MathUtils.ApportionAmount(amount, result);

            return result;
        }

        public static ApportionedValue[] ApportionAmountOverChildStocks(IReadOnlyCollection<Stock> childStocks, DateTime atDate, decimal amount)
        {
            ApportionedValue[] result = new ApportionedValue[childStocks.Count];
            int i = 0;
            foreach (Stock childStock in childStocks)
            {
                decimal percentageOfParent = childStock.PercentageOfParentCostBase(atDate);
                int relativeValue = (int)(percentageOfParent * 10000);

                result[i].Units = relativeValue;
                i++;
            }
            MathUtils.ApportionAmount(amount, result);

            return result;
        }
    }



}
