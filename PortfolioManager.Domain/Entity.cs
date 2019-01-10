using System;
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

        public virtual void End(DateTime date)
        {
            if (!EffectivePeriod.ToDate.Equals(DateUtils.NoEndDate))
                throw new Exception("Entity is not current");

            EffectivePeriod = new DateRange(EffectivePeriod.FromDate, date);
        }
    }


}
