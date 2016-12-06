using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using PortfolioManager.UI.Utilities;
using PortfolioManager.Service;
using PortfolioManager.Service.Utils;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.UI.ViewModels
{
    class PerformanceViewModel : PortfolioViewModel
    {

        public decimal OpeningBalance { get; private set; }
        public decimal Deposits { get; private set; }
        public decimal Withdrawls { get; private set; }
        public decimal Interest { get; private set; }
        public decimal Dividends { get; private set; }
        public decimal Fees { get; private set; }
        public decimal CapitalGains { get; private set; }
        public decimal ClosingBalance { get; private set; }

        public ObservableCollection<StockPerformanceItem> StockPerformance { get; private set; }

        public PerformanceViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Options.AllowStockSelection = false;
            Options.DateSelection = DateSelectionType.Range;

            StockPerformance = new ObservableCollection<StockPerformanceItem>();
        }


        public override void RefreshView()
        {
            var openingHoldings = _Parameter.Portfolio.ShareHoldingService.GetHoldings(_Parameter.StartDate);
            var openingCashBalance = _Parameter.Portfolio.CashAccountService.GetBalance(_Parameter.StartDate);

            decimal openingDRPCashBalance = 0.00m;
            foreach (var holding in openingHoldings)
                openingDRPCashBalance += _Parameter.Portfolio.IncomeService.GetDRPCashBalance(holding.Stock, _Parameter.StartDate);

            OpeningBalance = openingHoldings.Sum(x => x.MarketValue) + openingCashBalance + openingDRPCashBalance;

            var closingHoldings = _Parameter.Portfolio.ShareHoldingService.GetHoldings(_Parameter.EndDate);
            var closingCashBalance = _Parameter.Portfolio.CashAccountService.GetBalance(_Parameter.EndDate);
            decimal closingDRPCashBalance = 0.00m;
            foreach (var holding in closingHoldings)
                closingDRPCashBalance += _Parameter.Portfolio.IncomeService.GetDRPCashBalance(holding.Stock, _Parameter.EndDate);
            ClosingBalance = closingHoldings.Sum(x => x.MarketValue) + closingCashBalance + closingDRPCashBalance;

            var cashTransactions = _Parameter.Portfolio.CashAccountService.GetTransactions(_Parameter.StartDate, _Parameter.EndDate);

            Deposits = cashTransactions.Where(x => x.Type == CashAccountTransactionType.Deposit).Sum(x => x.Amount);
            Withdrawls = cashTransactions.Where(x => x.Type == CashAccountTransactionType.Withdrawl).Sum(x => x.Amount);
            Interest = cashTransactions.Where(x => x.Type == CashAccountTransactionType.Interest).Sum(x => x.Amount);
            Fees = cashTransactions.Where(x => x.Type == CashAccountTransactionType.Fee).Sum(x => x.Amount);

            var income = _Parameter.Portfolio.IncomeService.GetIncome(_Parameter.StartDate, _Parameter.EndDate);
            Dividends = income.Sum(x => x.CashIncome);

            CapitalGains = ClosingBalance - (Dividends + Interest + Fees) - (Deposits + Withdrawls) - OpeningBalance;

            PopulateStockPerformance(openingHoldings, closingHoldings);
            StockPerformance.Add(new ViewModels.StockPerformanceItem("Cash", openingCashBalance)
            {
                Dividends = Interest,
                ClosingBalance = closingCashBalance
            });
            StockPerformance.Add(new ViewModels.StockPerformanceItem("DRP Cash Balance", openingDRPCashBalance)
            { 
                ClosingBalance = closingDRPCashBalance,
                TotalReturn = 0.00m
            });

            OnPropertyChanged(""); 
        }

        private void PopulateStockPerformance(IEnumerable<ShareHolding> openingHoldings, IEnumerable<ShareHolding> closingHoldings)
        {

            StockPerformance.Clear();
        

            // Add opening holdings
            foreach (var holding in openingHoldings)
            {
                var stockPerfomance = new StockPerformanceItem(holding.Stock, holding.MarketValue);
                StockPerformance.Add(stockPerfomance);

                stockPerfomance._CashFlows.Add(_Parameter.StartDate, -holding.MarketValue);
            }

            // Process transactions during the period
            var transactions = _Parameter.Portfolio.TransactionService.GetTransactions(_Parameter.StartDate.AddDays(1), _Parameter.EndDate);
            foreach (var transaction in transactions)
            {               
                var stockPerformance = StockPerformance.FirstOrDefault(x => x.Stock.ASXCode == transaction.ASXCode);

                if (transaction.Type == TransactionType.Aquisition) 
                {
                    var aquisition = transaction as Aquisition;

                    if (stockPerformance == null)
                    {
                        var stock = _Parameter.Portfolio.StockService.Get(aquisition.ASXCode, aquisition.TransactionDate);
                        stockPerformance = new StockPerformanceItem(stock, 0.00m);
                        StockPerformance.Add(stockPerformance);
                    }

                    stockPerformance.Purchases += aquisition.Units * aquisition.AveragePrice;                  
                    stockPerformance._CashFlows.Add(aquisition.TransactionDate, -(aquisition.Units * aquisition.AveragePrice));
                }
                else if (transaction.Type == TransactionType.OpeningBalance)
                {
                    var openingBalance = transaction as OpeningBalance;

                    if (stockPerformance == null)
                    {
                        var stock = _Parameter.Portfolio.StockService.Get(openingBalance.ASXCode, openingBalance.TransactionDate);
                        stockPerformance = new StockPerformanceItem(stock, 0.00m);
                        StockPerformance.Add(stockPerformance);
                    }

                    stockPerformance.Purchases += openingBalance.CostBase;
                    stockPerformance._CashFlows.Add(openingBalance.TransactionDate, -openingBalance.CostBase);
                }
                else if (transaction.Type == TransactionType.Disposal)
                {
                    var disposal = transaction as Disposal;

                    if (stockPerformance == null)
                    {
                        var stock = _Parameter.Portfolio.StockService.Get(disposal.ASXCode, disposal.TransactionDate);
                        stockPerformance = new StockPerformanceItem(stock, 0.00m);
                        StockPerformance.Add(stockPerformance);
                    }

                    stockPerformance.Sales += disposal.Units * disposal.AveragePrice;
                    stockPerformance._CashFlows.Add(disposal.TransactionDate, disposal.Units * disposal.AveragePrice);
                }
                else if (transaction.Type == TransactionType.Income)
                {
                    var income = transaction as IncomeReceived;

                    if (stockPerformance == null)
                    {
                        var stock = _Parameter.Portfolio.StockService.Get(income.ASXCode, income.RecordDate);
                        stockPerformance = new StockPerformanceItem(stock, 0.00m);
                        StockPerformance.Add(stockPerformance);
                    }

                    stockPerformance.Dividends += income.CashIncome;
                    stockPerformance._CashFlows.Add(income.TransactionDate, income.CashIncome);
                }
            }

            // Update Closing Balance, Capital Gain and Total Return
            foreach (var stockPerformance in StockPerformance)
            {
                var holding = closingHoldings.FirstOrDefault(x => x.Stock.Id == stockPerformance.Stock.Id);
                if (holding != null)
                {
                    stockPerformance.ClosingBalance = holding.MarketValue;
                    stockPerformance._CashFlows.Add(_Parameter.EndDate, holding.MarketValue);
                }
                else
                    stockPerformance.ClosingBalance = 0.00m;

                stockPerformance.CapitalGain = stockPerformance.ClosingBalance - (stockPerformance.OpeningBalance + stockPerformance.Purchases - stockPerformance.Sales);
                stockPerformance.TotalReturn = stockPerformance.CapitalGain + stockPerformance.Dividends;

                stockPerformance.IRR = IRRCalculator.CalculateIRR(stockPerformance._CashFlows);       
            }
        }

    }



    public class StockPerformanceItem
    {
        internal CashFlows _CashFlows;

        public Stock Stock { get;  }
        public string CompanyName { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Purchases { get; set; }
        public decimal Sales { get; set; }      
        public decimal ClosingBalance { get; set; }
        public decimal Dividends { get; set; }
        public decimal CapitalGain { get; set; }
        public decimal TotalReturn { get; set; }
        public double IRR { get; set; }

        public StockPerformanceItem(string description, decimal openingBalance)
        {
            _CashFlows = new CashFlows();

            CompanyName = description;

            OpeningBalance = openingBalance;
            Purchases = 0.00m;
            Sales = 0.00m;
            Dividends = 0.00m;
            CapitalGain = 0.00m;
            ClosingBalance = 0.00m;
        }

        public StockPerformanceItem(Stock stock, decimal openingBalance)
            : this(string.Format("{0} ({1})", stock.Name, stock.ASXCode), openingBalance)
        {        
            Stock = stock; 
        }



    }
}
