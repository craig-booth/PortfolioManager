using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;


namespace PortfolioManager.Model.Stocks
{

    public class RelativeNTA: IEntity
    {
        private IStockDatabase _Database;

        public Guid Id { get; private set;}
        public DateTime Date { get; private set; }
        public Guid Parent { get; private set; }
        public Guid Child { get; private set; }
        public decimal Percentage { get; private set; }

        public RelativeNTA(IStockDatabase stockDatabase, DateTime date, Guid parent, Guid child, decimal percentage)
            : this(stockDatabase, Guid.NewGuid(), date, parent, child, percentage)
        {
        }

        public RelativeNTA(IStockDatabase stockDatabase, Guid id, DateTime date, Guid parent, Guid child, decimal percentage)
        {
            _Database = stockDatabase;
            Id = id;
            Date = date;
            Parent = parent;
            Child = child;
            Percentage = percentage;
        }

        public void ChangePercentage(decimal newPercentage)
        {
            Percentage = newPercentage;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.RelativeNTARepository.Update(this);
                unitOfWork.Save();
            }
        }

    }


}
