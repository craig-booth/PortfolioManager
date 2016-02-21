using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Stocks
{
    public class CorporateActionService
    {
        private IStockDatabase _Database;

        public CorporateActionService(IStockDatabase database)
        {
            _Database = database;
        }

        public IReadOnlyCollection<ICorporateAction> GetCorporateActions(Stock stock)
        {
            return _Database.CorporateActionQuery.Find(stock.Id, DateTimeConstants.NoStartDate, DateTimeConstants.NoEndDate);
        }

        public IReadOnlyCollection<ICorporateAction> GetCorporateActions(Stock stock, DateTime fromDate, DateTime toDate)
        {
            return _Database.CorporateActionQuery.Find(stock.Id, fromDate, toDate);
        }

        public void AddCorporateAction(ICorporateAction corporateAction)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Add(corporateAction);
                unitOfWork.Save();
            }
        }

        public void UpdateCorporateAction(ICorporateAction corporateAction)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Update(corporateAction);
                unitOfWork.Save();
            }
        }

        public void DeleteCorporateAction(ICorporateAction corporateAction)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Delete(corporateAction);
                unitOfWork.Save();
            }
        }

        public void DeleteCorporateAction(Guid id)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Delete(id);
                unitOfWork.Save();
            }
        }


    }
}
