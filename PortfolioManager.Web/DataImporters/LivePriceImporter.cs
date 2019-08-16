using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

using PortfolioManager.DataServices;
using PortfolioManager.Web.Services;
using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.DataImporters
{
    public class LivePriceImporter
    {
        private readonly ILiveStockPriceService _DataService;
        private readonly IStockQuery _StockQuery;
        private IStockService _StockService;
        private readonly ILogger _Logger;

        public LivePriceImporter(IStockQuery stockQuery, IStockService stockService, ILiveStockPriceService dataService, ILogger<LivePriceImporter> logger)
        {
            _StockQuery = stockQuery;
            _StockService = stockService;
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

                        _StockService.UpdateCurrentPrice(stock.Id, stockQuote.Price);
                    }
                        
                }
            }      
        }
    }
}
