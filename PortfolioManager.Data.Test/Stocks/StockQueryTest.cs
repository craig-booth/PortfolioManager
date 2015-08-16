using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.Test.Stocks
{
    class StockQueryTest : TestBase
    {

        [Test, Description("Test Get()")]
        public void Get()
        {
            Stock stock1, stock2, stock3, stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(database, new DateTime(2002, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock3 = new Stock(database, new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                unitOfWork.Save();
            }

            stock = database.StockQuery.Get(stock2.Id, new DateTime(2002, 01, 01));

            Assert.That(stock, Is.EqualTo(stock2).Using(new EffectiveDatedEntityComparer()));
        }

        [Test, Description("Test Get() at a particular date")]
        public void GetAtDate()
        {
            Stock stock1, stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock1.ChangeASXCode(new DateTime(2002, 01, 01), "DEF", "Test 2");

                unitOfWork.Save();
            }

            stock = database.StockQuery.Get(stock1.Id, new DateTime(2001, 01, 01));
            Assert.AreEqual(stock.ASXCode, "ABC");

            stock = database.StockQuery.Get(stock1.Id, new DateTime(2003, 01, 01));
            Assert.AreEqual(stock.ASXCode, "DEF");
        }

        [Test, Description("Test Get() for a stock that doesn't exist")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void GetNotExists()
        {
            Stock stock1, stock2, stock3, stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(database, new DateTime(2002, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock3 = new Stock(database, new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                unitOfWork.Save();
            }

            stock = database.StockQuery.Get(Guid.NewGuid(), new DateTime(2000, 01, 01));
        }

        [Test, Description("Test Get() for a stock before start date")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void GetBeforeStartDate()
        {
            Stock stock1, stock2, stock3, stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(database, new DateTime(2002, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock3 = new Stock(database, new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                unitOfWork.Save();
            }

            stock = database.StockQuery.Get(stock2.Id, new DateTime(2000, 01, 01));
        }

        [Test, Description("Test Get() for a stock after end date")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void GetAfterEndDate()
        {
            Stock stock1, stock2, stock3, stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(database, new DateTime(2002, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock2.Delist(new DateTime(2005, 01, 01));

                stock3 = new Stock(database, new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                unitOfWork.Save();
            }

            stock = database.StockQuery.Get(stock2.Id, new DateTime(2006, 01, 01));
        }

        [Test, Description("Test GetByASXCode()")]
        public void GetByASXCode()
        {
            Stock stock1, stock2, stock3, stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(database, new DateTime(2002, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock3 = new Stock(database, new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                unitOfWork.Save();
            }

            stock = database.StockQuery.GetByASXCode("DEF", new DateTime(2002, 01, 01));

            Assert.That(stock, Is.EqualTo(stock2).Using(new EffectiveDatedEntityComparer()));
        }

        [Test, Description("Test GetByASXCode() at a particular date")]
        public void GetByASXCodeAtDate()
        {
            Stock stock1, stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock1.ChangeASXCode(new DateTime(2002, 01, 01), "DEF", "Test 2");

                unitOfWork.Save();
            }

            stock = database.StockQuery.GetByASXCode("ABC", new DateTime(2001, 01, 01));
            Assert.That(stock, Is.EqualTo(stock1).Using(new EntityComparer()));

            stock = database.StockQuery.GetByASXCode("DEF", new DateTime(2003, 01, 01));
            Assert.That(stock, Is.EqualTo(stock1).Using(new EntityComparer()));
        }

        [Test, Description("Test GetByASXCode() for a stock that doesn't exist")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void GetByASXCodeNotExists()
        {
            Stock stock1, stock2, stock3, stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(database, new DateTime(2002, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock3 = new Stock(database, new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                unitOfWork.Save();
            }

            stock = database.StockQuery.GetByASXCode("XXX", new DateTime(2000, 01, 01));
        }

        [Test, Description("Test GetByASXCode() for a stock before start date")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void GetByASXCodeBeforeStartDate()
        {
            Stock stock1, stock2, stock3, stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(database, new DateTime(2002, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock3 = new Stock(database, new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                unitOfWork.Save();
            }

            stock = database.StockQuery.GetByASXCode("DEF", new DateTime(2000, 01, 01));
        }

        [Test, Description("Test GetByASXCode() for a stock after end date")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void GetByASXCodeAfterEndDate()
        {
            Stock stock1, stock2, stock3, stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(database, new DateTime(2002, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock2.Delist(new DateTime(2005, 01, 01));

                stock3 = new Stock(database, new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                unitOfWork.Save();
            }

            stock = database.StockQuery.GetByASXCode("DEF", new DateTime(2006, 01, 01));
        }

        [Test, Description("Test GetASXCode()")]
        public void GetASXCode()
        {
            Stock stock1, stock2, stock3;
            string asxCode;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(database, new DateTime(2002, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock3 = new Stock(database, new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                unitOfWork.Save();
            }

            asxCode = database.StockQuery.GetASXCode(stock1.Id, new DateTime(2000, 01, 01));

            Assert.That(asxCode, Is.EqualTo("ABC"));
        }

        [Test, Description("Test GetASXCode() at a particular date")]
        public void GetASXCodeAtDate()
        {
            Stock stock1;
            string asxCode;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock1.ChangeASXCode(new DateTime(2002, 01, 01), "DEF", "Test 2");

                unitOfWork.Save();
            }

            asxCode = database.StockQuery.GetASXCode(stock1.Id, new DateTime(2001, 01, 01));
            Assert.AreEqual(asxCode, "ABC");

            asxCode = database.StockQuery.GetASXCode(stock1.Id, new DateTime(2003, 01, 01));
            Assert.AreEqual(asxCode, "DEF");
        }

        [Test, Description("Test GetASXCode() for a stock that doesn't exist")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void GetASXCodeNotExists()
        {
            Stock stock1, stock2, stock3;
            string asxCode;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(database, new DateTime(2002, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock3 = new Stock(database, new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                unitOfWork.Save();
            }

            asxCode = database.StockQuery.GetASXCode(Guid.NewGuid(), new DateTime(2000, 01, 01));
        }

        [Test, Description("Test GetASXCode() for a stock before start date")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void GetASXCodeBeforeStartDate()
        {
            Stock stock1, stock2, stock3;
            string asxCode;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(database, new DateTime(2002, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock3 = new Stock(database, new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                unitOfWork.Save();
            }

            asxCode = database.StockQuery.GetASXCode(stock2.Id, new DateTime(2000, 01, 01));
        }

        [Test, Description("Test GetASXCode() for a stock after end date")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void GetASXCodeAfterEndDate()
        {
            Stock stock1, stock2, stock3;
            string asxCode;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(database, new DateTime(2002, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock2.Delist(new DateTime(2005, 01, 01));

                stock3 = new Stock(database, new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                unitOfWork.Save();
            }

            asxCode = database.StockQuery.GetASXCode(stock2.Id, new DateTime(2006, 01, 01));
        }

        [Test, Description("Test GetChildStocks()")]
        public void GetChildStocks()
        {
            Stock parent, child1, child2, child;
            IReadOnlyCollection<Stock> children;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                parent = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Parent", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(parent);

                child1 = new Stock(database, new DateTime(2000, 01, 01), "DEF", "Child 1", StockType.Ordinary, parent.Id);
                unitOfWork.StockRepository.Add(child1);

                child2 = new Stock(database, new DateTime(2000, 01, 01), "GHI", "Child 2", StockType.Trust, parent.Id);
                unitOfWork.StockRepository.Add(child2);

                unitOfWork.Save();
            }

            children = database.StockQuery.GetChildStocks(parent.Id, new DateTime(2000, 01, 01));
            Assert.AreEqual(children.Count, 2);

            child = children.First();
            Assert.AreEqual(child.ASXCode, "DEF");

            child = children.Last();
            Assert.AreEqual(child.ASXCode, "GHI");
        }


        [Test, Description("Test GetChildStocks() with no children")]
        public void GetChildStocksNoChildren()
        {
            Stock parent;
            IReadOnlyCollection<Stock> children;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                parent = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Parent", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(parent);

                unitOfWork.Save();
            }

            children = database.StockQuery.GetChildStocks(parent.Id, new DateTime(2000, 01, 01));
            Assert.AreEqual(children.Count, 0);   
        }

        [Test, Description("Test PercentOfParentCost()")]
        public void PercentOfParentCost()
        {
            Stock parent, child1, child2;
            decimal percent;


            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                parent = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Parent", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(parent);

                child1 = new Stock(database, new DateTime(2000, 01, 01), "DEF", "Child 1", StockType.Ordinary, parent.Id);
                unitOfWork.StockRepository.Add(child1);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(database, new DateTime(2000, 01, 01), parent.Id, child1.Id, 0.10M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(database, new DateTime(2001, 01, 01), parent.Id, child1.Id, 0.20M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(database, new DateTime(2002, 01, 01), parent.Id, child1.Id, 0.30M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(database, new DateTime(2003, 01, 01), parent.Id, child1.Id, 0.40M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(database, new DateTime(2004, 01, 01), parent.Id, child1.Id, 0.50M));


                child2 = new Stock(database, new DateTime(2000, 01, 01), "GHI", "Child 2", StockType.Trust, parent.Id);
                unitOfWork.StockRepository.Add(child2);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(database, new DateTime(2000, 01, 01), parent.Id, child2.Id, 0.80M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(database, new DateTime(2001, 01, 01), parent.Id, child2.Id, 0.70M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(database, new DateTime(2002, 01, 01), parent.Id, child2.Id, 0.60M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(database, new DateTime(2003, 01, 01), parent.Id, child2.Id, 0.50M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(database, new DateTime(2004, 01, 01), parent.Id, child2.Id, 0.40M));

                unitOfWork.Save();
            }

            percent = database.StockQuery.PercentOfParentCost(parent.Id, child1.Id, new DateTime(2000, 06, 30));
            Assert.AreEqual(percent, 0.10M);
            percent = database.StockQuery.PercentOfParentCost(parent.Id, child1.Id, new DateTime(2002, 06, 30));
            Assert.AreEqual(percent, 0.30M);
            percent = database.StockQuery.PercentOfParentCost(parent.Id, child1.Id, new DateTime(2005, 06, 30));
            Assert.AreEqual(percent, 0.50M);

            percent = database.StockQuery.PercentOfParentCost(parent.Id, child2.Id, new DateTime(2000, 06, 30));
            Assert.AreEqual(percent, 0.80M);
            percent = database.StockQuery.PercentOfParentCost(parent.Id, child2.Id, new DateTime(2002, 06, 30));
            Assert.AreEqual(percent, 0.60M);
            percent = database.StockQuery.PercentOfParentCost(parent.Id, child2.Id, new DateTime(2005, 06, 30));
            Assert.AreEqual(percent, 0.40M);
        }


        [Test, Description("Test PercentOfParentCost() with no parent")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void PercentOfParentCostNoParent()
        {
            Stock child;
            decimal percent;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                child = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Parent", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(child);

                unitOfWork.Save();
            }

            percent = database.StockQuery.PercentOfParentCost(child.ParentId, child.Id, new DateTime(2000, 06, 30));
        }


        [Test, Description("Test PercentOfParentCost() invalid id")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void PercentOfParentCostIdInvalid()
        {
            var database = CreateStockDatabase();
            database.StockQuery.PercentOfParentCost(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2000, 06, 30));
        }

        [Test, Description("Test PercentOfParentCost() no Relative NTA Records")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void PercentOfParentCostNoData()
        {
            Stock parent, child1, child2;
            decimal percent;


            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                parent = new Stock(database, new DateTime(2000, 01, 01), "ABC", "Parent", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(parent);

                child1 = new Stock(database, new DateTime(2000, 01, 01), "DEF", "Child 1", StockType.Ordinary, parent.Id);
                unitOfWork.StockRepository.Add(child1);


                child2 = new Stock(database, new DateTime(2000, 01, 01), "GHI", "Child 2", StockType.Trust, parent.Id);
                unitOfWork.StockRepository.Add(child2);

                unitOfWork.Save();
            }

            percent = database.StockQuery.PercentOfParentCost(parent.Id, child1.Id, new DateTime(2000, 06, 30));
        }
    }
}
