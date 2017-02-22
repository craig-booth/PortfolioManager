using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Service.Interface
{
    public interface IPortfolioPerformanceService : IPortfolioManagerService
    {
        Task<PortfolioPerformanceResponce> GetPerformance(DateTime fromDate, DateTime toDate);
    }

    public class PortfolioPerformanceResponce
    {
        public decimal OpeningBalance { get; set; }
        public decimal Dividends { get; set; }
        public decimal ChangeInMarketValue { get; set; }
        public decimal OutstandingDRPAmount { get; set; }
        public decimal ClosingBalance { get; set; }

        public decimal OpeningCashBalance { get; set; }
        public decimal Deposits { get; set; }
        public decimal Withdrawls { get; set; }
        public decimal Interest { get; set; }
        public decimal Fees { get; set; }
        public decimal ClosingCashBalance { get; set; }


        public List<HoldingPerformance> HoldingPerformance { get; set; }

        public PortfolioPerformanceResponce()
        {

        }
    }

    public class HoldingPerformance
    {
        public string ASXCode { get; set; }
        public string CompanyName { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Purchases { get; set; }
        public decimal Sales { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal Dividends { get; set; }
        public decimal CapitalGain { get; set; }
        public decimal DRPCashBalance { get; set; }
        public decimal TotalReturn { get; set; }
        public double IRR { get; set; }

        public HoldingPerformance(string asxCode, string companyName)
        {
            ASXCode = asxCode;
            CompanyName = companyName;
            OpeningBalance = 0.00m;
            Purchases = 0.00m;
            Sales = 0.00m;
            Dividends = 0.00m;
            CapitalGain = 0.00m;
            ClosingBalance = 0.00m;
            DRPCashBalance = 0.00m;
        }

    }
}
