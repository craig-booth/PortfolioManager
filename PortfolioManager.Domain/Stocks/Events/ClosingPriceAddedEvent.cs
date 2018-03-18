﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class ClosingPriceAddedEvent : IEvent
    {
        public Guid Id { get; }
        public DateTime Date { get; }
        public decimal ClosingPrice { get; }

        public ClosingPriceAddedEvent(Guid id, DateTime date, decimal closingPrice)
        {
            Id = id;
            Date = date;
            ClosingPrice = closingPrice;
        }
    }
}