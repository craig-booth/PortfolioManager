using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Data.Stocks;
using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.ImportData
{
    public class TradingDayImporter
    {
        private readonly ITradingDayService _DataService;
        private readonly IStockDatabase _Database;

        public TradingDayImporter(IStockDatabase database)
        {
            _Database = database;
            _DataService = new ASXDataService();
        }

        public async Task Import()
        {
            var nonTradingDays = await _DataService.NonTradingDays(DateTime.Today.Year);

            using (var unitOfWork = _Database.CreateUnitOfWork())
            {
                foreach (var nonTradingDay in nonTradingDays)
                {
                    unitOfWork.NonTradingDayRepository.Add(nonTradingDay);
                }

                unitOfWork.Save();
            }
        }
    }
}
