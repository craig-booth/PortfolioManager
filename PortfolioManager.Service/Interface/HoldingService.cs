using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Service.Interface
{
    public interface IHoldingService : IPortfolioManagerService
    {
        Task<GetOwnedStocksResponce> GetOwnedStocks(DateTime date);
    }

    public class GetOwnedStocksResponce : ServiceResponce
    {
        public List<StockItem> Stocks;

        public GetOwnedStocksResponce()
        {
            Stocks = new List<StockItem>();
        }
    }

}
