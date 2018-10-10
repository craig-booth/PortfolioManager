using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Domain.Portfolios;

namespace PortfolioManager.Web.Controllers.v2
{
    [Route("api/v2/portfolio/{portfolioId:guid}/holdings")]
    public class HoldingController : Controller
    {
        private IPortfolioCache _PortfolioCache;
        private Portfolio _Portfolio;
        private IMapper _Mapper;

        public HoldingController(IPortfolioCache portfolioCache, IMapper mapper)
        {
            _PortfolioCache = portfolioCache;
            _Mapper = mapper;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var portfolioId = (string)filterContext.RouteData.Values["portfolioId"];

            _Portfolio = _PortfolioCache.Get(Guid.Parse(portfolioId));
        }

        // GET:  holdings/id
        [HttpGet("{id:guid}")]
        public ActionResult<RestApi.Portfolios.Holding> Get([FromRoute]Guid id, [FromQuery]DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            var holding = _Portfolio.Holdings.Get(id);

            if (holding == null)
                return NotFound();

            return _Mapper.Map<RestApi.Portfolios.Holding>(holding, opts => opts.Items["date"] = requestedDate);
        }

        // GET:  holdings
        [HttpGet]
        public ActionResult<List<RestApi.Portfolios.Holding>> Get([FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var holdings = _Portfolio.Holdings.All(dateRange);

            return _Mapper.Map<List<RestApi.Portfolios.Holding>>(holdings, opts => opts.Items["date"] = dateRange.ToDate);
        }
    }
}