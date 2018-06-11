using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Common.Scheduler
{
    public class MonthlySchedule : DateSchedule
    {
        public int DayNumber { get; private set; }
        public Occurance Occurance { get; private set; }
        public Day Day { get; private set; }

        internal MonthlySchedule() : base(DateUnit.Months) { }

        public override DateSchedule Every(int count, DateUnit units)
        {
            if (units != DateUnit.Months)
                throw new Exception("Units must be Weeks for a monthly schedule");

            DateRepetition = new DateRepetition(count, units);

            return this;
        }

        public MonthlySchedule On(int dayNumber)
        {
            DayNumber = dayNumber;
            Occurance = Occurance.First;
            Day = Day.Sunday;

            return this;
        }

        public MonthlySchedule On(Occurance orrurance, Day day)
        {
            DayNumber = 0;
            Occurance = orrurance;
            Day = day;

            return this;
        }

        public override bool ValidDate(DateTime date)
        {
            var scheduledDay = SchdeduledDayForMonth(date.Year, date.Month);
            return (date.Day == scheduledDay);
        }

        public override DateTime FirstRunDate(DateTime after)
        {
            var scheduledDay = SchdeduledDayForMonth(after.Year, after.Month);

            if (after.Day <= scheduledDay)
                return new DateTime(after.Year, after.Month, scheduledDay);
            else
            {
                var nextMonth = after.AddMonths(1);
                scheduledDay = SchdeduledDayForMonth(nextMonth.Year, nextMonth.Month);

                return new DateTime(nextMonth.Year, nextMonth.Month, scheduledDay);
            }
        }

        public override DateTime NextRunDate(DateTime after)
        {
            var nextDate = after.AddMonths(DateRepetition.Count).Date;
            var dayForMonth = SchdeduledDayForMonth(nextDate.Year, nextDate.Month);

            return new DateTime(nextDate.Year, nextDate.Month, dayForMonth);
        }

        private int SchdeduledDayForMonth(int year, int month)
        {
            var lastDayOfMonth = DateTime.DaysInMonth(year, month);

            if (DayNumber == 0)
            {
                if (Occurance != Occurance.Last)
                {
                    var first = new DateTime(year, month, 1);
                    while (first.DayOfWeek != (DayOfWeek)Day)
                        first = first.AddDays(1);

                    var day = first.Day;

                    if (Occurance == Occurance.First)
                        return day;
                    else if (Occurance == Occurance.Second)
                        return day + 7;
                    else if (Occurance == Occurance.Third)
                        return day + 14;
                    else
                        return day + 21;
                }
                else
                {
                    var last = new DateTime(year, month, lastDayOfMonth);
                    while (last.DayOfWeek != (DayOfWeek)Day)
                        last = last.AddDays(-1);

                    return last.Day;
                }
            }
            else
            {
                if (DayNumber <= lastDayOfMonth)
                    return DayNumber;
                else
                    return lastDayOfMonth;
            }
        }


        public override string ToString()
        {
            string value;

            if (DayNumber != 0)
                value = String.Format("on the {0}st ", DayNumber);
            else if (Occurance == Occurance.Last)
                value = String.Format("on the last {0} ", (DayOfWeek)Day);
            else
                value = String.Format("on the {0} {1} ", Schedule.OccuranceDescription((int)Occurance), (DayOfWeek)Day);

            return value + DateRepetition.ToString();
        }
    }
}
