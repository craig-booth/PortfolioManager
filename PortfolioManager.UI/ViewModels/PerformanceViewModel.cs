using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using PortfolioManager.UI.Utilities;
//using PortfolioManager.Service;
//using PortfolioManager.Service.Utils;
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

            PopulateStockPerformance();
            StockPerformance.Add(new ViewModels.StockPerformanceItem()
            {
                CompanyName = "Cash",
                OpeningBalance = openingCashBalance,
                Purchases = 0.00m,
                Sales = 0.00m,
                Dividends = Interest,
                CapitalGain = 0.00m,
                ClosingBalance = closingCashBalance,
                TotalReturn = Interest
            });
            StockPerformance.Add(new ViewModels.StockPerformanceItem()
            {
                CompanyName = "DRP Cash Balance",
                OpeningBalance = openingDRPCashBalance,
                Purchases = 0.00m,
                Sales = 0.00m,
                Dividends = 0.00m,
                CapitalGain = 0.00m,
                ClosingBalance = closingDRPCashBalance,
                TotalReturn = 0.00m
            });

            OnPropertyChanged(""); 
        }

        private void PopulateStockPerformance()
        {
            StockPerformance.Clear();

            // Add opening holdings
            var openingHoldings = _Parameter.Portfolio.ShareHoldingService.GetHoldings(_Parameter.StartDate);
            foreach (var holding in openingHoldings)
            {
                var stockPerfomance = new StockPerformanceItem()
                {
                    Stock = holding.Stock,
                    CompanyName = string.Format("{0} ({1})", holding.Stock.Name, holding.Stock.ASXCode),
                    OpeningBalance = holding.MarketValue,
                    Purchases = 0.00m,
                    Sales = 0.00m,
                    Dividends = 0.00m,
                    CapitalGain = 0.00m,
                    ClosingBalance = 0.00m
                };

                StockPerformance.Add(stockPerfomance);
            }

            // Process transactions during the period
            var transactions = _Parameter.Portfolio.TransactionService.GetTransactions(_Parameter.StartDate.AddDays(1), _Parameter.EndDate);
            foreach (var transaction in transactions)
            {
                var stockPerfomance = StockPerformance.FirstOrDefault(x => x.Stock.ASXCode == transaction.ASXCode);

                if (transaction.Type == TransactionType.Aquisition) 
                {
                    var aquisition = transaction as Aquisition;

                    if (stockPerfomance == null)
                    {
                        var stock = _Parameter.Portfolio.StockService.Get(aquisition.ASXCode, aquisition.TransactionDate);
                        stockPerfomance = new StockPerformanceItem()
                        {
                            Stock = stock,
                            CompanyName = string.Format("{0} ({1})", stock.Name, stock.ASXCode),
                            OpeningBalance = 0.00m,
                            Purchases = 0.00m,
                            Sales = 0.00m,
                            Dividends = 0.00m,
                            CapitalGain = 0.00m,
                            ClosingBalance = 0.00m
                        };

                        StockPerformance.Add(stockPerfomance);
                    }

                    stockPerfomance.Purchases += aquisition.Units * aquisition.AveragePrice;
                }
                else if (transaction.Type == TransactionType.OpeningBalance)
                {
                    var openingBalance = transaction as OpeningBalance;

                    if (stockPerfomance == null)
                    {
                        var stock = _Parameter.Portfolio.StockService.Get(openingBalance.ASXCode, openingBalance.TransactionDate);
                        stockPerfomance = new StockPerformanceItem()
                        {
                            Stock = stock,
                            CompanyName = string.Format("{0} ({1})", stock.Name, stock.ASXCode),
                            OpeningBalance = 0.00m,
                            Purchases = 0.00m,
                            Sales = 0.00m,
                            Dividends = 0.00m,
                            CapitalGain = 0.00m,
                            ClosingBalance = 0.00m
                        };

                        StockPerformance.Add(stockPerfomance);
                    }

                    stockPerfomance.Purchases += openingBalance.CostBase; 
                }
                else if (transaction.Type == TransactionType.Disposal)
                {
                    var disposal = transaction as Disposal;

                    if (stockPerfomance == null)
                    {
                        var stock = _Parameter.Portfolio.StockService.Get(disposal.ASXCode, disposal.TransactionDate);
                        stockPerfomance = new StockPerformanceItem()
                        {
                            Stock = stock,
                            CompanyName = string.Format("{0} ({1})", stock.Name, stock.ASXCode),
                            OpeningBalance = 0.00m,
                            Purchases = 0.00m,
                            Sales = 0.00m,
                            Dividends = 0.00m,
                            CapitalGain = 0.00m,
                            ClosingBalance = 0.00m
                        };

                        StockPerformance.Add(stockPerfomance);
                    }

                    stockPerfomance.Sales += disposal.Units * disposal.AveragePrice;
                }
                else if (transaction.Type == TransactionType.Income)
                {
                    var income = transaction as IncomeReceived;

                    if (stockPerfomance == null)
                    {
                        var stock = _Parameter.Portfolio.StockService.Get(income.ASXCode, income.RecordDate);
                        stockPerfomance = new StockPerformanceItem()
                        {
                            Stock = stock,
                            CompanyName = string.Format("{0} ({1})", stock.Name, stock.ASXCode),
                            OpeningBalance = 0.00m,
                            Purchases = 0.00m,
                            Sales = 0.00m,
                            Dividends = 0.00m,
                            CapitalGain = 0.00m,
                            ClosingBalance = 0.00m
                        };

                        StockPerformance.Add(stockPerfomance);
                    }

                    stockPerfomance.Dividends += income.CashIncome;
                }
            }

            // Update Closing Balance, Capital Gain and Total Return
            var closingHoldings = _Parameter.Portfolio.ShareHoldingService.GetHoldings(_Parameter.EndDate);
            foreach (var stockPerformance in StockPerformance)
            {
                var holding = closingHoldings.FirstOrDefault(x => x.Stock.Id == stockPerformance.Stock.Id);
                if (holding != null)
                    stockPerformance.ClosingBalance = holding.MarketValue;

                stockPerformance.CapitalGain = stockPerformance.ClosingBalance - (stockPerformance.OpeningBalance + stockPerformance.Purchases - stockPerformance.Sales);
                stockPerformance.TotalReturn = stockPerformance.CapitalGain + stockPerformance.Dividends;
            }

        }

    }


    public class StockPerformanceItem
    {
        public Stock Stock { get; set; }
        public string CompanyName { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Purchases { get; set; }
        public decimal Sales { get; set; }      
        public decimal ClosingBalance { get; set; }
        public decimal Dividends { get; set; }
        public decimal CapitalGain { get; set; }
        public decimal TotalReturn { get; set; }
    }
}
