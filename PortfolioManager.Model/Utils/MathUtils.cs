using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Utils
{
    public enum RoundingRule {Round, Truncate}

    public struct ApportionedValue
    {
        public int Units;
        public decimal Amount;
    }

    public class MathUtils
    {
        public static decimal ToCurrency(decimal value, RoundingRule rule)
        {
            if (rule == RoundingRule.Round)
                return Math.Round(value, 2, MidpointRounding.AwayFromZero);
            else if (rule == RoundingRule.Truncate)
            {
                int cents = (int)(value * 100);

                return ((decimal)cents / 100);
            }
            else
                return Math.Round(value, 2);
        }

        public static void ApportionAmount(decimal amount, ApportionedValue[] values)
        {
            /* Calculate total */
            decimal totalUnits = values.Sum(x => x.Units);

            decimal totalAmount = MathUtils.ToCurrency(amount , RoundingRule.Truncate);
            for (int i = 0; i < values.Length; i++)
            {
                if (totalUnits > 0)
                    values[i].Amount = MathUtils.ToCurrency(totalAmount * (values[i].Units / totalUnits), RoundingRule.Round);
                else
                    values[i].Amount = 0.00m;

                totalUnits -= values[i].Units;
                totalAmount -= values[i].Amount;
            }
        }

        public static int ParseInt(string value)
        {
            int result;

            if (int.TryParse(value, out result))
                return result;
            else
                return 0;
        }

        public static int ParseInt(string value, int defaultValue)
        {
            int result;

            if (int.TryParse(value, out result))
                return result;
            else
                return defaultValue;
        }

        public static decimal ParseDecimal(string value)
        {
            decimal result;

            if (decimal.TryParse(value, out result))
                return result;
            else
                return 0.0m;
        }

        public static decimal ParseDecimal(string value, decimal defaultValue)
        {
            decimal result;

            if (decimal.TryParse(value, out result))
                return result;
            else
                return defaultValue;
        }

        public static string FormatCurrency(decimal value)
        {
            return value.ToString("###,##0.00###");
        }

        public static string FormatCurrency(decimal value, bool round)
        {
            if (round)
                return value.ToString("###,##0.00");  
            else
                return value.ToString("###,##0.00###");  
        }

        public static string FormatCurrency(decimal value, bool round, bool includeCurrencySymbol)
        {
            if (round)
                {
                if (includeCurrencySymbol)
                    return value.ToString("$###,##0.00");
                else
                    return value.ToString("###,##0.00");
                }
            else
            {
                if (includeCurrencySymbol)
                    return value.ToString("$###,##0.00###");
                else
                    return value.ToString("###,##0.00###");
            }
        }

    }
}
