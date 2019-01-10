﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Transactions;
using PortfolioManager.Domain.Utils;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Mapping;

namespace PortfolioManager.Web.Controllers.v2
{
    [Route("api/v2/portfolio/{portfolioId:guid}")]
    public class PortfolioController : BasePortfolioController
    {
        private IMapper _Mapper;
        private ITradingCalander _TradingCalander;

        public PortfolioController(IPortfolioCache portfolioCache, ITradingCalander tradingCalander,  IMapper mapper)
            : base(portfolioCache)
        {
            _Mapper = mapper;
            _TradingCalander = tradingCalander;
        }

        // GET: properties
        [Route("properties")]
        [HttpGet]
        public ActionResult<PortfolioPropertiesResponse> GetProperties()
        {
            var response = new PortfolioPropertiesResponse();

            foreach (var holding in _Portfolio.Holdings.All())
                response.StocksHeld.Add(holding.Stock.Convert(DateTime.Now));

            response.StartDate = _Portfolio.StartDate;
            response.EndDate = _Portfolio.EndDate;

            return response;
        }

        // GET: summary?date
        [Route("summary")]
        [HttpGet]
        public ActionResult<PortfolioSummaryResponse> GetSummary(DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            var response = new PortfolioSummaryResponse();

            response.Holdings.AddRange(_Portfolio.Holdings.All(requestedDate).Select(x => x.Convert(requestedDate)));
            response.CashBalance = _Portfolio.CashAccount.Balance(requestedDate);
            response.PortfolioValue = response.Holdings.Sum(x => x.Value) + response.CashBalance;
            response.PortfolioCost = response.Holdings.Sum(x => x.Cost) + response.CashBalance;

            var fromDate = requestedDate.AddYears(-1).AddDays(1);
            if (fromDate >= _Portfolio.StartDate)
                response.Return1Year = _Portfolio.CalculateIRR(new DateRange(fromDate, requestedDate));
            else
                response.Return1Year = null;

            fromDate = requestedDate.AddYears(-3).AddDays(1);
            if (fromDate >= _Portfolio.StartDate)
                response.Return3Year = _Portfolio.CalculateIRR(new DateRange(fromDate, requestedDate));
            else
                response.Return3Year = null;

            fromDate = requestedDate.AddYears(-5).AddDays(1);
            if (fromDate >= _Portfolio.StartDate)
                response.Return5Year = _Portfolio.CalculateIRR(new DateRange(fromDate, requestedDate));
            else
                response.Return5Year = null;

            if (requestedDate >= _Portfolio.StartDate)
                response.ReturnAll = _Portfolio.CalculateIRR(new DateRange(_Portfolio.StartDate, requestedDate));
            else
                response.ReturnAll = null;

            return response;
        }


        // GET: performance?fromDate&toDate
        [Route("performance")]
        [HttpGet]
        public ActionResult<PortfolioPerformanceResponse> GetPerformance(DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);
            var dateRangeExcludingFirstDay = new DateRange(dateRange.FromDate.AddDays(1), dateRange.ToDate);

            var openingHoldings = _Portfolio.Holdings.All(dateRange.FromDate);
            var closingHoldings = _Portfolio.Holdings.All(dateRange.ToDate);

            var response = new PortfolioPerformanceResponse();

            var workingList = new List<HoldingPerformanceWorkItem>();
            HoldingPerformanceWorkItem workItem;

            // Add opening holdings
            foreach (var holding in openingHoldings)
            {               
                workItem = new HoldingPerformanceWorkItem(holding.Stock.Convert(dateRange.FromDate));

                var value = holding.Value(dateRange.FromDate);
                workItem.HoldingPerformance.OpeningBalance = value;
                workItem.CashFlows.Add(dateRange.FromDate, -value);

                workingList.Add(workItem);
            }

