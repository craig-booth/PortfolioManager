using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Booth.Common;

using PortfolioManager.Domain.Stocks;
using PortfolioManager.RestApi.Stocks;
using PortfolioManager.Web.Services;
using PortfolioManager.Web.Mappers;


namespace PortfolioManager.Web.Controllers.v2
{
    [Authorize]
    [Route("api/v2/stocks")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private IStockService _StockService;

        public StockController(IStockService stockService)
        {
            _StockService = stockService;
        }

        // GET: api/stocks
        [HttpGet]
        public ActionResult<List<StockResponse>> Get([FromQuery]string query, [FromQuery]DateTime? date, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {       
            if (date != null)
            {
                var requestedDate = (DateTime)date;
                if (query == null)
                    return _StockService.Get("", requestedDate).ToList();
                else
                    return _StockService.Get(query, requestedDate).ToList();
            }
            else
            {
                var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateUtils.NoEndDate);

                if (query == null)
                    return _StockService.Get("", dateRange).ToList();
                else
                    return _StockService.Get(query, dateRange).ToList();
            }
        }

        // GET: api/stocks/{id}
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<StockResponse> Get([FromRoute]Guid id, [FromQuery]DateTime? date)
        {
            if (date != null)
                return _StockService.Get(id, (DateTime)date);
            else
                return _StockService.Get(id, DateTime.Today);
        }

        // GET : /api/stocks/{id}/history
        [HttpGet]
        [Route("{id:guid}/history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<StockHistoryResponse> GetHistory([FromRoute]Guid id)
        {
            return _StockService.GetHistory(id);
        }

        // GET : /api/stocks/{id}/closingprices
        [HttpGet]
        [Route("{id:guid}/closingprices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<StockPriceResponse> GetClosingPrices([FromRoute]Guid id, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateTime.Today.AddYears(-1), (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return _StockService.GetClosingPrices(id, dateRange);
        }

        // POST : /api/stocks
        [Authorize(Policy.CanMantainStocks)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public ActionResult CreateStock([FromBody] CreateStockCommand command)
        {
            if (command.ChildSecurities.Count == 0)
            {
                _StockService.ListStock(command.Id, command.AsxCode, command.Name, command.ListingDate, command.Trust, command.Category.ToDomain());
            }
            else
            {
                if (command.Trust)
                    return BadRequest("A Stapled security cannot be a trust");

                _StockService.ListStapledSecurity(command.Id, command.AsxCode, command.Name, command.ListingDate, command.Category.ToDomain(), command.ChildSecurities.Select(x => new StapledSecurityChild(x.ASXCode, x.Name, x.Trust)));
            }

            return Ok();
        }

        // POST : /api/stocks/{id}/change
        [Authorize(Policy.CanMantainStocks)]
        [Route("{id:guid}/change")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeStock([FromRoute]Guid id, [FromBody] ChangeStockCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Id)
                return BadRequest("Id in command doesn't match id on URL");

            _StockService.ChangeStock(id, command.ChangeDate, command.AsxCode, command.Name, command.Category.ToDomain());

            return Ok();
        }

        // POST : /api/stocks/{id}/delist
        [Authorize(Policy.CanMantainStocks)]
        [Route("{id:guid}/delist")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DelistStock([FromRoute]Guid id, [FromBody] DelistStockCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Id)
                return BadRequest("Id in command doesn't match id on URL");

            _StockService.DelistStock(id, command.DelistingDate);

            return Ok();
        }

        // POST : /api/stocks/{id}/closingprices
        [Authorize(Policy.CanMantainStocks)]
        [Route("{id:guid}/closingprices")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateClosingPrices([FromRoute]Guid id, [FromBody] UpdateClosingPricesCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Id)
                return BadRequest("Id in command doesn't match id on URL");

            var closingPrices = command.Prices.Select(x => new StockPrice(x.Date, x.Price));

            _StockService.UpdateClosingPrices(id, closingPrices);

            return Ok();
        }

        // POST : /api/stocks/{id}/changedividendrules
        [Authorize(Policy.CanMantainStocks)]
        [Route("{id}/changedividendrules")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeDividendRules([FromRoute]Guid id, [FromBody] ChangeDividendRulesCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Id)
                return BadRequest("Id in command doesn't match id on URL");

            _StockService.ChangeDividendRules(id, command.ChangeDate, command.CompanyTaxRate, command.DividendRoundingRule, command.DRPActive, command.DRPMethod.ToDomain());

            return Ok();
        }

        // GET : /api/stocks/{id}/relativenta
        [Route("{id:guid}/relativenta")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public RelativeNTAResponse GetRelativeNTA([FromRoute]Guid id, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);
            return _StockService.GetRelativeNTA(id, dateRange);
        }

        // POST : /api/stocks/{id}/relativenta
        [Authorize(Policy.CanMantainStocks)]
        [Route("{id:guid}/relativenta")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeRelativeNTA([FromRoute]Guid id, [FromBody] ChangeRelativeNTAsCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Id)
                return BadRequest("Id in command doesn't match id on URL");

            var ntas = command.RelativeNTAs.Select(x => new Tuple<string, decimal>(x.ChildSecurity, x.Percentage));

            _StockService.ChangeRelativeNTAs(id, command.ChangeDate, ntas);

            return Ok();
        }

    }

}
