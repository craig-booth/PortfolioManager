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
    public class CorporateActionRepositoryTest : TestBase
    {
        private Stock _Stock;
        private Stock _ResultStock1, _ResultStock2, _ResultStock3;
        private IStockDatabase _Database;

        [TestFixtureSetUp]
        public void Init()
        {
            _Database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                _Stock = new Stock(_Database, new DateTime(2000, 01, 01), "ABC", "Test", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(_Stock);
                _ResultStock1 = new Stock(_Database, new DateTime(2000, 01, 01), "DEF", "Result 1", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(_ResultStock1);
                _ResultStock2 = new Stock(_Database, new DateTime(2000, 01, 01), "GHI", "Result 2", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(_ResultStock2);
                _ResultStock3 = new Stock(_Database, new DateTime(2000, 01, 01), "JKL", "Result 3", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(_ResultStock3);
                unitOfWork.Save();
            }
        }

        [TestFixtureTearDown]
        public void Cleanup()
        { 
        }

        private ICorporateAction AddCorporateAction(ICorporateAction corporateAction)
        {
            ICorporateAction result;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Add(corporateAction);

                result = unitOfWork.CorporateActionRepository.Get(corporateAction.Id);
            }

            return result;
        }

        [Test, Description("Test adding a Dividend")]
        public void AddDividend()
        {
            Dividend dividend, result;

            dividend = new Dividend(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 1.00M, 1.00M, 0.30M, 0.00M, "");
            result = AddCorporateAction(dividend) as Dividend;

            Assert.That(result, Is.EqualTo(dividend).Using(new EntityComparer()));
        }

        [Test, Description("Test adding a Capital Return")]
        public void AddCapitalReturn()
        {
            CapitalReturn capitalReturn, result;

            capitalReturn = new CapitalReturn(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 1.50M, "");
            result = AddCorporateAction(capitalReturn) as CapitalReturn;

            Assert.That(result, Is.EqualTo(capitalReturn).Using(new EntityComparer()));
        }

        [Test, Description("Test adding a Transformation")]
        public void AddTransformation()
        {
            Transformation transformation, result;

            transformation = new Transformation(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, "Test");
            result = AddCorporateAction(transformation) as Transformation;
            Assert.That(result, Is.EqualTo(transformation).Using(new EntityComparer()));
        }

        [Test, Description("Test Get() for a corporate action that doesn't exist")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void GetNotExists()
        {
            ICorporateAction corporateAction;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                corporateAction = unitOfWork.CorporateActionRepository.Get(Guid.NewGuid());
            }
        }

        [Test, Description("Test Update() for a corporate action (header file)")]
        public void UpdateCorporateAction()
        {
            Dividend dividend, dividend2;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                dividend = new Dividend(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 1.00M, 1.00M, 0.30M, 0.00M, "");
                unitOfWork.CorporateActionRepository.Add(dividend);

                dividend.Change(new DateTime(2005, 10, 12), dividend.PaymentDate, dividend.DividendAmount);

                dividend2 = unitOfWork.CorporateActionRepository.Get(dividend.Id) as Dividend;
                Assert.AreEqual(dividend2.ActionDate, new DateTime(2005, 10, 12));
            } 
        }

        [Test, Description("Test Update() for a dividend")]
        public void UpdateDividend()
        {
            Dividend dividend, dividend2;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                dividend = new Dividend(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 1.00M, 1.00M, 0.30M, 0.00M, "");
                unitOfWork.CorporateActionRepository.Add(dividend);

                dividend.Change(dividend.ActionDate, dividend.PaymentDate, 1.20M);

                dividend2 = unitOfWork.CorporateActionRepository.Get(dividend.Id) as Dividend;
                Assert.AreEqual(dividend2.DividendAmount, 1.20M);
            }       
        }

        [Test, Description("Test Update() for a capital return")]
        public void UpdateCapitalReturn()
        {
            CapitalReturn capitalReturn, capitalReturn2;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                capitalReturn = new CapitalReturn(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 1.50M, "");
                unitOfWork.CorporateActionRepository.Add(capitalReturn);

                capitalReturn.Change(capitalReturn.ActionDate, capitalReturn.PaymentDate, 1.20M, capitalReturn.Description);

                capitalReturn2 = unitOfWork.CorporateActionRepository.Get(capitalReturn.Id) as CapitalReturn;
                Assert.AreEqual(capitalReturn2.Amount, 1.20M);
            }  
        }

        [Test, Description("Test Update() for a transformation")]
        public void UpdateTransformation()
        {
            Transformation transformation, transformation2;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                transformation = new Transformation(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, "Test");
                unitOfWork.CorporateActionRepository.Add(transformation);

                transformation.Change(transformation.ActionDate, transformation.ImplementationDate, 3.00M, transformation.Description);

                transformation2 = unitOfWork.CorporateActionRepository.Get(transformation.Id) as Transformation;
                Assert.AreEqual(transformation2.CashComponent, 3.00M);
            }  
        }

        [Test, Description("Test Update() for a transformation - add result stock")]
        public void UpdateTransformationAddResultStock()
        {
            Transformation transformation, transformation2;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                transformation = new Transformation(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, "Test");
                unitOfWork.CorporateActionRepository.Add(transformation);

                transformation.AddResultStock(_ResultStock1.Id, 1, 2, 0.40M);
                transformation.AddResultStock(_ResultStock2.Id, 1, 5, 0.80M);              
                transformation.AddResultStock(_ResultStock3.Id, 2, 7, 1.40M);

                transformation2 = unitOfWork.CorporateActionRepository.Get(transformation.Id) as Transformation;
                Assert.AreEqual(transformation2.ResultingStocks.Count, 3);
                Assert.AreEqual(transformation2.ResultingStocks.Find(x => x.Stock == _ResultStock2.Id).NewUnits, 5);
            }
        }

        [Test, Description("Test Update() for a transformation - update result stock")]
        public void UpdateTransformationUpdateResultStock()
        {
            Transformation transformation, transformation2;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                transformation = new Transformation(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, "Test");
                transformation.AddResultStock(_ResultStock1.Id, 1, 2, 0.40M);
                transformation.AddResultStock(_ResultStock2.Id, 1, 5, 0.60M);
                unitOfWork.CorporateActionRepository.Add(transformation);

                transformation.ChangeResultStock(_ResultStock1.Id, 1, 7, 0.40M);

                transformation2 = unitOfWork.CorporateActionRepository.Get(transformation.Id) as Transformation;
                Assert.AreEqual(transformation2.ResultingStocks.Find(x => x.Stock == _ResultStock1.Id).NewUnits, 7);
            }
        }

        [Test, Description("Test Update() for a transformation - remove result stock")]
        public void UpdateTransformationRemoveResultStock()
        {
            Transformation transformation, transformation2;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                transformation = new Transformation(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, "Test");
                transformation.AddResultStock(_ResultStock1.Id, 1, 2, 0.40M);
                transformation.AddResultStock(_ResultStock2.Id, 1, 5, 0.80M);
                unitOfWork.CorporateActionRepository.Add(transformation);

                transformation.ResultingStocks.RemoveAt(1);
                unitOfWork.CorporateActionRepository.Update(transformation);

                transformation2 = unitOfWork.CorporateActionRepository.Get(transformation.Id) as Transformation;
                Assert.AreEqual(transformation2.ResultingStocks.Count, 1);
                Assert.AreEqual(transformation2.ResultingStocks[0].NewUnits, 2);
            }
        }

        [Test, Description("Test Delete() for a dividend")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteDividend()
        {
            Dividend dividend;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                dividend = new Dividend(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 1.00M, 1.00M, 0.30M, 0.00M, "");
                unitOfWork.CorporateActionRepository.Add(dividend);

                unitOfWork.CorporateActionRepository.Delete(dividend);

                unitOfWork.CorporateActionRepository.Get(dividend.Id);
            }  
        }

        [Test, Description("Test Delete() for a capital return")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteCapitalReturn()
        {
            CapitalReturn capitalReturn;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                capitalReturn = new CapitalReturn(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 1.50M, "");
                unitOfWork.CorporateActionRepository.Add(capitalReturn);

                unitOfWork.CorporateActionRepository.Delete(capitalReturn);

                unitOfWork.CorporateActionRepository.Get(capitalReturn.Id);
            } 
        }

        [Test, Description("Test Delete() for a transformation")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteTransformation()
        {
            Transformation transformation;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                transformation = new Transformation(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, "Test");
                unitOfWork.CorporateActionRepository.Add(transformation);

                unitOfWork.CorporateActionRepository.Delete(transformation);

                unitOfWork.CorporateActionRepository.Get(transformation.Id);
            }  
        }

        [Test, Description("Test Delete() for a transformation with result stocks")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteTransformationWithResultStocks()
        {
            Transformation transformation;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                transformation = new Transformation(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, "Test");
                unitOfWork.CorporateActionRepository.Add(transformation);

                transformation.AddResultStock(_ResultStock1.Id, 1, 2, 0.40M);
                transformation.AddResultStock(_ResultStock2.Id, 1, 5, 0.80M);              

                unitOfWork.CorporateActionRepository.Delete(transformation);

                unitOfWork.CorporateActionRepository.Get(transformation.Id);
            }
        }
    }

}
