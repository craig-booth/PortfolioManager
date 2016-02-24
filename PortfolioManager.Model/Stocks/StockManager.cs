using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Stocks
{

    public class StockManager
    {
        private readonly IStockDatabase _Database;

        public CorporateActionService CorporateActionService { get; private set; }
        public StockService2 StockService { get; private set; }

        public StockManager(IStockDatabase database)
        {
            _Database = database;

            CorporateActionService = new CorporateActionService(_Database);
            StockService = new StockService2(_Database);
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
