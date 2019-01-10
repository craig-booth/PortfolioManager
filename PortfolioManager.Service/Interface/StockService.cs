using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortfolioManager.Service.Interface
{
    public interface IStockService
    {
        Task<GetStockResponce> GetStocks(DateTime date, bool includeStapledSecurities, bool includeChildStocks);
        Task<CorporateActionsResponce> GetCorporateActions(Guid stock, DateTime fromDate, DateTime toDate);

    }

    public class GetStockResponce : ServiceResponce
    {
        public List<StockItem> Stocks;

        public GetStockResponce()
        {
            Stocks = new List<Interface.StockItem>();
        }
    }

    public class CorporateActionsResponce : ServiceResponce
    {
        public List<CorporateActionItem> CorporateActions { get; set; }

        public CorporateActionsResponce()
            : base()
        {
            CorporateActions = new List<CorporateActionItem>();
        }
    }
}
