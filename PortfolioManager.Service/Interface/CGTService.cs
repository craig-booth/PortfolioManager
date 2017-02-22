using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.Interface
{
    public interface ICGTService : IPortfolioManagerService
    {
        Task<SimpleCGTResponce> GetSimpleCGT(DateTime date);
        Task<SimpleCGTResponce> GetSimpleCGT(Guid stockId, DateTime date);
        Task<DetailedCGTResponce> GetDetailedCGT(DateTime date);
        Task<DetailedCGTResponce> GetDetailedCGT(Guid stockId, DateTime date);
    }

    public class SimpleCGTResponce
    {
        public List<SimpleCGTResponceItem> CGTItems;

        public SimpleCGTResponce()
        {
            CGTItems = new List<SimpleCGTResponceItem>();
        }
    }

    public class SimpleCGTResponceItem
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


    public class DetailedCGTResponce
    {
        public List<DetailedCGTResponceItem> CGTItems;

        public DetailedCGTResponce()
        {
            CGTItems = new List<DetailedCGTResponceItem>();
        }
    }

    public class DetailedCGTResponceItem : SimpleCGTResponceItem
    {
        public List<DetailedCGTParcelHistoryItem> History;

        public DetailedCGTResponceItem()
        {
            History = new List<DetailedCGTParcelHistoryItem>();
        }

    }


    public class DetailedCGTParcelHistoryItem
    {
        public DateTime Date { get; set; }
        public TransactionType TransactionType { get; set; }
        public int Units { get; set; }
        public decimal Amount { get; set; }
        public decimal CostBase { get; set; }
        public string Comment { get; set; }
    }
}
