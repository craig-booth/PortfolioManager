using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace PortfolioManager.Web
{
    public class PortfolioManagerSettings
    {
        public int Port { get; set; }
        public string EventStore { get; set; }
        public bool EnableAuthentication { get; set; }
        
        public JwtTokenConfiguration JwtTokenConfiguration { get; set; }
    }

    public class JwtTokenConfiguration
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string KeyFile { get; set; }

        private SymmetricSecurityKey _Key = null;

        public SymmetricSecurityKey GetKey()
        {
            if (_Key == null)
            {
                var base64Key = System.IO.File.ReadAllText(KeyFile);
                var key = Convert.FromBase64String(base64Key);
                _Key = new SymmetricSecurityKey(key);
            }

            return _Key;
        }
    }
}
