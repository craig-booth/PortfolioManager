using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.ImportData
{
    public class HistoricalPriceImporter
    {
        private IHistoricalStockPriceService _DataService;
        private readonly StockExchange _StockExchange;

        public HistoricalPriceImporter(StockExchange stockExchange, IHistoricalStockPriceService dataService)
        {
            _StockExchange = stockExchange;
            _DataService = dataService;
        }

        private class StockToImport
        {
            public Stock Stock;
            public DateTime FromDate;
            public DateTime ToDate;
        }

        public async Task Import(CancellationToken cancellationToken)
        {
            var lastExpectedDate = DateTime.Today.AddDays(-1);
            while (! _StockExchange.TradingCalander.IsTradingDay(lastExpectedDate))
                lastExpectedDate = lastExpectedDate.AddDays(-1);

            foreach (var stock in _StockExchange.Stocks.All())
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var latestDate = stock.DateOfLastestPrice(); 

                if (latestDate < lastExpectedDate)
                {                 
                    var fromDate = latestDate.AddDays(1);
                    var toDate = lastExpectedDate;
                    var asxCode = stock.Properties.ClosestTo(toDate).ASXCode;

                    var data = await _DataService.GetHistoricalPriceData(asxCode, fromDate, toDate, cancellationToken);
                    foreach (var stockPrice in data)
                        stock.UpdateClosingPrice(stockPrice.Date, stockPrice.Price);
                }
            }
        }
    }


}
