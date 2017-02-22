using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.Interface
{
    public interface ICapitalGainService : IPortfolioManagerService
    {
        Task<SimpleUnrealisedGainsResponce> GetSimpleUnrealisedGains(DateTime date);
        Task<SimpleUnrealisedGainsResponce> GetSimpleUnrealisedGains(Guid stockId, DateTime date);
        Task<DetailedUnrealisedGainsResponce> GetDetailedUnrealisedGains(DateTime date);
        Task<DetailedUnrealisedGainsResponce> GetDetailedUnrealisedGains(Guid stockId, DateTime date);

        Task<CGTLiabilityResponce> GetCGTLiability(DateTime fromDate, DateTime toDate);
    }

    public class SimpleUnrealisedGainsResponce
    {
        public List<SimpleUnrealisedGainsItem> CGTItems;

        public SimpleUnrealisedGainsResponce()
        {
            CGTItems = new List<SimpleUnrealisedGainsItem>();
        }
    }

    public class SimpleUnrealisedGainsItem
    {
        public Guid Id { get; set; }
        public string ASXCode { get; set; }
        public string CompanyName { get; set; }

        public DateTime AquisitionDate { get; set; }
        public int Units { get; set; }
        public decimal CostBase { get; set; }
        public decimal MarketValue { get; set; }
        public decimal CapitalGain { get; set; }
        public decimal DiscoutedGain { get; set; }
        public CGTMethod DiscountMethod { get; set; }
    }


    public class DetailedUnrealisedGainsResponce
    {
        public List<DetailedUnrealisedGainsItem> CGTItems;

        public DetailedUnrealisedGainsResponce()
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
        public int Units { get; set; }
        public decimal Amount { get; set; }
        public decimal CostBase { get; set; }
        public string Comment { get; set; }
    }

    public class CGTLiabilityResponce
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
        public string ASXCode { get; set; }
        public string CompanyName { get; set; }
        public DateTime EventDate { get; set; }
        public decimal CostBase { get; set; }
        public decimal AmountReceived { get; set; }
        public decimal CapitalGain { get; set; }
        public CGTMethod Method { get; set; }
    }
}
