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
    public class CorporateActionRepositoryCapitalReturnTest : CorporateActionRepositoryTestBase
    {
        [Test, Description("Test adding a Capital Return")]
        public void AddCapitalReturn()
        {
            CapitalReturn capitalReturn, result;

            capitalReturn = new CapitalReturn(_Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 1.50M, "");
            result = AddCorporateAction(capitalReturn) as CapitalReturn;

            Assert.That(result, EntityConstraint.EqualTo(capitalReturn));
        }

        [Test, Description("Test Update() for a capital return")]
        public void UpdateCapitalReturn()
        {
            CapitalReturn capitalReturn, capitalReturn2;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                capitalReturn = new CapitalReturn(_Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 1.50M, "");
                unitOfWork.CorporateActionRepository.Add(capitalReturn);

                capitalReturn.Amount = 1.20m;
                unitOfWork.CorporateActionRepository.Update(capitalReturn);
     
                capitalReturn2 = unitOfWork.CorporateActionRepository.Get(capitalReturn.Id) as CapitalReturn;
                Assert.AreEqual(capitalReturn2.Amount, 1.20M);
            }
        }

        [Test, Description("Test Delete() for a capital return")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteCapitalReturn()
        {
            CapitalReturn capitalReturn;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                capitalReturn = new CapitalReturn(_Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 1.50M, "");
                unitOfWork.CorporateActionRepository.Add(capitalReturn);

                unitOfWork.CorporateActionRepository.Delete(capitalReturn);

                unitOfWork.CorporateActionRepository.Get(capitalReturn.Id);
            }
        }
    }
}
