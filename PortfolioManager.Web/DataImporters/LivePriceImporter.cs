using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.DataServices;
using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.DataImporters
{
    public class LivePriceImporter
    {
        private readonly ILiveStockPriceService _DataService;
        private readonly IStockQuery _StockQuery;
        private IRepository<StockPriceHistory> _StockPriceHistoryRepository;
        private readonly ILogger _Logger;

        public LivePriceImporter(IStockQuery stockQuery, IRepository<StockPriceHistory> stockPriceHistoryRepository, ILiveStockPriceService dataService, ILogger<LivePriceImporter> logger)
        {
            _StockQuery = stockQuery;
            _StockPriceHistoryRepository = stockPriceHistoryRepository;
            _DataService = dataService;
            _Logger = logger;
        }

        public async Task Import(CancellationToken cancellationToken)
        {
            var asxCodes = _StockQuery.All(DateTime.Today).Select(x => x.Properties[DateTime.Today].ASXCode);
                
            var stockQuotes = await _DataService.GetMultiplePrices(asxCodes, cancellationToken);

            foreach (var stockQuote in stockQuotes)
            {
                if (stockQuote.Date == DateTime.Today)
                {
                    var stock = _StockQuery.Get(stockQuote.ASXCode, stockQuote.Date);
                    if (stock != null)
                    {
                        _Logger?.LogInformation("Updating current price foe {0}: {1}", stockQuote.ASXCode, stockQuote.Price);

                        var stockPriceHistory = _StockPriceHistoryRepository.Get(stock.Id);
                        stockPriceHistory.UpdateCurrentPrice(stockQuote.Price);
                    }
                        
                }
            }      
        }
    }
}
