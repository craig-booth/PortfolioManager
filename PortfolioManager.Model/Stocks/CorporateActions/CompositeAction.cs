using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Stocks
{
    class CompositeAction : ICorporateAction
    {

        private IStockDatabase _Database;
        private List<ICorporateAction> _Children; 

        public Guid Id { get; private set; }
        public Guid Stock { get; private set; }
        public DateTime ActionDate { get; private set; }
        public IReadOnlyList<ICorporateAction> Children { get { return _Children; }  }
        public string Description { get; private set; }

        public CorporateActionType Type
        {
            get
            {
                return CorporateActionType.Composite;
            }
        }

        public CompositeAction(IStockDatabase stockDatabase, Guid stock, DateTime actionDate, string description)
            : this(stockDatabase, Guid.NewGuid(), stock, actionDate, description)
        {
        }

        public CompositeAction(IStockDatabase stockDatabase, Guid id, Guid stock, DateTime actionDate, string description)
        {
            _Database = stockDatabase;
            Id = id;
            ActionDate = actionDate;
            Stock = stock;
            Description = description;

            _Children = new List<ICorporateAction>();
        }

        public void AddChildAction(ICorporateAction childAction)
        {
            _Children.Add(childAction);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Update(this);

                unitOfWork.Save();
            }
        }

        public void RemoveChildAction(ICorporateAction childAction)
        {
            _Children.Remove(childAction);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Update(this);

                unitOfWork.Save();
            }
        }
    }
}
