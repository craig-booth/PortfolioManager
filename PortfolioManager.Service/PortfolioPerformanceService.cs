using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service;
using PortfolioManager.Service.Utils;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Service
{

    public class PortfolioPerformanceService
    {
        private readonly ShareHoldingService _ShareHoldingService;
        private readonly CashAccountService _CashAccountService;
        private readonly TransactionService _TransactionService;
        private readonly StockService _StockService;
        private readonly IncomeService _IncomeService;

        public PortfolioPerformanceService(ShareHoldingService shareHoldingService, CashAccountService cashAccountService, TransactionService transactionService, StockService stockService, IncomeService incomeService)
        {
            _ShareHoldingService = shareHoldingService;
            _CashAccountService = cashAccountService;
            _TransactionService = transactionService;
            _StockService = stockService;
            _IncomeService = incomeService;
        }

        public Task<PortfolioPerformanceResponce> GetPerformance(DateTime fromDate, DateTime toDate)
        {
            var responce = new PortfolioPerformanceResponce();

            var cashTransactions = _CashAccountService.GetTransactions(fromDate, toDate);          
            responce.OpeningCashBalance = _CashAccountService.GetBalance(fromDate);           
            responce.Deposits = cashTransactions.Where(x => x.Type == CashAccountTransactionType.Deposit).Sum(x => x.Amount);
            responce.Withdrawls = cashTransactions.Where(x => x.Type == CashAccountTransactionType.Withdrawl).Sum(x => x.Amount);
            responce.Interest = cashTransactions.Where(x => x.Type == CashAccountTransactionType.Interest).Sum(x => x.Amount);
            responce.Fees = cashTransactions.Where(x => x.Type == CashAccountTransactionType.Fee).Sum(x => x.Amount);
            responce.ClosingCashBalance = _CashAccountService.GetBalance(toDate);

            var openingHoldings = _ShareHoldingService.GetHoldings(fromDate);
            var closingHoldings = _ShareHoldingService.GetHoldings(toDate);      
                     
            responce.HoldingPerformance = CalculateHoldingPerformance(fromDate, toDate, openingHoldings, closingHoldings);
            responce.OpeningBalance = openingHoldings.Sum(x => x.MarketValue);
            responce.Dividends = responce.HoldingPerformance.Sum(x => x.Dividends);
            responce.ChangeInMarketValue = responce.HoldingPerformance.Sum(x => x.CapitalGain);
            responce.OutstandingDRPAmount = -responce.HoldingPerformance.Sum(x => x.DRPCashBalance);
            responce.ClosingBalance = closingHoldings.Sum(x => x.MarketValue);


            return Task.FromResult<PortfolioPerformanceResponce>(responce);
        }

        private class HoldingPerformanceWorkItem
        {
            public HoldingPerformance HoldingPerformance;
            public Stock Stock;
            public CashFlows CashFlows;

            public HoldingPerformanceWorkItem(Stock stock)
            {
                HoldingPerformance = new HoldingPerformance(stock.ASXCode, stock.Name);
                Stock = stock;
                CashFlows = new Utils.CashFlows();
            }
        }

        private List<HoldingPerformance> CalculateHoldingPerformance(DateTime startDate, DateTime endDate, IEnumerable<ShareHolding> openingHoldings, IEnumerable<ShareHolding> closingHoldings)
        {

            var workingList = new List<HoldingPerformanceWorkItem>();
            HoldingPerformanceWorkItem workItem;

            // Add opening holdings
            foreach (var holding in openingHoldings)
            {
                workItem = new HoldingPerformanceWorkItem(holding.Stock);            
                workItem.HoldingPerformance.OpeningBalance = holding.MarketValue;
                workItem.CashFlows.Add(startDate, -holding.MarketValue);

                workingList.Add(workItem);
            }

            // Process transactions during the period
            var transactions = _TransactionService.GetTransactions(startDate.AddDays(1), endDate);
            foreach (var transaction in transactions)
            {
                if ((transaction.Type != TransactionType.Aquisition) &&
                            (transaction.Type != TransactionType.OpeningBalance) &&
                            (transaction.Type != TransactionType.Disposal) &&
                            (transaction.Type != TransactionType.Income))
                    continue;


                var stock = _StockService.Get(transaction.ASXCode, transaction.RecordDate);
                if (stock.ParentId != Guid.Empty)
                    stock = _StockService.Get(stock.ParentId, transaction.RecordDate);

                workItem = workingList.FirstOrDefault(x => x.Stock.Id == stock.Id);
                if (workItem == null)
                {
                    workItem = new HoldingPerformanceWorkItem(stock);
                    workingList.Add(workItem);
                }

                if (transaction.Type == TransactionType.Aquisition)
                {
                    var aquisition = transaction as Aquisition;

                    workItem.HoldingPerformance.Purchases = aquisition.Units * aquisition.AveragePrice;
                    workItem.CashFlows.Add(aquisition.TransactionDate, -(aquisition.Units * aquisition.AveragePrice));
                }
                else if (transaction.Type == TransactionType.OpeningBalance)
                {
                    var openingBalance = transaction as OpeningBalance;

                    workItem.HoldingPerformance.Purchases += openingBalance.CostBase;
                    workItem.CashFlows.Add(openingBalance.TransactionDate, -openingBalance.CostBase);
                }
                else if (transaction.Type == TransactionType.Disposal)
                {
                    var disposal = transaction as Disposal;

                    workItem.HoldingPerformance.Sales += disposal.Units * disposal.AveragePrice;
                    workItem.CashFlows.Add(disposal.TransactionDate, disposal.Units * disposal.AveragePrice);
                }
                else if (transaction.Type == TransactionType.Income)
                {
                    var income = transaction as IncomeReceived;

                    workItem.HoldingPerformance.Dividends += income.CashIncome;
                    workItem.CashFlows.Add(income.TransactionDate, income.CashIncome);
                } 
            }

            // Populate HoldingPerformace from work list
            var holdingPerformance = new List<HoldingPerformance>();
            foreach (var item in workingList)
            {
                var holding = closingHoldings.FirstOrDefault(x => x.Stock.Id == item.Stock.Id);
                if (holding != null)
                {
                    item.HoldingPerformance.ClosingBalance = holding.MarketValue;
                    item.CashFlows.Add(endDate, holding.MarketValue);

                    item.HoldingPerformance.DRPCashBalance = _IncomeService.GetDRPCashBalance(holding.Stock, endDate);
                }
                else
                    item.HoldingPerformance.ClosingBalance = 0.00m;

                item.HoldingPerformance.CapitalGain = item.HoldingPerformance.ClosingBalance - (item.HoldingPerformance.OpeningBalance + item.HoldingPerformance.Purchases - item.HoldingPerformance.Sales);
                item.HoldingPerformance.TotalReturn = item.HoldingPerformance.CapitalGain + item.HoldingPerformance.Dividends;

                item.HoldingPerformance.IRR = IRRCalculator.CalculateIRR(item.CashFlows);

                holdingPerformance.Add(item.HoldingPerformance);
            }
            
            return holdingPerformance;
        }
    }

    public class PortfolioPerformanceResponce
    {
        public decimal OpeningBalance { get; set; }
        public decimal Dividends { get; set; }
        public decimal ChangeInMarketValue { get; set; }
        public decimal OutstandingDRPAmount { get; set; }
        public decimal ClosingBalance { get; set; }

        public decimal OpeningCashBalance { get; set; }
        public decimal Deposits { get; set; }
        public decimal Withdrawls { get; set; }
        public decimal Interest { get; set; }
        public decimal Fees { get; set; }
        public decimal ClosingCashBalance { get; set; }
        

        public List<HoldingPerformance> HoldingPerformance { get; set; }

        public PortfolioPerformanceResponce()
        {
    
        }
    }

    public class HoldingPerformance
    {
        public string ASXCode { get; set; }
        public string CompanyName { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Purchases { get; set; }
        public decimal Sales { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal Dividends { get; set; }
        public decimal CapitalGain { get; set; }
        public decimal DRPCashBalance { get; set; }
        public decimal TotalReturn { get; set; }
        public double IRR { get; set; }

        public HoldingPerformance(string asxCode, string companyName)
        {
            ASXCode = asxCode;
            CompanyName = companyName;
            OpeningBalance = 0.00m;
            Purchases = 0.00m;
            Sales = 0.00m;
            Dividends = 0.00m;
            CapitalGain = 0.00m;
            ClosingBalance = 0.00m;
            DRPCashBalance = 0.00m;
        }

    }

}
