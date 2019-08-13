using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.Common;
using PortfolioManager.DataServices;

namespace PortfolioManager.Test.ImportDataTests
{

    [TestFixture]
    class LivePriceImportTest
    {

        [Test]
        [TestCaseSource(typeof(AvailableDataServices), "LiveStockPriceServices")]
        public async Task LiveStockPriceServiceSingle(ILiveStockPriceService dataService)
        {
            var price = await dataService.GetSinglePrice("BHP", CancellationToken.None);

            var lastTradingDay = DateTime.Today;
            while (!lastTradingDay.WeekDay())
                lastTradingDay = lastTradingDay.AddDays(-1);

            Assert.That(price.Date, Is.EqualTo(lastTradingDay));
            Assert.That(price.Price, Is.Not.EqualTo(0.00m));
        }

        [Test]
        [TestCaseSource(typeof(AvailableDataServices), "LiveStockPriceServices")]
        public async Task LiveStockPriceServiceMultiple(ILiveStockPriceService dataService)
        {
            var asxCodes = new string[] { "BHP", "ARG", "NAB" }; 
            var priceData = await dataService.GetMultiplePrices(asxCodes, CancellationToken.None);

            Assert.That(priceData, Has.Exactly(3).Items);
        }
    }
}
