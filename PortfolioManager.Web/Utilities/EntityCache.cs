﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using PortfolioManager.Domain;

namespace PortfolioManager.Web.Utilities
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
        private ConcurrentDictionary<Guid, T> _Entities = new ConcurrentDictionary<Guid, T>();

        public T Get(Guid id)
        {
            if (_Entities.ContainsKey(id))
                return _Entities[id];
            else
                return default(T);
        }

        public void Add(T entity)
        {
            _Entities.TryAdd(entity.Id, entity);
        }

        public void Remove(Guid id)
        {
            _Entities.TryRemove(id, out var entity);
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

    public static class EntityCacheExtensions
    {
        public static IEntityCache<T> PopulateCache<T>(this IEntityCache<T> entityCache, IRepository<T> repository)
            where T : ITrackedEntity
        {
            entityCache.Clear();

            foreach (var entity in repository.All())
                entityCache.Add(entity);

            return entityCache;
        }
    }

}
