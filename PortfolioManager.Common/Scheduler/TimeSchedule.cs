using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Common.Scheduler
{
    public class TimeSchedule : ISchedule
    {
        private readonly DateSchedule _DateSchedule;
        public TimeRepetition TimeRepetition { get; private set; }
        public TimeSpan RunAtTime { get; private set; }
        public TimeSpan FromTime { get; private set; }
        public TimeSpan ToTime { get; private set; }

        public DateTime LastRunTime { get; private set; }

        private DateTime _RunDate;

        internal TimeSchedule(DateSchedule dateSchedule)
        {
            _DateSchedule = dateSchedule;
            TimeRepetition = null;
            RunAtTime = new TimeSpan(0);
            FromTime = new TimeSpan(0);
            ToTime = new TimeSpan(0);
            LastRunTime = new DateTime(0001, 01, 01);
        }

        internal TimeSchedule(DateSchedule dateSchedule, int hour, int minute)
            : this(dateSchedule)
        {
            RunAtTime = new TimeSpan(hour, minute, 0);
        }

        internal TimeSchedule(DateSchedule dateSchedule, int count, TimeUnit units)
            : this(dateSchedule)
        {
            TimeRepetition = new TimeRepetition(count, units);
        }

        public TimeSchedule From(int hour, int minute)
        {
            FromTime = new TimeSpan(hour, minute, 0);

            return this;
        }

        public TimeSchedule To(int hour, int minute)
        {
            ToTime = new TimeSpan(hour, minute, 0);

            return this;
        }

        public DateTime FirstRunTime()
        {
            return FirstRunTime(DateTime.Now);
        }

        public DateTime FirstRunTime(DateTime starting)
        {
            var startDate = _DateSchedule.FirstRunDate(starting.Date);
            var firstTime = FirstRunForDate(startDate);

            var runTime = startDate.Add(firstTime);
            _RunDate = startDate;
            LastRunTime = runTime;

            if (runTime < starting)
            {
                while (runTime < starting)
                    runTime = NextRunTime();

                _RunDate = runTime.Date; ;
                LastRunTime = runTime;
            }

            return LastRunTime;
        }

        public DateTime NextRunTime()
        {
            if (!NextRunForDate(LastRunTime, out TimeSpan time))
            {
                _RunDate = _DateSchedule.NextRunDate(_RunDate);
                time = FirstRunForDate(_RunDate);
            }

            LastRunTime = _RunDate.Add(time);
            return LastRunTime;
        }

        private TimeSpan FirstRunForDate(DateTime date)
        {

            if (TimeRepetition != null)
            {
                if (FromTime.Ticks != 0)
                    return FromTime;
                else
                    return new TimeSpan(0);
            }
            else
                return RunAtTime;
        }

        private bool NextRunForDate(DateTime lastRunTime, out TimeSpan time)
        {
            if (TimeRepetition != null)
            {
                DateTime nextRunTime;

                if (TimeRepetition.Units == TimeUnit.Minutes)
                    nextRunTime = lastRunTime.AddMinutes(TimeRepetition.Count);
                else if (TimeRepetition.Units == TimeUnit.Hours)
                    nextRunTime = lastRunTime.AddHours(TimeRepetition.Count);
                else
                    nextRunTime = lastRunTime;

                // Check if we have run out of times for the date
                if (nextRunTime.Date != lastRunTime.Date)
                {
                    time = new TimeSpan(0);
                    return false;
                }

                time = nextRunTime.TimeOfDay;

                // Check if we are past the end time
                if ((ToTime.Ticks != 0) && (time > ToTime))
                {
                    time = new TimeSpan(0);
                    return false;
                }

                return true;
            }

            time = new TimeSpan(0);
            return false;
        }

        public override string ToString()
        {
            var value = "Run " + _DateSchedule.ToString();

            if (TimeRepetition != null)
            {
                value += " every " + TimeRepetition.ToString();

                if ((FromTime.Ticks != 0) && (ToTime.Ticks != 0))
                    value += " between " + FromTime.ToString(@"hh\:mm") + " and " + ToTime.ToString(@"hh\:mm");
                else if (FromTime.Ticks != 0)
                    value += " from " + FromTime.ToString(@"hh\:mm");
                else if (ToTime.Ticks != 0)
                    value += " until " + ToTime.ToString(@"hh\:mm");
            }
            else
                value += " at " + RunAtTime.ToString(@"hh\:mm");

            return value;
        }
    }

    public class TimeRepetition
    {
        public readonly int Count;
        public readonly TimeUnit Units;

        public TimeRepetition(int count, TimeUnit units)
        {
            Count = count;
            Units = units;
        }

        public override string ToString()
        {
            if (Count == 1)
                return Schedule.TimeUnitDescriptions[(int)Units];
            else if (Count <= 4)
                return String.Format("{0} {1}", Schedule.OccuranceDescription(Count), Schedule.TimeUnitDescriptions[(int)Units]);
            else
                return String.Format("{0} {1}", Count, Schedule.TimeUnitPluralDescriptions[(int)Units]);
        }

    }
}
