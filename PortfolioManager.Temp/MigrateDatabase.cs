using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using PortfolioManager.Data.Stocks;
using PortfolioManager.RestApi.Client;
using PortfolioManager.RestApi.Commands;
using PortfolioManager.RestApi.Responses;
using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.Temp
{
    class MigrateDatabase
    {

        public async Task LoadTradingCalander()
        {
            var dataService = new ASXDataService();

            for (var year = 2010; year <= 2018; year++)
            {
                var nonTradingDays = await dataService.NonTradingDays(year, CancellationToken.None);
            }

          //  var restClient = new RestClient("http://localhost", Guid.Empty);

         //   var nonTradingDays = await restClient.TradingCalander.Get(2018);
          /*  using (var unitOfWork = stockDatabase.CreateReadOnlyUnitOfWork())
            {
                var nonTradingDays = unitOfWork.StockQuery.NonTradingDays().GroupBy(x => x.Year);

                foreach (var nonTradingDayYear in nonTradingDays)
                    await restClient.TradingCalander.Update(nonTradingDayYear.Key, nonTradingDayYear.ToList());
            }  */        
        }
    }
}
