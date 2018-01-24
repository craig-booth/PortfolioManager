using System;
using System.Collections.Generic;

namespace PortfolioManager.Common
{
    public class ServiceFactory<I> 
    {
        private class ServiceInstance
        {
            public I Instance;
            public bool Created;
            public Func<I> Factory;

            public ServiceInstance(I instance)
            {
                Created = true;
                Instance = instance;
            }

            public ServiceInstance(Func<I> factory)
            {
                Created = false;
                Factory = factory;
            }

            public I GetInstance()
            {
                if (!Created)
                {
                    Instance = Factory();
                    Created = true;
                }

                return Instance;
            }
        }

        private readonly Dictionary<Type, ServiceInstance> _Services;

        public ServiceFactory()
        {
            _Services = new Dictionary<Type, ServiceInstance>();
        }

        public ServiceFactory<I> Register<T>(I instance)
        {
            _Services.Add(typeof(T), new ServiceInstance(instance));

            return this;
        }

        public ServiceFactory<I> Register<T>(Func<I> factory)
        {
            _Services.Add(typeof(T), new ServiceInstance(factory));

            return this;
        }

        public void Clear()
        {
            _Services.Clear();
        }

        public I GetService(Type type)
        {
            if (_Services.TryGetValue(type, out var serviceInstance))
            {
                return serviceInstance.GetInstance();
            }

            return default(I);
        }

        public I GetService<T>()
        {
            return GetService(typeof(T));
        }

        public I GetService(object obj)
        {
            return GetService(obj.GetType());
        }
    }
}
