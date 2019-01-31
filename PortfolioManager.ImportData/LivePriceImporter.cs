using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.ImportData
{
    public class LivePriceImporter
    {
        private readonly ILiveStockPriceService _DataService;
        private readonly IStockRepository _StockRepository;
        private readonly ILogger _Logger;

        public LivePriceImporter(IStockRepository stockRepository, ILiveStockPriceService dataService, ILogger<LivePriceImporter> logger)
        {
            _StockRepository = stockRepository;
            _DataService = dataService;
            _Logger = logger;
        }

        public async Task Import(CancellationToken cancellationToken)
        {
            var asxCodes = _StockRepository.All(DateTime.Today).Select(x => x.Properties[DateTime.Today].ASXCode);
                
            var stockQuotes = await _DataService.GetMultiplePrices(asxCodes, cancellationToken);

            foreach (var stockQuote in stockQuotes)
            {
                if (stockQuote.Date == DateTime.Today)
                {
                    var stock = _StockRepository.Get(stockQuote.ASXCode, stockQuote.Date);
                    if (stock != null)
                    {
                        _Logger?.LogInformation("Updating current price foe {0}: {1}", stockQuote.ASXCode, stockQuote.Price);
                        stock.UpdateCurrentPrice(stockQuote.Price);
                    }
                        
                }
            }      
        }
    }
}
