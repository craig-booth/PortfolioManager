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
    [Portfolio]
    public abstract class PortfolioControllerBase : ControllerBase
    {
        private readonly IAuthorizationService _AuthorizationService;

        protected IRepository<Portfolio> _PortfolioRepository;
        protected Portfolio _Portfolio { get; private set; }

        public PortfolioControllerBase(IRepository<Portfolio> portfolioRepository, IAuthorizationService authorizationService)
        {
            _PortfolioRepository = portfolioRepository;
            _AuthorizationService = authorizationService;


        }

        public async Task<bool> SetPortfolio(Guid id)
        {
            _Portfolio = _PortfolioRepository.Get(id);

            var authorizationResult = await _AuthorizationService.AuthorizeAsync(User, _Portfolio, Policy.IsPortfolioOwner);
            return authorizationResult.Succeeded;
        }
    }

    public class PortfolioAttribute : ActionFilterAttribute
    {

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is PortfolioControllerBase portfolioController)
            {
                var portfolioId = (string)context.RouteData.Values["portfolioId"];

                var successfull = await portfolioController.SetPortfolio(Guid.Parse(portfolioId));
                if (!successfull)
                    context.Result = new ForbidResult();
            }

            await next();
        }
    }

}
