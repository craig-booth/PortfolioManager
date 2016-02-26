using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Data;

namespace StockManager.Service.Test
{
    [TestFixture]
    public class StockTest : StockServiceTestBase 
    {
        [Test, Description("Change ASX Code")]
        public void ChangeASXCode()
        {
            var serviceRepository = CreateStockServiceRepository();

            var stock = serviceRepository.StockService.Add("ABC", "Old Name");
            serviceRepository.StockService.ChangeASXCode(stock, new DateTime(2010, 10, 15), "DEF", "New Name");

            var stock1 = serviceRepository.StockService.GetStock("ABC", new DateTime(2010, 01, 01));
            Assert.That(stock1.ASXCode, Is.EqualTo("ABC"), "Stock 1 ASX code wrong");
            Assert.That(stock1.Name, Is.EqualTo("Old Name"), "Stock 1 name wrong");

            var stock2 = serviceRepository.StockService.GetStock("DEF", new DateTime(2010, 10, 20));
            Assert.That(stock2.ASXCode, Is.EqualTo("DEF"), "Stock 2 ASX code wrong");
            Assert.That(stock2.Name, Is.EqualTo("New Name"), "Stock 2 name wrong");

            Assert.That(stock1.Id, Is.EqualTo(stock2.Id), "Stock 1 != Stock2");
            Assert.That(stock1.ToDate, Is.EqualTo(new DateTime(2010, 10, 14)), "Stock 1 date wrong");
            Assert.That(stock2.FromDate, Is.EqualTo(new DateTime(2010, 10, 15)), "Stock 2 date wrong");
        }

        [Test, Description("Delist")]
        public void Delist()
        {
            var serviceRepository = CreateStockServiceRepository();

            var stock = serviceRepository.StockService.Add("ABC", "Stapled Security");
            serviceRepository.StockService.Delist(stock, new DateTime(2010, 06, 15));

            var stock1 = serviceRepository.StockService.GetStock("ABC", new DateTime(2010, 01, 01));
            Assert.That(stock1.ASXCode, Is.EqualTo("ABC"));
            Assert.That(stock1.ToDate, Is.EqualTo(new DateTime(2010, 06, 14)));
        }

        [Test, Description("Add Child Stock")]
        public void AddChildStock()
        {
            var serviceRepository = CreateStockServiceRepository();

            var parentStock = serviceRepository.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);
            var childStock1 = serviceRepository.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            serviceRepository.StockService.AddChildStock(parentStock, childStock1);

            var childStock2 = serviceRepository.StockService.Add("CH2", "Child 2", StockType.Trust);
            serviceRepository.StockService.AddChildStock(parentStock, childStock2);

            var stock1 = serviceRepository.StockService.GetStock("ABC");
            var children = serviceRepository.StockService.GetChildStocks(stock1);

            Assert.That(children.Count, Is.EqualTo(2));
            Assert.That(children.First().ASXCode, Is.EqualTo("CH1"));
            Assert.That(children.Last().ASXCode, Is.EqualTo("CH2"));
        }

        [Test, Description("Add Child Stock for a non stapled security")]
        [ExpectedException(typeof(NotStapledSecurityException))]
        public void AddChildStockNonStapled()
        {
            var serviceRepository = CreateStockServiceRepository();

            var parentStock = serviceRepository.StockService.Add("ABC", "Stapled Security", StockType.Ordinary);
            var childStock1 = serviceRepository.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            serviceRepository.StockService.AddChildStock(parentStock, childStock1);;
        }

        [Test, Description("Get Child stocks for a non stapled security")]
        public void GetChildStocksNonStapled()
        {
            var serviceRepository = CreateStockServiceRepository();

            var parentStock = serviceRepository.StockService.Add("ABC", "Ordinary Security", StockType.Ordinary);

            var stock1 = serviceRepository.StockService.GetStock("ABC");
            var children = serviceRepository.StockService.GetChildStocks(stock1);

            Assert.That(children.Count, Is.EqualTo(0));
        }

        [Test, Description("Remove Child stock")]
        public void RemoveChildStock()
        {
            var serviceRepository = CreateStockServiceRepository();

            var parentStock = serviceRepository.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);
            var childStock1 = serviceRepository.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            serviceRepository.StockService.AddChildStock(parentStock, childStock1);;

            var childStock2 = serviceRepository.StockService.Add("CH2", "Child 2", StockType.Trust);
            serviceRepository.StockService.AddChildStock(parentStock, childStock2);;

            var stock1 = serviceRepository.StockService.GetStock("ABC");
            var children = serviceRepository.StockService.GetChildStocks(stock1);
            Assert.That(children.Count, Is.EqualTo(2));

