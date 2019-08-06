﻿using System;
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
        void PopulateCache();
    }

    public class Repository<T> : IRepository<T>
         where T : ITrackedEntity
    {
        protected IEventStream<T> _EventStream;
        protected IEntityCache<T> _Cache;
        protected IEntityFactory<T> _EntityFactory;
        protected Func<string, T> _CreateFunction;

        private Repository(IEventStream<T> eventStream, IEntityCache<T> cache)
        {
            _EventStream = eventStream;
            _Cache = cache;
        }

        public Repository(IEventStream<T> eventStream, IEntityCache<T> cache, IEntityFactory<T> entityFactory)
            : this(eventStream, cache)
        {
            _EntityFactory = entityFactory;
        }

        public Repository(IEventStream<T> eventStream, IEntityCache<T> cache, Func<string, T> createFunction)
            : this(eventStream, cache)
        {
            _CreateFunction = createFunction;
        }

        public T Get(Guid id)
        {
            var entity = _Cache.Get(id);
            if (entity != null)
                return entity;

            var storedEntity = _EventStream.Get(id);
            if (storedEntity == null)
                return default(T);

            entity = CreateEntity(storedEntity);
            _Cache.Add(entity);

            return entity;
        }

        protected T CreateEntity(StoredEntity storedEntity)
        {
            T entity;

            if (_EntityFactory != null)
                entity = _EntityFactory.Create(storedEntity.Type);
            else
                entity = _CreateFunction(storedEntity.Type);

            ((dynamic)entity).ApplyEvents(storedEntity.Events);

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

        public void PopulateCache()
        {
            var storedEntities = _EventStream.GetAll();
            foreach (var storedEntity in storedEntities)
            {
                var entity = CreateEntity(storedEntity);
                if (entity != null)
                    _Cache.Add(entity);
            }
        }
    }
    
}
