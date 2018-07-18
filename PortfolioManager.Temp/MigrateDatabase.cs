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

        public MigrateDatabase(IStockDatabase stockDatabase, RestClient restClient)
        {
            _StockDatabase = stockDatabase;

            _RestClient = restClient;
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
                if (_RestClient != null)
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

                    if (_RestClient != null)
                        await _RestClient.Stocks.CreateStock(createCommand);
                }
                else
                {
                    var createCommand = new CreateStockCommand()
                    {
                        Id = id,
                        ListingDate = firstRecord.FromDate,
                        AsxCode = firstRecord.ASXCode,
                        Name = firstRecord.Name,
                        Trust = false,
                        Category = firstRecord.Category,
                    };

                    var childSecurities = unitOfWork.StockQuery.GetChildStocks(id, firstRecord.FromDate);
                    createCommand.ChildSecurities.AddRange(
                        childSecurities.Select(
                            x => new CreateStockCommand.StapledSecurityChild(x.ASXCode, x.Name, x.Type == StockType.Trust)
                        )
                    );

                    if (_RestClient != null)
                        await _RestClient.Stocks.CreateStock(createCommand);

                    // Add Relative NTAs
                    var dates = unitOfWork.StockQuery.GetRelativeNTAs(id, childSecurities.First().Id).Select(x => x.Date);
                    foreach (var date in dates)
                    {
                        var changeRelativeNTAsCommnad = new ChangeRelativeNTAsCommand()
                        {
                            Id = id,
                            ChangeDate = date
                        };

                        foreach (var childSecurity in childSecurities)
                        {
                            var nta = unitOfWork.StockQuery.GetRelativeNTA(id, childSecurity.Id, date);
                            changeRelativeNTAsCommnad.RelativeNTAs.Add(
                                new ChangeRelativeNTAsCommand.RelativeNTA()
                                {
                                    ChildSecurity = childSecurity.ASXCode,
                                    Percentage = nta.Percentage
                                }
                            );
                        }

                        if (_RestClient != null)
                            await _RestClient.Stocks.ChangeReleativeNTAs(changeRelativeNTAsCommnad);
                    }

                }

                // DRP
                if (firstRecord.DRPMethod != 0)
                {
                    var changeDRPCommand = new ChangeDividendReinvestmentPlanCommand()
                    {
                        Id = id,
                        ChangeDate = firstRecord.FromDate,
                        DRPActive = true,
                        DividendRoundingRule = firstRecord.DividendRoundingRule,
                        DRPMethod = firstRecord.DRPMethod
                    };
                    if (_RestClient != null)
                        await _RestClient.Stocks.ChangeDRP(changeDRPCommand);
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

                    if (_RestClient != null)
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

                    if (_RestClient != null)
                        await _RestClient.Stocks.DelistStock(delistCommand);
                }           
            }
        }
    }
}
