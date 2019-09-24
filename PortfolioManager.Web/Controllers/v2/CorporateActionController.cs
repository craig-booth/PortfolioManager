using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using PortfolioManager.Common;
using PortfolioManager.RestApi.CorporateActions;
using PortfolioManager.Web.Services;


namespace PortfolioManager.Web.Controllers.v2
{
    [Authorize]
    [Route("api/v2/stocks/{stockId:guid}/corporateactions")]
    [ApiController]
    public class CorporateActionController : ControllerBase
    {
        private readonly ICorporateActionService _Service;

        public CorporateActionController(ICorporateActionService service)
        {
            _Service = service;
        }

        // GET : /api/stocks/{stockId}/corporateactions
        [Route("")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<CorporateAction>> GetCorporateActions([FromRoute]Guid stockId, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {     
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return _Service.GetCorporateActions(stockId, dateRange).ToList();
        }

        // GET : /api/stocks/{stockId}/corporateactions/{id}
        [Route("{id:guid}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]        
        public ActionResult<CorporateAction> GetCorporateAction([FromRoute]Guid stockId, [FromRoute]Guid id)
        {
            return _Service.GetCorporateAction(stockId, id);  
        }

        // POST : /api/stocks/{stockId}/corporateactions
        [Authorize(Policy.CanMantainStocks)]
        [Route("")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult AddCorporateAction([FromRoute]Guid stockId, [FromBody] RestApi.CorporateActions.CorporateAction corporateAction)
        {
            if (corporateAction == null)
                throw new UnknownCorporateActionType();

            // Check id in URL and id in command match
            if (stockId != corporateAction.Stock)
                return BadRequest("Id in command doesn't match id on URL");

            _Service.AddCorporateAction(stockId, corporateAction);

            return Ok();
        }
    }

}