            // Process transactions during the period
            var transactions = _Portfolio.Transactions.InDateRange(dateRangeExcludingFirstDay);
            foreach (var transaction in transactions)
            {
                if ((transaction is Aquisition) ||
                    (transaction is OpeningBalance) ||
                    (transaction is Disposal) ||
                    (transaction is IncomeReceived))
                {
                    workItem = workingList.FirstOrDefault(x => x.HoldingPerformance.Stock.Id == transaction.Stock.Id);
                    if (workItem == null)
                    {
                        workItem = new HoldingPerformanceWorkItem(transaction.Stock.Convert(transaction.RecordDate));
                        workingList.Add(workItem);
                    }

                    if (transaction is Aquisition)
                    {
                        var aquisition = transaction as Aquisition;

                        workItem.HoldingPerformance.Purchases += aquisition.Units * aquisition.AveragePrice;
                        workItem.CashFlows.Add(aquisition.Date, -(aquisition.Units * aquisition.AveragePrice));
                    }
                    else if (transaction is OpeningBalance)
                    {
                        var openingBalance = transaction as OpeningBalance;

                        workItem.HoldingPerformance.Purchases += openingBalance.CostBase;
                        workItem.CashFlows.Add(openingBalance.Date, -openingBalance.CostBase);
                    }
                    else if (transaction is Disposal)
                    {
                        var disposal = transaction as Disposal;

                        workItem.HoldingPerformance.Sales += disposal.Units * disposal.AveragePrice;
                        workItem.CashFlows.Add(disposal.Date, disposal.Units * disposal.AveragePrice);
                    }
                    else if (transaction is IncomeReceived)
                    {
                        var income = transaction as IncomeReceived;

                        workItem.HoldingPerformance.Dividends += income.CashIncome;
                        workItem.CashFlows.Add(income.Date, income.CashIncome);
                    }
                }
            }

            // Populate HoldingPerformance from work list
            foreach (var item in workingList)
            {
                var holding = closingHoldings.FirstOrDefault(x => x.Stock.Id == item.HoldingPerformance.Stock.Id);
                if (holding != null)
                {
                    var value = holding.Value(dateRange.ToDate);
                    item.HoldingPerformance.ClosingBalance = value;
                    item.CashFlows.Add(dateRange.ToDate, value);

                    item.HoldingPerformance.DRPCashBalance = holding.DrpAccount.Balance(dateRange.ToDate);
                }
                else
                    item.HoldingPerformance.ClosingBalance = 0.00m;

                item.HoldingPerformance.CapitalGain = item.HoldingPerformance.ClosingBalance - (item.HoldingPerformance.OpeningBalance + item.HoldingPerformance.Purchases - item.HoldingPerformance.Sales);
                item.HoldingPerformance.TotalReturn = item.HoldingPerformance.CapitalGain + item.HoldingPerformance.Dividends;

                var irr = IrrCalculator.CalculateIrr(item.CashFlows);
                item.HoldingPerformance.IRR = (decimal)Math.Round(irr, 5); 

                response.HoldingPerformance.Add(item.HoldingPerformance);
            }

            var cashTransactions = _Portfolio.CashAccount.Transactions.InDateRange(dateRangeExcludingFirstDay);
            response.OpeningCashBalance = _Portfolio.CashAccount.Balance(dateRange.FromDate);
            response.Deposits = cashTransactions.Where(x => x.Type == BankAccountTransactionType.Deposit).Sum(x => x.Amount);
            response.Withdrawls = cashTransactions.Where(x => x.Type == BankAccountTransactionType.Withdrawl).Sum(x => x.Amount);
            response.Interest = cashTransactions.Where(x => x.Type == BankAccountTransactionType.Interest).Sum(x => x.Amount);
            response.Fees = cashTransactions.Where(x => x.Type == BankAccountTransactionType.Fee).Sum(x => x.Amount);
            response.ClosingCashBalance = _Portfolio.CashAccount.Balance(dateRange.ToDate);

