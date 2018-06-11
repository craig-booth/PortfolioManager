using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Common.Scheduler
{
    public abstract class DateSchedule
    {
        public DateRepetition DateRepetition { get; protected set; }

        internal DateSchedule(DateUnit dateUnit)
        {
            DateRepetition = new DateRepetition(1, dateUnit);
        }

        public abstract DateSchedule Every(int count, DateUnit units);

        public abstract DateTime FirstRunDate(DateTime after);
        public abstract DateTime NextRunDate(DateTime after);

        public TimeSchedule At(int hour, int minute)
        {
            return new TimeSchedule(this, hour, minute);
        }

        public TimeSchedule Every(int count, TimeUnit units)
        {
            return new TimeSchedule(this, count, units);
        }

        public abstract bool ValidDate(DateTime date);

        public override string ToString()
        {
            string value = "";

            if (DateRepetition != null)
                value = DateRepetition.ToString();

            return value;
        }
    }

    public class DateRepetition
    {
        public readonly int Count;
        public readonly DateUnit Units;

        public DateRepetition(int count, DateUnit units)
        {
            Count = count;
            Units = units;
        }

        public override string ToString()
        {
            if (Count == 1)
                return Schedule.DateUnitVerbDescriptions[(int)Units];
            else if (Count <= 4)
                return String.Format("every {0} {1}", Schedule.OccuranceDescription(Count), Schedule.DateUnitDescriptions[(int)Units]);
            else
                return String.Format("every {0} {1}", Count, Schedule.DateUnitPluralDescriptions[(int)Units]);
        }

    }
}
