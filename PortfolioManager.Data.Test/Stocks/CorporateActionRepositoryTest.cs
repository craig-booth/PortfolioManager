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
    public class CorporateActionRepositoryTestBase : TestBase
    {
        protected Stock _Stock;
        protected Stock _ResultStock1, _ResultStock2, _ResultStock3;
        protected IStockDatabase _Database;

        [TestFixtureSetUp]
        public void Init()
        {
            _Database = CreateStockDatabase();
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                _Stock = new Stock(new DateTime(2000, 01, 01), "ABC", "Test", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(_Stock);
                _ResultStock1 = new Stock(new DateTime(2000, 01, 01), "DEF", "Result 1", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(_ResultStock1);
                _ResultStock2 = new Stock(new DateTime(2000, 01, 01), "GHI", "Result 2", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(_ResultStock2);
                _ResultStock3 = new Stock(new DateTime(2000, 01, 01), "JKL", "Result 3", StockType.StapledSecurity, Guid.Empty);
                unitOfWork.StockRepository.Add(_ResultStock3);
                unitOfWork.Save();
            }
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
        }

        protected CorporateAction AddCorporateAction(CorporateAction corporateAction)
        {
            CorporateAction result;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Add(corporateAction);

                result = unitOfWork.CorporateActionRepository.Get(corporateAction.Id);
            }

            return result;
        }

        protected void UpdateCorporateAction(CorporateAction corporateAction)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Update(corporateAction);;
            }
        }

        protected CorporateAction GetCorporateAction(Guid id)
        {
            CorporateAction result;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                result = unitOfWork.CorporateActionRepository.Get(id);
            }

            return result;
        }

        [Test, Description("Test Get() for a corporate action that doesn't exist")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void GetNotExists()
        {
            CorporateAction corporateAction;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                corporateAction = unitOfWork.CorporateActionRepository.Get(Guid.NewGuid());
            }
        }

    }

}
