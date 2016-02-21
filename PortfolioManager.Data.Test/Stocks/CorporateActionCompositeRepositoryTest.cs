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

            var result = AddCorporateAction(compositeAction) as CompositeAction;
            Assert.That(result, EntityConstraint.EqualTo((compositeAction)));
        }

        [Test, Description("Test Update() for a Composite Action")]
        public void UpdateCompositeAction()
        {
            CompositeAction compositeAction1, compositeAction2;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                var actionDate = new DateTime(2005, 10, 10);
                compositeAction1 = new CompositeAction(_Database, _Stock.Id, actionDate, "Test");

                unitOfWork.CorporateActionRepository.Add(compositeAction1);

                compositeAction1.Change(new DateTime(2005, 10, 15), "New Description");

                compositeAction2 = unitOfWork.CorporateActionRepository.Get(compositeAction1.Id) as CompositeAction;

                Assert.AreEqual(compositeAction2.ActionDate, new DateTime(2005, 10, 15));
                Assert.AreEqual(compositeAction2.Description, "New Description");
            } 
        }

        [Test, Description("Test adding child action")]
        public void UpdateCompositeActionAddChildAction()
        {
            var actionDate = new DateTime(2005, 10, 10);

            var compositeAction = new CompositeAction(_Database, _Stock.Id, actionDate, "Test");
            AddCorporateAction(compositeAction);

            var childAction1 = new CapitalReturn(_Stock.Id, actionDate, new DateTime(2005, 10, 15), 5.00m, "Test");
            compositeAction.AddChildAction(childAction1);
            var childAction2 = new Dividend(_Stock.Id, actionDate, new DateTime(2005, 11, 12), 0.45m, 100.00m, 30.00m, 0.00m, "Test");
            compositeAction.AddChildAction(childAction2);

            var result = GetCorporateAction(compositeAction.Id) as CompositeAction;

            Assert.That(result, EntityConstraint.EqualTo((compositeAction)));
        }

        [Test, Description("Test Update() updating child action")]
        public void UpdateCompositeActionUpdateChildAction()
        {
            throw new NotSupportedException();
            /*
            var actionDate = new DateTime(2005, 10, 10);

            var compositeAction = new CompositeAction(_Database, _Stock.Id, actionDate, "Test");
            AddCorporateAction(compositeAction);

            var childAction1 = new CapitalReturn(_Database, _Stock.Id, actionDate, new DateTime(2005, 10, 15), 5.00m, "Test");
            compositeAction.AddChildAction(childAction1);
            var childAction2 = new Dividend(_Database, _Stock.Id, actionDate, new DateTime(2005, 11, 12), 0.45m, 100.00m, 30.00m, 0.00m, "Test");
            compositeAction.AddChildAction(childAction2);

            var compositeAction2 = GetCorporateAction(compositeAction.Id) as CompositeAction;

            var capitalReturn = compositeAction2.Children[0] as CapitalReturn;
            capitalReturn.Change()


            Assert.That(result, EntityConstraint.EqualTo((compositeAction))); */
        }

        [Test, Description("Test Update() for a Composite Action - remove child action")]
        public void UpdateCompositeActionRemoveChildAction()
        {
            var actionDate = new DateTime(2005, 10, 10);

            var compositeAction = new CompositeAction(_Database, _Stock.Id, actionDate, "Test");
            AddCorporateAction(compositeAction);

            var childAction1 = new CapitalReturn(_Stock.Id, actionDate, new DateTime(2005, 10, 15), 5.00m, "Test");
            compositeAction.AddChildAction(childAction1);
            var childAction2 = new Dividend( _Stock.Id, actionDate, new DateTime(2005, 11, 12), 0.45m, 100.00m, 30.00m, 0.00m, "Test");
            compositeAction.AddChildAction(childAction2);

            var compositeAction2 = GetCorporateAction(compositeAction.Id) as CompositeAction;

            compositeAction2.RemoveChildAction(compositeAction2.Children[0]);

            var result = GetCorporateAction(compositeAction.Id) as CompositeAction;

            Assert.That(result.Children, Has.Count.EqualTo(1));
            Assert.That(result.Children[0], EntityConstraint.EqualTo(childAction2));

        }

        [Test, Description("Test Delete() for a Composite Action")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteCompositeAction()
        {
            var actionDate = new DateTime(2005, 10, 10);

            var compositeAction = new CompositeAction(_Database, _Stock.Id, actionDate, "Test");
            AddCorporateAction(compositeAction);

            var childAction1 = new CapitalReturn(_Stock.Id, actionDate, new DateTime(2005, 10, 15), 5.00m, "Test");
            compositeAction.AddChildAction(childAction1);
            var childAction2 = new Dividend(_Stock.Id, actionDate, new DateTime(2005, 11, 12), 0.45m, 100.00m, 30.00m, 0.00m, "Test");
            compositeAction.AddChildAction(childAction2);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Delete(compositeAction);

                unitOfWork.CorporateActionRepository.Get(compositeAction.Id);
            } 
        }

    }
}
