using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using NUnitExtension;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Data.Test.Stocks
{
    [TestFixture]
    public class StockRepositoryTest : TestBase
    {

        [Test, Description("Test adding a stock")]
        public void Add()
        {
            Stock stock ,stock2;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock = new Stock(new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);       
                unitOfWork.StockRepository.Add(stock);

                stock2 = unitOfWork.StockRepository.Get(stock.Id);

                Assert.That(stock2.ASXCode, Is.EqualTo("ABC"));
            }          
        }

        [Test, Description("Test adding a stock with the same ASX code as an existing stock")]
        [ExpectedException(typeof(DuplicateRecordException))]
        public void AddDuplicate()
        {
            Stock stock, stock2;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock = new Stock(new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock);

                stock2 = new Stock(new DateTime(2000, 01, 01), "ABC", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock);               
            }
        }

        [Test, Description("Test Get()")]
        public void Get()
        {
            Stock stock1, stock2, stock3, stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(new DateTime(2002, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock3 = new Stock(new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                stock = unitOfWork.StockRepository.Get(stock2.Id);

                Assert.That(stock, EntityConstraint.EqualTo((stock2)));
            }
        }

        [Test, Description("Test Get() at a particular date")]
        public void GetAtDate()
        {
            Stock stock1, stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(Guid.NewGuid(), new DateTime(2000, 01, 01), new DateTime(2001, 12, 31), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                var stock2 = new Stock(stock1.Id, new DateTime(2002, 01, 01), DateTimeConstants.NoEndDate, "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);
              
                stock = unitOfWork.StockRepository.Get(stock1.Id, new DateTime(2001, 01, 01));
                Assert.AreEqual(stock.ASXCode, "ABC");

                stock = unitOfWork.StockRepository.Get(stock1.Id, new DateTime(2003, 01, 01));
                Assert.AreEqual(stock.ASXCode, "DEF");
            }        
        }

        [Test, Description("Test Get() for a stock that doesn't exist")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void GetNotExists()
        {
            Stock stock1, stock2, stock3, stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(new DateTime(2002, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock3 = new Stock(new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                stock = unitOfWork.StockRepository.Get(Guid.NewGuid());

                Assert.That(stock, EntityConstraint.EqualTo((stock2)));
            }
        }

        [Test, Description("Test Get() for a stock before start date")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void GetBeforeStartDate()
        {
            Stock stock1, stock2, stock3, stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(new DateTime(2002, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock3 = new Stock(new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                stock = unitOfWork.StockRepository.Get(stock2.Id, new DateTime(2000, 01, 01));
            }
        }

        [Test, Description("Test Get() for a stock after end date")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void GetAfterEndDate()
        {
            Stock stock1, stock2, stock3, stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(Guid.NewGuid(), new DateTime(2002, 01, 01), new DateTime(2005, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);


                stock3 = new Stock(new DateTime(2003, 01, 01), "GHI", "Test 3", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                stock = unitOfWork.StockRepository.Get(stock2.Id, new DateTime(2006, 01, 01));
            }
        }

        [Test, Description("Test Update()")]
        public void Update()
        {
            Stock stock1, stock2, stock3;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(Guid.NewGuid(), new DateTime(2000, 01, 01), new DateTime(2001, 12, 31), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(stock1.Id, new DateTime(2002, 01, 01), DateTimeConstants.NoEndDate, "DEF", "Changed!!", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);


                stock3 = unitOfWork.StockRepository.Get(stock1.Id);
                Assert.AreEqual(stock3.Name, "Changed!!");
            }         
        }

        [Test, Description("Test Delete()")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void Delete()
        {
            Stock stock, stock2;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock = new Stock(new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock);

                stock2 = unitOfWork.StockRepository.Get(stock.Id);
                Assert.That(stock, EntityConstraint.EqualTo((stock2)));

                unitOfWork.StockRepository.Delete(stock2);

                unitOfWork.StockRepository.Get(stock.Id);
            }
        }

        [Test, Description("Test Delete() for stock not existing")]
        public void DeleteNotExist()
        {
            Stock stock, stock2;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock = new Stock(new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock);

                stock2 = new Stock(new DateTime(2000, 01, 01), "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
               
                unitOfWork.StockRepository.Delete(stock2);
            }
        }

        [Test, Description("Test Delete() by Id")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteById()
        {
            Stock stock, stock2;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock = new Stock(new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock);

                stock2 = unitOfWork.StockRepository.Get(stock.Id);
                Assert.That(stock, EntityConstraint.EqualTo((stock2)));

                unitOfWork.StockRepository.Delete(stock2.Id);

                unitOfWork.StockRepository.Get(stock.Id);
            }
        }

        [Test, Description("Test Delete() by Id for multiple effective dated records")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteAllEffectiveDatedRecords()
        {
            Stock stock1, stock2;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(Guid.NewGuid(), new DateTime(2000, 01, 01), new DateTime(2001, 12, 31), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(stock1.Id, new DateTime(2002, 01, 01), DateTimeConstants.NoEndDate, "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                unitOfWork.StockRepository.Delete(stock1.Id);

                unitOfWork.StockRepository.Get(stock1.Id);
            }
        }

        [Test, Description("Test Delete() by Id and date")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteByIdAtDate()
        {
            Stock stock1, stock2, stock3;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(Guid.NewGuid(), new DateTime(2000, 01, 01), new DateTime(2001, 12, 31), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(stock1.Id, new DateTime(2002, 01, 01), DateTimeConstants.NoEndDate, "DEF", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock3 = unitOfWork.StockRepository.Get(stock1.Id, new DateTime(2002, 01, 01));
                Assert.That(stock3.ASXCode, Is.EqualTo("DEF"));

                unitOfWork.StockRepository.Delete(stock1.Id, new DateTime(2002, 01, 01));      

                unitOfWork.StockRepository.Get(stock1.Id, new DateTime(2002, 01, 01));
            }
        }

        [Test, Description("Test Delete() by Id and date not exists")]
        public void DeleteByIdAtDateNotExists()
        {
            Stock stock, stock1;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock = new Stock(new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock);

                unitOfWork.StockRepository.Delete(stock.Id, new DateTime(2000, 01, 02));

                stock1 = unitOfWork.StockRepository.Get(stock.Id, new DateTime(2000, 01, 01));
                Assert.That(stock, EntityConstraint.EqualTo((stock1)));
            }
        }
        
    }
}
