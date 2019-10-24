using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Booth.Common;

using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Services;
using Microsoft.AspNetCore.Http;

namespace PortfolioManager.Web.Controllers.v2
{
    [Route("api/v2/portfolio/{portfolioId:guid}/holdings")]
    [ApiController]
    public class HoldingController : ControllerBase
    {

        // GET:
        [HttpGet]
        public ActionResult<List<RestApi.Portfolios.Holding>> Get([FromServices] IPortfolioService service, [FromRoute] Guid portfolioId, [FromQuery]DateTime? date, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            if (date != null)
            {
                var requestedDate = (DateTime)date;
                return service.GetHoldings(portfolioId, requestedDate).ToList();
            }
            else
            {
                var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);
                return service.GetHoldings(portfolioId, dateRange).ToList();
            }           
        }

        // GET:  id
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<RestApi.Portfolios.Holding> Get([FromServices] IPortfolioService service, [FromRoute] Guid portfolioId, [FromRoute]Guid id, [FromQuery]DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            return service.GetHolding(portfolioId, id, requestedDate);
        }

        // GET: properties
        [Route("{id:guid}/changedrpparticipation")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeDrpParticipation([FromServices] IPortfolioService service, [FromRoute] Guid portfolioId, [FromRoute] Guid id, [FromQuery] bool participate)
        {
            service.ChangeDrpParticipation(portfolioId, id, participate);

            return Ok();
        }

        // GET: id/value?fromDate&toDate
        [Route("{id:guid}/value")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PortfolioValueResponse> GetValue([FromServices] IPortfolioValueService service, [FromRoute] Guid portfolioId, [FromRoute]Guid id, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] ValueFrequency? frequency)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);
            var requestedFrequency = (frequency != null) ? (ValueFrequency)frequency : ValueFrequency.Daily;

            return service.GetValue(portfolioId, id, dateRange, requestedFrequency);
        } 

        // GET: transactions?fromDate&toDate
        [Route("{id:guid}/transactions")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TransactionsResponse> GetTransactions([FromServices] IPortfolioTransactionService service, [FromRoute] Guid portfolioId, [FromRoute] Guid id, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return service.GetTransactions(portfolioId, id, dateRange);
        } 

        // GET: capitalgains?date
        [Route("{id:guid}/capitalgains")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SimpleUnrealisedGainsResponse> GetCapitalGains([FromServices] IPortfolioCapitalGainsService service, [FromRoute] Guid portfolioId, [FromRoute] Guid id, [FromQuery] DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            return service.GetCapitalGains(portfolioId, id, requestedDate);
        } 

        // GET: detailedcapitalgains?date
        [Route("{id:guid}/detailedcapitalgains")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<DetailedUnrealisedGainsResponse> GetDetailedCapitalGains([FromServices] IPortfolioCapitalGainsService service, [FromRoute] Guid portfolioId, [FromRoute] Guid id, [FromQuery] DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            return service.GetDetailedCapitalGains(portfolioId, id, requestedDate);
        }

        // GET: corporateactions
        [Route("{id:guid}/corporateactions")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CorporateActionsResponse> GetCorporateActions([FromServices] IPortfolioCorporateActionsService service, [FromRoute] Guid portfolioId, [FromRoute] Guid id)
        {
            return service.GetCorporateActions(portfolioId, id);
        } 
    } 
}