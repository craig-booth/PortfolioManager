using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.ImportData
{
    public class HistoricalPriceImporter
    {
        private IHistoricalStockPriceService _DataService;
        private readonly IStockDatabase _Database;

        public HistoricalPriceImporter(IStockDatabase database)
        {
            _Database = database;
            _DataService = new FloatComAuDataService();
        }

        public async Task Import()
        {
            var lastExpectedDate = DateTime.Today.AddDays(-1);
            while (!_Database.StockQuery.TradingDay(lastExpectedDate))
                lastExpectedDate = lastExpectedDate.AddDays(-1);

            var stocks = _Database.StockQuery.GetAll();

            using (var unitOfWork = _Database.CreateUnitOfWork())
            {           
                foreach (var stock in stocks)
                {
                    if (stock.ParentId == Guid.Empty)
                    {
                        var latestDate = _Database.StockQuery.GetLatestClosingPrice(stock.Id);

                        if (latestDate < lastExpectedDate)
                        {
                            var data = await _DataService.GetHistoricalPriceData(stock.ASXCode, latestDate.AddDays(1), lastExpectedDate);
                            UpdateStockPrices(unitOfWork, data);
                        }
                    }
                }

                unitOfWork.Save();
            }
        }

        private void UpdateStockPrices(IStockUnitOfWork unitOfWork, IEnumerable<StockPrice> stockPrices)
        {
            Stock stock = null;

            foreach (var stockPrice in stockPrices)
            {
                if (_Database.StockQuery.TryGetByASXCode(stockPrice.ASXCode, stockPrice.Date, out stock))
                {
                    if (unitOfWork.StockPriceRepository.Exists(stock.Id, stockPrice.Date))
                        unitOfWork.StockPriceRepository.Update(stock.Id, stockPrice.Date, stockPrice.Price, false);
                    else
                        unitOfWork.StockPriceRepository.Add(stock.Id, stockPrice.Date, stockPrice.Price, false);
                }
            }
        }
    }
}