            serviceRepository.StockService.RemoveChildStock(stock1, childStock1);
            var children2 = serviceRepository.StockService.GetChildStocks(stock1);
            Assert.That(children2.Count, Is.EqualTo(1));
            Assert.That(children2.First().ASXCode, Is.EqualTo("CH2"));

        }

        [Test, Description("Remove Child stock not exists")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void RemoveChildStockNotExists()
        {
            var serviceRepository = CreateStockServiceRepository();

            var parentStock = serviceRepository.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);
            var childStock1 = serviceRepository.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            serviceRepository.StockService.AddChildStock(parentStock, childStock1);;

            var childStock2 = serviceRepository.StockService.Add("CH2", "Child 2", StockType.Trust);
            serviceRepository.StockService.AddChildStock(parentStock, childStock2);;

            var stock1 = serviceRepository.StockService.GetStock("ABC");
            var children = serviceRepository.StockService.GetChildStocks(stock1);
            Assert.That(children.Count, Is.EqualTo(2));


            var childStock3 = serviceRepository.StockService.Add("CH3", "Child 3", StockType.Trust);
            serviceRepository.StockService.RemoveChildStock(stock1, childStock3);
        }

        [Test, Description("Remove Child stock for a non stapled security")]
        [ExpectedException(typeof(NotStapledSecurityException))]    
        public void RemoveChildStockNonStapled()
        {
            var serviceRepository = CreateStockServiceRepository();

            var parentStock = serviceRepository.StockService.Add("ABC", "Stapled Security", StockType.Ordinary);
            var childStock = serviceRepository.StockService.Add("CH3", "Child 3", StockType.Trust);
            serviceRepository.StockService.RemoveChildStock(parentStock, childStock);
        }

        [Test, Description("Add Relative NTA")]
        public void AddRelativeNTA()
        {
            Stock stock;
            decimal percent;

            var serviceRepository = CreateStockServiceRepository();

            var parentStock = serviceRepository.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);

            var childStock1 = serviceRepository.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            serviceRepository.StockService.AddChildStock(parentStock, childStock1);;
            serviceRepository.StockService.AddRelativeNTA(childStock1, new DateTime(2005, 12, 31), 0.50M);
            serviceRepository.StockService.AddRelativeNTA(childStock1, new DateTime(2006, 12, 31), 0.60M);

            var childStock2 = serviceRepository.StockService.Add("CH2", "Child 2", StockType.Trust);
            serviceRepository.StockService.AddChildStock(parentStock, childStock2);;
            serviceRepository.StockService.AddRelativeNTA(childStock2, new DateTime(2005, 12, 31), 0.50M);
            serviceRepository.StockService.AddRelativeNTA(childStock2, new DateTime(2006, 12, 31), 0.40M);


            stock = serviceRepository.StockService.GetStock("CH1");
            percent = serviceRepository.StockService.PercentageOfParentCostBase(stock, new DateTime(2006, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));
            percent = serviceRepository.StockService.PercentageOfParentCostBase(stock, new DateTime(2008, 06, 30));
            Assert.That(percent, Is.EqualTo(0.60M));

            stock = serviceRepository.StockService.GetStock("CH2");
            percent = serviceRepository.StockService.PercentageOfParentCostBase(stock, new DateTime(2006, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));
            percent = serviceRepository.StockService.PercentageOfParentCostBase(stock, new DateTime(2008, 06, 30));
            Assert.That(percent, Is.EqualTo(0.40M));
        }

        [Test, Description("Add Relative NTA to a non stapled secuity")]
        [ExpectedException(typeof(NotStapledSecurityComponentException))]    
        public void AddRelativeNTANonStapled()
        {
            var serviceRepository = CreateStockServiceRepository();

            var parentStock = serviceRepository.StockService.Add("ABC", "Stapled Security", StockType.Ordinary);
            var childStock1 = serviceRepository.StockService.Add("CH1", "Child 1", StockType.Ordinary);

            serviceRepository.StockService.AddRelativeNTA(childStock1, new DateTime(2005, 12, 31), 0.50M);
        }

        [Test, Description("Get Relative NTA for a non stapled secuity")]
        [ExpectedException(typeof(NotStapledSecurityComponentException))]
        public void GetRelativeNTANonStapled()
        {
            var serviceRepository = CreateStockServiceRepository();

            var parentStock = serviceRepository.StockService.Add("ABC", "Stapled Security", StockType.Ordinary);

            serviceRepository.StockService.PercentageOfParentCostBase(parentStock, new DateTime(2005, 12, 31));
        }

        [Test, Description("Update a Relative NTA")]
        public void UpdateRelativeNTA()
        {
            Stock stock;
            decimal percent;

            var serviceRepository = CreateStockServiceRepository();

            var parentStock = serviceRepository.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);

            var childStock1 = serviceRepository.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            serviceRepository.StockService.AddChildStock(parentStock, childStock1);;
            serviceRepository.StockService.AddRelativeNTA(childStock1, new DateTime(2005, 12, 31), 0.50M);
            var nta = serviceRepository.StockService.AddRelativeNTA(childStock1, new DateTime(2006, 12, 31), 0.60M);
       
            stock = serviceRepository.StockService.GetStock("CH1");
            percent = serviceRepository.StockService.PercentageOfParentCostBase(stock, new DateTime(2006, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));
            percent = serviceRepository.StockService.PercentageOfParentCostBase(stock, new DateTime(2008, 06, 30));
            Assert.That(percent, Is.EqualTo(0.60M));

            nta.Percentage = 0.70m;
            serviceRepository.StockService.UpdateRelativeNTA(nta);

            stock = serviceRepository.StockService.GetStock("CH1");
            percent = serviceRepository.StockService.PercentageOfParentCostBase(stock, new DateTime(2006, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));
            percent = serviceRepository.StockService.PercentageOfParentCostBase(stock, new DateTime(2008, 06, 30));
            Assert.That(percent, Is.EqualTo(0.70M));
        }

        [Test, Description("Update a Relative NTA that doesn't exist")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void UpdateRelativeNTANotExists()
        {
            var serviceRepository = CreateStockServiceRepository();

            var parentStock = serviceRepository.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);

            var childStock1 = serviceRepository.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            serviceRepository.StockService.AddChildStock(parentStock, childStock1);;
            serviceRepository.StockService.AddRelativeNTA(childStock1, new DateTime(2005, 12, 31), 0.50M);
            serviceRepository.StockService.AddRelativeNTA(childStock1, new DateTime(2006, 12, 31), 0.60M);
            serviceRepository.StockService.AddRelativeNTA(childStock1, new DateTime(2007, 12, 31), 0.70M);

            var nta = new RelativeNTA(new DateTime(2006, 12, 20), parentStock.Id, childStock1.Id, 0.80m);
            serviceRepository.StockService.UpdateRelativeNTA(nta); 
        }

        [Test, Description("Update a Relative NTA from a non stapled secuity")]
        [ExpectedException(typeof(NotStapledSecurityComponentException))]
        public void UpdateRelativeNTANonStapled()
        {
            var serviceRepository = CreateStockServiceRepository();

            var stock = serviceRepository.StockService.Add("ABC", "Stapled Security", StockType.Ordinary);

            serviceRepository.StockService.UpdateRelativeNTA(stock, new DateTime(2006, 12, 20), 0.75M);

        }

        [Test, Description("Delete Relative NTA")]
        public void DeleteRelativeNTA()
        {
            Stock stock;
            decimal percent;

            var serviceRepository = CreateStockServiceRepository();

            var parentStock = serviceRepository.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);

            var childStock1 = serviceRepository.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            serviceRepository.StockService.AddChildStock(parentStock, childStock1);;
            serviceRepository.StockService.AddRelativeNTA(childStock1, new DateTime(2005, 12, 31), 0.50M);
            serviceRepository.StockService.AddRelativeNTA(childStock1, new DateTime(2006, 12, 31), 0.60M);
            serviceRepository.StockService.AddRelativeNTA(childStock1, new DateTime(2007, 12, 31), 0.70M);

            /* Test that NTAs added correctly */
            stock = serviceRepository.StockService.GetStock("CH1");
            percent = serviceRepository.StockService.PercentageOfParentCostBase(stock, new DateTime(2007, 06, 30));
            Assert.That(percent, Is.EqualTo(0.60M));
            stock = serviceRepository.StockService.GetStock("CH1");
            percent = serviceRepository.StockService.PercentageOfParentCostBase(stock, new DateTime(2006, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));
            percent = serviceRepository.StockService.PercentageOfParentCostBase(stock, new DateTime(2007, 06, 30));
            Assert.That(percent, Is.EqualTo(0.60M));
            percent = serviceRepository.StockService.PercentageOfParentCostBase(stock, new DateTime(2008, 06, 30));
            Assert.That(percent, Is.EqualTo(0.70M));

            /* Remove middle NTA */
            serviceRepository.StockService.DeleteRelativeNTA(childStock1, new DateTime(2006, 12, 31));
            percent = serviceRepository.StockService.PercentageOfParentCostBase(stock, new DateTime(2007, 06, 30));
            Assert.That(percent, Is.EqualTo(0.50M));          
        }

        [Test, Description("Delete Relative NTA that doesn't exist")]
        public void DeleteRelativeNTANotExists()
        {
            var serviceRepository = CreateStockServiceRepository();

            var parentStock = serviceRepository.StockService.Add("ABC", "Stapled Security", StockType.StapledSecurity);

            var childStock1 = serviceRepository.StockService.Add("CH1", "Child 1", StockType.Ordinary);
            serviceRepository.StockService.AddChildStock(parentStock, childStock1);;
            serviceRepository.StockService.AddRelativeNTA(childStock1, new DateTime(2005, 12, 31), 0.50M);
            serviceRepository.StockService.AddRelativeNTA(childStock1, new DateTime(2006, 12, 31), 0.60M);
            serviceRepository.StockService.AddRelativeNTA(childStock1,  new DateTime(2007, 12, 31), 0.70M);

            serviceRepository.StockService.DeleteRelativeNTA(childStock1, new DateTime(2006, 12, 20));
        }

        [Test, Description("Delete Relative NTA from a non stapled secuity")]
        [ExpectedException(typeof(NotStapledSecurityComponentException))]
        public void DeleteRelativeNTANonStapled()
        {
            var serviceRepository = CreateStockServiceRepository();

            var stock = serviceRepository.StockService.Add("ABC", "Stapled Security", StockType.Ordinary);

            serviceRepository.StockService.DeleteRelativeNTA(stock, new DateTime(2006, 12, 20)); 
        }
    }
}
