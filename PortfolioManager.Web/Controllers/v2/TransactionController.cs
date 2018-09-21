using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using PortfolioManager.Common;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.RestApi.Transactions;

namespace PortfolioManager.Web.Controllers.v2
{
    [Route("api/v2/portfolio/{portfolioId:guid}/transactions")]
    public class TransactionController : Controller
    {
        private IPortfolioCache _PortfolioCache;
        private Portfolio _Portfolio;

        public TransactionController(IPortfolioCache portfolioCache)
        {
            _PortfolioCache = portfolioCache;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var portfolioId = (string)filterContext.RouteData.Values["portfolioId"];

            _Portfolio = _PortfolioCache.Get(Guid.Parse(portfolioId));
        }

        // GET:  transactions/id
        [HttpGet("{id:guid}")]
        public ActionResult Get(Guid id)
        {
            return NotFound();
        }

        // GET: transactions?stock&fromDate&toDate
        [HttpGet]
        public ActionResult Get(Guid? stock, DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return NotFound();
        }


        // POST: transactions
        [HttpPost]
        public ActionResult Post([FromBody]Transaction transaction)
        {
            return NotFound();
        } 
    }
}
