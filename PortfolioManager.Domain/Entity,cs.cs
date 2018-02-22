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

    public interface IEffectiveEntity : IEntity
    {
        DateRange EffectivePeriod { get; }
    }

    public abstract class EffectiveEntity<T> where T : EffectiveData
    {
        protected List<T> _Data = new List<T>();
        protected T _CurrentValues;

        public Guid Id { get; }
        public DateRange EffectivePeriod { get; private set; }

        public EffectiveEntity(Guid id, DateTime fromDate)
        {
            Id = id;
            EffectivePeriod = new DateRange(fromDate, DateUtils.NoEndDate);
            _CurrentValues = null;
        }

        public T Get(DateTime date)
        {
            if (IsEffectiveAt(date))
            {
                return _Data.FirstOrDefault(x => x.IsEffectiveAt(date));
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
            var match = _Data.FirstOrDefault(x => predicate(x));
            return (match != null);
        }

        public bool Matches(DateTime date, Func<T, bool> predicate)
        {
            var match = _Data.FirstOrDefault(x => x.IsEffectiveAt(date) && predicate(x));
            return (match != null);
        }

        public bool Matches(DateRange dateRange, Func<T, bool> predicate)
        {
            var match = _Data.FirstOrDefault(x => x.IsEffectiveDuring(dateRange) && predicate(x));
            return (match != null);
        }

        private void Change(DateTime date, Action<T> change)
        {
            if (_CurrentValues == null)
                throw new Exception("Entity is not current");

            if (!_CurrentValues.IsEffectiveAt(date))
                throw new Exception("Only the current period can be modified");


            if (_CurrentValues.EffectivePeriod.FromDate.Equals(date))
            {
                change(_CurrentValues);
            }
            else
            {
                _CurrentValues.End(date.AddDays(-1));

                _CurrentValues = (T)_CurrentValues.Copy(new DateRange(date, DateUtils.NoDate));
                change(_CurrentValues);
                _Data.Add(_CurrentValues);
            }

        }

        public void End(DateTime date)
        {
            if (_CurrentValues == null)
                throw new Exception("Entity is not current");

            _CurrentValues.End(date);
            _CurrentValues = null;

            EffectivePeriod = new DateRange(EffectivePeriod.FromDate, date);
        }
    }

    public abstract class EffectiveData
    {
        public Guid Id { get; }
        public DateRange EffectivePeriod { get; private set; }

        public EffectiveData(Guid id, DateTime fromDate, DateTime toDate)
        {
            Id = Guid.NewGuid();
            EffectivePeriod = new DateRange(fromDate, toDate);
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

        public abstract EffectiveData Copy(DateRange newRange);
    }
}
