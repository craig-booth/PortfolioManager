using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;

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
            Stock stock1, stock2, stock3, stock;

            var database = CreateStockDatabase();

            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                stock1 = new Stock(Guid.NewGuid(), new DateTime(2000, 01, 01), new DateTime(2001, 12, 31), "ABC", "Test", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock1);

                stock2 = new Stock(stock1.Id, new DateTime(2002, 01, 01), DateTimeConstants.NoEndDate, "DEF", "New Name", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock2);

                stock3 = new Stock(new DateTime(2000, 01, 01), "GHI", "Test 2", StockType.Ordinary, Guid.Empty);
                unitOfWork.StockRepository.Add(stock3);

                unitOfWork.Save();
            }


            stock = database.StockQuery.Get(stock1.Id, new DateTime(2000, 01, 01));
            Assert.That(stock.Name, Is.EqualTo("Test"));

            stock = database.StockQuery.Get(stock1.Id, new DateTime(2003, 01, 01));
            Assert.That(stock.Name, Is.EqualTo("New Name"));

            stock = database.StockQuery.Get(stock3.Id, new DateTime(2000, 01, 01));
            Assert.That(stock.Name, Is.EqualTo("Test 2"));
        }
    }
}
