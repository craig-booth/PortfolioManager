using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public enum ParcelEvent { Aquisition, Disposal, OpeningBalance, CostBaseReduction};

    public class ShareParcel: IEffectiveDatedEntity 
    {
        private IPortfolioDatabase _Database;

        public Guid Id { get; private set; }
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }
        public Guid Stock { get; private set; }
        public DateTime AquisitionDate { get; private set; }
        public int Units { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Amount { get; private set; }
        public decimal CostBase { get; private set; }
        public ParcelEvent Event { get; private set; }
        public Guid Parent { get; set; }
        public bool IncludeInHoldings { get; set; }
        public bool IncludeInParcels { get; set; }

        private ShareParcel(IPortfolioDatabase database) 
        {
            _Database = database;

            Id = Guid.NewGuid();
            Parent = Guid.Empty;
            IncludeInHoldings = true;
            IncludeInParcels = true;
        }

        protected internal ShareParcel(IPortfolioDatabase database, DateTime aquisitionDate, Guid stock, int units, decimal unitPrice, decimal amount, decimal costBase, ParcelEvent parcelEvent)
            : this(database)
        {
            FromDate = aquisitionDate;
            ToDate = DateTimeConstants.NoEndDate();
            Stock = stock;
            AquisitionDate = aquisitionDate;
            Amount = amount;
            Units = units;
            UnitPrice = unitPrice;
            CostBase = costBase;
            Event = parcelEvent;
        }

        protected internal ShareParcel Copy(DateTime fromDate)
        {
            /* Create copy */
            var newParcel = new ShareParcel(_Database)
            {
                Id = Id,
                FromDate = fromDate,
                ToDate = DateTimeConstants.NoEndDate(),
                Stock = Stock,
                AquisitionDate = AquisitionDate,
                Amount = Amount,
                Units = Units,
                UnitPrice = UnitPrice,
                CostBase = CostBase,
                Event = Event
            };

            return newParcel;
        }

        protected internal void Modify(DateTime changeDate, ParcelEvent parcelEvent, int newUnits, decimal newCostBase, string description)
        {
            using (IPortfolioUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                /* Update old effective dated record */
                ToDate = changeDate.AddDays(-1);
                unitOfWork.ParcelRepository.Update(this);

                /* Add new record */
                if (newUnits > 0)
                {
                    var newParcel = Copy(changeDate);
                    newParcel.Id = this.Id;
                    newParcel.Event = parcelEvent;
                    newParcel.Units = newUnits;
                    newParcel.CostBase = newCostBase;
                    unitOfWork.ParcelRepository.Add(newParcel);
                }

                unitOfWork.Save();
            }
        }

        protected internal void CancelParcel(DateTime cancelDate)
        {
            using (IPortfolioUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                /* Update old effective dated record */
                ToDate = cancelDate;
                unitOfWork.ParcelRepository.Update(this);

                unitOfWork.Save();
            }
        }

        public IReadOnlyCollection<ShareParcel> GetChildParcels()
        {
            return _Database.PortfolioQuery.GetChildParcels(this.Id);
        }
    }

}
