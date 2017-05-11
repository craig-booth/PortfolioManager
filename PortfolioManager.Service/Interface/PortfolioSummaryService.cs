using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.Service.Interface
{
    public interface IPortfolioSummaryService : IPortfolioService
    {
        Task<PortfolioPropertiesResponce> GetProperties();
        Task<PortfolioSummaryResponce> GetSummary(DateTime date);
    }

    public class PortfolioPropertiesResponce : ServiceResponce
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<StockItem> StocksHeld { get; set; }

        public PortfolioPropertiesResponce()
            : base()
        {
            StocksHeld = new List<StockItem>();
        }
    }

    public class PortfolioSummaryResponce : ServiceResponce
    {
        public decimal PortfolioValue { get; set; }
        public decimal PortfolioCost { get; set; }

        public decimal? Return1Year { get; set; }
        public decimal? Return3Year { get; set; }
        public decimal? Return5Year { get; set; }
        public decimal? ReturnAll { get; set; }

        public decimal CashBalance { get; set; }

        public List<HoldingItem> Holdings { get; private set; }

        public PortfolioSummaryResponce()
            : base()
        {
            Holdings = new List<HoldingItem>();
        }
    }

}
