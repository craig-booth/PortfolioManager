using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.CorporateActions.Events
{
    public class TransformationAddedEvent : CorporateActionAddedEvent
    {
        public DateTime ImplementationDate { get; set; }
        public decimal CashComponent { get; set; }
        public bool RolloverRefliefApplies { get; set; }

        public List<ResultingStock> ResultingStocks = new List<ResultingStock>();

        public TransformationAddedEvent(Guid entityId, int version, Guid actionId, DateTime actionDate, string description, DateTime implementationDate, decimal cashComponent, bool rolloverReliefApplies, IEnumerable<ResultingStock> resultingStocks)
            : base(entityId, version, actionId, actionDate, description)
        {
            ImplementationDate = implementationDate;
            CashComponent = cashComponent;
            RolloverRefliefApplies = rolloverReliefApplies;
            ResultingStocks.AddRange(resultingStocks);
        }

        public class ResultingStock
        {
            public Guid Stock { get; set; }
            public int OriginalUnits { get; set; }
            public int NewUnits { get; set; }
            public decimal CostBase { get; set; }
            public DateTime AquisitionDate { get; set; }

            public ResultingStock(Guid stock, int originalUnits, int newUnits, decimal costBase, DateTime aquisitionDate)
            {
                Stock = stock;
                OriginalUnits = originalUnits;
                NewUnits = newUnits;
                CostBase = costBase;
                AquisitionDate = aquisitionDate;
            }
        }
    }
}
