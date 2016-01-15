﻿using System;
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

        public static ApportionedCurrencyValue[] ApportionAmountOverChildStocks(IReadOnlyCollection<Stock> childStocks, DateTime atDate, decimal amount)
        {
            ApportionedCurrencyValue[] result = new ApportionedCurrencyValue[childStocks.Count];
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
