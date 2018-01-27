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
    class TradingDayImportTest
    {

        [Test]
        [TestCaseSource(typeof(AvailableDataServices), "TradingDayServices")]
        public async Task TradingDayService(ITradingDayService dataService)
        {
            var year = DateTime.Today.Year;
            var newYearsDay = new DateTime(year, 01, 01);
            var christmasDay = new DateTime(year, 12, 25);

            var tradingDays = await dataService.NonTradingDays(year);

            Assert.That(tradingDays, Is.Not.Empty);

            Assert.That(tradingDays, Contains.Item(newYearsDay));
            Assert.That(tradingDays, Contains.Item(christmasDay));
        }
    }
}
