using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Domain.TradingCalanders;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Services;

namespace PortfolioManager.Web.Controllers.v2
{
    [Route("api/v2/portfolio/{portfolioId:guid}/holdings")]
    public class HoldingController : BasePortfolioController
    {
        private IMapper _Mapper;
        private ITradingCalander _TradingCalander;

        public HoldingController(IPortfolioCache portfolioCache, ITradingCalander tradingCalander, IMapper mapper)
            : base(portfolioCache)
        {
            _Mapper = mapper;
            _TradingCalander = tradingCalander;
        }

        // GET:
        [HttpGet]
        public ActionResult<List<Holding>> Get([FromQuery]DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            var holdings = _Portfolio.Holdings.All(requestedDate);

            return _Mapper.Map<List<RestApi.Portfolios.Holding>>(holdings, opts => opts.Items["date"] = requestedDate);
        }

        // GET:
        [HttpGet]
        public ActionResult<List<Holding>> Get([FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var holdings = _Portfolio.Holdings.All(dateRange);

            return _Mapper.Map<List<RestApi.Portfolios.Holding>>(holdings, opts => opts.Items["date"] = dateRange.ToDate);
        }

        // GET:  id
        [HttpGet("{id:guid}")]
        public ActionResult<Holding> Get([FromRoute]Guid id, [FromQuery]DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            var holding = _Portfolio.Holdings.Get(id);

            if (holding == null)
                return NotFound();

            return _Mapper.Map<RestApi.Portfolios.Holding>(holding, opts => opts.Items["date"] = requestedDate);
        }

        // GET: id/value?fromDate&toDate
        [Route("{id:guid}/value")]
        [HttpGet]
        public ActionResult<PortfolioValueResponse> GetValue(Guid id, DateTime? fromDate, DateTime? toDate, ValueFrequency? frequency)
        {
            var holding = _Portfolio.Holdings.Get(id);
            if (holding == null)
                return NotFound();

            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);
            var requestedFrequency = (frequency != null) ? (ValueFrequency)frequency : ValueFrequency.Daily;

            var service = new PortfolioValueService(_Portfolio, _TradingCalander);

            return service.GetValue(holding, dateRange, requestedFrequency);
        } 

        // GET: transactions?fromDate&toDate
        [Route("{id:guid}/transactions")]
        [HttpGet]
        public ActionResult<TransactionsResponse> GetTransactions(Guid id, DateTime? fromDate, DateTime? toDate)
        {
            var holding = _Portfolio.Holdings.Get(id);
            if (holding == null)
                return NotFound();

            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var service = new PortfolioTransactionService(_Portfolio, _Mapper);

            return service.GetTransactions(holding, dateRange);
        } 

        // GET: capitalgains?date
        [Route("{id:guid}/capitalgains")]
        [HttpGet]
        public ActionResult<SimpleUnrealisedGainsResponse> GetCapitalGains(Guid id, DateTime? date)
        {
            var holding = _Portfolio.Holdings.Get(id);
            if (holding == null)
                return NotFound();

            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            var service = new PortfolioCapitalGainsService(_Portfolio);

            return service.GetCapitalGains(holding, requestedDate);
        } 

        // GET: detailedcapitalgains?date
        [Route("{id:guid}/detailedcapitalgains")]
        [HttpGet]
        public ActionResult<DetailedUnrealisedGainsResponse> GetDetailedCapitalGains(Guid id, DateTime? date)
        {
            var holding = _Portfolio.Holdings.Get(id);
            if (holding == null)
                return NotFound();

            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            var service = new PortfolioCapitalGainsService(_Portfolio);

            return service.GetDetailedCapitalGains(holding, requestedDate);
        }

        // GET: corporateactions
        [Route("{id:guid}/corporateactions")]
        [HttpGet]
        public ActionResult<CorporateActionsResponse> GetCorporateActions(Guid id)
        {
            var holding = _Portfolio.Holdings.Get(id);
            if (holding == null)
                return NotFound();

            var service = new PortfolioCorporateActionsService(_Portfolio);

            return service.GetCorporateActions(holding);
        }
    } 
}