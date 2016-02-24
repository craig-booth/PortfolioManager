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
    public class StockTest : PortfolioTestBase
    {
        [Test, Description("Change ASX Code")]
        public void ChangeASXCode()
        {
            var manager = CreateStockManager();

            var stock = manager.StockService.Add("ABC", "Old Name");
            manager.StockService.ChangeASXCode(stock, new DateTime(2010, 10, 15), "DEF", "New Name");

            var stock1 = manager.StockService.GetStock("ABC", new DateTime(2010, 01, 01));
            Assert.That(stock1.ASXCode, Is.EqualTo("ABC"), "Stock 1 ASX code wrong");
            Assert.That(stock1.Name, Is.EqualTo("Old Name"), "Stock 1 name wrong");

            var stock2 = manager.StockService.GetStock("DEF", new DateTime(2010, 10, 20));
            Assert.That(stock2.ASXCode, Is.EqualTo("DEF"), "Stock 2 ASX code wrong");
            Assert.That(stock2.Name, Is.EqualTo("New Name"), "Stock 2 name wrong");

            Assert.That(stock1.Id, Is.EqualTo(stock2.Id), "Stock 1 != Stock2");
            Assert.That(stock1.ToDate, Is.EqualTo(new DateTime(2010, 10, 14)), "Stock 1 date wrong");
            Assert.That(stock2.FromDate, Is.EqualTo(new DateTime(2010, 10, 15)), "Stock 2 date wrong");
        }

        [Test, Description("Delist")]
        public void Delist()
        {
            var manager = CreateStockManager();

            var stock = manager.StockService.Add("ABC", "Stapled Security");
            manager.StockService.Delist(stock, new DateTime(2010, 06, 15));

            var stock1 = manager.StockService.GetStock("ABC", new DateTime(2010, 01, 01));
            Assert.That(stock1.ASXCode, Is.EqualTo("ABC"));
            Assert.That(stock1.ToDate, Is.EqualTo(new DateTime(2010, 06, 14)));
        }

        [Test, Description("Add Child Stock")]
        public void AddChildStock()
        {
            var manager = CreateStockManager();

            var parentStock = manager.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);
            var childStock1 = manager.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            manager.StockService.AddChildStock(parentStock, childStock1);

            var childStock2 = manager.StockService.Add("CH2", "Child 2", StockType.Trust);
            manager.StockService.AddChildStock(parentStock, childStock2);

            var stock1 = manager.StockService.GetStock("ABC");
            var children = manager.StockService.GetChildStocks(stock1);

            Assert.That(children.Count, Is.EqualTo(2));
            Assert.That(children.First().ASXCode, Is.EqualTo("CH1"));
            Assert.That(children.Last().ASXCode, Is.EqualTo("CH2"));
        }

        [Test, Description("Add Child Stock for a non stapled security")]
        [ExpectedException(typeof(NotStapledSecurityException))]
        public void AddChildStockNonStapled()
        {
            var manager = CreateStockManager();

            var parentStock = manager.StockService.Add("ABC", "Stapled Security", StockType.Ordinary);
            var childStock1 = manager.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            manager.StockService.AddChildStock(parentStock, childStock1);;
        }

        [Test, Description("Get Child stocks for a non stapled security")]
        public void GetChildStocksNonStapled()
        {
            var manager = CreateStockManager();

            var parentStock = manager.StockService.Add("ABC", "Ordinary Security", StockType.Ordinary);

            var stock1 = manager.StockService.GetStock("ABC");
            var children = manager.StockService.GetChildStocks(stock1);

            Assert.That(children.Count, Is.EqualTo(0));
        }

        [Test, Description("Remove Child stock")]
        public void RemoveChildStock()
        {
            var manager = CreateStockManager();

            var parentStock = manager.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);
            var childStock1 = manager.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            manager.StockService.AddChildStock(parentStock, childStock1);;

            var childStock2 = manager.StockService.Add("CH2", "Child 2", StockType.Trust);
            manager.StockService.AddChildStock(parentStock, childStock2);;

            var stock1 = manager.StockService.GetStock("ABC");
            var children = manager.StockService.GetChildStocks(stock1);
            Assert.That(children.Count, Is.EqualTo(2));

            manager.StockService.RemoveChildStock(stock1, childStock1);
            var children2 = manager.StockService.GetChildStocks(stock1);
            Assert.That(children2.Count, Is.EqualTo(1));
            Assert.That(children2.First().ASXCode, Is.EqualTo("CH2"));

        }

        [Test, Description("Remove Child stock not exists")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void RemoveChildStockNotExists()
        {
            var manager = CreateStockManager();

            var parentStock = manager.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);
            var childStock1 = manager.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            manager.StockService.AddChildStock(parentStock, childStock1);;

            var childStock2 = manager.StockService.Add("CH2", "Child 2", StockType.Trust);
            manager.StockService.AddChildStock(parentStock, childStock2);;

            var stock1 = manager.StockService.GetStock("ABC");
            var children = manager.StockService.GetChildStocks(stock1);
            Assert.That(children.Count, Is.EqualTo(2));


            var childStock3 = manager.StockService.Add("CH3", "Child 3", StockType.Trust);
            manager.StockService.RemoveChildStock(stock1, childStock3);
        }

        [Test, Description("Remove Child stock for a non stapled security")]
        [ExpectedException(typeof(NotStapledSecurityException))]    
        public void RemoveChildStockNonStapled()
        {
            var manager = CreateStockManager();

            var parentStock = manager.StockService.Add("ABC", "Stapled Security", StockType.Ordinary);
            var childStock = manager.StockService.Add("CH3", "Child 3", StockType.Trust);
            manager.StockService.RemoveChildStock(parentStock, childStock);
        }

        [Test, Description("Add Relative NTA")]
        public void AddRelativeNTA()
        {
            Stock stock;
            decimal percent;

            var manager = CreateStockManager();

            var parentStock = manager.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);

            var childStock1 = manager.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            manager.StockService.AddChildStock(parentStock, childStock1);;
            manager.StockService.AddRelativeNTA(childStock1, new DateTime(2005, 12, 31), 0.50M);
            manager.StockService.AddRelativeNTA(childStock1, new DateTime(2006, 12, 31), 0.60M);

            var childStock2 = manager.StockService.Add("CH2", "Child 2", StockType.Trust);
            manager.StockService.AddChildStock(parentStock, childStock2);;
            manager.StockService.AddRelativeNTA(childStock2, new DateTime(2005, 12, 31), 0.50M);
            manager.StockService.AddRelativeNTA(childStock2, new DateTime(2006, 12, 31), 0.40M);


            stock = manager.StockService.GetStock("CH1");
            percent = manager.StockService.PercentageOfParentCostBase(stock, new DateTime(2006, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));
            percent = manager.StockService.PercentageOfParentCostBase(stock, new DateTime(2008, 06, 30));
            Assert.That(percent, Is.EqualTo(0.60M));

            stock = manager.StockService.GetStock("CH2");
            percent = manager.StockService.PercentageOfParentCostBase(stock, new DateTime(2006, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));
            percent = manager.StockService.PercentageOfParentCostBase(stock, new DateTime(2008, 06, 30));
            Assert.That(percent, Is.EqualTo(0.40M));
        }

        [Test, Description("Add Relative NTA to a non stapled secuity")]
        [ExpectedException(typeof(NotStapledSecurityComponentException))]    
        public void AddRelativeNTANonStapled()
        {
            var manager = CreateStockManager();

            var parentStock = manager.StockService.Add("ABC", "Stapled Security", StockType.Ordinary);
            var childStock1 = manager.StockService.Add("CH1", "Child 1", StockType.Ordinary);

            manager.StockService.AddRelativeNTA(childStock1, new DateTime(2005, 12, 31), 0.50M);
        }

        [Test, Description("Get Relative NTA for a non stapled secuity")]
        [ExpectedException(typeof(NotStapledSecurityComponentException))]
        public void GetRelativeNTANonStapled()
        {
            var manager = CreateStockManager();

            var parentStock = manager.StockService.Add("ABC", "Stapled Security", StockType.Ordinary);

            manager.StockService.PercentageOfParentCostBase(parentStock, new DateTime(2005, 12, 31));
        }

        [Test, Description("Update a Relative NTA")]
        public void UpdateRelativeNTA()
        {
            Stock stock;
            decimal percent;

            var manager = CreateStockManager();

            var parentStock = manager.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);

            var childStock1 = manager.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            manager.StockService.AddChildStock(parentStock, childStock1);;
            manager.StockService.AddRelativeNTA(childStock1, new DateTime(2005, 12, 31), 0.50M);
            var nta = manager.StockService.AddRelativeNTA(childStock1, new DateTime(2006, 12, 31), 0.60M);
       
            stock = manager.StockService.GetStock("CH1");
            percent = manager.StockService.PercentageOfParentCostBase(stock, new DateTime(2006, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));
            percent = manager.StockService.PercentageOfParentCostBase(stock, new DateTime(2008, 06, 30));
            Assert.That(percent, Is.EqualTo(0.60M));

            nta.Percentage = 0.70m;
            manager.StockService.UpdateRelativeNTA(nta);

            stock = manager.StockService.GetStock("CH1");
            percent = manager.StockService.PercentageOfParentCostBase(stock, new DateTime(2006, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));
            percent = manager.StockService.PercentageOfParentCostBase(stock, new DateTime(2008, 06, 30));
            Assert.That(percent, Is.EqualTo(0.70M));
        }

        [Test, Description("Update a Relative NTA that doesn't exist")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void UpdateRelativeNTANotExists()
        {
            var manager = CreateStockManager();

            var parentStock = manager.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);

            var childStock1 = manager.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            manager.StockService.AddChildStock(parentStock, childStock1);;
            manager.StockService.AddRelativeNTA(childStock1, new DateTime(2005, 12, 31), 0.50M);
            manager.StockService.AddRelativeNTA(childStock1, new DateTime(2006, 12, 31), 0.60M);
            manager.StockService.AddRelativeNTA(childStock1, new DateTime(2007, 12, 31), 0.70M);

            var nta = new RelativeNTA(new DateTime(2006, 12, 20), parentStock.Id, childStock1.Id, 0.80m);
            manager.StockService.UpdateRelativeNTA(nta); 
        }

        [Test, Description("Update a Relative NTA from a non stapled secuity")]
        [ExpectedException(typeof(NotStapledSecurityComponentException))]
        public void UpdateRelativeNTANonStapled()
        {
            var manager = CreateStockManager();

            var stock = manager.StockService.Add("ABC", "Stapled Security", StockType.Ordinary);

            manager.StockService.UpdateRelativeNTA(stock, new DateTime(2006, 12, 20), 0.75M);

        }

        [Test, Description("Delete Relative NTA")]
        public void DeleteRelativeNTA()
        {
            Stock stock;
            decimal percent;

            var manager = CreateStockManager();

            var parentStock = manager.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);

            var childStock1 = manager.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            manager.StockService.AddChildStock(parentStock, childStock1);;
            manager.StockService.AddRelativeNTA(childStock1, new DateTime(2005, 12, 31), 0.50M);
            manager.StockService.AddRelativeNTA(childStock1, new DateTime(2006, 12, 31), 0.60M);
            manager.StockService.AddRelativeNTA(childStock1, new DateTime(2007, 12, 31), 0.70M);

            /* Test that NTAs added correctly */
            stock = manager.StockService.GetStock("CH1");
            percent = manager.StockService.PercentageOfParentCostBase(stock, new DateTime(2007, 06, 30));
            Assert.That(percent, Is.EqualTo(0.60M));
            stock = manager.StockService.GetStock("CH1");
            percent = manager.StockService.PercentageOfParentCostBase(stock, new DateTime(2006, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));
            percent = manager.StockService.PercentageOfParentCostBase(stock, new DateTime(2007, 06, 30));
            Assert.That(percent, Is.EqualTo(0.60M));
            percent = manager.StockService.PercentageOfParentCostBase(stock, new DateTime(2008, 06, 30));
            Assert.That(percent, Is.EqualTo(0.70M));

            /* Remove middle NTA */
            manager.StockService.DeleteRelativeNTA(childStock1, new DateTime(2006, 12, 31));
            percent = manager.StockService.PercentageOfParentCostBase(stock, new DateTime(2007, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));          
        }

        [Test, Description("Delete Relative NTA that doesn't exist")]
        public void DeleteRelativeNTANotExists()
        {
            var manager = CreateStockManager();

            var parentStock = manager.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);

            var childStock1 = manager.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            manager.StockService.AddChildStock(parentStock, childStock1);;
            manager.StockService.AddRelativeNTA(childStock1, new DateTime(2005, 12, 31), 0.50M);
            manager.StockService.AddRelativeNTA(childStock1, new DateTime(2006, 12, 31), 0.60M);
            manager.StockService.AddRelativeNTA(childStock1,  new DateTime(2007, 12, 31), 0.70M);

            manager.StockService.DeleteRelativeNTA(childStock1, new DateTime(2006, 12, 20));
        }

        [Test, Description("Delete Relative NTA from a non stapled secuity")]
        [ExpectedException(typeof(NotStapledSecurityComponentException))]
        public void DeleteRelativeNTANonStapled()
        {
            var manager = CreateStockManager();

            var stock = manager.StockService.Add("ABC", "Stapled Security", StockType.Ordinary);

            manager.StockService.DeleteRelativeNTA(stock, new DateTime(2006, 12, 20)); 
        }
    }
}
