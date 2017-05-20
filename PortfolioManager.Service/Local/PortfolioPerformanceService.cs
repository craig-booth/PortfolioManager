using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Service.Utils;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;

using PortfolioManager.Model.Data;
using PortfolioManager.Service.Interface;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.Local
{

    class PortfolioPerformanceService : IPortfolioPerformanceService
    {
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly Obsolete.ShareHoldingService _ShareHoldingService;
        private readonly Obsolete.StockService _StockService;

        public PortfolioPerformanceService(IPortfolioQuery portfolioQuery, Obsolete.ShareHoldingService shareHoldingService, Obsolete.StockService stockService)
        {
            _PortfolioQuery = portfolioQuery;
            _ShareHoldingService = shareHoldingService;
            _StockService = stockService;
        }

        public Task<PortfolioPerformanceResponce> GetPerformance(DateTime fromDate, DateTime toDate)
        {
            var responce = new PortfolioPerformanceResponce();

            var cashTransactions = _PortfolioQuery.GetCashAccountTransactions(fromDate, toDate);          
            responce.OpeningCashBalance = _PortfolioQuery.GetCashBalance(fromDate);           
            responce.Deposits = cashTransactions.Where(x => x.Type == BankAccountTransactionType.Deposit).Sum(x => x.Amount);
            responce.Withdrawls = cashTransactions.Where(x => x.Type == BankAccountTransactionType.Withdrawl).Sum(x => x.Amount);
            responce.Interest = cashTransactions.Where(x => x.Type == BankAccountTransactionType.Interest).Sum(x => x.Amount);
            responce.Fees = cashTransactions.Where(x => x.Type == BankAccountTransactionType.Fee).Sum(x => x.Amount);
            responce.ClosingCashBalance = _PortfolioQuery.GetCashBalance(toDate);

            var openingHoldings = _ShareHoldingService.GetHoldings(fromDate);
            var closingHoldings = _ShareHoldingService.GetHoldings(toDate);      
                     
            responce.HoldingPerformance = CalculateHoldingPerformance(fromDate, toDate, openingHoldings, closingHoldings);
            responce.OpeningBalance = openingHoldings.Sum(x => x.MarketValue);
            responce.Dividends = responce.HoldingPerformance.Sum(x => x.Dividends);
            responce.ChangeInMarketValue = responce.HoldingPerformance.Sum(x => x.CapitalGain);
            responce.OutstandingDRPAmount = -responce.HoldingPerformance.Sum(x => x.DRPCashBalance);
            responce.ClosingBalance = closingHoldings.Sum(x => x.MarketValue);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<PortfolioPerformanceResponce>(responce);
        }

        private class HoldingPerformanceWorkItem
        {
            public HoldingPerformance HoldingPerformance;
            public CashFlows CashFlows;

            public HoldingPerformanceWorkItem(Stock stock)
            {
                HoldingPerformance = new HoldingPerformance();
                HoldingPerformance.Stock = new StockItem(stock);
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
            var transactions = _PortfolioQuery.GetTransactions(startDate.AddDays(1), endDate);
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

                workItem = workingList.FirstOrDefault(x => x.HoldingPerformance.Stock.Id == stock.Id);
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
                var holding = closingHoldings.FirstOrDefault(x => x.Stock.Id == item.HoldingPerformance.Stock.Id);
                if (holding != null)
                {
                    item.HoldingPerformance.ClosingBalance = holding.MarketValue;
                    item.CashFlows.Add(endDate, holding.MarketValue);

                    item.HoldingPerformance.DRPCashBalance = _PortfolioQuery.GetDRPCashBalance(holding.Stock.Id, endDate);
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



}
