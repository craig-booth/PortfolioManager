using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;

namespace StockManager.Service
{

    class StockPriceRecord
    {
        public string ASXCode { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }

    }

    abstract class StockPriceImporter
    {
        protected abstract StockPriceRecord GetRecord();

     /*   private StockPrice Convert(StockPriceRecord priceRecord)
        {
            return null;
        } */

        public void ImportToDatabase(IStockQuery stockQuery, IStockUnitOfWork unitOfWork)
        {
            var stockPriceRecord = GetRecord();
            while (stockPriceRecord != null)
            {
                Stock stock;
                if (stockQuery.TryGetByASXCode(stockPriceRecord.ASXCode, stockPriceRecord.Date, out stock))
                {
                    decimal price;
                    if (stockQuery.TryGetClosingPrice(stock.Id, stockPriceRecord.Date, out price))
                        unitOfWork.StockPriceRepository.Update(stock.Id, stockPriceRecord.Date, stockPriceRecord.Price);
                    else
                        unitOfWork.StockPriceRepository.Add(stock.Id, stockPriceRecord.Date, stockPriceRecord.Price);
                }
                
                stockPriceRecord = GetRecord();
            }
        }
    }

    class StockEasyPriceImporter: StockPriceImporter
    {
        private readonly CsvReader csvFile;

        public StockEasyPriceImporter(string fileName)
        {
            csvFile = new CsvReader(new StreamReader(fileName));

            csvFile.Configuration.RegisterClassMap(new StockEasyClassMap());
            csvFile.Configuration.HasHeaderRecord = false;       
        }

        protected override StockPriceRecord GetRecord()
        {
            if (!csvFile.Read())
                return null;

            return csvFile.GetRecord<StockPriceRecord>();
        }
    }
    
    class StockEasyClassMap : CsvClassMap<StockPriceRecord>
    {
        public StockEasyClassMap()
        {               
            Map(x => x.ASXCode).Index(0);
            Map(x => x.Date).Index(1).TypeConverterOption("yyyyMMdd");
            Map(x => x.Price).Index(5);
        }
    } 

   
}
