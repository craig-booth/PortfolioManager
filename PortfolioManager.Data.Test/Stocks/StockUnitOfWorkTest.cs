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
    [TestFixture]
    public class StockUnitOfWorkTest : TestBase
    {

        [Test, Description("Test commmit")]
        public void Commit()
        {
            Stock stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock = new Stock(new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);       
                unitOfWork.StockRepository.Add(stock);

                unitOfWork.Save();
            }

            database.StockQuery.Get(stock.Id, new DateTime(2000, 01, 01));

            Assert.That(stock.ASXCode, Is.EqualTo("ABC"));
        }

        [Test, Description("Test rollback")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void Rollback()
        {
            Stock stock;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock = new Stock(new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock);

                /* no save done */
            }

            database.StockQuery.Get(stock.Id, new DateTime(2000, 01, 01));
        }

        [Test, Description("Test nested transactions")]
        public void NestedTransactions()
        {
            Stock stock1, stock2, stock;

            var database = CreateStockDatabase();
            var stockService = new StockService2(database);
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(new DateTime(2000, 01, 01), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stockService.ChangeASXCode(stock1, new DateTime(2002, 01, 01), "DEF", "New Name");

                stock2 = new Stock(new DateTime(2000, 01, 01), "GHI", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                unitOfWork.Save();
            }


            stock = database.StockQuery.Get(stock1.Id, new DateTime(2000, 01, 01));
            Assert.That(stock.Name, Is.EqualTo("Test"));

            stock = database.StockQuery.Get(stock1.Id, new DateTime(2003, 01, 01));
            Assert.That(stock.Name, Is.EqualTo("New Name"));

            stock = database.StockQuery.Get(stock2.Id, new DateTime(2000, 01, 01));
            Assert.That(stock.Name, Is.EqualTo("Test 2"));
        }
    }
}
