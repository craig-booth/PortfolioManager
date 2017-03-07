using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Service.Interface
{
    public interface IStockService : IPortfolioManagerService
    {
        Task<GetStockResponce> GetStocks(DateTime date, bool IncludeStapledSecurities, bool IncludeChildStocks);
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
