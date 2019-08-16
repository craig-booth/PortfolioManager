using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.TradingCalanders;
using PortfolioManager.DataServices;
using PortfolioManager.Web.Utilities;
using PortfolioManager.Web.Services;

namespace PortfolioManager.Web.DataImporters
{
    public class HistoricalPriceImporter
    {
        private readonly IHistoricalStockPriceService _DataService;
        private readonly IStockQuery _StockQuery;
        private IStockService _StockService;
        private readonly ITradingCalander _TradingCalander;
        private readonly ILogger _Logger;

        public HistoricalPriceImporter(IStockQuery stockQuery, IStockService stockService, ITradingCalander tradingCalander, IHistoricalStockPriceService dataService, ILogger<HistoricalPriceImporter> logger)
        {
            _StockQuery = stockQuery;
            _StockService = stockService;
            _TradingCalander = tradingCalander;
            _DataService = dataService;
            _Logger = logger;
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
                 
                    var closingPrices = data.Select(x => new Tuple<DateTime, decimal>(x.Date, x.Price)).ToList();
                    _Logger?.LogInformation("{0} closing prices found", closingPrices.Count);
                    if (closingPrices.Count > 0)
                    {
                        try
                        {
                            _StockService.UpdateClosingPrices(stock.Id, closingPrices);
                        }
                        catch (Exception e)
                        {
                            _Logger.LogError(e, "Error occurred importing closing prices for {0}", asxCode);
                        }
                    }
                     
                }
            }
        }
    }


}