            response.OpeningBalance = openingHoldings.Sum(x => x.Value(dateRange.FromDate));
            response.Dividends = response.HoldingPerformance.Sum(x => x.Dividends);
            response.ChangeInMarketValue = response.HoldingPerformance.Sum(x => x.CapitalGain);
            response.OutstandingDRPAmount = -response.HoldingPerformance.Sum(x => x.DRPCashBalance);
            response.ClosingBalance = closingHoldings.Sum(x => x.Value(dateRange.ToDate));

            return response;
        }

        private class HoldingPerformanceWorkItem
        {
            public PortfolioPerformanceResponse.HoldingPerformanceItem HoldingPerformance;
            public CashFlows CashFlows;

            public HoldingPerformanceWorkItem(RestApi.Portfolios.Stock stock)
            {
                HoldingPerformance = new PortfolioPerformanceResponse.HoldingPerformanceItem();
                HoldingPerformance.Stock = stock;
                CashFlows = new CashFlows();
            }
        }

        // GET: value?fromDate&toDate
        [Route("value")]
        [HttpGet]
        public ActionResult<PortfolioValueResponse> GetValue(DateTime? fromDate, DateTime? toDate, ValueFrequency? frequency)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var response = new PortfolioValueResponse();

            IEnumerable<DateTime> dates = null;
            if ((frequency == null) || (frequency == ValueFrequency.Daily))
                dates = _TradingCalander.TradingDays(dateRange);
            else if (frequency == ValueFrequency.Weekly)
                dates = DateUtils.WeekEndingDays(dateRange.FromDate, dateRange.ToDate);
            else if (frequency == ValueFrequency.Monthly)
                dates = DateUtils.MonthEndingDays(dateRange.FromDate, dateRange.ToDate);

            var holdings = _Portfolio.Holdings.All(dateRange);
            var closingBalances = _Portfolio.CashAccount.EffectiveBalances(dateRange);
            var closingBalanceEnumerator = closingBalances.GetEnumerator();
            closingBalanceEnumerator.MoveNext();

            foreach (var date in dates)
            {
                var amount = 0.00m;

                // Add holding values
                foreach (var holding in holdings)
                    amount += holding.Value(date);

                // Add cash account balances
                if (date > closingBalanceEnumerator.Current.EffectivePeriod.ToDate)
                    closingBalanceEnumerator.MoveNext();
                amount += closingBalanceEnumerator.Current.Balance;

                var value = new PortfolioValueResponse.ValueItem()
                {
                    Date = date,
                    Amount = amount
                };

                response.Values.Add(value);
            }

            

