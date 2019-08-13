using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain
{
    public interface IRepository<T> where T : ITrackedEntity
    {
        T Get(Guid id);
        IEnumerable<T> All();
        void Add(T entity);
        void Update(T entity);
    }

    public class Repository<T> : IRepository<T>
         where T : ITrackedEntity
    {
        protected IEventStream<T> _EventStream;
        protected IEntityFactory<T> _EntityFactory;
        protected Func<Guid, string, T> _CreateFunction;

        private Repository(IEventStream<T> eventStream)
        {
            _EventStream = eventStream;
        }

        public Repository(IEventStream<T> eventStream, IEntityFactory<T> entityFactory)
            : this(eventStream)
        {
            _EntityFactory = entityFactory;
        }

        public Repository(IEventStream<T> eventStream, Func<Guid, string, T> createFunction)
            : this(eventStream)
        {
            _CreateFunction = createFunction;
        }

        public T Get(Guid id)
        {
            var storedEntity = _EventStream.Get(id);
            if (storedEntity == null)
                return default(T);

            var entity = CreateEntity(storedEntity);

            return entity;
        }

        public IEnumerable<T> All()
        {
            var storedEntities = _EventStream.GetAll();
            foreach (var storedEntity in storedEntities)
            {
                var entity = CreateEntity(storedEntity);
                if (entity != null)
                    yield return entity;
            }
        }

        protected T CreateEntity(StoredEntity storedEntity)
        {
            T entity;

            if (_EntityFactory != null)
                entity = _EntityFactory.Create(storedEntity.EntityId, storedEntity.Type);
            else
                entity = _CreateFunction(storedEntity.EntityId, storedEntity.Type);

            ((dynamic)entity).ApplyEvents(storedEntity.Events);

            return entity;
        }

        public void Add(T entity)
        {
            var newEvents = entity.FetchEvents();

            _EventStream.Add(entity.Id, entity.GetType().Name, newEvents);
        }

        public void Update(T entity)
        {
            var newEvents = entity.FetchEvents();

            _EventStream.AppendEvents(entity.Id, newEvents);
        }

    }
    
}
