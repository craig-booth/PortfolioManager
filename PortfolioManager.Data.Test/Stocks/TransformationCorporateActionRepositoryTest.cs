using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using NUnitExtension;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.Test.Stocks
{

    [TestFixture]
    public class TransformationCorporateActionRepositoryTest : CorporateActionRepositoryTestBase
    {
        [Test, Description("Test adding a Transformation")]
        public void AddTransformation()
        {
            Transformation transformation, result;

            transformation = new Transformation(_Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, true, "Test");
            result = AddCorporateAction(transformation) as Transformation;

            Assert.That(result, EntityConstraint.EqualTo((transformation)));
        }

        [Test, Description("Test Update() for a transformation")]
        public void UpdateTransformation()
        {
            Transformation transformation, transformation2;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                transformation = new Transformation(_Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, true, "Test");
                unitOfWork.CorporateActionRepository.Add(transformation);

                transformation.CashComponent = 3.00m;
                unitOfWork.CorporateActionRepository.Update(transformation);

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
                transformation = new Transformation(_Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, true, "Test");
                unitOfWork.CorporateActionRepository.Add(transformation);

                transformation.AddResultStock(_ResultStock1.Id, 1, 2, 0.40M);
                transformation.AddResultStock(_ResultStock2.Id, 1, 5, 0.80M);
                transformation.AddResultStock(_ResultStock3.Id, 2, 7, 1.40M);
                unitOfWork.CorporateActionRepository.Update(transformation);

                transformation2 = unitOfWork.CorporateActionRepository.Get(transformation.Id) as Transformation;
                Assert.AreEqual(transformation2.ResultingStocks.Count(), 3);
                Assert.AreEqual(transformation2.ResultingStocks.Where(x => x.Stock == _ResultStock2.Id).First().NewUnits, 5);
            }
        }

        [Test, Description("Test Update() for a transformation - update result stock")]
        public void UpdateTransformationUpdateResultStock()
        {
            Transformation transformation, transformation2;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                transformation = new Transformation(_Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, true, "Test");
                transformation.AddResultStock(_ResultStock1.Id, 1, 2, 0.40M);
                transformation.AddResultStock(_ResultStock2.Id, 1, 5, 0.60M);
                unitOfWork.CorporateActionRepository.Add(transformation);

                var resultStock = transformation.GetResultStock(_ResultStock1.Id);
                resultStock.OriginalUnits = 1;
                resultStock.NewUnits = 7;
                resultStock.CostBase = 0.40m;
                unitOfWork.CorporateActionRepository.Update(transformation);

                transformation2 = unitOfWork.CorporateActionRepository.Get(transformation.Id) as Transformation;
                Assert.AreEqual(transformation2.ResultingStocks.Where(x => x.Stock == _ResultStock1.Id).First().NewUnits, 7);
            }
        }

        [Test, Description("Test Update() for a transformation - remove result stock")]
        public void UpdateTransformationRemoveResultStock()
        {
            Transformation transformation, transformation2;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                transformation = new Transformation(_Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, true, "Test");
                transformation.AddResultStock(_ResultStock1.Id, 1, 2, 0.40M);
                transformation.AddResultStock(_ResultStock2.Id, 1, 5, 0.80M);
                unitOfWork.CorporateActionRepository.Add(transformation);

                transformation.DeleteResultStock(_ResultStock1.Id);
                unitOfWork.CorporateActionRepository.Update(transformation);

                transformation2 = unitOfWork.CorporateActionRepository.Get(transformation.Id) as Transformation;
                Assert.AreEqual(transformation2.ResultingStocks.Count(), 1);
                Assert.AreEqual(transformation2.ResultingStocks.First().NewUnits, 5);
            }
        }

        [Test, Description("Test Delete() for a transformation")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteTransformation()
        {
            Transformation transformation;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                transformation = new Transformation(_Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, true, "Test");
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
                transformation = new Transformation(_Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, true, "Test");
                unitOfWork.CorporateActionRepository.Add(transformation);

                transformation.AddResultStock(_ResultStock1.Id, 1, 2, 0.40M);
                transformation.AddResultStock(_ResultStock2.Id, 1, 5, 0.80M);

                unitOfWork.CorporateActionRepository.Delete(transformation);

                unitOfWork.CorporateActionRepository.Get(transformation.Id);
            }
        }


    }

}
