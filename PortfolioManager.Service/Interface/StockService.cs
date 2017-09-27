using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortfolioManager.Service.Interface
{
    public interface IStockService : IPortfolioService
    {
        Task<GetStockResponce> GetStocks(DateTime date, bool includeStapledSecurities, bool includeChildStocks);
    }

    public class GetStockResponce : ServiceResponce
    {
        public List<StockItem> Stocks;

        public GetStockResponce()
        {
            Stocks = new List<Interface.StockItem>();
        }
    }
}
