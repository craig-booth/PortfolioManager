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

    public abstract class EffectiveEntity<T> where T : struct
    {
        protected Stack<EffectiveProperties<T>> _Properties = new Stack<EffectiveProperties<T>>();

        public Guid Id { get; }
        public DateRange EffectivePeriod { get; private set; }

        public EffectiveEntity(Guid id, DateTime fromDate)
        {
            Id = id;
            EffectivePeriod = new DateRange(fromDate, DateUtils.NoEndDate);
        }

        public T GetProperties(DateTime date)
        {
            if (IsEffectiveAt(date))
            {
                return _Properties.FirstOrDefault(x => x.IsEffectiveAt(date)).Properties;
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

        protected void Change(DateTime date, T newProperties)
        {
            if (!EffectivePeriod.Contains(date))
                throw new Exception("Entity is not current at that time");

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
            _Properties.Push(new EffectiveProperties<T>(Id, date, newProperties));
        }

        public void End(DateTime date)
        {
            if (! EffectivePeriod.ToDate.Equals(DateUtils.NoEndDate))
                throw new Exception("Entity is not current");

            EffectivePeriod = new DateRange(EffectivePeriod.FromDate, date);

            if (_Properties.Count > 0)
            {
                var currentProperties = _Properties.Peek();
                currentProperties.End(date);

            }       
        }
    }

    public class EffectiveProperties<T> where T : struct
    {
        public Guid Id { get; }
        public DateRange EffectivePeriod { get; private set; }
        public T Properties { get; }

        public EffectiveProperties(Guid id, DateTime fromDate, T properties)
        {
            Id = Guid.NewGuid();
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
