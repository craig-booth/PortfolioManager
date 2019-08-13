using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.DataServices;

namespace PortfolioManager.Test.ImportDataTests
{
    class AvailableDataServices
    {
        private static readonly object[] _DataServices = {
            new ASXDataService(),
            new FloatComAuDataService(),
            new GoogleDataService(),
            new AlphaVantageService()
        };

        public static IEnumerable<TestCaseData> HistoricalStockPriceServices()
        {          
            foreach (var dataService in _DataServices)
            {
                if (dataService is IHistoricalStockPriceService requestDataService)
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
                if (dataService is ILiveStockPriceService requestDataService)
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
                if (dataService is ITradingDayService requestDataService)
                {
                    var testData = new TestCaseData(requestDataService);
                    testData.SetName(requestDataService.GetType().Name + " - Trading Days");
                    yield return testData;
                }
            }
        }
    }

}
