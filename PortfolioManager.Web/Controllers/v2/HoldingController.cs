using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.TradingCalanders;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Services;
using PortfolioManager.Web.Utilities;
using Microsoft.AspNetCore.Http;

namespace PortfolioManager.Web.Controllers.v2
{
    [Route("api/v2/portfolio/{portfolioId:guid}/holdings")]
    [ApiController]
    public class HoldingController : PortfolioControllerBase
    {
        private readonly IMapper _Mapper;
        private readonly IStockQuery _StockQuery;
        private readonly ITradingCalander _TradingCalander;

        public HoldingController(IRepository<Portfolio> portfolioRepository, IStockQuery stockQuery, ITradingCalander tradingCalander, IMapper mapper, IAuthorizationService authorizationService)
            : base(portfolioRepository,authorizationService)
        {
            _StockQuery = stockQuery;
            _TradingCalander = tradingCalander;
            _Mapper = mapper;
        }

        // GET:
        [HttpGet]
        public ActionResult<List<RestApi.Portfolios.Holding>> Get([FromQuery]DateTime? date, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            DateTime requestedDate;
            IEnumerable<Domain.Portfolios.Holding> holdings;
            if (date != null)
            {
                requestedDate = (DateTime)date;
                holdings = _Portfolio.Holdings.All(requestedDate);
            }
            else
            {
                var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

                requestedDate = dateRange.ToDate;
                holdings = _Portfolio.Holdings.All(dateRange);
            }

            return _Mapper.Map<List<RestApi.Portfolios.Holding>>(holdings, opts => opts.Items["date"] = requestedDate);
        }

        // GET:  id
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<RestApi.Portfolios.Holding> Get([FromRoute]Guid id, [FromQuery]DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            var holding = _Portfolio.Holdings.Get(id);

            if (holding == null)
                return NotFound();

            return _Mapper.Map<RestApi.Portfolios.Holding>(holding, opts => opts.Items["date"] = requestedDate);
        }

        // GET: properties
        [Route("{id:guid}/changedrpparticipation")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeDrpParticipation(Guid id, bool participate)
        {
            var holding = _Portfolio.Holdings.Get(id);
            if (holding == null)
                return NotFound();

            var service = new PortfolioService(_Portfolio, _PortfolioRepository);

            service.ChangeDrpParticipation(holding.Id, participate);

            return Ok();
        }

        // GET: id/value?fromDate&toDate
        [Route("{id:guid}/value")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TransactionsResponse> GetTransactions(Guid id, DateTime? fromDate, DateTime? toDate)
        {
            var holding = _Portfolio.Holdings.Get(id);
            if (holding == null)
                return NotFound();

            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var service = new PortfolioTransactionService(_Portfolio, _PortfolioRepository, _StockQuery, _Mapper);

            return service.GetTransactions(holding, dateRange);
        } 

        // GET: capitalgains?date
        [Route("{id:guid}/capitalgains")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CorporateActionsResponse> GetCorporateActions(Guid id)
        {
            var holding = _Portfolio.Holdings.Get(id);
            if (holding == null)
                return NotFound();

            var service = new PortfolioCorporateActionsService(_Portfolio, _Mapper);

            return service.GetCorporateActions(holding);
        } 
    } 
}