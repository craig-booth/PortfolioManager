using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.CorporateActions;
using PortfolioManager.RestApi.Stocks;
using PortfolioManager.RestApi.CorporateActions;

namespace PortfolioManager.Web.Controllers
{

    [Route("api/stocks/{id:guid}/corporateactions")]
    public class CorporateActionController : Controller
    {
        private IStockRepository _StockRepository;

        public CorporateActionController(IStockRepository stockRepository)
        {
            _StockRepository = stockRepository;
        }

        // GET : /api/stocks/{id}/corporateactions
        [Route("")]
        [HttpGet]
        public ActionResult GetCorporateActions([FromRoute]Guid id, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return Ok(stock.CorporateActions(dateRange).Select(x => x.ToCorporateActionResponse()));
        }

        // GET : /api/stocks/{id}/corporateactions/{id}
        [Route("{actionid:guid}")]
        [HttpGet]
        public ActionResult GetCorporateAction([FromRoute]Guid id, [FromRoute]Guid actionId)
        {
            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            var corporateAction = stock.CorporateAction(actionId);
            if (corporateAction == null)
                return NotFound();

            if (corporateAction.Type == CorporateActionType.Dividend)
                return Ok((corporateAction as Dividend).ToDividendResponse());
            else if (corporateAction.Type == CorporateActionType.CapitalReturn)
                return Ok((corporateAction as CapitalReturn).ToCapitalReturnResponse());
            else
                return BadRequest("Unknown corporate action type");
        }

        // GET : /api/stocks/{id}/corporateactions/dividends
        [Route("dividends")]
        [HttpGet]
        public ActionResult GetDividends([FromRoute]Guid id, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return Ok(stock.CorporateActions<Dividend>(dateRange).Select(x => x.ToDividendResponse()));
        }

        // GET : /api/stocks/{id}/corporateactions/dividends/{id}
        [Route("dividends/{actionid:guid}")]
        [HttpGet]
        public ActionResult GetDividend([FromRoute]Guid id, [FromRoute]Guid actionId)
        {
            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            var corporateAction = stock.CorporateAction<Dividend>(actionId);
            if (corporateAction == null)
                return NotFound();

            return Ok(corporateAction.ToDividendResponse());
        }

        // POST : /api/stocks/{id}/corporateactions/dividends
        [Route("dividends")]
        [HttpPost]
        public ActionResult AddDividend([FromRoute]Guid id, [FromBody] AddDividendCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Stock)
                return BadRequest("Id in command doesn't match id on URL");

            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            try
            {
                stock.AddDividend(command.Id, command.ActionDate, command.Description, command.PaymentDate, command.DividendAmount, command.CompanyTaxRate, command.PercentFranked, command.DRPPrice);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // GET : /api/stocks/{id}/corporateactions/capitalreturn
        [Route("capitalreturns")]
        [HttpGet]
        public ActionResult GetCapitalReturns([FromRoute]Guid id, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return Ok(stock.CorporateActions<CapitalReturn>(dateRange).Select(x => x.ToCapitalReturnResponse()));
        }

        // GET : /api/stocks/{id}/corporateactions/capitalreturns/{id}
        [Route("capitalreturns/{actionid:guid}")]
        [HttpGet]
        public ActionResult GetCapitalReturn([FromRoute]Guid id, [FromRoute]Guid actionId)
        {
            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            var corporateAction = stock.CorporateAction<CapitalReturn>(actionId);
            if (corporateAction == null)
                return NotFound();

            return Ok(corporateAction.ToCapitalReturnResponse());
        }

        // POST : /api/stocks/{id}/corporateactions/capitalreturns
        [Route("capitalreturns")]
        [HttpPost]
        public ActionResult AddCapitalReturn([FromRoute]Guid id, [FromBody] AddCapitalReturnCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Stock)
                return BadRequest("Id in command doesn't match id on URL");

            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            try
            {
                stock.AddCapitalReturn(command.Id, command.ActionDate, command.Description, command.PaymentDate, command.Amount);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }

}

