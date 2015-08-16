using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Test.Stocks
{
    [TestFixture]
    public class StockManagerTest: PortfolioTestBase 
    {
        [Test, Description("Add Stock(string asxCode, string name)")]
        public void AddStock1()
        {
            var manager = CreateStockManager();

            manager.Add("ABC", "Test");

            var stock = manager.GetStock("ABC");

            Assert.That(stock.ASXCode, Is.EqualTo("ABC"));
            Assert.That(stock.Type, Is.EqualTo(StockType.Ordinary));
            Assert.That(stock.FromDate, Is.EqualTo(DateTimeConstants.NoStartDate()));
            Assert.That(stock.ToDate, Is.EqualTo(DateTimeConstants.NoEndDate()));
            Assert.That(stock.ParentId, Is.EqualTo(Guid.Empty));
        }

        [Test, Description("Add Stock(string asxCode, string name, DateTime fromDate)")]
        public void AddStock2()
        {
            var manager = CreateStockManager();

            var fromDate = new DateTime(2001, 06, 01);
            manager.Add("ABC", "Test", fromDate);

            var stock = manager.GetStock("ABC");

            Assert.That(stock.ASXCode, Is.EqualTo("ABC"));
            Assert.That(stock.Type, Is.EqualTo(StockType.Ordinary));
            Assert.That(stock.FromDate, Is.EqualTo(fromDate));
            Assert.That(stock.ToDate, Is.EqualTo(DateTimeConstants.NoEndDate()));
            Assert.That(stock.ParentId, Is.EqualTo(Guid.Empty));
        }

        [Test, Description("(string asxCode, string name, StockType type)")]
        public void AddStock3()
        {
            var manager = CreateStockManager();

            manager.Add("ABC", "Test", StockType.Trust);

            var stock = manager.GetStock("ABC");

            Assert.That(stock.ASXCode, Is.EqualTo("ABC"));
            Assert.That(stock.Type, Is.EqualTo(StockType.Trust));
            Assert.That(stock.FromDate, Is.EqualTo(DateTimeConstants.NoStartDate()));
            Assert.That(stock.ToDate, Is.EqualTo(DateTimeConstants.NoEndDate()));
            Assert.That(stock.ParentId, Is.EqualTo(Guid.Empty));
        }

        [Test, Description("Add Stock(string asxCode, string name, DateTime fromDate, StockType type)")]
        public void AddStock4()
        {
            var manager = CreateStockManager();

            var fromDate = new DateTime(2001, 06, 01);
            manager.Add("ABC", "Test", fromDate, StockType.Trust);

            var stock = manager.GetStock("ABC");

            Assert.That(stock.ASXCode, Is.EqualTo("ABC"));
            Assert.That(stock.Type, Is.EqualTo(StockType.Trust));
            Assert.That(stock.FromDate, Is.EqualTo(fromDate));
            Assert.That(stock.ToDate, Is.EqualTo(DateTimeConstants.NoEndDate()));
            Assert.That(stock.ParentId, Is.EqualTo(Guid.Empty));
        }

        [Test, Description("Get Stock")]
        public void GetStock()
        {
            var manager = CreateStockManager();

            var fromDate = new DateTime(2001, 06, 01);
            manager.Add("ABC", "Test", fromDate);

            var stock = manager.GetStock("ABC");

            Assert.That(stock.ASXCode, Is.EqualTo("ABC"));
            Assert.That(stock.Type, Is.EqualTo(StockType.Ordinary));
            Assert.That(stock.FromDate, Is.EqualTo(fromDate));
            Assert.That(stock.ToDate, Is.EqualTo(DateTimeConstants.NoEndDate()));
            Assert.That(stock.ParentId, Is.EqualTo(Guid.Empty));
        }

        /* TODO: Priority Low, Delete stock should also remove NTAs and Corporate actions */

        /* TODO: Priority Low, Test delete of stapled security when children exist */
        /*       result: exception - children exist */

    }
}
