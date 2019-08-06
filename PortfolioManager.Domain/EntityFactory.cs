using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain
{
    public interface IEntityFactory<T>
    {
        T Create(string storedEntityType);
    }

    public class DefaultEntityFactory<T> : IEntityFactory<T> where T : new()
    {
        public T Create(string storedEntityType)
        {
            return new T();
        }
    }
}
