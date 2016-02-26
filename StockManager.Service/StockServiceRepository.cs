using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace StockManager.Service
{

    public class StockServiceRepository
    {
        private readonly IStockDatabase _Database;

        public CorporateActionService CorporateActionService { get; private set; }
        public StockService StockService { get; private set; }

        public StockServiceRepository(IStockDatabase database)
        {
            _Database = database;

            CorporateActionService = new CorporateActionService(_Database);
            StockService = new StockService(_Database);
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
