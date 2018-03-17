﻿using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;

namespace PortfolioManager.Domain
{
    public abstract class EffectiveEntity
    {
        public Guid Id { get; }
        public DateRange EffectivePeriod { get; private set; }

        public EffectiveEntity(Guid id, DateTime fromDate)
        {
            Id = id;
            EffectivePeriod = new DateRange(fromDate, DateUtils.NoEndDate);
        }

        public bool IsEffectiveAt(DateTime date)
        {
            return EffectivePeriod.Contains(date);
        }

        public bool IsEffectiveDuring(DateRange dateRange)
        {
            return EffectivePeriod.Overlaps(dateRange);
        }

        protected virtual void End(DateTime date)
        {
            if (!EffectivePeriod.ToDate.Equals(DateUtils.NoEndDate))
                throw new Exception("Entity is not current");

            EffectivePeriod = new DateRange(EffectivePeriod.FromDate, date);
        }
    }

    public class EffectiveProperties<T> 
        where T :struct
    {
        protected Stack<EffectivePropertyValues<T>> _Properties = new Stack<EffectivePropertyValues<T>>();

        public T this[DateTime date]
        {
            get 
            {
                return _Properties.First(x => x.IsEffectiveAt(date)).Properties;
            }
        }

        public T ClosestTo(DateTime date)
        {
           if (date <= _Properties.First().EffectivePeriod.ToDate)
                return _Properties.First().Properties;
           else if (date >= _Properties.Last().EffectivePeriod.FromDate)
                return _Properties.Last().Properties;
           else
                return _Properties.First(x => x.IsEffectiveAt(date)).Properties;
        }

        public bool Matches(Func<T, bool> predicate)
        {
            var match = _Properties.FirstOrDefault(x => predicate(x.Properties));
            return (match != null);
        }

        public bool Matches(DateTime date, Func<T, bool> predicate)
        {
            var match = _Properties.FirstOrDefault(x => x.IsEffectiveAt(date) && predicate(x.Properties));
            return (match != null);
        }

        public bool Matches(DateRange dateRange, Func<T, bool> predicate)
        {
            var match = _Properties.FirstOrDefault(x => x.IsEffectiveDuring(dateRange) && predicate(x.Properties));
            return (match != null);
        }

        public void Change(DateTime date, T newProperties)
        {
            if (_Properties.Count > 0)
            {
                var currentProperties = _Properties.Peek();

                if (!currentProperties.IsEffectiveAt(date))
                    throw new Exception("Only the current period can be modified");

                if (currentProperties.EffectivePeriod.FromDate.Equals(date))
                    _Properties.Pop();
                else
                    currentProperties.End(date.AddDays(-1));
            }
            _Properties.Push(new EffectivePropertyValues<T>(date, newProperties));
        }

        public void End(DateTime date)
        {
            if (_Properties.Count > 0)
            {
                var currentProperties = _Properties.Peek();
                currentProperties.End(date);
            }
        }
    }

    public class EffectivePropertyValues<T> where T : struct
    {
        public DateRange EffectivePeriod { get; private set; }
        public T Properties { get; }

        public EffectivePropertyValues(DateTime fromDate, T properties)
        {
            EffectivePeriod = new DateRange(fromDate, DateUtils.NoEndDate);
            Properties = properties;
        }

        public void End(DateTime date)
        {
            EffectivePeriod = new DateRange(EffectivePeriod.FromDate, date);
        }

        public bool IsEffectiveAt(DateTime date)
        {
            return EffectivePeriod.Contains(date);
        }

        public bool IsEffectiveDuring(DateRange dateRange)
        {
            return EffectivePeriod.Overlaps(dateRange);
        }
    }
}
