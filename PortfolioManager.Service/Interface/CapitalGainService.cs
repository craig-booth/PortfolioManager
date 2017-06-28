using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.Service.Interface
{
    public interface ICapitalGainService : IPortfolioService
    {
        Task<SimpleUnrealisedGainsResponce> GetSimpleUnrealisedGains(DateTime date);
        Task<SimpleUnrealisedGainsResponce> GetSimpleUnrealisedGains(Guid stockId, DateTime date);
        Task<DetailedUnrealisedGainsResponce> GetDetailedUnrealisedGains(DateTime date);
        Task<DetailedUnrealisedGainsResponce> GetDetailedUnrealisedGains(Guid stockId, DateTime date);

        Task<CGTLiabilityResponce> GetCGTLiability(DateTime fromDate, DateTime toDate);
    }

    public class SimpleUnrealisedGainsResponce : ServiceResponce
    {
        public List<SimpleUnrealisedGainsItem> CGTItems;

        public SimpleUnrealisedGainsResponce()
            : base()
        {
            CGTItems = new List<SimpleUnrealisedGainsItem>();
        }
    }

    public class SimpleUnrealisedGainsItem
    {
        public Guid Id { get; set; }
        public StockItem Stock { get; set; }

        public DateTime AquisitionDate { get; set; }
        public int Units { get; set; }
        public decimal CostBase { get; set; }
        public decimal MarketValue { get; set; }
        public decimal CapitalGain { get; set; }
        public decimal DiscoutedGain { get; set; }
        public CGTMethod DiscountMethod { get; set; }
    }


    public class DetailedUnrealisedGainsResponce: ServiceResponce
    {
        public List<DetailedUnrealisedGainsItem> CGTItems;

        public DetailedUnrealisedGainsResponce()
            : base()
        {
            CGTItems = new List<DetailedUnrealisedGainsItem>();
        }
    }

    public class DetailedUnrealisedGainsItem : SimpleUnrealisedGainsItem
    {
        public List<CGTEventItem> CGTEvents;

        public DetailedUnrealisedGainsItem()
        {
            CGTEvents = new List<CGTEventItem>();
        }

    }

    public class CGTEventItem
    {
        public DateTime Date { get; set; }
        public TransactionType TransactionType { get; set; }
        public int UnitChange { get; set; }
        public int Units { get; set; }
        public decimal CostBaseChange { get; set; }
        public decimal CostBase { get; set; }
        public string Comment { get; set; }
    }

    public class CGTLiabilityResponce : ServiceResponce
    {
        public decimal CurrentYearCapitalGainsOther { get; set; }
        public decimal CurrentYearCapitalGainsDiscounted { get; set; }
        public decimal CurrentYearCapitalGainsTotal { get; set; }
        public decimal CurrentYearCapitalLossesOther { get; set; }
        public decimal CurrentYearCapitalLossesDiscounted { get; set; }
        public decimal CurrentYearCapitalLossesTotal { get; set; }
        public decimal GrossCapitalGainOther { get; set; }
        public decimal GrossCapitalGainDiscounted { get; set; }
        public decimal GrossCapitalGainTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal NetCapitalGainOther { get; set; }
        public decimal NetCapitalGainDiscounted { get; set; }
        public decimal NetCapitalGainTotal { get; set; }

        public List<CGTLiabilityItem> Items { get; set; }

        public CGTLiabilityResponce()
        {
            Items = new List<CGTLiabilityItem>();
        }
    }

    public class CGTLiabilityItem
    {
        public StockItem Stock { get; set; }
        public DateTime EventDate { get; set; }
        public decimal CostBase { get; set; }
        public decimal AmountReceived { get; set; }
        public decimal CapitalGain { get; set; }
        public CGTMethod Method { get; set; }
    }
}