            return response;
        }

        // GET: transactions?fromDate&toDate
        // GET: transactions?stock&fromDate&toDate
        [Route("transactions")]
        [HttpGet]
        public ActionResult<TransactionsResponse> GetTransactions(Guid? Stock, DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var response = new TransactionsResponse();

            foreach (var transaction in _Portfolio.Transactions.InDateRange(dateRange))
            {
                var t = _Mapper.Map<TransactionsResponse.TransactionItem>(transaction, opts => opts.Items["date"] = dateRange.ToDate);
                response.Transactions.Add(t);
            }

            return response;
        }

        // GET: capitalgains?date
        // GET: capitalgains?stock&date
        [Route("capitalgains")]
        [HttpGet]
        public ActionResult<SimpleUnrealisedGainsResponse> GetCapitalGains(Guid? stock, DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            var response = new SimpleUnrealisedGainsResponse();

            foreach (var holding in _Portfolio.Holdings.All(requestedDate))
            {
                foreach (var parcel in holding.Parcels(requestedDate))
                {
                    var properties = parcel.Properties[requestedDate];

                    var value = properties.Units * holding.Stock.GetPrice(requestedDate);
                    var capitalGain = value - properties.CostBase;
                    var discountMethod = CgtCalculator.CgtMethodForParcel(parcel.AquisitionDate, requestedDate);
                    var discoutedGain = (discountMethod == CGTMethod.Discount) ? CgtCalculator.CgtDiscount(capitalGain) : capitalGain;

                    var unrealisedGain = new SimpleUnrealisedGainsItem()
                    {
                        Stock = holding.Stock.Convert(requestedDate),
                        AquisitionDate = parcel.AquisitionDate,
                        Units = properties.Units,
                        CostBase = properties.CostBase,
                        MarketValue = value,
                        CapitalGain = capitalGain,
                        DiscoutedGain = discoutedGain,
                        DiscountMethod = discountMethod
                    };

                    response.UnrealisedGains.Add(unrealisedGain);

                }
            }

            return response;
        }

        // GET: detailedcapitalgains?date
        // GET: detailedcapitalgains?stock&date
        [Route("detailedcapitalgains")]
        [HttpGet]
        public ActionResult<DetailedUnrealisedGainsResponse> GetDetailedCapitalGains(Guid? stock, DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            var response = new DetailedUnrealisedGainsResponse();

            foreach (var holding in _Portfolio.Holdings.All(requestedDate))
            {
                foreach (var parcel in holding.Parcels(requestedDate))
                {
                    var properties = parcel.Properties[requestedDate];

                    var value = properties.Units * holding.Stock.GetPrice(requestedDate);
                    var capitalGain = value - properties.CostBase;
                    var discountMethod = CgtCalculator.CgtMethodForParcel(parcel.AquisitionDate, requestedDate);
                    var discoutedGain = (discountMethod == CGTMethod.Discount) ? CgtCalculator.CgtDiscount(capitalGain) : capitalGain;

                    var unrealisedGain = new DetailedUnrealisedGainsItem()
                    {
                        Stock = holding.Stock.Convert(requestedDate),
                        AquisitionDate = parcel.AquisitionDate,
                        Units = properties.Units,
                        CostBase = properties.CostBase,
                        MarketValue = value,
                        CapitalGain = capitalGain,
                        DiscoutedGain = discoutedGain,
                        DiscountMethod = discountMethod
                    };

                    int units = 0;
                    decimal costBase = 0.00m;
                    foreach (var auditRecord in parcel.Audit.TakeWhile(x => x.Date <= date))
                    {
                        units += auditRecord.UnitCountChange;
                        costBase += auditRecord.CostBaseChange;

                        var cgtEvent = new DetailedUnrealisedGainsItem.CGTEventItem()
                        {                           
                            Date = auditRecord.Date,
                            Description = auditRecord.Transaction.Description,
                            Units = units,
                            CostBaseChange = auditRecord.CostBaseChange,
                            CostBase = costBase,
                        };

                        unrealisedGain.CGTEvents.Add(cgtEvent);
                    }

                    response.UnrealisedGains.Add(unrealisedGain);

                }
            }

            return response;
        }

        // GET: cgtliability?fromDate&toDate
        [Route("cgtliability")]
        [HttpGet]
        public ActionResult<CgtLiabilityResponse> GetCGTLiability(DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var response = new CgtLiabilityResponse();

            // Get a list of all the cgt events for the year
            var cgtEvents = _Portfolio.CgtEvents.InDateRange(dateRange);
            foreach (var cgtEvent in cgtEvents)
            {
                var item = new CgtLiabilityResponse.CgtLiabilityEvent()
                {
                    Stock = cgtEvent.Stock.Convert(cgtEvent.Date),
                    EventDate = cgtEvent.Date,
                    CostBase = cgtEvent.CostBase,
                    AmountReceived = cgtEvent.AmountReceived,
                    CapitalGain = cgtEvent.CapitalGain,
                    Method = cgtEvent.CgtMethod
                };

                response.Events.Add(item);
            
                // Apportion capital gains
                if (cgtEvent.CapitalGain < 0)
                    response.CurrentYearCapitalLossesTotal += -cgtEvent.CapitalGain;
                else if (cgtEvent.CgtMethod == CGTMethod.Discount)
                    response.CurrentYearCapitalGainsDiscounted += cgtEvent.CapitalGain;
                else
                    response.CurrentYearCapitalGainsOther += cgtEvent.CapitalGain;
            }

            response.CurrentYearCapitalGainsTotal = response.CurrentYearCapitalGainsOther + response.CurrentYearCapitalGainsDiscounted;

            if (response.CurrentYearCapitalGainsOther > response.CurrentYearCapitalLossesTotal)
                response.CurrentYearCapitalLossesOther = response.CurrentYearCapitalLossesTotal;
            else
                response.CurrentYearCapitalLossesOther = response.CurrentYearCapitalGainsOther;

            if (response.CurrentYearCapitalGainsOther > response.CurrentYearCapitalLossesTotal)
                response.CurrentYearCapitalLossesDiscounted = 0.00m;
            else
                response.CurrentYearCapitalLossesDiscounted = response.CurrentYearCapitalLossesTotal - response.CurrentYearCapitalGainsOther;

            response.GrossCapitalGainOther = response.CurrentYearCapitalGainsOther - response.CurrentYearCapitalLossesOther;
            response.GrossCapitalGainDiscounted = response.CurrentYearCapitalGainsDiscounted - response.CurrentYearCapitalLossesDiscounted;
            response.GrossCapitalGainTotal = response.GrossCapitalGainOther + response.GrossCapitalGainDiscounted;
            if (response.GrossCapitalGainDiscounted > 0)
                response.Discount = (response.GrossCapitalGainDiscounted / 2).ToCurrency(RoundingRule.Round);
            else
                response.Discount = 0.00m;
            response.NetCapitalGainOther = response.GrossCapitalGainOther;
            response.NetCapitalGainDiscounted = response.GrossCapitalGainDiscounted - response.Discount;
            response.NetCapitalGainTotal = response.NetCapitalGainOther + response.NetCapitalGainDiscounted;

            return response;
        }

        // GET: cashaccount?fromDate&toDate
        [Route("cashaccount")]
        [HttpGet]
        public ActionResult<CashAccountTransactionsResponse> GetCashAccountTransactions(DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var response = new CashAccountTransactionsResponse();

            var transactions = _Portfolio.CashAccount.Transactions.InDateRange(dateRange);

            response.OpeningBalance = _Portfolio.CashAccount.Balance(dateRange.FromDate);
            response.ClosingBalance = _Portfolio.CashAccount.Balance(dateRange.ToDate);

            response.Transactions.AddRange(_Mapper.Map<IEnumerable<CashAccountTransactionsResponse.TransactionItem>>(transactions));

            return response;
        }

        // GET: income?fromDate&toDate
        [Route("income")]
        [HttpGet]
        public ActionResult<IncomeResponse> GetIncome(DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var response = new IncomeResponse();

            var incomes = _Portfolio.Transactions.InDateRange(dateRange)
                .Where(x => x is IncomeReceived)
                .Select(x => x as IncomeReceived)
                .GroupBy(x => x.Stock,
                        x => x,
                        (key, result) => new IncomeResponse.IncomeItem()
                        {
                            Stock = key.Convert(dateRange.ToDate),
                            UnfrankedAmount = result.Sum(x => x.UnfrankedAmount),
                            FrankedAmount = result.Sum(x => x.FrankedAmount),
                            FrankingCredits = result.Sum(x => x.FrankingCredits),
                            NettIncome = result.Sum(x => x.CashIncome),
                            GrossIncome = result.Sum(x => x.TotalIncome)
                        });

            response.Income.AddRange(incomes);

            return response;
        }

        // GET: corporateactions
        [Route("corporateactions")]
        [HttpGet]
        public ActionResult<CorporateActionsResponse> GetCorporateActions()
        {
            var response = new CorporateActionsResponse();

            return response;
        }
    }

}
