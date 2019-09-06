using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;

using PortfolioManager.Domain;
using PortfolioManager.Domain.Portfolios;
using System.Security.Claims;

namespace PortfolioManager.Web.Controllers.v2
{
    public abstract class BasePortfolioController : Controller
    {
        private readonly IAuthorizationService _AuthorizationService;

        protected IRepository<Portfolio> _PortfolioRepository;
        protected Portfolio _Portfolio { get; private set; }

        public BasePortfolioController(IRepository<Portfolio> portfolioRepository, IAuthorizationService authorizationService)
        {
            _PortfolioRepository = portfolioRepository;
            _AuthorizationService = authorizationService;
        }

        public override async void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var portfolioId = (string)filterContext.RouteData.Values["portfolioId"];

            _Portfolio = _PortfolioRepository.Get(Guid.Parse(portfolioId));

            var authorizationResult = await _AuthorizationService.AuthorizeAsync(User, _Portfolio, Policy.IsPortfolioOwner);
            if (!authorizationResult.Succeeded)
            {
                filterContext.Result = new ForbidResult();
            }
        }
    }

}
