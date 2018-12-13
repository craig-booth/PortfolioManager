using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Mapping;

namespace PortfolioManager.Web.Controllers.v2
{
    [Route("api/v2/portfolio/{portfolioId:guid}")]
    public class PortfolioController : BasePortfolioController
    {
        private IMapper _Mapper;

        public PortfolioController(IPortfolioCache portfolioCache, IMapper mapper)
            : base(portfolioCache)
        {
            _Mapper = mapper;
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

            foreach (var holding in _Portfolio.Holdings.All(requestedDate))
            {
                var properties = holding.Properties[requestedDate];
                response.Holdings.Add(new RestApi.Portfolios.Holding()
                {
                    Stock = holding.Stock.Convert(requestedDate),
                    Units = properties.Units,
                    Cost = properties.Amount,
                    CostBase = properties.CostBase,
                    Value = properties.Units * holding.Stock.GetPrice(requestedDate)
                });
            }
            response.CashBalance = _Portfolio.CashAccount.Balance(requestedDate);

            response.PortfolioValue = response.Holdings.Sum(x => x.Value) + response.CashBalance;
            response.PortfolioCost = response.Holdings.Sum(x => x.Cost) + response.CashBalance;

            response.Return1Year = null;
            response.Return3Year = null;
            response.Return5Year = null;
            response.ReturnAll = null;       

            return response;
        }


        // GET: performance?fromDate&toDate
        [Route("performance")]
        [HttpGet]
        public ActionResult<PortfolioPerformanceResponse> GetPerformance(DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var response = new PortfolioPerformanceResponse()
            {
                OpeningBalance = 0.00m,
                Dividends = 0.00m,
                ChangeInMarketValue = 0.00m,
                OutstandingDRPAmount = 0.00m,
                ClosingBalance = 0.00m,

                OpeningCashBalance = 0.00m,
                Deposits = 0.00m,
                Withdrawls = 0.00m,
                Interest = 0.00m,
                Fees = 0.00m,
                ClosingCashBalance = 0.00m,
            };

            return response;
        }

        // GET: value?fromDate&toDate
        [Route("value")]
        [HttpGet]
        public ActionResult<PortfolioValueResponse> GetValue(DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var response = new PortfolioValueResponse();

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
            if (date == null)
                date = DateTime.Today;

            var response = new SimpleUnrealisedGainsResponse();

            return response;
        }

        // GET: detailedcapitalgains?date
        // GET: detailedcapitalgains?stock&date
        [Route("detailedcapitalgains")]
        [HttpGet]
        public ActionResult<DetailedUnrealisedGainsResponse> GetDetailedCapitalGains(Guid? stock, DateTime? date)
        {
            if (date == null)
                date = DateTime.Today;

            var response = new DetailedUnrealisedGainsResponse();

            return response;
        }

        // GET: cgtliability?fromDate&toDate
        [Route("cgtliability")]
        [HttpGet]
        public ActionResult<CGTLiabilityResponse> GetCGTLiability(DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var response = new CGTLiabilityResponse();

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
    }

}
