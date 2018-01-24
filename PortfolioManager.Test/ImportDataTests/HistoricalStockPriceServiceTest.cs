using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.ImportData.DataServices;


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
            var toDate = DateTime.Today;

            var priceData = await dataService.GetHistoricalPriceData("BHP", fromDate, toDate);

            Assert.That(priceData, Is.Not.Empty);

            var firstPrice = priceData.First();
            Assert.That(firstPrice.Date, Is.EqualTo(fromDate));
            Assert.That(firstPrice.Price, Is.Not.EqualTo(0.00m));

            var lastPrice = priceData.First();
            Assert.That(lastPrice.Date, Is.EqualTo(toDate));
            Assert.That(lastPrice.Price, Is.Not.EqualTo(0.00m));
        }
    }


}