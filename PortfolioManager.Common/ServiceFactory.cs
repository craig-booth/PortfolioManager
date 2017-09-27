using System;
using System.Collections.Generic;

namespace PortfolioManager.Common
{
    public class ServiceFactory<I>
    {
        private readonly Dictionary<Type, I> _Services;
        private readonly Dictionary<Type, Func<I>> _ServiceFactories;

        public ServiceFactory()
        {
            _Services = new Dictionary<Type, I>();
            _ServiceFactories = new Dictionary<Type, Func<I>>();
        }

        public void Register<T>(Func<I> factory)
        {
            _ServiceFactories.Add(typeof(T), factory);
        }

        public void Clear()
        {
            _Services.Clear();
            _ServiceFactories.Clear();
        }

        public I GetService<T>()
        {
            I service;

            if (!_Services.TryGetValue(typeof(T), out service))
            {
                var factory = _ServiceFactories[typeof(T)];
                service = factory();

                _Services.Add(typeof(T), service);
            }

            return service;
        }

        public I GetService(object obj)
        {
            I service;

            if (!_Services.TryGetValue(obj.GetType(), out service))
            {
                var factory = _ServiceFactories[obj.GetType()];
                service = factory();

                _Services.Add(obj.GetType(), service);
            }

            return service;
        }
    }
}
