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
    public class CorporateActionRepositoryCompositeTest : CorporateActionRepositoryTestBase
    {
        [Test, Description("Test adding a Composite Action")]
        public void AddCompositeAction()
        {         
            var actionDate = new DateTime(2005, 10, 10);

            var compositeAction = new CompositeAction(_Database, _Stock.Id, actionDate, "Test");

            var childAction1 = new CapitalReturn(_Database, _Stock.Id, actionDate, new DateTime(2005, 10, 15), 5.00m, "test c1");
            compositeAction.AddChildAction(childAction1);
            var childAction2 = new Dividend(_Database, _Stock.Id, actionDate, new DateTime(2005, 11, 12), 0.45m, 100.00m, 30.00m, 0.00m, "test c2");
            compositeAction.AddChildAction(childAction2);

            var result = AddCorporateAction(compositeAction) as CompositeAction;
            Assert.That(result, EntityConstraint.EqualTo((compositeAction)));
        }

        [Test, Description("Test Update() for a Composite Action")]
        public void UpdateCompositeAction()
        {
            throw new NotSupportedException();
            /*            Transformation transformation, transformation2;

                        using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
                        {
                            transformation = new Transformation(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, "Test");
                            unitOfWork.CorporateActionRepository.Add(transformation);

                            transformation.Change(transformation.ActionDate, transformation.ImplementationDate, 3.00M, transformation.Description);

                            transformation2 = unitOfWork.CorporateActionRepository.Get(transformation.Id) as Transformation;
                            Assert.AreEqual(transformation2.CashComponent, 3.00M);
                        } */
        }

        [Test, Description("Test Update() for a Composite Action - add child action")]
        public void UpdateCompositeActionAddChildAction()
        {
            throw new NotSupportedException();
            /*         Transformation transformation, transformation2;

                     using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
                     {
                         transformation = new Transformation(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, "Test");
                         unitOfWork.CorporateActionRepository.Add(transformation);

                         transformation.AddResultStock(_ResultStock1.Id, 1, 2, 0.40M);
                         transformation.AddResultStock(_ResultStock2.Id, 1, 5, 0.80M);
                         transformation.AddResultStock(_ResultStock3.Id, 2, 7, 1.40M);

                         transformation2 = unitOfWork.CorporateActionRepository.Get(transformation.Id) as Transformation;
                         Assert.AreEqual(transformation2.ResultingStocks.Count(), 3);
                         Assert.AreEqual(transformation2.ResultingStocks.Where(x => x.Stock == _ResultStock2.Id).First().NewUnits, 5);
                     } */
        }

        [Test, Description("Test Update() for a Composite Action - update child action")]
        public void UpdateCompositeActionUpdateChildAction()
        {
            throw new NotSupportedException();
            /*          Transformation transformation, transformation2;

                      using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
                      {
                          transformation = new Transformation(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, "Test");
                          transformation.AddResultStock(_ResultStock1.Id, 1, 2, 0.40M);
                          transformation.AddResultStock(_ResultStock2.Id, 1, 5, 0.60M);
                          unitOfWork.CorporateActionRepository.Add(transformation);

                          transformation.ChangeResultStock(_ResultStock1.Id, 1, 7, 0.40M);

                          transformation2 = unitOfWork.CorporateActionRepository.Get(transformation.Id) as Transformation;
                          Assert.AreEqual(transformation2.ResultingStocks.Where(x => x.Stock == _ResultStock1.Id).First().NewUnits, 7);
                      } */
        }

        [Test, Description("Test Update() for a Composite Action - remove child action")]
        public void UpdateCompositeActionRemoveChildAction()
        {
            throw new NotSupportedException();
            /*            Transformation transformation, transformation2;

                        using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
                        {
                            transformation = new Transformation(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, "Test");
                            transformation.AddResultStock(_ResultStock1.Id, 1, 2, 0.40M);
                            transformation.AddResultStock(_ResultStock2.Id, 1, 5, 0.80M);
                            unitOfWork.CorporateActionRepository.Add(transformation);

                            transformation.DeleteResultStock(_ResultStock1.Id);
                            unitOfWork.CorporateActionRepository.Update(transformation);

                            transformation2 = unitOfWork.CorporateActionRepository.Get(transformation.Id) as Transformation;
                            Assert.AreEqual(transformation2.ResultingStocks.Count(), 1);
                            Assert.AreEqual(transformation2.ResultingStocks.First().NewUnits, 5);
                        } */
        }

        [Test, Description("Test Delete() for a Composite Action")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteCompositeAction()
        {
            throw new NotSupportedException();
            /*            Transformation transformation;

                        using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
                        {
                            transformation = new Transformation(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, "Test");
                            unitOfWork.CorporateActionRepository.Add(transformation);

                            unitOfWork.CorporateActionRepository.Delete(transformation);

                            unitOfWork.CorporateActionRepository.Get(transformation.Id);
                        } */
        }

        [Test, Description("Test Delete() for a Composite Action with child actions")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteCompositeActionWithChildAction()
        {
            throw new NotSupportedException();
            /*          Transformation transformation;

                      using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
                      {
                          transformation = new Transformation(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 2.50M, "Test");
                          unitOfWork.CorporateActionRepository.Add(transformation);

                          transformation.AddResultStock(_ResultStock1.Id, 1, 2, 0.40M);
                          transformation.AddResultStock(_ResultStock2.Id, 1, 5, 0.80M);

                          unitOfWork.CorporateActionRepository.Delete(transformation);

                          unitOfWork.CorporateActionRepository.Get(transformation.Id);
                      } */
        }
    }
}
