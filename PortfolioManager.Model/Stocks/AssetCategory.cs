using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Stocks
{
    public enum AssetClass { Growth, Income, Cash }
    public enum AssetCategory { AustralianStocks, InternationalStocks, AustralianProperty, InternationalProperty, AustralianFixedInterest, InternationlFixedInterest, Cash}
  
    public static class AssetCategoryExtension
    {

        public static AssetClass AssetClass(this AssetCategory assetCategory)
        {
            if ((assetCategory == AssetCategory.AustralianFixedInterest) || (assetCategory == AssetCategory.InternationlFixedInterest))
                return Stocks.AssetClass.Income;
            else if (assetCategory == AssetCategory.Cash)
                return Stocks.AssetClass.Cash;
            else
                return Stocks.AssetClass.Growth;
        }

        public static string ToString(this AssetCategory assetCategory)
        {
            if (assetCategory == AssetCategory.AustralianStocks)
                return "Australian Stocks";
            else if (assetCategory == AssetCategory.InternationalStocks)
                return "International Stocks";
            else if (assetCategory == AssetCategory.AustralianProperty)
                return "Australian Property";
            else if (assetCategory == AssetCategory.InternationalProperty)
                return "International Property";
            else if (assetCategory == AssetCategory.AustralianFixedInterest)
                return "Australian Fixed Interest";
            else if (assetCategory == AssetCategory.InternationlFixedInterest)
                return "International Fixed Interest";
            else if (assetCategory == AssetCategory.Cash)
                return "Cash";

            return "Unknown";
        }
    }
}
