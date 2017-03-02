using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace StockManager.Service
{
    public class CorporateActionService
    {
        private IStockDatabase _Database;

        public CorporateActionService(IStockDatabase database)
        {
            _Database = database;
        }

        public IReadOnlyCollection<CorporateAction> GetCorporateActions(Stock stock)
        {
            return _Database.CorporateActionQuery.Find(stock.Id, DateUtils.NoStartDate, DateUtils.NoEndDate);
        }

        public IReadOnlyCollection<CorporateAction> GetCorporateActions(Stock stock, DateTime fromDate, DateTime toDate)
        {
            return _Database.CorporateActionQuery.Find(stock.Id, fromDate, toDate);
        }

        public void AddCorporateAction(CorporateAction corporateAction)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Add(corporateAction);
                unitOfWork.Save();
            }
        }

        public void UpdateCorporateAction(CorporateAction corporateAction)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Update(corporateAction);
                unitOfWork.Save();
            }
        }

        public void DeleteCorporateAction(CorporateAction corporateAction)
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
