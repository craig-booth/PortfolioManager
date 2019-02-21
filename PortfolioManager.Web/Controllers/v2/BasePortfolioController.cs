using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using PortfolioManager.Domain;
using PortfolioManager.Domain.Portfolios;

namespace PortfolioManager.Web.Controllers.v2
{
    public abstract class BasePortfolioController : Controller
    {
        protected IRepository<Portfolio> _PortfolioRepository;
        protected Portfolio _Portfolio { get; private set; }

        public BasePortfolioController(IRepository<Portfolio> portfolioRepository)
        {
            _PortfolioRepository = portfolioRepository;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var portfolioId = (string)filterContext.RouteData.Values["portfolioId"];

            _Portfolio = _PortfolioRepository.Get(Guid.Parse(portfolioId));
        }
    }
}
