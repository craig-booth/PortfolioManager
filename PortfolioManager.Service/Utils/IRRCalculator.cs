using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Service.Utils
{

    public class CashFlows
    {
        private Dictionary<DateTime, decimal> _Amounts;

        public CashFlows()
        {
            _Amounts = new Dictionary<DateTime, decimal>();
        }

        public void Add(DateTime date, decimal amount)
        {
            if (_Amounts.ContainsKey(date))
                _Amounts[date] += amount;
            else
                _Amounts.Add(date, amount);
        }

        public void GetCashFlows(out double[] values, out double[]periods)
        {
            values = new double[_Amounts.Count];
            periods = new double[_Amounts.Count];

            int i = 0;
            var startDate = _Amounts.First().Key;
            foreach (var amount in _Amounts)
            {
                values[i] = (double)amount.Value;
                periods[i] = (amount.Key - startDate).Days / 365.0;

                i++;
            }

        }

    }

    public class IRRCalculator
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

        public static double CalculateIRR(CashFlows cashFlows)
        {
            return CalculateIRR(cashFlows, 0.10);
        }

        public static double CalculateIRR(CashFlows cashFlows, double guess)
        {
            double[] values;
            double[] periods;

            cashFlows.GetCashFlows(out values, out periods);

            return Calculate(values, periods, guess);
        }

    }



}
