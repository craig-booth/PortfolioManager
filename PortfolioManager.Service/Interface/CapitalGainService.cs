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

        Task<CGTResponce> GetCGTLiability(DateTime fromDate, DateTime toDate);
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

    public class CGTResponce
    {
    }

}
