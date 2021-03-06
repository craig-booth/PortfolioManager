﻿using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.EventStore;
using PortfolioManager.Domain.CorporateActions;
using PortfolioManager.Domain.CorporateActions.Events;
using PortfolioManager.Domain.Utils;

namespace PortfolioManager.Domain.Stocks
{
    public class CorporateActionCollection :
        TransactionList<CorporateAction>,
        ITransactionList<CorporateAction>
    {
        private EventList _Events;
        public Stock Stock { get; }

        internal CorporateActionCollection(Stock stock, EventList eventList)
        {
            Stock = stock;
            _Events = eventList;
        }

        protected void PublishEvent(Event @event)
        {
            _Events.Add(@event);
        }

        public void AddCapitalReturn(Guid id, DateTime recordDate, string description, DateTime paymentDate, decimal amount)
        {
            if (description == "")
                description = "Capital Return " + amount.ToString("$#,##0.00###");

            var @event = new CapitalReturnAddedEvent(Stock.Id, Stock.Version, id, recordDate, description, paymentDate, amount);

            Apply(@event);
            PublishEvent(@event);
        }

        public void Apply(CapitalReturnAddedEvent @event)
        {
            var capitalReturn = new CapitalReturn(@event.ActionId, Stock, @event.ActionDate, @event.Description, @event.PaymentDate, @event.Amount);

            Add(capitalReturn);
        }

        public void AddDividend(Guid id, DateTime recordDate, string description, DateTime paymentDate, decimal dividendAmount, decimal percentFranked, decimal drpPrice)
        {
            if (description == "")
                description = "Dividend " + MathUtils.FormatCurrency(dividendAmount, false, true);

            var @event = new DividendAddedEvent(Stock.Id, Stock.Version, id, recordDate, description, paymentDate, dividendAmount, percentFranked, drpPrice);

            Apply(@event);
            PublishEvent(@event);
        }

        public void Apply(DividendAddedEvent @event)
        {
            var dividend = new Dividend(@event.ActionId, Stock, @event.ActionDate, @event.Description, @event.PaymentDate, @event.DividendAmount, @event.PercentFranked, @event.DRPPrice);

            Add(dividend);
        }

        public void AddTransformation(Guid id, DateTime recordDate, string description, DateTime implementationDate, decimal cashComponent, bool rolloverReliefApplies, IEnumerable<Transformation.ResultingStock> resultingStocks)
        {
            var eventResultingStocks = resultingStocks.Select(x => new TransformationAddedEvent.ResultingStock(x.Stock, x.OriginalUnits, x.NewUnits, x.CostBase, x.AquisitionDate));

            var @event = new TransformationAddedEvent(Stock.Id, Stock.Version, id, recordDate, description, implementationDate, cashComponent, rolloverReliefApplies, eventResultingStocks);                

            Apply(@event);
            PublishEvent(@event);
        }

        public void Apply(TransformationAddedEvent @event)
        {
            var transformationResultingStocks = @event.ResultingStocks.Select(x => new Transformation.ResultingStock(x.Stock, x.OriginalUnits, x.NewUnits, x.CostBase, x.AquisitionDate));
            var transformation = new Transformation(@event.ActionId, Stock, @event.ActionDate, @event.Description, @event.ImplementationDate, @event.CashComponent, @event.RolloverRefliefApplies, transformationResultingStocks);

            Add(transformation);
        }
    }
}
