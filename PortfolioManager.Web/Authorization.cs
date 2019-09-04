using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace PortfolioManager.Web
{

   public static class Roles
    {
        public const string Administrator = "Administrator";
    }

    public static class Policies
    {
        public const string IsAdministrator = "IsAdministrator";
        public const string IsPortfolioOwner = "CanAccessPortfolio";
    } 


}
