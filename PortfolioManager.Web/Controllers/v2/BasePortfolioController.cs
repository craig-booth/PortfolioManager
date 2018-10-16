using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using PortfolioManager.Domain.Portfolios;

namespace PortfolioManager.Web.Controllers.v2
{
    public abstract class BasePortfolioController : Controller
    {
        protected IPortfolioCache _PortfolioCache { get; private set; }
        protected Portfolio _Portfolio { get; private set; }

        public BasePortfolioController(IPortfolioCache portfolioCache)
        {
            _PortfolioCache = portfolioCache;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var portfolioId = (string)filterContext.RouteData.Values["portfolioId"];

            _Portfolio = _PortfolioCache.Get(Guid.Parse(portfolioId));
        }
    }
}
