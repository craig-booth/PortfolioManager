using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;

namespace PortfolioManager.Domain
{
    public interface IEntity
    {
        Guid Id { get; }
    }

    public interface IEffectiveEntity<T> : IEntity where T : EffectiveProperties 
    {
        DateRange EffectivePeriod { get; }

        T Get(DateTime date);
        bool IsEffectiveAt(DateTime date);
        bool IsEffectiveDuring(DateRange dateRange);
        bool Matches(Func<T, bool> predicate);
        bool Matches(DateTime date, Func<T, bool> predicate);
        bool Matches(DateRange dateRange, Func<T, bool> predicate);
    }

    public abstract class EffectiveEntity<T> : IEffectiveEntity<T>  where T : EffectiveProperties
    {
        protected List<T> _Properties = new List<T>();
        protected T _CurrentProperties;

        public Guid Id { get; }
        public DateRange EffectivePeriod { get; private set; }

        public EffectiveEntity(Guid id, DateTime fromDate)
        {
            Id = id;
            EffectivePeriod = new DateRange(fromDate, DateUtils.NoEndDate);
            _CurrentProperties = null;
        }

        public T Get(DateTime date)
        {
            if (IsEffectiveAt(date))
            {
                return _Properties.FirstOrDefault(x => x.IsEffectiveAt(date));
            }
            else
                throw new ArgumentOutOfRangeException();
        }

        public bool IsEffectiveAt(DateTime date)
        {
            return EffectivePeriod.Contains(date);
        }

        public bool IsEffectiveDuring(DateRange dateRange)
        {
            return EffectivePeriod.Overlaps(dateRange);
        }

        public bool Matches(Func<T, bool> predicate)
        {
            var match = _Properties.FirstOrDefault(x => predicate(x));
            return (match != null);
        }

        public bool Matches(DateTime date, Func<T, bool> predicate)
        {
            var match = _Properties.FirstOrDefault(x => x.IsEffectiveAt(date) && predicate(x));
            return (match != null);
        }

        public bool Matches(DateRange dateRange, Func<T, bool> predicate)
        {
            var match = _Properties.FirstOrDefault(x => x.IsEffectiveDuring(dateRange) && predicate(x));
            return (match != null);
        }

        protected void Change(DateTime date, Action<T> change)
        {
            if (_CurrentProperties == null)
                throw new Exception("Entity is not current");

            if (!_CurrentProperties.IsEffectiveAt(date))
                throw new Exception("Only the current period can be modified");

            if (_CurrentProperties.EffectivePeriod.FromDate.Equals(date))
            {
                change(_CurrentProperties);
            }
            else
            {
                _CurrentProperties.End(date.AddDays(-1));

                _CurrentProperties = (T)_CurrentProperties.Copy(new DateRange(date, DateUtils.NoDate));
                change(_CurrentProperties);
                _Properties.Add(_CurrentProperties);
            }

        }

        public void End(DateTime date)
        {
            if (_CurrentProperties == null)
                throw new Exception("Entity is not current");

            _CurrentProperties.End(date);
            _CurrentProperties = null;

            EffectivePeriod = new DateRange(EffectivePeriod.FromDate, date);
        }
    }

    public abstract class EffectiveProperties
    {
        public Guid Id { get; }
        public DateRange EffectivePeriod { get; private set; }

        public EffectiveProperties(Guid id, DateTime fromDate)
        {
            Id = Guid.NewGuid();
            EffectivePeriod = new DateRange(fromDate, DateUtils.NoEndDate);
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

        public abstract EffectiveProperties Copy(DateRange newRange);
    }
}
