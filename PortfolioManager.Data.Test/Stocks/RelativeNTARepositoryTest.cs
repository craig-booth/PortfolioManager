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
    public class RelativeNTARepositoryTest : TestBase
    {

        [Test, Description("Test adding a Relative NTA")]
        public void Add()
        {
            Stock parent, child;
            RelativeNTA nta, nta1;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                parent = new Stock(new DateTime(2000, 01, 01), "ABC", "Parent", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(parent);

                child = new Stock(new DateTime(2000, 01, 01), "DEF", "Child", StockType.Ordinary, parent.Id);
                unitOfWork.StockRepository.Add(child);
                nta = new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child.Id, 0.10M);
                unitOfWork.RelativeNTARepository.Add(nta);

                nta1 =  unitOfWork.RelativeNTARepository.Get(nta.Id);
            }

            Assert.That(nta1.Parent, Is.EqualTo(parent.Id));
            Assert.That(nta1.Child, Is.EqualTo(child.Id));
            Assert.That(nta1.Percentage, Is.EqualTo(0.10M));
        }

        [Test, Description("Test adding a relative nta with the same details as an existing entry")]
        [ExpectedException(typeof(DuplicateRecordException))]
        public void AddDuplicate()
        {
            Stock parent, child;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                parent = new Stock(new DateTime(2000, 01, 01), "ABC", "Parent", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(parent);

                child = new Stock(new DateTime(2000, 01, 01), "DEF", "Child", StockType.Ordinary, parent.Id);
                unitOfWork.StockRepository.Add(child);

                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child.Id, 0.10M));

                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child.Id, 0.30M));
            }
        }

        [Test, Description("Test Get()")]
        public void Get()
        {
            Stock parent, child1, child2;
            RelativeNTA nta1, nta2, nta3, nta4;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                parent = new Stock(new DateTime(2000, 01, 01), "ABC", "Parent", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(parent);

                child1 = new Stock(new DateTime(2000, 01, 01), "DEF", "Child 1", StockType.Ordinary, parent.Id);
                unitOfWork.StockRepository.Add(child1);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child1.Id, 0.10M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2001, 01, 01), parent.Id, child1.Id, 0.20M));
                nta1 = new RelativeNTA(new DateTime(2002, 01, 01), parent.Id, child1.Id, 0.30M);
                unitOfWork.RelativeNTARepository.Add(nta1);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2003, 01, 01), parent.Id, child1.Id, 0.40M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2004, 01, 01), parent.Id, child1.Id, 0.50M));


                child2 = new Stock(new DateTime(2000, 01, 01), "GHI", "Child 2", StockType.Trust, parent.Id);
                unitOfWork.StockRepository.Add(child2);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child2.Id, 0.80M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2001, 01, 01), parent.Id, child2.Id, 0.70M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2002, 01, 01), parent.Id, child2.Id, 0.60M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2003, 01, 01), parent.Id, child2.Id, 0.50M));
                nta2 = new RelativeNTA(new DateTime(2004, 01, 01), parent.Id, child2.Id, 0.40M);
                unitOfWork.RelativeNTARepository.Add(nta2);


                nta3 = unitOfWork.RelativeNTARepository.Get(nta1.Id);
                nta4 = unitOfWork.RelativeNTARepository.Get(nta2.Id);
            }

            Assert.That(nta3.Parent, Is.EqualTo(parent.Id));
            Assert.That(nta3.Child, Is.EqualTo(child1.Id));
            Assert.That(nta3.Percentage, Is.EqualTo(0.30M));

            Assert.That(nta4.Parent, Is.EqualTo(parent.Id));
            Assert.That(nta4.Child, Is.EqualTo(child2.Id));
            Assert.That(nta4.Percentage, Is.EqualTo(0.40M));
        }


        [Test, Description("Test Get() for a relative NTA that doesn't exist")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void GetNotExists()
        {
            Stock parent, child1, child2;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                parent = new Stock(new DateTime(2000, 01, 01), "ABC", "Parent", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(parent);

                child1 = new Stock(new DateTime(2000, 01, 01), "DEF", "Child 1", StockType.Ordinary, parent.Id);
                unitOfWork.StockRepository.Add(child1);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child1.Id, 0.10M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2001, 01, 01), parent.Id, child1.Id, 0.20M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2002, 01, 01), parent.Id, child1.Id, 0.30M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2003, 01, 01), parent.Id, child1.Id, 0.40M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2004, 01, 01), parent.Id, child1.Id, 0.50M));


                child2 = new Stock(new DateTime(2000, 01, 01), "GHI", "Child 2", StockType.Trust, parent.Id);
                unitOfWork.StockRepository.Add(child2);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child2.Id, 0.80M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2001, 01, 01), parent.Id, child2.Id, 0.70M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2002, 01, 01), parent.Id, child2.Id, 0.60M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2003, 01, 01), parent.Id, child2.Id, 0.50M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2004, 01, 01), parent.Id, child2.Id, 0.40M));


                unitOfWork.RelativeNTARepository.Get(Guid.NewGuid());
            }

        }

        [Test, Description("Test Update()")]
        public void Update()
        {
            Stock parent, child;
            RelativeNTA nta, nta1;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                parent = new Stock(new DateTime(2000, 01, 01), "ABC", "Parent", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(parent);

                child = new Stock(new DateTime(2000, 01, 01), "DEF", "Child 1", StockType.Ordinary, parent.Id);
                unitOfWork.StockRepository.Add(child);
                nta = new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child.Id, 0.10M);
                unitOfWork.RelativeNTARepository.Add(nta);

                nta.Percentage = 0.40m;
                unitOfWork.RelativeNTARepository.Update(nta);

                nta1 = unitOfWork.RelativeNTARepository.Get(nta.Id);

                Assert.That(nta1.Percentage, Is.EqualTo(0.40M));
            }
        }

        [Test, Description("Test Delete()")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void Delete()
        {
            Stock parent, child;
            RelativeNTA nta;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                parent = new Stock(new DateTime(2000, 01, 01), "ABC", "Parent", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(parent);

                child = new Stock(new DateTime(2000, 01, 01), "DEF", "Child 1", StockType.Ordinary, parent.Id);
                unitOfWork.StockRepository.Add(child);
                nta = new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child.Id, 0.10M);
                unitOfWork.RelativeNTARepository.Add(nta);

                unitOfWork.RelativeNTARepository.Delete(nta);

                unitOfWork.RelativeNTARepository.Get(nta.Id);

            }
        }

        [Test, Description("Test Delete() by Id")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteById()
        {
            Stock parent, child;
            RelativeNTA nta;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                parent = new Stock(new DateTime(2000, 01, 01), "ABC", "Parent", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(parent);

                child = new Stock(new DateTime(2000, 01, 01), "DEF", "Child 1", StockType.Ordinary, parent.Id);
                unitOfWork.StockRepository.Add(child);
                nta = new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child.Id, 0.10M);
                unitOfWork.RelativeNTARepository.Add(nta);

                unitOfWork.RelativeNTARepository.Delete(nta.Id);

                unitOfWork.RelativeNTARepository.Get(nta.Id);
            }
        }

        [Test, Description("Test Delete() by Id not exists")]
        public void DeleteByIdNotExists()
        {
            Stock parent, child;
            RelativeNTA nta;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                parent = new Stock(new DateTime(2000, 01, 01), "ABC", "Parent", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(parent);

                child = new Stock(new DateTime(2000, 01, 01), "DEF", "Child 1", StockType.Ordinary, parent.Id);
                unitOfWork.StockRepository.Add(child);
                nta = new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child.Id, 0.10M);
                unitOfWork.RelativeNTARepository.Add(nta);

                unitOfWork.RelativeNTARepository.Delete(Guid.NewGuid());
            }
        }

        [Test, Description("Test Delete() by Parent")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteByParent()
        {
            Stock parent, child1, child2;
            RelativeNTA nta1, nta2, nta3, nta4;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                parent = new Stock(new DateTime(2000, 01, 01), "ABC", "Parent", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(parent);

                child1 = new Stock(new DateTime(2000, 01, 01), "DEF", "Child 1", StockType.Ordinary, parent.Id);
                unitOfWork.StockRepository.Add(child1);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child1.Id, 0.10M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2001, 01, 01), parent.Id, child1.Id, 0.20M));
                nta1 = new RelativeNTA(new DateTime(2002, 01, 01), parent.Id, child1.Id, 0.30M);
                unitOfWork.RelativeNTARepository.Add(nta1);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2003, 01, 01), parent.Id, child1.Id, 0.40M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2004, 01, 01), parent.Id, child1.Id, 0.50M));


                child2 = new Stock(new DateTime(2000, 01, 01), "GHI", "Child 2", StockType.Trust, parent.Id);
                unitOfWork.StockRepository.Add(child2);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child2.Id, 0.80M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2001, 01, 01), parent.Id, child2.Id, 0.70M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2002, 01, 01), parent.Id, child2.Id, 0.60M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2003, 01, 01), parent.Id, child2.Id, 0.50M));
                nta2 = new RelativeNTA(new DateTime(2004, 01, 01), parent.Id, child2.Id, 0.40M);
                unitOfWork.RelativeNTARepository.Add(nta2);


                nta3 = unitOfWork.RelativeNTARepository.Get(nta1.Id);
                nta4 = unitOfWork.RelativeNTARepository.Get(nta2.Id);

                Assert.That(nta3.Parent, Is.EqualTo(parent.Id));
                Assert.That(nta3.Child, Is.EqualTo(child1.Id));
                Assert.That(nta3.Percentage, Is.EqualTo(0.30M));

                Assert.That(nta4.Parent, Is.EqualTo(parent.Id));
                Assert.That(nta4.Child, Is.EqualTo(child2.Id));
                Assert.That(nta4.Percentage, Is.EqualTo(0.40M));

                unitOfWork.RelativeNTARepository.DeleteAll(parent.Id);

                nta3 = unitOfWork.RelativeNTARepository.Get(nta1.Id);
                nta4 = unitOfWork.RelativeNTARepository.Get(nta2.Id);
            } 
        }

        [Test, Description("Test Delete() by Parent,Child")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteByParentChild()
        {
            Stock parent, child1, child2;
            RelativeNTA nta1, nta2, nta3, nta4;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                parent = new Stock(new DateTime(2000, 01, 01), "ABC", "Parent", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(parent);

                child1 = new Stock(new DateTime(2000, 01, 01), "DEF", "Child 1", StockType.Ordinary, parent.Id);
                unitOfWork.StockRepository.Add(child1);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child1.Id, 0.10M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2001, 01, 01), parent.Id, child1.Id, 0.20M));
                nta1 = new RelativeNTA(new DateTime(2002, 01, 01), parent.Id, child1.Id, 0.30M);
                unitOfWork.RelativeNTARepository.Add(nta1);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2003, 01, 01), parent.Id, child1.Id, 0.40M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2004, 01, 01), parent.Id, child1.Id, 0.50M));


                child2 = new Stock(new DateTime(2000, 01, 01), "GHI", "Child 2", StockType.Trust, parent.Id);
                unitOfWork.StockRepository.Add(child2);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child2.Id, 0.80M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2001, 01, 01), parent.Id, child2.Id, 0.70M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2002, 01, 01), parent.Id, child2.Id, 0.60M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2003, 01, 01), parent.Id, child2.Id, 0.50M));
                nta2 = new RelativeNTA(new DateTime(2004, 01, 01), parent.Id, child2.Id, 0.40M);
                unitOfWork.RelativeNTARepository.Add(nta2);


                nta3 = unitOfWork.RelativeNTARepository.Get(nta1.Id);
                nta4 = unitOfWork.RelativeNTARepository.Get(nta2.Id);

                Assert.That(nta3.Parent, Is.EqualTo(parent.Id));
                Assert.That(nta3.Child, Is.EqualTo(child1.Id));
                Assert.That(nta3.Percentage, Is.EqualTo(0.30M));

                Assert.That(nta4.Parent, Is.EqualTo(parent.Id));
                Assert.That(nta4.Child, Is.EqualTo(child2.Id));
                Assert.That(nta4.Percentage, Is.EqualTo(0.40M));

                unitOfWork.RelativeNTARepository.DeleteAll(child1.ParentId, child1.Id);

                nta3 = unitOfWork.RelativeNTARepository.Get(nta1.Id);
            }           
        }

        [Test, Description("Test Delete() by Parent,Child,Date")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteByParentChildDate()
        {
            Stock parent, child1, child2;
            RelativeNTA nta1, nta2, nta3, nta4;

            var database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                parent = new Stock(new DateTime(2000, 01, 01), "ABC", "Parent", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(parent);

                child1 = new Stock(new DateTime(2000, 01, 01), "DEF", "Child 1", StockType.Ordinary, parent.Id);
                unitOfWork.StockRepository.Add(child1);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child1.Id, 0.10M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2001, 01, 01), parent.Id, child1.Id, 0.20M));
                nta1 = new RelativeNTA(new DateTime(2002, 01, 01), parent.Id, child1.Id, 0.30M);
                unitOfWork.RelativeNTARepository.Add(nta1);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2003, 01, 01), parent.Id, child1.Id, 0.40M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2004, 01, 01), parent.Id, child1.Id, 0.50M));


                child2 = new Stock(new DateTime(2000, 01, 01), "GHI", "Child 2", StockType.Trust, parent.Id);
                unitOfWork.StockRepository.Add(child2);
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2000, 01, 01), parent.Id, child2.Id, 0.80M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2001, 01, 01), parent.Id, child2.Id, 0.70M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2002, 01, 01), parent.Id, child2.Id, 0.60M));
                unitOfWork.RelativeNTARepository.Add(new RelativeNTA(new DateTime(2003, 01, 01), parent.Id, child2.Id, 0.50M));
                nta2 = new RelativeNTA(new DateTime(2004, 01, 01), parent.Id, child2.Id, 0.40M);
                unitOfWork.RelativeNTARepository.Add(nta2);


                nta3 = unitOfWork.RelativeNTARepository.Get(nta1.Id);
                nta4 = unitOfWork.RelativeNTARepository.Get(nta2.Id);

                Assert.That(nta3.Parent, Is.EqualTo(parent.Id));
                Assert.That(nta3.Child, Is.EqualTo(child1.Id));
                Assert.That(nta3.Percentage, Is.EqualTo(0.30M));

                Assert.That(nta4.Parent, Is.EqualTo(parent.Id));
                Assert.That(nta4.Child, Is.EqualTo(child2.Id));
                Assert.That(nta4.Percentage, Is.EqualTo(0.40M));

                unitOfWork.RelativeNTARepository.Delete(child1.ParentId, child1.Id, nta1.Date);

                nta3 = unitOfWork.RelativeNTARepository.Get(nta1.Id);
            } 
        }

    }
}
