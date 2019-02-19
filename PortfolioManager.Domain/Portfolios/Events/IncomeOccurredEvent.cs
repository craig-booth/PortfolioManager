using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Portfolios.Events
{
    public class IncomeOccurredEvent : TransactionnOccurredEvent
    {
        public DateTime RecordDate { get; set; }
        public decimal FrankedAmount { get; set; }
        public decimal UnfrankedAmount { get; set; }
        public decimal FrankingCredits { get; set; }
        public decimal Interest { get; set; }
        public decimal TaxDeferred { get; set; }
        public bool CreateCashTransaction { get; set; }
        public decimal DRPCashBalance { get; set; }

        public IncomeOccurredEvent(Guid entityId, int version, DateTime date, Guid stock, string comment)
            : base(entityId, version, date, stock, comment)
        {

        }

    }
}
