using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Service.Interface
{
    public interface IIncomeService : IPortfolioManagerService
    {
        Task<IncomeResponce> GetIncome(DateTime fromDate, DateTime toDate);
    }


    public class IncomeResponce : ServiceResponce
    {
        public List<IncomeItem> Income { get; set; }

        public IncomeResponce()
        {
            Income = new List<Interface.IncomeItem>();
        }
    }

    public class IncomeItem
    {
        public StockItem Stock { get; set; }
        public decimal UnfrankedAmount { get; set; }
        public decimal FrankedAmount { get; set; }
        public decimal FrankingCredits { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
