using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Service.Interface
{
    public interface IPortfolioService
    {

    }

    public interface IPortfolioManagerService
    {
        T GetService<T>() where T : IPortfolioService;
    }

}
