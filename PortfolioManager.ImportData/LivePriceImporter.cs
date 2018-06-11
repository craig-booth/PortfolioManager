using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.ImportData
{
    public class LivePriceImporter
    {
        private ILiveStockPriceService _DataService;
        private readonly StockExchange _StockExchange;

        public LivePriceImporter(StockExchange stockExchange, ILiveStockPriceService dataService)
        {
            _StockExchange = stockExchange;
            _DataService = dataService;
        }

        public async Task Import(CancellationToken cancellationToken)
        {
            var asxCodes = _StockExchange.Stocks.All(DateTime.Today).Select(x => x.Properties[DateTime.Today].ASXCode);
                
            var stockQuotes = await _DataService.GetMultiplePrices(asxCodes, cancellationToken);

            foreach (var stockQuote in stockQuotes)
            {
                if (stockQuote.Date == DateTime.Today)
                {
                    var stock = _StockExchange.Stocks.Get(stockQuote.ASXCode, stockQuote.Date);
                    if (stock != null)
                        stock.UpdateCurrentPrice(stockQuote.Price);
                }
            }      
        }
    }
}
