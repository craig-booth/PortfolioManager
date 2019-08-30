using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Domain;
using PortfolioManager.Domain.Users;

namespace PortfolioManager.Web.Services
{
    public interface IUserService
    {
        User Authenticate(string userName, string password);
    }

    public class UserService : IUserService
    {
        private readonly IRepository<User> _UserRepository;

        public UserService(IRepository<User> userRepository)
        {
            _UserRepository = userRepository;
        }

        public User Authenticate(string userName, string password)
        {
            var user = _UserRepository.FindFirst("UserName", userName);
            if (user != null)
            {
                if (user.PasswordCorrect(password))
                    return user;
            }

            return null;
        }
    }
}
