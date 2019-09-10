using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Security;

using PortfolioManager.RestApi.Users;

namespace PortfolioManager.RestApi.Client
{
    public class UserResource : RestResource
    {
        public UserResource(ClientSession session)
            : base(session)
        {

        }

        public async Task<bool> Authenticate(string userName, SecureString password)
        {
            var command = new AuthenticateCommand()
            {
                UserName = userName,
                Password = new System.Net.NetworkCredential(string.Empty, password).Password
            };

            var response = await PostAsync<AuthenticationResponse, AuthenticateCommand>("/api/v2/users/authenticate", command);

            if (response != null)
            {
                _Session.JwtToken = response.Token;
                return true;
            }
            else
                return false;
        }
    }
}
