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
    public class CorporateActionRepositoryDividendTest : CorporateActionRepositoryTestBase
    {
        [Test, Description("Test adding a Dividend")]
        public void AddDividend()
        {
            Dividend dividend, result;

            dividend = new Dividend(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 1.00M, 1.00M, 0.30M, 0.00M, "");
            result = AddCorporateAction(dividend) as Dividend;

            Assert.That(result, EntityConstraint.EqualTo((dividend)));
        }

        [Test, Description("Test Update() for a dividend")]
        public void UpdateDividend()
        {
            Dividend dividend, dividend2;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                dividend = new Dividend(_Database, _Stock.Id, new DateTime(2005, 10, 10), new DateTime(2005, 10, 12), 1.00M, 1.00M, 0.30M, 0.00M, "");
                unitOfWork.CorporateActionRepository.Add(dividend);

                dividend.Change(dividend.ActionDate, dividend.PaymentDate, 1.20M, "");

                dividend2 = unitOfWork.CorporateActionRepository.Get(dividend.Id) as Dividend;
                Assert.AreEqual(dividend2.DividendAmount, 1.20M);
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
    }

}
