using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PortfolioManager.Common.Scheduler
{
    public class WeeklySchedule : DateSchedule
    {
        private List<Day> _RunOnDays = new List<Day>();

        internal WeeklySchedule() : base(DateUnit.Weeks) { }

        public override DateSchedule Every(int count, DateUnit units)
        {
            if (units != DateUnit.Weeks)
                throw new Exception("Units must be Weeks for a weekly schedule");

            DateRepetition = new DateRepetition(count, units);

            return this;
        }

        public WeeklySchedule On(Day day)
        {
            if (!_RunOnDays.Contains(day))
                _RunOnDays.Add(day);

            return this;
        }

        public override string ToString()
        {
            if (_RunOnDays.Count != 0)
            {
                var value = "on ";
                foreach (var day in _RunOnDays.OrderBy(x => (int)x))
                {
                    value += ((DayOfWeek)day) + ",";
                }
                return value.TrimEnd(',') + " " + base.ToString();
            }
            else
                return base.ToString();
        }

        public override bool ValidDate(DateTime date)
        {
            return _RunOnDays.Contains((Day)date.DayOfWeek);
        }

        public override DateTime FirstRunDate(DateTime after)
        {
            if (ValidDate(after))
                return after.Date;
            else
            {
                var firstRunTime = after.Date.AddDays(1);

                while (!ValidDate(firstRunTime))
                    firstRunTime = firstRunTime.AddDays(1);

                return firstRunTime;
            }
        }

        public override DateTime NextRunDate(DateTime after)
        {
            if (!ValidDate(after))
            {
                var nextRunTime = after.AddDays(1);

                while (!ValidDate(nextRunTime))
                    nextRunTime = nextRunTime.AddDays(1);

                return nextRunTime.Date;
            }
            else
                return after.AddDays(DateRepetition.Count * 7).Date;
        }
    }
}
