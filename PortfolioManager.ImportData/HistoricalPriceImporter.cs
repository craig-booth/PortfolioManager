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

        private class StockToImport
        {
            public Stock Stock;
            public DateTime FromDate;
            public DateTime ToDate;
        }

        public async Task Import()
        {
            var stocksToImport = new List<StockToImport>();

            using (var unitOfWork = _Database.CreateReadOnlyUnitOfWork())
            {
                var lastExpectedDate = DateTime.Today.AddDays(-1);
                while (!unitOfWork.StockQuery.TradingDay(lastExpectedDate))
                    lastExpectedDate = lastExpectedDate.AddDays(-1);

                var stocks = unitOfWork.StockQuery.GetAll();

                foreach (var stock in stocks)
                {
                    if (stock.ParentId == Guid.Empty)
                    {
                        var latestDate = unitOfWork.StockQuery.GetLatestClosingPrice(stock.Id);

                        if (latestDate < lastExpectedDate)
                        {
                            stocksToImport.Add(new StockToImport()
                            {
                                Stock = stock,
                                FromDate = latestDate.AddDays(1),
                                ToDate = lastExpectedDate
                            });
                        }
                    }
                }
            }


            foreach (var stock in stocksToImport)
            {
                var data = await _DataService.GetHistoricalPriceData(stock.Stock.ASXCode, stock.FromDate, stock.ToDate);
                using (var unitOfWork = _Database.CreateUnitOfWork())
                {
                    foreach (var stockPrice in data)
                    {
                        if (unitOfWork.StockPriceRepository.Exists(stock.Stock.Id, stockPrice.Date))
                            unitOfWork.StockPriceRepository.Update(stock.Stock.Id, stockPrice.Date, stockPrice.Price, false);
                        else
                            unitOfWork.StockPriceRepository.Add(stock.Stock.Id, stockPrice.Date, stockPrice.Price, false);
                    }

                    unitOfWork.Save();
                }
            }
        }
    }


}
