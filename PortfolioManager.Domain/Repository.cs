using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain
{
    public interface IRepository<T> where T : ITrackedEntity
    {
        T Get(Guid id);
        void Save(T entity);
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

            entity = new T();

            var events = _EventStream.RetrieveEvents(id);
            entity.ApplyEvents(events);

            _Cache.Add(entity);

            return entity;
        }

        public void Save(T entity)
        {
            var newEvents = entity.FetchEvents();

            _EventStream.StoreEvents(entity.Id, newEvents);
        }
    }
}
