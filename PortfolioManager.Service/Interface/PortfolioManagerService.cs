using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Service.Interface
{
    public interface IPortfolioManagerService
    {

    }

    public interface IPortfolioManagerServiceFactory
    {
        T GetService<T>() where T : IPortfolioManagerService;
    }

    public abstract class PortfolioManagerService : IPortfolioManagerServiceFactory
    {
        private readonly Dictionary<Type, IPortfolioManagerService> _Services;
        private readonly Dictionary<Type, Func<IPortfolioManagerService>> _ServiceFactories;

        public PortfolioManagerService()
        {
            _Services = new Dictionary<Type, IPortfolioManagerService>();
            _ServiceFactories = new Dictionary<Type, Func<IPortfolioManagerService>>();
        }

        public void Register<T>(Func<IPortfolioManagerService> factory) where T : IPortfolioManagerService
        {
            _ServiceFactories.Add(typeof(T), (Func<IPortfolioManagerService>)factory);
        }

        public T GetService<T>() where T : IPortfolioManagerService
        {
            IPortfolioManagerService service;

            if (!_Services.TryGetValue(typeof(T), out service))
            {
                var factory = _ServiceFactories[typeof(T)];
                service = factory();

                _Services.Add(typeof(T), service);
            }

            return (T)service;
        }
    }
}
