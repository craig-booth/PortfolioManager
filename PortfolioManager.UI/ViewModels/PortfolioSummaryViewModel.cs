using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Drawing;
using System.Globalization;

using PortfolioManager.Service;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class PortfolioSummaryViewModel: PortfolioViewModel
    {

        public decimal MarketValue { get; private set; }
        public decimal DollarChangeInValue { get; private set; }
        public decimal PercentChangeInValue { get; private set; }

        public decimal? Return1Year { get; private set; }
        public decimal? Return3Year { get; private set; }
        public decimal? Return5Year { get; private set; }
        public decimal ReturnAll { get; private set; }

        public List<HoldingItemViewModel> Holdings { get; private set; }

        public PortfolioSummaryViewModel(string label, Portfolio portfolio)
            : base(label, portfolio)
        {
            Holdings = new List<HoldingItemViewModel>();
        }

        public override void Activate()
        {
            ShowReport();
        }

        private void ShowReport()
        {
            var holdings = Portfolio.ShareHoldingService.GetHoldings(DateTime.Today);

            Holdings.Clear();
            foreach (var holding in holdings)
                Holdings.Add(new HoldingItemViewModel(holding));

            /* Retrieve the stock price information */
            var asxCodes = new string[Holdings.Count];
            for (var i = 0; i < Holdings.Count; i++)
                asxCodes[i] = Holdings[i].ASXCode;
            var stockPrices = Portfolio.StockPriceService.GetCurrentPrice(asxCodes);        

            /* Add prices to the Holdings list */
            foreach (var holding in Holdings)
            {
                var stockPrice = stockPrices.FirstOrDefault(x => x.ASXCode == holding.ASXCode);

                if (stockPrice != null)
                {
                    holding.CurrentValue = holding.Units * stockPrice.Price;
                    holding.ChangeInValue = new ChangeInValue(holding.CostBase, holding.CurrentValue);
                }
            }

            MarketValue = Holdings.Sum(x => x.CurrentValue);
            var totalCost = holdings.Sum(x => x.TotalCostBase);
            DollarChangeInValue = MarketValue - totalCost;
            if (totalCost == 0)
                PercentChangeInValue = 0;
            else
                PercentChangeInValue = DollarChangeInValue / totalCost;

            Return1Year = 0.0560m;
            Return3Year = CalculateIRR(DateTime.Today.AddYears(-3), DateTime.Today);
            Return5Year = null;
            ReturnAll = 0.0503m;
        }

        private decimal CalculateIRR(DateTime startDate, DateTime endDate)
        {
         /*   // create cashFlow array
            int yearNumber = 1;
            DateTime periodEnd = startDate.AddYears(1);
            while (periodEnd < endDate)
            {
                yearNumber++;
                periodEnd = periodEnd.AddYears(1);
            }
            var cashFlows = new decimal[yearNumber + 1];

            // Get the initial portfolio value
            var initialHoldings = Portfolio.ShareHoldingService.GetHoldings(startDate);
            cashFlows[0] -= initialHoldings.Sum(x => x.MarketValue);

            // generate list of cashFlows
            var transactions = Portfolio.TransactionService.GetTransactions(startDate.AddDays(1), endDate);
            foreach (var transaction in transactions)
            {
                yearNumber = 1;
                periodEnd = startDate.AddYears(1);
                while (transaction.TransactionDate >= periodEnd)
                {
                    yearNumber++;
                    periodEnd = periodEnd.AddYears(1);
                }
                      

                if (transaction.Type == TransactionType.Aquisition)
                {
                    var aquisition = transaction as Aquisition;
                    cashFlows[yearNumber] -= (aquisition.Units * aquisition.AveragePrice) + aquisition.TransactionCosts;
                }
                else if (transaction.Type == TransactionType.Disposal)
                {
                    var disposal = transaction as Disposal;
                    cashFlows[yearNumber] += (disposal.Units * disposal.AveragePrice) - disposal.TransactionCosts;
                }
                else if (transaction.Type == TransactionType.Income)
                {
                    var income = transaction as IncomeReceived;
                    cashFlows[yearNumber] += income.CashIncome;
                }
                else if (transaction.Type == TransactionType.OpeningBalance)
                {

                }
                else if (transaction.Type == TransactionType.ReturnOfCapital)
                {

                }

            }

            // Get the finaltfolio value
            var finalHoldings = Portfolio.ShareHoldingService.GetHoldings(endDate);
            cashFlows[cashFlows.Length - 1] += finalHoldings.Sum(x => x.MarketValue);
            */
            return 0.00m; 
        }
    }

    class HoldingItemViewModel
    {
        public string ASXCode { get; set; }
        public string CompanyName { get; set; }
        public int Units { get; set; }
        public decimal CurrentValue { get; set;  }
        public decimal CostBase { get; set; }
        public ChangeInValue ChangeInValue { get; set; }

        public HoldingItemViewModel(ShareHolding holding)
        {
            ASXCode = holding.Stock.ASXCode;
            CompanyName = string.Format("{0} ({1})", holding.Stock.Name, holding.Stock.ASXCode);
            Units = holding.Units;
            CostBase = holding.TotalCostBase;
            CurrentValue = 0.00m;
            ChangeInValue = new ChangeInValue(CostBase, CurrentValue);
        }

    }

    enum ChangeDirection { Increase, Decrease, Nuetral };
    struct ChangeInValue
    {
        public decimal Value { get; private set; }
        public decimal Percentage { get; private set; }
        public ChangeDirection Direction { get; private set; }

        public ChangeInValue(decimal originalValue, decimal currentValue)
        {
            Value = currentValue - originalValue;
            if (originalValue == 0)
                Percentage = 0;
            else
                Percentage = Value / originalValue;

            if (Value < 0)
                Direction = ChangeDirection.Decrease;
            else if (Value > 0)
                Direction = ChangeDirection.Increase;
            else
                Direction = ChangeDirection.Nuetral;
        }
    }

}
