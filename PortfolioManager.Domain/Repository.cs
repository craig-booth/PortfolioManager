using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain
{
    public interface IRepository<T> where T : ITrackedEntity
    {
        T Get(Guid id);
        void Add(T entity);
        void Update(T entity);
    }
    
    public interface ILoadableRepository<T>
    {
        void LoadFromEventStream();
    }

    public class Repository<T> : IRepository<T>
         where T : ITrackedEntity, new()
    {
        protected IEventStream<T> _EventStream;
        protected IEntityCache<T> _Cache;

        public Repository(IEventStream<T> eventStream, IEntityCache<T> cache)
        {
            _EventStream = eventStream;
            _Cache = cache;
        }

        public T Get(Guid id)
        {
            var entity = _Cache.Get(id);
            if (entity != null)
                return entity;

            var storedEntity = _EventStream.Get(id);
            if (storedEntity == null)
                return default(T);

            entity = new T();
            entity.ApplyEvents(storedEntity.Events);
            _Cache.Add(entity);

            return entity;
        }

        protected virtual T CreateEntity(StoredEntity storedEntity)
        {
            var entity = new T();
            entity.ApplyEvents(storedEntity.Events);

            return entity;
        }

        public void Add(T entity)
        {
            var newEvents = entity.FetchEvents();

            _EventStream.Add(entity.Id, entity.GetType().Name, newEvents);

            _Cache.Add(entity);
        }

        public void Update(T entity)
        {
            var newEvents = entity.FetchEvents();

            _EventStream.AppendEvents(entity.Id, newEvents);
        }
    }
}
