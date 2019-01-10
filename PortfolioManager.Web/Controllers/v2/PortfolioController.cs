using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Services;

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
            var service = new PortfolioPropertiesService(_Portfolio);

            return service.GetProperties();
        }

        // GET: summary?date
        [Route("summary")]
        [HttpGet]
        public ActionResult<PortfolioSummaryResponse> GetSummary(DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            var service = new PortfolioSummaryService(_Portfolio);

            return service.GetSummary(requestedDate);        
        }


        // GET: performance?fromDate&toDate
        [Route("performance")]
        [HttpGet]
        public ActionResult<PortfolioPerformanceResponse> GetPerformance(DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var service = new PortfolioPerformanceService(_Portfolio);

            return service.GetPerformance(dateRange);
        }

        // GET: value?fromDate&toDate
        [Route("value")]
        [HttpGet]
        public ActionResult<PortfolioValueResponse> GetValue(DateTime? fromDate, DateTime? toDate, ValueFrequency? frequency)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);
            var requestedFrequency = (frequency != null) ? (ValueFrequency)frequency : ValueFrequency.Daily;

            var service = new PortfolioValueService(_Portfolio, _TradingCalander);

            return service.GetValue(dateRange, requestedFrequency);
        }

        // GET: transactions?fromDate&toDate
        [Route("transactions")]
        [HttpGet]
        public ActionResult<TransactionsResponse> GetTransactions(DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var service = new PortfolioTransactionService(_Portfolio, _Mapper);

            return service.GetTransactions(dateRange);

        }

        // GET: capitalgains?date
        [Route("capitalgains")]
        [HttpGet]
        public ActionResult<SimpleUnrealisedGainsResponse> GetCapitalGains(DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            var service = new PortfolioCapitalGainsService(_Portfolio);

            return service.GetCapitalGains(requestedDate);
        }

        // GET: detailedcapitalgains?date
        [Route("detailedcapitalgains")]
        [HttpGet]
        public ActionResult<DetailedUnrealisedGainsResponse> GetDetailedCapitalGains(DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            var service = new PortfolioCapitalGainsService(_Portfolio);

            return service.GetDetailedCapitalGains(requestedDate);         
        }

        // GET: cgtliability?fromDate&toDate
        [Route("cgtliability")]
        [HttpGet]
        public ActionResult<CgtLiabilityResponse> GetCGTLiability(DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var service = new PortfolioCgtLiabilityService(_Portfolio);

            return service.GetCGTLiability(dateRange);
        }

        // GET: cashaccount?fromDate&toDate
        [Route("cashaccount")]
        [HttpGet]
        public ActionResult<CashAccountTransactionsResponse> GetCashAccountTransactions(DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var service = new CashAccountService(_Portfolio.CashAccount, _Mapper);

            return service.GetCashAccountTransactions(dateRange);
        }

        // GET: income?fromDate&toDate
        [Route("income")]
        [HttpGet]
        public ActionResult<IncomeResponse> GetIncome(DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var service = new PortfolioIncomeService(_Portfolio);

            return service.GetIncome(dateRange);
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
