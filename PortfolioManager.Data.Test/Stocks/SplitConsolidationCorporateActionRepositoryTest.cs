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
    public class SplitConsolidationCorporateActionRepositoryTest : CorporateActionRepositoryTest
    {
        [Test, Description("Test adding a SplitConsolidation")]
        public void AddSplitConsolidation()
        {
            SplitConsolidation split, result;

            split = new SplitConsolidation(_Database, _Stock.Id, new DateTime(2005, 10, 10), 1, 2, "");
            result = AddCorporateAction(split) as SplitConsolidation;

            Assert.That(result, Is.EqualTo(split).Using(new EntityComparer())); 
        }

        [Test, Description("Test Update() for a SplitConsolidation")]
        public void UpdateSplitConsolidation()
        {
            SplitConsolidation split, split2;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                split = new SplitConsolidation(_Database, _Stock.Id, new DateTime(2005, 10, 10), 1, 2, "");
                unitOfWork.CorporateActionRepository.Add(split);

                split.Change(split.ActionDate, 2, 3, "");

                split2 = unitOfWork.CorporateActionRepository.Get(split.Id) as SplitConsolidation;
                Assert.AreEqual(split2.OldUnits, 2);
                Assert.AreEqual(split2.NewUnits, 3);
            } 
        }

        [Test, Description("Test Delete() for a SplitConsolidation")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteSplitConsolidation()
        {
            SplitConsolidation split;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                split = new SplitConsolidation(_Database, _Stock.Id, new DateTime(2005, 10, 10), 1, 2, "");
                unitOfWork.CorporateActionRepository.Add(split);

                unitOfWork.CorporateActionRepository.Delete(split);

                unitOfWork.CorporateActionRepository.Get(split.Id);
            }
        }

    }
}
