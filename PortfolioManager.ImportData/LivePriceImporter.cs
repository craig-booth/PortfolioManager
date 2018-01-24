using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.ImportData
{
    public class LivePriceImporter
    {

        private ILiveStockPriceService _DataService;
        private readonly IStockDatabase _Database;

        public LivePriceImporter(IStockDatabase database, ILiveStockPriceService dataService)
        {
            _Database = database;
            _DataService = dataService;
        }

        public async Task Import()
        {
            var asxCodes = new List<string>();
            IEnumerable<Stock> stocks;
            using (var unitOfWork = _Database.CreateReadOnlyUnitOfWork())
            {
                stocks = unitOfWork.StockQuery.GetAll(DateTime.Today);
      
                foreach (var stock in stocks)
                {
                    if (stock.ParentId == Guid.Empty)
                        asxCodes.Add(stock.ASXCode);
                }
            }
                
            var stockQuotes = await _DataService.GetMultiplePrices(asxCodes);

            using (var unitOfWork = _Database.CreateUnitOfWork())
            {
                foreach (var stockQuote in stockQuotes)
                {
                    if (stockQuote.Date == DateTime.Today)
                    {
                        var stock = stocks.FirstOrDefault(x => x.ASXCode == stockQuote.ASXCode);
                        if (stock != null)
                        {
                            if (unitOfWork.StockPriceRepository.Exists(stock.Id, DateTime.Today))
                            {
                                unitOfWork.StockPriceRepository.Update(stock.Id, DateTime.Today, stockQuote.Price, true);
                            }
                            else
                            {
                                unitOfWork.StockPriceRepository.Add(stock.Id, DateTime.Today, stockQuote.Price, true);
                            }

                            if (stock.Type == StockType.StapledSecurity)
                            {
                                var childStocks = stocks.Where(x => x.ParentId == stock.Id);
                                foreach (var childStock in childStocks)
                                {
                                    var percentOfPrice = unitOfWork.StockQuery.PercentOfParentCost(stock.Id, childStock.Id, DateTime.Today);

                                    if (unitOfWork.StockPriceRepository.Exists(stock.Id, DateTime.Today))
                                    {
                                        unitOfWork.StockPriceRepository.Update(childStock.Id, DateTime.Today, stockQuote.Price * percentOfPrice, true);
                                    }
                                    else
                                    {
                                        unitOfWork.StockPriceRepository.Add(childStock.Id, DateTime.Today, stockQuote.Price * percentOfPrice, true);
                                    }
                                }
                            }
                        }
                    }
                }

                unitOfWork.Save();
            } 
        }


    }
}
