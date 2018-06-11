using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Common.Scheduler
{
    public class DailySchedule : DateSchedule
    {
        internal DailySchedule() : base(DateUnit.Days) { }

        public override DateSchedule Every(int count, DateUnit units)
        {
            if (units != DateUnit.Days)
                throw new Exception("Units must be Days for a daily schedule");

            DateRepetition = new DateRepetition(count, units);

            return this;
        }

        public override bool ValidDate(DateTime date)
        {
            return true;
        }

        public override DateTime FirstRunDate(DateTime after)
        {
            return after.Date;
        }

        public override DateTime NextRunDate(DateTime after)
        {
            return after.AddDays(DateRepetition.Count).Date;
        }
    }
}
