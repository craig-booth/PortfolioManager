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
    class HistoricalStockPriceServiceTest
    {

        [Test]
        [TestCaseSource(typeof(AvailableDataServices), "HistoricalStockPriceServices")]
        public async Task HistoricalStockPriceService(IHistoricalStockPriceService dataService)
        {
            var fromDate = DateTime.Today.AddDays(-30);
            while (!fromDate.WeekDay())
                fromDate = fromDate.AddDays(-1);

            var toDate = DateTime.Today.AddDays(-1);
            while (!toDate.WeekDay())
                toDate = toDate.AddDays(-1);

            var priceData = await dataService.GetHistoricalPriceData("BHP", fromDate, toDate, CancellationToken.None);

            Assert.That(priceData, Is.Not.Empty);

            var firstPrice = priceData.First();
            Assert.That(firstPrice.Date, Is.EqualTo(fromDate));
            Assert.That(firstPrice.Price, Is.Not.EqualTo(0.00m));

            var lastPrice = priceData.Last();
            Assert.That(lastPrice.Date, Is.EqualTo(toDate));
            Assert.That(lastPrice.Price, Is.Not.EqualTo(0.00m));
        }
    }


}