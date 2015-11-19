using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Portfolios
{
    class ParcelService
    {
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly StockService _StockService;

        public ParcelService(IPortfolioQuery portfolioQuery, StockService stockService)
        {
            _PortfolioQuery = portfolioQuery;
            _StockService = stockService;
        }

        public IReadOnlyCollection<ShareParcel> GetParcels(Stock stock, DateTime date)
        {
            return _PortfolioQuery.GetParcelsForStock(Guid.Empty, stock.Id, date);
        }

        public IReadOnlyCollection<ShareParcel> GetStapledSecurityParcels(Stock stock, DateTime date)
        {
            var stapledParcels = new List<ShareParcel>();

            var childStocks = _StockService.GetChildStocks(stock, date);

            foreach (var childStock in childStocks)
            {
                var childParcels = GetParcels(childStock, date);

                foreach (var childParcel in childParcels)
                {
                    var stapledParcel = stapledParcels.FirstOrDefault(x => x.PurchaseId == childParcel.PurchaseId);
                    if (stapledParcel == null)
                    {
                        stapledParcel = new ShareParcel(childParcel.AquisitionDate, stock.Id, childParcel.Units, childParcel.UnitPrice, childParcel.Amount, childParcel.CostBase, childParcel.PurchaseId, ParcelEvent.OpeningBalance);
                        stapledParcels.Add(stapledParcel);
                    }
                    else
                    {
                        stapledParcel.Amount += childParcel.Amount;
                        stapledParcel.CostBase += childParcel.CostBase;
                        stapledParcel.UnitPrice += childParcel.UnitPrice;
                    }

                }
            }

            return stapledParcels;
        }


        public ShareParcel GetParcel(Guid id, DateTime date)
        {
            return _PortfolioQuery.GetParcel(id, date);
        }

        public void AddParcel(IPortfolioUnitOfWork unitOfWork, DateTime aquisitionDate, Stock stock, int units, decimal unitPrice, decimal amount, decimal costBase, ParcelEvent parcelEvent)
        {
            /* Handle Stapled Securities */
            if (stock.Type == StockType.StapledSecurity)
            {
                /* Get child stocks */
                var childStocks = stock.GetChildStocks(aquisitionDate);

                /* Apportion amount and cost base */
                ApportionedValue[] apportionedAmounts = new ApportionedValue[childStocks.Count];
                ApportionedValue[] apportionedCostBases = new ApportionedValue[childStocks.Count];
                ApportionedValue[] apportionedUnitPrices = new ApportionedValue[childStocks.Count];
                int i = 0;
                foreach (Stock childStock in childStocks)
                {
                    decimal percentageOfParent = childStock.PercentageOfParentCostBase(aquisitionDate);
                    int relativeValue = (int)(percentageOfParent * 10000);

                    apportionedAmounts[i].Units = relativeValue;
                    apportionedCostBases[i].Units = relativeValue;
                    apportionedUnitPrices[i].Units = relativeValue;
                    i++;
                }
                MathUtils.ApportionAmount(amount, apportionedAmounts);
                MathUtils.ApportionAmount(costBase, apportionedCostBases);
                MathUtils.ApportionAmount(unitPrice, apportionedUnitPrices);

                i = 0;
                var purchaseId = Guid.NewGuid();
                foreach (Stock childStock in childStocks)
                {
                    var childParcel = new ShareParcel(aquisitionDate, childStock.Id, units, apportionedUnitPrices[i].Amount, apportionedAmounts[i].Amount, apportionedCostBases[i].Amount, purchaseId, parcelEvent);

                    unitOfWork.ParcelRepository.Add(childParcel);

                    i++;
                }
            }
            else
            {
                var parcel = new ShareParcel(aquisitionDate, stock.Id, units, unitPrice, amount, costBase, parcelEvent);
                unitOfWork.ParcelRepository.Add(parcel);
            }
        }

        public void ModifyParcel(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime changeDate, ParcelEvent parcelEvent, int newUnits, decimal newCostBase, string description)
        {

            // Check that this is the latest version of this parcel
            if (parcel.ToDate != DateTimeConstants.NoEndDate())
                throw new AttemptToModifyPreviousParcelVersion(parcel.Id, "");

            if (parcel.FromDate == changeDate)
                throw new Exception("Parcel already modified today !!!");

            /* Update old effective dated record */
            parcel.ToDate = changeDate.AddDays(-1);
            unitOfWork.ParcelRepository.Update(parcel);

            /* Add new record */
            if (newUnits > 0)
            {
                var newParcel = parcel.Clone();
                newParcel.FromDate = changeDate;
                newParcel.ToDate = DateTimeConstants.NoEndDate();
                newParcel.Event = parcelEvent;
                newParcel.Units = newUnits;
                newParcel.CostBase = newCostBase;
                unitOfWork.ParcelRepository.Add(newParcel);
            }
        }

        public void DisposeOfParcel(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime disposalDate, int units, decimal amountReceived, string description)
        {
            decimal costBase;
            CGTEvent cgtEvent;

            /* Modify Parcel */
            costBase = parcel.CostBase * ((decimal)units / parcel.Units);
            ModifyParcel(unitOfWork, parcel, disposalDate, ParcelEvent.Disposal, parcel.Units - units, parcel.CostBase - costBase, description);

            /* Record CGT Event */
            cgtEvent = new CGTEvent(parcel.Stock, disposalDate, units, costBase, amountReceived);
            unitOfWork.CGTEventRepository.Add(cgtEvent);
        }
    }
}
