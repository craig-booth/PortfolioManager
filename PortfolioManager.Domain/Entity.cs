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

    interface IEffectiveEntity
    {
        Guid Id { get; }
        DateRange EffectivePeriod { get; }
    }
    
    public abstract class EffectiveEntity :
        IEntity,
        IEffectiveEntity
    {
        public Guid Id { get; private set; }
        public DateRange EffectivePeriod { get; private set; }

        protected virtual void Start(Guid id, DateTime date)
        {
            if (!EffectivePeriod.FromDate.Equals(DateUtils.NoStartDate))
                throw new Exception("Entity already started");

            Id = id;
            EffectivePeriod = new DateRange(date, DateUtils.NoEndDate);
        }

        protected virtual void End(DateTime date)
        {
            if (!EffectivePeriod.ToDate.Equals(DateUtils.NoEndDate))
                throw new Exception("Entity is not current");

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
