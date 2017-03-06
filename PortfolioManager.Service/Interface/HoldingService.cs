using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.Service.Interface
{
    public interface IHoldingService : IPortfolioManagerService
    {
        Task<OwnedStocksResponce> GetOwnedStocks(DateTime date);
        Task<HoldingResponce> GetHolding(Guid stock, DateTime date);
        Task<HoldingsResponce> GetHoldings(DateTime date);
    }

    public class OwnedStocksResponce : ServiceResponce
    {
        public List<StockItem> Stocks;

        public OwnedStocksResponce()
        {
            Stocks = new List<StockItem>();
        }
    }

    public class HoldingResponce : ServiceResponce
    {
        public HoldingItem Holding { get; set; }
    }

    public class HoldingsResponce : ServiceResponce
    {
        public List<HoldingItem> Holdings { get; set; }

        public HoldingsResponce()
        {
            Holdings = new List<HoldingItem>();
        }
    }

    public class HoldingItem
    {
        public StockItem Stock;
        public AssetCategory Category;
        public int Units;
        public decimal Value;
        public decimal Cost;
    }
}
