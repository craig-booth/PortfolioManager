using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.RestApi.Client;
using PortfolioManager.RestApi.Stocks;
using PortfolioManager.RestApi.TradingCalander;
using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.Temp
{
    class MigrateDatabase
    {
        private IStockDatabase _StockDatabase;
        private RestClient _RestClient;

        public MigrateDatabase(IStockDatabase stockDatabase)
        {
            _StockDatabase = stockDatabase;

            _RestClient = new RestClient("http://localhost", Guid.Empty);
        }

        public async Task LoadTradingCalander()
        {
            var dataService = new ASXDataService();

            for (var year = 2010; year <= 2018; year++)
            {
                var nonTradingDays = await dataService.NonTradingDays(year, CancellationToken.None);

                var command = new UpdateTradingCalanderCommand();
                command.Year = year;
                command.NonTradingDays.AddRange(nonTradingDays.Select(x => new UpdateTradingCalanderCommand.NonTradingDay(x.Date, x.Desciption)));
                await _RestClient.TradingCalander.Update(command);
            }           
        }

        public IEnumerable<Guid> ListStocks()
        {
            using (var unitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
            {
                return unitOfWork.StockQuery.GetAll().Where(x => x.ParentId == Guid.Empty).Select(x => x.Id).Distinct();
            }
        }

        public async Task LoadStock(Guid id)
        {
            using (var unitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
            {
                var stockRecords = unitOfWork.StockQuery.GetAll().Where(x => x.Id == id);

                var firstRecord = stockRecords.First();

                // List Stock
                if (firstRecord.Type != StockType.StapledSecurity)
                {
                    var createCommand = new CreateStockCommand()
                    {
                        Id = id,
                        ListingDate = firstRecord.FromDate,
                        AsxCode = firstRecord.ASXCode,
                        Name = firstRecord.Name,
                        Trust = (firstRecord.Type == StockType.Trust),
                        Category = firstRecord.Category
                    };

                    await _RestClient.Stocks.CreateStock(createCommand);
                }
                else
                {
                    // Add Relative NTAs
                }

                // Property changes
                foreach (var stockRecord in stockRecords.Skip(1))
                {
                    var changeCommand = new ChangeStockCommand()
                    {
                        Id = id,
                        ChangeDate = stockRecord.FromDate,
                        AsxCode = stockRecord.ASXCode,
                        Name = stockRecord.Name,
                        Category = stockRecord.Category
                    };

                    await _RestClient.Stocks.ChangeStock(changeCommand);
                }

                // Add Dividends

                // Add Capital Returns

                // Add closing prices

                // Delist Stock
                var lastRecord = stockRecords.Last();
                if (lastRecord.ToDate != DateUtils.NoEndDate)
                {
                    var delistCommand = new DelistStockCommand()
                    {
                        Id = id,
                        DelistingDate = lastRecord.ToDate
                    };

                    await _RestClient.Stocks.DelistStock(delistCommand);
                }
            }
        }
    }
}
