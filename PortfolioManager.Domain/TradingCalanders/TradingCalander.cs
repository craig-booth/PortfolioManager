﻿using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.EventStore;
using PortfolioManager.Domain.TradingCalanders.Events;

namespace PortfolioManager.Domain.TradingCalanders
{

    public static class TradingCalanderIds
    {
        public static Guid ASX => new Guid("712E464B-1CE6-4B21-8FB2-D679DFFE3EE3");
    }

    public interface ITradingCalander
    {
        IEnumerable<NonTradingDay> NonTradingDays(int year);
        bool IsTradingDay(DateTime date);
        IEnumerable<DateTime> TradingDays(DateRange range);

        void SetNonTradingDays(int year, IEnumerable<NonTradingDay> nonTradingDays);
    }

    public class TradingCalander :
        ITradingCalander,
        ITrackedEntity
    {
        public Guid Id { get; }

        public int Version { get; protected set; } = 0;
        private EventList _Events = new EventList();

        private List<NonTradingDay> _NonTradingDays = new List<NonTradingDay>();

        public TradingCalander(Guid id)
        {
            Id = id;
        }

        public void SetNonTradingDays(int year, IEnumerable<NonTradingDay> nonTradingDays)
        {
            // Check that each day is in the correct year
            var invalidDate = nonTradingDays.FirstOrDefault(x => x.Date.Year != year);
            if (invalidDate != null)
                throw new Exception(String.Format("Date {0} is not in calander year {1}", invalidDate, year));

            var @event = new NonTradingDaysSetEvent(Id, Version, year, nonTradingDays);
            Apply(@event);

            PublishEvent(@event);
        }

        protected void PublishEvent(Event @event)
        {
            _Events.Add(@event);
        }

        public void Apply(NonTradingDaysSetEvent @event)
        {
            Version++;

            // Remove any existing non trading days for the year
            _NonTradingDays.RemoveAll(x => x.Date.Year == @event.Year);

            foreach (var nonTradingDay in @event.NonTradingDays)
            {
                var newNonTradingDay = new NonTradingDay(nonTradingDay.Date, nonTradingDay.Desciption);
                var index = _NonTradingDays.BinarySearch(newNonTradingDay);
                if (index < 0)
                    _NonTradingDays.Insert(~index, newNonTradingDay);
            }
        }

        public IEnumerable<NonTradingDay> NonTradingDays(int year)
        {
            return _NonTradingDays.Where(x => x.Date.Year == year);
        }

        public bool IsTradingDay(DateTime date)
        {
            return (_NonTradingDays.BinarySearch(new NonTradingDay(date,"")) < 0);
        }

        public IEnumerable<DateTime> TradingDays(DateRange range)
        {
            return DateUtils.Days(range.FromDate, range.ToDate).Where(x => x.WeekDay() && IsTradingDay(x));
        }

        public IEnumerable<Event> FetchEvents()
        {
            return _Events.Fetch();
        }

        public void ApplyEvents(IEnumerable<Event> events)
        {
            foreach (var @event in events)
            {
                dynamic dynamicEvent = @event;
                Apply(dynamicEvent);
            }
        }
    }

    public class NonTradingDay : IComparable<NonTradingDay>
    {
        public DateTime Date { get; set; }
        public string Desciption { get; set; }

        public NonTradingDay(DateTime date, string description)
        {
            Date = date;
            Desciption = description;
        }

        public int CompareTo(NonTradingDay other)
        {
            return Date.CompareTo(other.Date);
        }
    }
}
