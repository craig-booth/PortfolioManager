using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Common.Scheduler
{
    public enum TimeUnit { Minutes, Hours }
    public enum DateUnit { Days, Weeks, Months, Years }
    public enum Day { Sunday, Monday, Wednesday, Thrursday, Friday, Saturday }
    public enum Occurance { First, Second, Third, Fourth, Last }

    public interface ISchedule
    {
        DateTime FirstRunTime();
        DateTime FirstRunTime(DateTime starting);
        DateTime NextRunTime();

        string ToString();
    }

    public static class Schedule
    {
        public static DailySchedule Daily()
        {
            return new DailySchedule();
        }

        public static WeeklySchedule Weekly()
        {
            return new WeeklySchedule();
        }

        public static MonthlySchedule Monthly()
        {
            return new MonthlySchedule();
        }

        public static string[] TimeUnitDescriptions = { "minute", "hour" };
        public static string[] TimeUnitPluralDescriptions = { "minutes", "hours" };
        public static string[] TimeUnitVerbDescriptions = { "every minute", "hourly" };

        public static string[] DateUnitDescriptions = { "day", "week", "month", "year" };
        public static string[] DateUnitPluralDescriptions = { "days", "weeks", "months", "years" };
        public static string[] DateUnitVerbDescriptions = { "daily", "weekly", "monthly", "annually" };

        public static string OccuranceDescription(int value)
        {
            if (value == 1)
                return "first";
            else if (value == 2)
                return "second";
            else if (value == 3)
                return "third";
            else if (value == 4)
                return "fourth";
            else
                return value.ToString();
        }
    }
}
