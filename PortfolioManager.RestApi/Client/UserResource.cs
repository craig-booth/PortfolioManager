using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.RestApi.Users;

namespace PortfolioManager.RestApi.Client
{
    public class UserResource : RestResource
    {
        public UserResource(ClientSession session)
            : base(session)
        {

        }

        public async Task<bool> Authenticate(string userName, string password)
        {
            var command = new AuthenticateCommand()
            {
                UserName = userName,
                Password = password
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
