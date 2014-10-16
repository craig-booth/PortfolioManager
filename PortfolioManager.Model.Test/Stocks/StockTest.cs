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
    public class StockTest : TestBase
    {
        [Test, Description("Change ASX Code")]
        public void ChangeASXCode()
        {
            var manager = CreateStockManager();

            var stock = manager.AddStock("ABC", "Old Name");
            stock.ChangeASXCode(new DateTime(2010, 10, 15), "DEF", "New Name");

            var stock1 = manager.GetStock("ABC");
            Assert.That(stock1.ASXCode, Is.EqualTo("ABC"));
            Assert.That(stock1.Name, Is.EqualTo("Old Name"));

            var stock2 = manager.GetStock("DEF");
            Assert.That(stock2.ASXCode, Is.EqualTo("DEF"));
            Assert.That(stock2.Name, Is.EqualTo("New Name"));

            Assert.That(stock1.Id, Is.EqualTo(stock2.Id));
            Assert.That(stock1.ToDate, Is.EqualTo(new DateTime(2010, 10, 14)));
            Assert.That(stock2.FromDate, Is.EqualTo(new DateTime(2010, 10, 15)));
        }

        [Test, Description("Delist")]
        public void Delist()
        {
            var manager = CreateStockManager();

            var stock = manager.AddStock("ABC", "Stapled Security");
            stock.Delist(new DateTime(2010, 06, 15));

            var stock1 = manager.GetStock("ABC");
            Assert.That(stock1.ASXCode, Is.EqualTo("ABC"));
            Assert.That(stock1.ToDate, Is.EqualTo(new DateTime(2010, 06, 14)));
        }

        [Test, Description("Add Child Stock")]
        public void AddChildStock()
        {
            var manager = CreateStockManager();

            var parentStock = manager.AddStock("ABC", "Stapled Security", StockType.StapledSecurity);
            var childStock1 = manager.AddStock("CH1", "Child 1", StockType.Ordinary);
            parentStock.AddChildStock(childStock1);

            var childStock2 = manager.AddStock("CH2", "Child 2", StockType.Trust);
            parentStock.AddChildStock(childStock2);

            var stock1 = manager.GetStock("ABC");
            var children = stock1.GetChildStocks();

            Assert.That(children.Count, Is.EqualTo(2));
            Assert.That(children.First().ASXCode, Is.EqualTo("CH1"));
            Assert.That(children.Last().ASXCode, Is.EqualTo("CH2"));
        }

        [Test, Description("Add Child Stock for a non stapled security")]
        [ExpectedException(typeof(NotStapledSecurityException))]
        public void AddChildStockNonStapled()
        {
            var manager = CreateStockManager();

            var parentStock = manager.AddStock("ABC", "Stapled Security", StockType.Ordinary);
            var childStock1 = manager.AddStock("CH1", "Child 1", StockType.Ordinary);
            parentStock.AddChildStock(childStock1);
        }

        [Test, Description("Get Child stocks for a non stapled security")]
        public void GetChildStocksNonStapled()
        {
            var manager = CreateStockManager();

            var parentStock = manager.AddStock("ABC", "Ordinary Security", StockType.Ordinary);

            var stock1 = manager.GetStock("ABC");
            var children = stock1.GetChildStocks();

            Assert.That(children.Count, Is.EqualTo(0));
        }

        [Test, Description("Remove Child stock")]
        public void RemoveChildStock()
        {
            var manager = CreateStockManager();

            var parentStock = manager.AddStock("ABC", "Stapled Security", StockType.StapledSecurity);
            var childStock1 = manager.AddStock("CH1", "Child 1", StockType.Ordinary);
            parentStock.AddChildStock(childStock1);

            var childStock2 = manager.AddStock("CH2", "Child 2", StockType.Trust);
            parentStock.AddChildStock(childStock2);

            var stock1 = manager.GetStock("ABC");
            var children = stock1.GetChildStocks();
            Assert.That(children.Count, Is.EqualTo(2));

            stock1.RemoveChildStock(childStock1);
            var children2 = stock1.GetChildStocks();
            Assert.That(children2.Count, Is.EqualTo(1));
            Assert.That(children2.First().ASXCode, Is.EqualTo("CH2"));

        }

        [Test, Description("Remove Child stock not exists")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void RemoveChildStockNotExists()
        {
            var manager = CreateStockManager();

            var parentStock = manager.AddStock("ABC", "Stapled Security", StockType.StapledSecurity);
            var childStock1 = manager.AddStock("CH1", "Child 1", StockType.Ordinary);
            parentStock.AddChildStock(childStock1);

            var childStock2 = manager.AddStock("CH2", "Child 2", StockType.Trust);
            parentStock.AddChildStock(childStock2);

            var stock1 = manager.GetStock("ABC");
            var children = stock1.GetChildStocks();
            Assert.That(children.Count, Is.EqualTo(2));


            var childStock3 = manager.AddStock("CH3", "Child 3", StockType.Trust);
            stock1.RemoveChildStock(childStock3);
        }

        [Test, Description("Remove Child stock for a non stapled security")]
        [ExpectedException(typeof(NotStapledSecurityException))]    
        public void RemoveChildStockNonStapled()
        {
            var manager = CreateStockManager();

            var parentStock = manager.AddStock("ABC", "Stapled Security", StockType.Ordinary);
            var childStock = manager.AddStock("CH3", "Child 3", StockType.Trust);
            parentStock.RemoveChildStock(childStock);
        }

        [Test, Description("Add Relative NTA")]
        public void AddRelativeNTA()
        {
            Stock stock;
            decimal percent;

            var manager = CreateStockManager();

            var parentStock = manager.AddStock("ABC", "Stapled Security", StockType.StapledSecurity);

            var childStock1 = manager.AddStock("CH1", "Child 1", StockType.Ordinary);
            parentStock.AddChildStock(childStock1);
            childStock1.AddRelativeNTA(new DateTime(2005, 12, 31), 0.50M);
            childStock1.AddRelativeNTA(new DateTime(2006, 12, 31), 0.60M);

            var childStock2 = manager.AddStock("CH2", "Child 2", StockType.Trust);
            parentStock.AddChildStock(childStock2);
            childStock2.AddRelativeNTA(new DateTime(2005, 12, 31), 0.50M);
            childStock2.AddRelativeNTA(new DateTime(2006, 12, 31), 0.40M);


            stock = manager.GetStock("CH1");
            percent = stock.PercentageOfParentCostBase(new DateTime(2006, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));
            percent = stock.PercentageOfParentCostBase(new DateTime(2008, 06, 30));
            Assert.That(percent, Is.EqualTo(0.60M));

            stock = manager.GetStock("CH2");
            percent = stock.PercentageOfParentCostBase(new DateTime(2006, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));
            percent = stock.PercentageOfParentCostBase(new DateTime(2008, 06, 30));
            Assert.That(percent, Is.EqualTo(0.40M));
        }

        [Test, Description("Add Relative NTA to a non stapled secuity")]
        [ExpectedException(typeof(NotStapledSecurityComponentException))]    
        public void AddRelativeNTANonStapled()
        {
            var manager = CreateStockManager();

            var parentStock = manager.AddStock("ABC", "Stapled Security", StockType.Ordinary);
            var childStock1 = manager.AddStock("CH1", "Child 1", StockType.Ordinary);

            childStock1.AddRelativeNTA(new DateTime(2005, 12, 31), 0.50M);
        }

        [Test, Description("Get Relative NTA for a non stapled secuity")]
        [ExpectedException(typeof(NotStapledSecurityComponentException))]
        public void GetRelativeNTANonStapled()
        {
            var manager = CreateStockManager();

            var parentStock = manager.AddStock("ABC", "Stapled Security", StockType.Ordinary);

            parentStock.PercentageOfParentCostBase(new DateTime(2005, 12, 31));
        }

        [Test, Description("Update a Relative NTA")]
        public void UpdateRelativeNTA()
        {
            Stock stock;
            decimal percent;

            var manager = CreateStockManager();

            var parentStock = manager.AddStock("ABC", "Stapled Security", StockType.StapledSecurity);

            var childStock1 = manager.AddStock("CH1", "Child 1", StockType.Ordinary);
            parentStock.AddChildStock(childStock1);
            childStock1.AddRelativeNTA(new DateTime(2005, 12, 31), 0.50M);
            childStock1.AddRelativeNTA(new DateTime(2006, 12, 31), 0.60M);
       
            stock = manager.GetStock("CH1");
            percent = stock.PercentageOfParentCostBase(new DateTime(2006, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));
            percent = stock.PercentageOfParentCostBase(new DateTime(2008, 06, 30));
            Assert.That(percent, Is.EqualTo(0.60M));

            childStock1.ChangeRelativeNTA(new DateTime(2006, 12, 31), 0.70M);

            stock = manager.GetStock("CH1");
            percent = stock.PercentageOfParentCostBase(new DateTime(2006, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));
            percent = stock.PercentageOfParentCostBase(new DateTime(2008, 06, 30));
            Assert.That(percent, Is.EqualTo(0.70M));
        }

        [Test, Description("Update a Relative NTA that doesn't exist")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void UpdateRelativeNTANotExists()
        {
            var manager = CreateStockManager();

            var parentStock = manager.AddStock("ABC", "Stapled Security", StockType.StapledSecurity);

            var childStock1 = manager.AddStock("CH1", "Child 1", StockType.Ordinary);
            parentStock.AddChildStock(childStock1);
            childStock1.AddRelativeNTA(new DateTime(2005, 12, 31), 0.50M);
            childStock1.AddRelativeNTA(new DateTime(2006, 12, 31), 0.60M);
            childStock1.AddRelativeNTA(new DateTime(2007, 12, 31), 0.70M);

            childStock1.ChangeRelativeNTA(new DateTime(2006, 12, 20), 0.80M); 
        }

        [Test, Description("Update a Relative NTA from a non stapled secuity")]
        [ExpectedException(typeof(NotStapledSecurityComponentException))]
        public void UpdateRelativeNTANonStapled()
        {
            var manager = CreateStockManager();

            var stock = manager.AddStock("ABC", "Stapled Security", StockType.Ordinary);

            stock.ChangeRelativeNTA(new DateTime(2006, 12, 20), 0.75M);
        }

        [Test, Description("Delete Relative NTA")]
        public void DeleteRelativeNTA()
        {
            Stock stock;
            decimal percent;

            var manager = CreateStockManager();

            var parentStock = manager.AddStock("ABC", "Stapled Security", StockType.StapledSecurity);

            var childStock1 = manager.AddStock("CH1", "Child 1", StockType.Ordinary);
            parentStock.AddChildStock(childStock1);
            childStock1.AddRelativeNTA(new DateTime(2005, 12, 31), 0.50M);
            childStock1.AddRelativeNTA(new DateTime(2006, 12, 31), 0.60M);
            childStock1.AddRelativeNTA(new DateTime(2007, 12, 31), 0.70M);

            /* Test that NTAs added correctly */
            stock = manager.GetStock("CH1");
            percent = stock.PercentageOfParentCostBase(new DateTime(2007, 06, 30));
            Assert.That(percent, Is.EqualTo(0.60M));
            stock = manager.GetStock("CH1");
            percent = stock.PercentageOfParentCostBase(new DateTime(2006, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));
            percent = stock.PercentageOfParentCostBase(new DateTime(2007, 06, 30));
            Assert.That(percent, Is.EqualTo(0.60M));
            percent = stock.PercentageOfParentCostBase(new DateTime(2008, 06, 30));
            Assert.That(percent, Is.EqualTo(0.70M));

            /* Remove middle NTA */
            childStock1.DeleteRelativeNTA(new DateTime(2006, 12, 31));
            percent = stock.PercentageOfParentCostBase(new DateTime(2007, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));          
        }

        [Test, Description("Delete Relative NTA that doesn't exist")]
        public void DeleteRelativeNTANotExists()
        {
            var manager = CreateStockManager();

            var parentStock = manager.AddStock("ABC", "Stapled Security", StockType.StapledSecurity);

            var childStock1 = manager.AddStock("CH1", "Child 1", StockType.Ordinary);
            parentStock.AddChildStock(childStock1);
            childStock1.AddRelativeNTA(new DateTime(2005, 12, 31), 0.50M);
            childStock1.AddRelativeNTA(new DateTime(2006, 12, 31), 0.60M);
            childStock1.AddRelativeNTA(new DateTime(2007, 12, 31), 0.70M);

            childStock1.DeleteRelativeNTA(new DateTime(2006, 12, 20)); 
        }

        [Test, Description("Delete Relative NTA from a non stapled secuity")]
        [ExpectedException(typeof(NotStapledSecurityComponentException))]
        public void DeleteRelativeNTANonStapled()
        {
            var manager = CreateStockManager();

            var stock = manager.AddStock("ABC", "Stapled Security", StockType.Ordinary);

            stock.DeleteRelativeNTA(new DateTime(2006, 12, 20)); 
        }
    }
}
