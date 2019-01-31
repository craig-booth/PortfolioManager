using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain
{
    public interface IEntityCache<T> where T : IEntity
    {
        T Get(Guid id);

        void Add(T entity);
        void Remove(Guid id);
        void Clear();

        IEnumerable<T> All();
    }

    public class EntityCache<T> :
        IEntityCache<T>
        where T : IEntity
    {
        private Dictionary<Guid, T> _Entities = new Dictionary<Guid, T>();

        public T Get(Guid id)
        {
            if (_Entities.ContainsKey(id))
                return _Entities[id];
            else
                return default(T);
        }

        public void Add(T entity)
        {
            _Entities.Add(entity.Id, entity);
        }

        public void Remove(Guid id)
        {
            _Entities.Remove(id);
        }

        public void Clear()
        {
            _Entities.Clear();
        }

        public IEnumerable<T> All()
        {
            return _Entities.Values;
        }
    }
}
