using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using PortfolioManager.Common;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.TradingCalanders;
using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.ImportData
{
    public class HistoricalPriceImporter
    {
        private readonly IHistoricalStockPriceService _DataService;
        private readonly IStockQuery _StockQuery;
        private readonly IRepository<Stock> _StockRepository;
        private readonly ITradingCalander _TradingCalander;
        private readonly ILogger _Logger;

        public HistoricalPriceImporter(IStockQuery stockQuery, IRepository<Stock> stockRepository, ITradingCalander tradingCalander, IHistoricalStockPriceService dataService, ILogger<HistoricalPriceImporter> logger)
        {
            _StockQuery = stockQuery;
            _StockRepository = stockRepository;
            _TradingCalander = tradingCalander;
            _DataService = dataService;
            _Logger = logger;
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
            while (! _TradingCalander.IsTradingDay(lastExpectedDate))
                lastExpectedDate = lastExpectedDate.AddDays(-1);

            foreach (var stock in _StockQuery.All())
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var latestDate = stock.DateOfLastestPrice(); 

                if (latestDate < lastExpectedDate)
                {                 
                    var fromDate = latestDate.AddDays(1);
                    var toDate = lastExpectedDate;
                    var asxCode = stock.Properties.ClosestTo(toDate).ASXCode;

                    _Logger?.LogInformation("Importing closing prices for {0} between {1:d} and {2:d}", asxCode, fromDate, toDate);

                    var data = await _DataService.GetHistoricalPriceData(asxCode, fromDate, toDate, cancellationToken);

                    _Logger?.LogInformation("{0} closing prices found", data.Count());
                    foreach (var stockPrice in data)
                    {
                        _Logger?.LogInformation("Updating closing price for {0:d} : {1}", stockPrice.Date, stockPrice.Price);
                        try
                        {
                            stock.UpdateClosingPrice(stockPrice.Date, stockPrice.Price);
                        }
                        catch (Exception e)
                        {
                            _Logger.LogError(e, "Error occurred importing closing prices for {0} on {1:d}", asxCode, stockPrice.Date);
                        }
                    }
                        
                }
            }
        }
    }


}
