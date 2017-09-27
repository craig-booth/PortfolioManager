using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using PortfolioManager.Common;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.Web.Controllers
{
    [Route("api/stock")]
    public class StockController : Controller
    {
        private IServiceProvider _ServiceProvider;

        public StockController(IServiceProvider serviceProvider)
        {
            _ServiceProvider = serviceProvider;
        }

        // GET: api/stock/stocks?date=&includeStapled&includeChildren
        [Route("stocks")]
        [HttpGet]
        public async Task<GetStockResponce> Get(DateTime? date, bool? includeStapledSecurities, bool? includeChildStocks)
        {
            var service = _ServiceProvider.GetRequiredService<IStockService>();

            if (date == null)
                date = DateTime.Today;
            if (includeStapledSecurities == null)
                includeStapledSecurities = true;
            if (includeChildStocks == null)
                includeChildStocks = false;

            return await service.GetStocks((DateTime)date, (bool)includeStapledSecurities, (bool)includeChildStocks);
        }

  
    }
}
