using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.Test.ImportDataTests
{
    class AvailableDataServices
    {
        private static object[] _DataServices = {
            new ASXDataService(),
            new FloatComAuDataService(),
            new GoogleDataService(),
            new AlphaVantageService()
        };

        public static IEnumerable<TestCaseData> HistoricalStockPriceServices()
        {          
            foreach (var dataService in _DataServices)
            {
                var requestDataService = dataService as IHistoricalStockPriceService;
                if (requestDataService != null)
                {
                    var testData = new TestCaseData(requestDataService);
                    testData.SetName(requestDataService.GetType().Name + " - Historical Data");
                    yield return testData;
                }
            }
        }

        public static IEnumerable<TestCaseData> LiveStockPriceServices()
        {
            foreach (var dataService in _DataServices)
            {
                var requestDataService = dataService as ILiveStockPriceService;
                if (requestDataService != null)
                {
                    var testData = new TestCaseData(requestDataService);
                    testData.SetName(requestDataService.GetType().Name + " - Live Data");
                    yield return testData;
                }
            }
        }

        public static IEnumerable<TestCaseData> TradingDayServices()
        {
            foreach (var dataService in _DataServices)
            {
                var requestDataService = dataService as ITradingDayService;
                if (requestDataService != null)
                    if (requestDataService != null)
                    {
                        var testData = new TestCaseData(requestDataService);
                        testData.SetName(requestDataService.GetType().Name + " - Trading Days");
                        yield return testData;
                    }
            }
        }
    }

}
