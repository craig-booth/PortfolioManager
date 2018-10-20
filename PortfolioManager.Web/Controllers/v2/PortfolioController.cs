using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.RestApi.Portfolios;

namespace PortfolioManager.Web.Controllers.v2
{
    [Route("api/v2/portfolio/{portfolioId:guid}")]
    public class PortfolioController : BasePortfolioController
    {
        private IStockRepository _StockRepository;

        public PortfolioController(IPortfolioCache portfolioCache, IStockRepository stockRepository)
            : base(portfolioCache)
        {
            _StockRepository = stockRepository;
        }

        // GET: summary?date
        [Route("summary")]
        [HttpGet]
        public ActionResult<PortfolioSummaryResponse> GetSummary(DateTime? date)
        {
            if (date == null)
                date = DateTime.Today;

            var response = new PortfolioSummaryResponse()
            {
                PortfolioValue = 0.00m,
                PortfolioCost = 0.00m,

                Return1Year = null,
                Return3Year = null,
                Return5Year = null,
                ReturnAll = null,

                CashBalance = 0.00m
            };

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

            return response;
        }
    }

}
