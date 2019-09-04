using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using PortfolioManager.Domain.Portfolios;


namespace PortfolioManager.Web
{

    public class PortfolioOwnerRequirement : IAuthorizationRequirement { }

    public class PortfolioOwnerAuthorizationHandler : AuthorizationHandler<PortfolioOwnerRequirement, Portfolio>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                        PortfolioOwnerRequirement requirement,
                                                        Portfolio resource)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            var userId = new Guid(userIdClaim.Value);
            if (resource.Owner == userId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
