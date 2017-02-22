using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Service.Utils;

using PortfolioManager.Service.Interface;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.Local
{
    class CapitalGainService : ICapitalGainService
    {
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly TransactionService _TransactionService;
        private readonly StockService _StockService;

        public CapitalGainService(IPortfolioQuery portfolioQuery, StockService stockService, TransactionService transactionService)
        {
            _PortfolioQuery = portfolioQuery;
            _TransactionService = transactionService;
            _StockService = stockService;
        }

        public Task<SimpleUnrealisedGainsResponce> GetSimpleUnrealisedGains(DateTime date)
        {
            var parcels = _PortfolioQuery.GetAllParcels(date, date);

            var responce = GetSimpleCGT(parcels, date);
            return Task.FromResult<SimpleUnrealisedGainsResponce>(responce);
        }

        public Task<SimpleUnrealisedGainsResponce> GetSimpleUnrealisedGains(Guid stockId, DateTime date)
        {
            var parcels = _PortfolioQuery.GetParcelsForStock(stockId, date, date);

            var responce = GetSimpleCGT(parcels, date);
            return Task.FromResult<SimpleUnrealisedGainsResponce>(responce);
        }

        private SimpleUnrealisedGainsResponce GetSimpleCGT(IEnumerable<ShareParcel> parcels, DateTime date)
        {
            var responce = new SimpleUnrealisedGainsResponce();

            Stock currentStock = null;
            Guid previousStock = Guid.Empty;
            decimal unitPrice = 0.00m;

            foreach (var parcel in parcels)
            {
                if (parcel.Stock != previousStock)
                {
                    currentStock = _StockService.Get(parcel.Stock, date);
                    unitPrice = _StockService.GetPrice(currentStock, date);

                    previousStock = parcel.Stock;
                }

                var item = new SimpleUnrealisedGainsItem()
                {
                    Id = parcel.Id,
                    ASXCode = currentStock.ASXCode,
                    CompanyName = currentStock.Name,
                    AquisitionDate = parcel.AquisitionDate,
                    Units = parcel.Units,
                    CostBase = parcel.CostBase,
                    MarketValue = parcel.Units * unitPrice,
                    CapitalGain = parcel.Units * unitPrice - parcel.CostBase,
                    DiscoutedGain = 0.00m
                };
                item.DiscountMethod = CGTCalculator.CGTMethodForParcel(parcel, date);
                if (item.DiscountMethod == CGTMethod.Discount)
                {
                    item.DiscoutedGain = CGTCalculator.CGTDiscount(item.CapitalGain);
                }

                responce.CGTItems.Add(item);
            }

            responce.CGTItems.OrderBy(x => x.CompanyName).ThenBy(x => x.AquisitionDate);

            return responce;
        }


        public Task<DetailedUnrealisedGainsResponce> GetDetailedUnrealisedGains(DateTime date)
        {
            var parcels = _PortfolioQuery.GetAllParcels(date, date);

            var responce = GetDetailedCGT(parcels, date);
            return Task.FromResult<DetailedUnrealisedGainsResponce>(responce);
        }

        public Task<DetailedUnrealisedGainsResponce> GetDetailedUnrealisedGains(Guid stockId, DateTime date)
        {
            var parcels = _PortfolioQuery.GetParcelsForStock(stockId, date, date);
            var responce = GetDetailedCGT(parcels, date);
            return Task.FromResult<DetailedUnrealisedGainsResponce>(responce);
        }

        private DetailedUnrealisedGainsResponce GetDetailedCGT(IEnumerable<ShareParcel> parcels, DateTime date)
        {
            var responce = new DetailedUnrealisedGainsResponce();

            Stock currentStock = null;
            Guid previousStock = Guid.Empty;
            decimal unitPrice = 0.00m;

            foreach (var parcel in parcels)
            {
                if (parcel.Stock != previousStock)
                {
                    currentStock = _StockService.Get(parcel.Stock, date);
                    unitPrice = _StockService.GetPrice(currentStock, date);

                    previousStock = parcel.Stock;
                }

                var item = new DetailedUnrealisedGainsItem()
                {
                    Id = parcel.Id,
                    ASXCode = currentStock.ASXCode,
                    CompanyName = currentStock.Name,
                    AquisitionDate = parcel.AquisitionDate,
                    Units = parcel.Units,
                    CostBase = parcel.CostBase,
                    MarketValue = parcel.Units * unitPrice,
                    CapitalGain = parcel.Units * unitPrice - parcel.CostBase,
                    DiscoutedGain = 0.00m
                };
                item.DiscountMethod = CGTCalculator.CGTMethodForParcel(parcel, date);
                if (item.DiscountMethod == CGTMethod.Discount)
                {
                    item.DiscoutedGain = CGTCalculator.CGTDiscount(item.CapitalGain);
                }

                AddParcelHistory(item, date);

                responce.CGTItems.Add(item);
            }

            responce.CGTItems.OrderBy(x => x.CompanyName).ThenBy(x => x.AquisitionDate);

            return responce;
        }

        private void AddParcelHistory(DetailedUnrealisedGainsItem parcelItem, DateTime date)
        {
            var parcelAudit = _PortfolioQuery.GetParcelAudit(parcelItem.Id, parcelItem.AquisitionDate, date);

            decimal costBase = 0.00m;
            foreach (var auditRecord in parcelAudit)
            {
                costBase += auditRecord.CostBaseChange;

                var transaction = _TransactionService.GetTransaction(auditRecord.Transaction);

                var cgtEvent = new CGTEventItem()
                {
                    TransactionType = transaction.Type,
                    Date = auditRecord.Date,
                    Units = auditRecord.UnitCount,
                    Amount = auditRecord.CostBaseChange,
                    CostBase = costBase,
                    Comment = transaction.Description
                };

                parcelItem.CGTEvents.Add(cgtEvent);
            }

        }

        public Task<CGTResponce> GetCGTLiability(DateTime fromDate, DateTime toDate)
        {
            var responce = new CGTResponce();

            return Task.FromResult<CGTResponce>(responce);
        }

    }
}
