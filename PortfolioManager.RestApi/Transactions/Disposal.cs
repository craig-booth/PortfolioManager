using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.RestApi.Transactions
{
    public class Disposal : Transaction
    {
        public override string Type
        {
            get { return "disposal"; }
        }
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public string CGTMethod { get; set; }
        public bool CreateCashTransaction { get; set; }
    }

    public static class CGTMethodMapping
    {
        public static string ToRest(CGTCalculationMethod cgtMethod)
        {
            if (cgtMethod == CGTCalculationMethod.FirstInFirstOut)
                return "fifo";
            else if (cgtMethod == CGTCalculationMethod.LastInFirstOut)
                return "lifo";
            else if (cgtMethod == CGTCalculationMethod.MaximizeGain)
                return "maximise";
            else if (cgtMethod == CGTCalculationMethod.MinimizeGain)
                return "minimize";

            return "";
        }

        public static CGTCalculationMethod ToDomain(string cgtMethod)
        {
            if (cgtMethod == "fifo")
                return CGTCalculationMethod.FirstInFirstOut;
            else if (cgtMethod == "lifo")
                return CGTCalculationMethod.LastInFirstOut;
            else if (cgtMethod == "maximise")
                return CGTCalculationMethod.MaximizeGain;
            else if (cgtMethod == "minimize")
                return CGTCalculationMethod.MinimizeGain;

            return CGTCalculationMethod.MinimizeGain;
        }

    }



}
