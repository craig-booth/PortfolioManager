using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

using PortfolioManager.Domain;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PortfolioOwnerAuthorizeAttribute : ActionFilterAttribute, IFilterFactory
    {
        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var portfolioCache = serviceProvider.GetRequiredService<IPortfolioCache>();
            var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();

            return new InternalPortfolioAttribute(portfolioCache, authorizationService);
        }

        private class InternalPortfolioAttribute : ActionFilterAttribute
        {
            private readonly IPortfolioCache _PortfolioCache;
            private readonly IAuthorizationService _AuthorizationService;

            public InternalPortfolioAttribute(IPortfolioCache portfolioCache, IAuthorizationService authorizationService)
            {
                _PortfolioCache = portfolioCache;
                _AuthorizationService = authorizationService;
            }

            public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var portfolioParameter = (string)context.RouteData.Values["portfolioId"];
                if (Guid.TryParse(portfolioParameter, out var portfolioId))
                {
                    if (_PortfolioCache.TryGet(portfolioId, out var portfolio))
                    {
                        var authorizationResult = await _AuthorizationService.AuthorizeAsync(context.HttpContext.User, portfolio, Policy.IsPortfolioOwner);
                        if (!authorizationResult.Succeeded)
                            context.Result = new ForbidResult();
                    }
                    else
                        context.Result = new ForbidResult();
                }
                else
                    context.Result = new ForbidResult();

                await next();
            }
        }

    }
}
