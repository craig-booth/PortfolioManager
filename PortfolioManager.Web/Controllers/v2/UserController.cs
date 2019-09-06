using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

using PortfolioManager.RestApi.Users;
using PortfolioManager.Web.Services;


namespace PortfolioManager.Web.Controllers.v2
{

    [Route("api/v2/users")]
    public class UserController : Controller
    {
        private readonly IUserService _UserService;

        public UserController(IUserService userService)
        {
            _UserService = userService;
        }

        // POST : /api/v2/users/authenticate       
        [AllowAnonymous]
        [Route("authenticate")]
        [HttpPost]
        public ActionResult<AuthenticationResponse> CreateStock([FromBody] AuthenticateCommand command)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = _UserService.Authenticate(command.UserName, command.Password);
                if (user == null)
                    return Forbid();

                var key = System.Text.Encoding.ASCII.GetBytes("this is a secret");

                // Authentication successful so generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();              
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = "http://portfolio.boothfamily.id.au",
                    Audience = "http://portfolio.boothfamily.id.au",
                    Expires = DateTime.UtcNow.AddHours(1),
                    IssuedAt = DateTime.UtcNow,
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, Role.Administrator)
                    }),
                    
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                var response = new AuthenticationResponse()
                {
                    Token = tokenHandler.WriteToken(token)
                };

                return response;
                   
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }



    }
}
