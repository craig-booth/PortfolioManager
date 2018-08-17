using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.EventStore;
using PortfolioManager.Domain.CorporateActions;
using PortfolioManager.Domain.CorporateActions.Events;

namespace PortfolioManager.Domain.Stocks
{
    public class CorporateActionCollection : IReadOnlyCollection<CorporateAction>
    {
        protected IEventStream _EventStream;
        public Stock Stock { get; }

        private readonly Dictionary<Guid, CorporateAction> _CorporateActions = new Dictionary<Guid, CorporateAction>();

        public CorporateActionCollection(Stock stock, IEventStream eventStream)
        {
            Stock = stock;
            _EventStream = eventStream;
        }

        public void AddCapitalReturn(Guid id, DateTime recordDate, string description, DateTime paymentDate, decimal amount)
        {
            if (description == "")
                description = "Capital Return " + amount.ToString("$#,##0.00###");

            var @event = new CapitalReturnAddedEvent(Stock.Id, Stock.Version, id, recordDate, description, paymentDate, amount);

            Apply(@event);
            _EventStream.StoreEvent(@event);
        }

        public void Apply(CapitalReturnAddedEvent @event)
        {
            var capitalReturn = new CapitalReturn(@event.ActionId, Stock, @event.ActionDate, @event.Description, @event.PaymentDate, @event.Amount);

            _CorporateActions.Add(capitalReturn.Id, capitalReturn);
        }

        public void AddDividend(Guid id, DateTime recordDate, string description, DateTime paymentDate, decimal dividendAmount, decimal companyTaxRate, decimal percentFranked, decimal drpPrice)
        {
            if (description == "")
                description = "Dividend " + MathUtils.FormatCurrency(dividendAmount, false, true);

            var @event = new DividendAddedEvent(Stock.Id, Stock.Version, id, recordDate, description, paymentDate, dividendAmount, companyTaxRate, percentFranked, drpPrice);

            Apply(@event);
            _EventStream.StoreEvent(@event);
        }

        public void Apply(DividendAddedEvent @event)
        {
            var dividend = new Dividend(@event.ActionId, Stock, @event.ActionDate, @event.Description, @event.PaymentDate, @event.DividendAmount, @event.CompanyTaxRate, @event.PercentFranked, @event.DRPPrice);

            _CorporateActions.Add(dividend.Id, dividend);
        }

        public void AddTransformation(Guid id, DateTime recordDate, string description, DateTime implementationDate, decimal cashComponent, bool rolloverReliefApplies, IEnumerable<Transformation.ResultingStock> resultingStocks)
        {
            var eventResultingStocks = resultingStocks.Select(x => new TransformationAddedEvent.ResultingStock(x.Stock, x.OriginalUnits, x.NewUnits, x.CostBase, x.AquisitionDate));

            var @event = new TransformationAddedEvent(Stock.Id, Stock.Version, id, recordDate, description, implementationDate, cashComponent, rolloverReliefApplies, eventResultingStocks);                

            Apply(@event);
            _EventStream.StoreEvent(@event);
        }

        public void Apply(TransformationAddedEvent @event)
        {
            var transformationResultingStocks = @event.ResultingStocks.Select(x => new Transformation.ResultingStock(x.Stock, x.OriginalUnits, x.NewUnits, x.CostBase, x.AquisitionDate));
            var transformation = new Transformation(@event.ActionId, Stock, @event.ActionDate, @event.Description, @event.ImplementationDate, @event.CashComponent, @event.RolloverRefliefApplies, transformationResultingStocks);

            _CorporateActions.Add(transformation.Id, transformation);
        }

        public CorporateAction this[Guid id] 
        {
            get
            {
                if (_CorporateActions.ContainsKey(id))
                    return _CorporateActions[id];
                else
                    return null;
            }
        } 

        public T Get<T>(Guid id) where T : CorporateAction
        {
            if (_CorporateActions.ContainsKey(id))
                return _CorporateActions[id] as T;
            else
                return null;
        } 
     
        public IEnumerable<CorporateAction> Get(DateRange dateRange)
        {
            return _CorporateActions.Values.Where(x => dateRange.Contains(x.ActionDate));
        } 
   
        public IEnumerable<T> Get<T>() where T : CorporateAction
        {
            return _CorporateActions.Values.Where(x => x is T).Select(x => x as T);
        } 

        public IEnumerable<T> Get<T>(DateRange dateRange) where T : CorporateAction
        {
            return _CorporateActions.Values.Where(x => x is T && dateRange.Contains(x.ActionDate)).Select(x => x as T);
        }

        public int Count
        {
            get { return _CorporateActions.Count; }
        }

        public IEnumerator<CorporateAction> GetEnumerator()
        {
            return _CorporateActions.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _CorporateActions.Values.GetEnumerator();
        }
    }
}
