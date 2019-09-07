using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Users
{
    public class AuthenticateCommand
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
