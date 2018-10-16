using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using AutoMapper;

using PortfolioManager.Common;

namespace PortfolioManager.Web.Controllers.v2
{
    [Route("api/v2/portfolio/{portfolioId:guid}/holdings")]
    public class HoldingController : BasePortfolioController
    {
        private IMapper _Mapper;

        public HoldingController(IPortfolioCache portfolioCache, IMapper mapper)
            : base(portfolioCache)
        {
            _Mapper = mapper;
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