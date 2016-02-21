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
    public class CorporateActionSplitConsolidationRepositoryTest : CorporateActionRepositoryTestBase
    {
        [Test, Description("Test adding a SplitConsolidation")]
        public void AddSplitConsolidation()
        {
            SplitConsolidation split, result;

            split = new SplitConsolidation(_Stock.Id, new DateTime(2005, 10, 10), 1, 2, "");
            result = AddCorporateAction(split) as SplitConsolidation;

            Assert.That(result, EntityConstraint.EqualTo((split)));
        }

        [Test, Description("Test Update() for a SplitConsolidation")]
        public void UpdateSplitConsolidation()
        {
            SplitConsolidation split, split2;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                split = new SplitConsolidation(_Stock.Id, new DateTime(2005, 10, 10), 1, 2, "");
                unitOfWork.CorporateActionRepository.Add(split);

                split.OldUnits = 2;
                split.NewUnits = 3;
                unitOfWork.CorporateActionRepository.Update(split);

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
                split = new SplitConsolidation(_Stock.Id, new DateTime(2005, 10, 10), 1, 2, "");
                unitOfWork.CorporateActionRepository.Add(split);

                unitOfWork.CorporateActionRepository.Delete(split);

                unitOfWork.CorporateActionRepository.Get(split.Id);
            }
        }

    }
}
