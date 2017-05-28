using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Service.Interface
{
    public interface IHoldingService : IPortfolioService
    {
        Task<HoldingResponce> GetHolding(Guid stockId, DateTime date);
        Task<HoldingsResponce> GetHoldings(DateTime date);
        Task<HoldingsResponce> GetTradeableHoldings(DateTime date);
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
}
