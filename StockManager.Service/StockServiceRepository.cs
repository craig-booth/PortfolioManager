using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StockManager.Service.Utils;
using PortfolioManager.Model.Data;

namespace StockManager.Service
{

    public class StockServiceRepository
    {
        private readonly IStockDatabase _Database;
        private readonly ILiveStockPriceDatabase _LivePriceDatabase;

        public CorporateActionService CorporateActionService { get; private set; }
        public StockService StockService { get; private set; }
        public StockPriceService StockPriceService { get; private set; }

        public StockServiceRepository(IStockDatabase database, ILiveStockPriceDatabase livePriceDatabase)
        {
            _Database = database;
            _LivePriceDatabase = livePriceDatabase;

            CorporateActionService = new CorporateActionService(_Database);
            StockService = new StockService(_Database);
            StockPriceService = new StockPriceService(_Database, _LivePriceDatabase, new GoogleStockPriceDownloader(), new FloatComAUHistoricalPriceDownloader(), new ASXTradingDayDownloader());
        }

        public async Task DownloadUpdatedData()
        {
            Task[] downloadTasks = new Task[2]
            {
                StockPriceService.DownloadCurrentPrices(),
                StockPriceService.DownloadNonTradingDays(2010).ContinueWith(x => StockPriceService.DownloadHistoricalPrices())
            };

            await Task.WhenAll(downloadTasks);
        }

        public void ImportStockPrices(string fileName)
        {
            var importer = new StockEasyPriceImporter(fileName);

            using (var unitOfWork = _Database.CreateUnitOfWork())
            {
                importer.ImportToDatabase(_Database.StockQuery, unitOfWork);

                unitOfWork.Save();
            }      
        }

    }
}
