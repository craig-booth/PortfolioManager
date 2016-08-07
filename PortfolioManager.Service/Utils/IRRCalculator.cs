using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Service.Utils
{

    class CashFlow
    {
        public DateTime Date;
        public decimal Amount;

        public CashFlow(DateTime date, decimal amount)
        {
            Date = date;
            Amount = amount;
        }
    }

    class IRRCalculator
    {
        private const double RequiredPrecision = 0.000001;
        private const int MaximumIterations = 50;

        private static double CalculateResult(double[] values, double[] period, double guess)
        {
            double r = guess + 1;
            double result = values[0];

            for (var i = 1; i < values.Length; i++)
            {
                //result += values[i] / Math.pow(r, (dates[i] - dates[0]) / 365);
                result += values[i] / Math.Pow(r, period[i]);
            }

            return result;
        }

        private static double CalculateDerivative(double[] values, double[] period, double guess)
        {
            double r = guess + 1;
            double result = 0;

            for (var i = 1; i < values.Length; i++)
            {
                //var frac = (dates[i] - dates[0]) / 365;
                var frac = period[i];
                result -= frac * values[i] / Math.Pow(r, frac + 1);
            }

            return result;
        }


        private static double Calculate(double[] values, double[] periods, double guess)
        {
            var irr = guess;

            var iteration = 0;
            do
            {
                iteration++;

                var resultValue = CalculateResult(values, periods, irr);
                var derivativeValue = CalculateDerivative(values, periods, irr);

                var newIRR = irr - (resultValue / derivativeValue);

                if (double.IsNaN(newIRR))
                    return 0;

                var epsilon = Math.Abs(newIRR - irr);
                irr = newIRR;

                // Check if required precision achieved
                if (epsilon < RequiredPrecision)
                    break;

            } while (iteration < MaximumIterations);

            return irr;
        }

        public static double CalculateIRR(double[] cashFlows)
        {
            return CalculateIRR(cashFlows, 0.10);
        }

        public static double CalculateIRR(double[] cashFlows, double guess)
        {
            var periods = new double[cashFlows.Length];
            for (var i = 0; i < cashFlows.Length; i++)
                periods[i] = i;

            return Calculate(cashFlows, periods, guess);
        }

        public static double CalculateIRR(IReadOnlyList<CashFlow> cashFlows)
        {
            return CalculateIRR(cashFlows, 0.10);
        }

        public static double CalculateIRR(IReadOnlyList<CashFlow> cashFlows, double guess)
        {
            var values = new double[cashFlows.Count];
            var periods = new double[cashFlows.Count];
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = (double)cashFlows[i].Amount;
                periods[i] = (cashFlows[i].Date - cashFlows[0].Date).Days / 365.0;
            }

            return Calculate(values, periods, guess);
        }
    }



}
