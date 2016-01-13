using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;
namespace PortfolioManager.Model.Stocks
{
    public class SplitConsolidation : ICorporateAction
    {
        private IStockDatabase _Database;

        public Guid Id { get; private set; }
        public Guid Stock { get; private set; }
        public DateTime ActionDate { get; private set; }
        public int OldUnits { get; private set; }
        public int NewUnits { get; private set; }
        public string Description { get; private set; }

        public CorporateActionType Type
        {
            get
            {
                return CorporateActionType.SplitConsolidation;
            }
        }

        public SplitConsolidation(IStockDatabase stockDatabase, Guid stock, DateTime actionDate, int oldUnits, int newUnits, string description)
            : this(stockDatabase, Guid.NewGuid(), stock, actionDate, oldUnits, newUnits, description)
        {
        }

        public SplitConsolidation(IStockDatabase stockDatabase, Guid id, Guid stock, DateTime actionDate, int oldUnits, int newUnits, string description)
        {
            _Database = stockDatabase;
            Id = id;
            ActionDate = actionDate;
            Stock = stock;
            OldUnits = oldUnits;
            NewUnits = newUnits;
            if (description != "")
                Description = description;
            else
            {
                if (NewUnits > OldUnits)
                    Description = string.Format("Stock split ratio {0}:{1}", OldUnits, NewUnits);
                else
                    Description = string.Format("Stock consolidation ratio {0}:{1}", OldUnits, NewUnits);
            }
        }

        public void Change(DateTime newActionDate, int newOldUnits, int newNewUnits, string newDescription)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                ActionDate = newActionDate;
                OldUnits = newOldUnits;
                NewUnits = newNewUnits;
                Description = newDescription;

                unitOfWork.CorporateActionRepository.Update(this);

                unitOfWork.Save();
            }
        }

    }
}

