using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace PortfolioManager.Web
{

   public static class Role
    {
        public const string Administrator = "Administrator";
    }

    public static class Policy
    {
        public const string CanMantainStocks = "CanMaintainStocks";
        public const string IsPortfolioOwner = "CanAccessPortfolio";
    } 


}
