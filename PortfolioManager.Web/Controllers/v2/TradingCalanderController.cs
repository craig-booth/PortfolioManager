using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using PortfolioManager.Web.Services;
using PortfolioManager.RestApi.TradingCalanders;


namespace PortfolioManager.Web.Controllers.v2
{
    [Authorize]
    [Route("api/v2/tradingcalander")]
    [ApiController]
    public class TradingCalanderController : ControllerBase
    {
        private ITradingCalanderService _Service;

        public TradingCalanderController(ITradingCalanderService tradingCalanderService)
        {
            _Service = tradingCalanderService;
        }

        // GET: api/tradingcalander/{year}
        [HttpGet]
        [Route("{year:int}")]
        public ActionResult<TradingCalanderResponse> Get([FromRoute]int year)
        {
            return _Service.Get(year);
        }

        // POST: api/tradingcalander/{year}
        [Authorize(Policy.CanMantainStocks)]
        [HttpPost]
        [Route("{year:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public ActionResult Post([FromRoute]int year, [FromBody] UpdateTradingCalanderCommand command)
        {
            if (year != command.Year)
                return BadRequest(String.Format("Year in command doesn't match year on URL"));

            _Service.Update(year, command.NonTradingDays.Select(x => new Domain.TradingCalanders.NonTradingDay(x.Date, x.Desciption)));

            return Ok();
        } 
    }
}
