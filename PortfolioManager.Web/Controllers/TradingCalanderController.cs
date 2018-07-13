using Microsoft.AspNetCore.Mvc;
using PortfolioManager.Domain.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.RestApi.TradingCalander;

namespace PortfolioManager.Web.Controllers
{
    [Route("api/tradingcalander")]
    public class TradingCalanderController : Controller
    {
        private ITradingCalander _TradingCalander;

        public TradingCalanderController(ITradingCalander tradingCalander)
        {
            _TradingCalander = tradingCalander;
        }

        // GET: api/tradingcalander/{year}
        [HttpGet]
        [Route("{year}")]
        public ActionResult<TradingCalanderResponse> Get([FromRoute]int year)
        {
            var nonTradingDays = _TradingCalander.NonTradingDays(year);

            return Ok(nonTradingDays);
        }

        // POST: api/tradingcalander/{year}
        [HttpPost]
        [Route("{year}")]
        public ActionResult Post([FromRoute]int year, [FromBody] UpdateTradingCalanderCommand command)
        {
            if (year != command.Year)
                return BadRequest(String.Format("Year in command doesn't match year on URL"));

            _TradingCalander.SetNonTradingDays(year, command.NonTradingDays.Select(x => new NonTradingDay(x.Date, x.Desciption)));

            return Ok();
        }
    }
}
