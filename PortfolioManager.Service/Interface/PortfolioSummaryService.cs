using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Service.Interface
{
    public interface IPortfolioSummaryService : IPortfolioManagerService
    {
        Task<PortfolioPropertiesResponce> GetProperties();
        Task<PortfolioSummaryResponce> GetSummary(DateTime date);
    }

    public class PortfolioPropertiesResponce
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<StockItem> StocksHeld { get; set; }

        public PortfolioPropertiesResponce()
        {
            StocksHeld = new List<StockItem>();
        }
    }

    public class StockItem
    {
        public Guid Id { get; set; }
        public string ASXCode { get; set; }
        public string Name { get; set; }

        public StockItem(Guid id, string asxCode, string name)
        {
            Id = id;
            ASXCode = asxCode;
            Name = name;
        }
    }

    public class PortfolioSummaryResponce
    {
        public decimal PortfolioValue { get; set; }
        public decimal PortfolioCost { get; set; }

        public decimal? Return1Year { get; set; }
        public decimal? Return3Year { get; set; }
        public decimal? Return5Year { get; set; }
        public decimal? ReturnAll { get; set; }

        public decimal CashBalance { get; set; }

        public List<Holding> Holdings { get; private set; }

        public PortfolioSummaryResponce()
        {
            Holdings = new List<Holding>();
        }
    }

    public class Holding
    {
        public string ASXCode;
        public string CompanyName;

        public AssetCategory Category;

        public int Units;

        public decimal Value;
        public decimal Cost;
    }
}
