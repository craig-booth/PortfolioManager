using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

using PortfolioManager.Service.Utils;
using PortfolioManager.Service.Interface;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.Local
{
    class CapitalGainService : ICapitalGainService
    {
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly Obsolete.TransactionService _TransactionService;
        private readonly StockService _StockService;
        private readonly CGTService _CGTService;

        public CapitalGainService(IPortfolioQuery portfolioQuery, StockService stockService, Obsolete.TransactionService transactionService, CGTService cgtService)
        {
            _PortfolioQuery = portfolioQuery;
            _TransactionService = transactionService;
            _StockService = stockService;
            _CGTService = cgtService;
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
                    Stock = new StockItem(currentStock.Id, currentStock.ASXCode, currentStock.Name),
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

            responce.CGTItems.OrderBy(x => x.Stock.Name).ThenBy(x => x.AquisitionDate);

            responce.SetStatusToSuccessfull();

            return responce;
        }


        public Task<DetailedUnrealisedGainsResponce> GetDetailedUnrealisedGains(DateTime date)
        {
            var parcels = _PortfolioQuery.GetAllParcels(date, date);

            var responce = GetDetailedCGT(parcels, date);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<DetailedUnrealisedGainsResponce>(responce);
        }

        public Task<DetailedUnrealisedGainsResponce> GetDetailedUnrealisedGains(Guid stockId, DateTime date)
        {
            var parcels = _PortfolioQuery.GetParcelsForStock(stockId, date, date);
            var responce = GetDetailedCGT(parcels, date);

            responce.SetStatusToSuccessfull();

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
                    Stock = new StockItem(currentStock.Id, currentStock.ASXCode, currentStock.Name),
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

            responce.CGTItems.OrderBy(x => x.Stock.Name).ThenBy(x => x.AquisitionDate);

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

        public Task<CGTLiabilityResponce> GetCGTLiability(DateTime fromDate, DateTime toDate)
        {
            var responce = new CGTLiabilityResponce();

            responce.CurrentYearCapitalGainsOther = 0.00m;
            responce.CurrentYearCapitalGainsDiscounted = 0.00m;
            responce.CurrentYearCapitalLossesTotal = 0.00m;

            // Get a list of all the cgt events for the year
            var cgtEvents = _CGTService.GetEvents(fromDate, toDate);
            foreach (var cgtEvent in cgtEvents)
            {
                var stock = _StockService.Get(cgtEvent.Stock, cgtEvent.EventDate);

                var item = new CGTLiabilityItem()
                {
                    Stock = new StockItem(stock.Id, stock.ASXCode, stock.Name),
                    EventDate = cgtEvent.EventDate,
                    CostBase = cgtEvent.CostBase,
                    AmountReceived = cgtEvent.AmountReceived,
                    CapitalGain = cgtEvent.CapitalGain,
                    Method = cgtEvent.CGTMethod
                };
                responce.Items.Add(item);

                // Apportion capital gains
                if (cgtEvent.CapitalGain < 0)
                    responce.CurrentYearCapitalLossesTotal += -cgtEvent.CapitalGain;
                else if (cgtEvent.CGTMethod == CGTMethod.Discount)
                    responce.CurrentYearCapitalGainsDiscounted += cgtEvent.CapitalGain;
                else
                    responce.CurrentYearCapitalGainsOther += cgtEvent.CapitalGain;                
            }


            responce.CurrentYearCapitalGainsTotal = responce.CurrentYearCapitalGainsOther + responce.CurrentYearCapitalGainsDiscounted;

            if (responce.CurrentYearCapitalGainsOther > responce.CurrentYearCapitalLossesTotal)
                responce.CurrentYearCapitalLossesOther = responce.CurrentYearCapitalLossesTotal;
            else
                responce.CurrentYearCapitalLossesOther = responce.CurrentYearCapitalGainsOther;

            if (responce.CurrentYearCapitalGainsOther > responce.CurrentYearCapitalLossesTotal)
                responce.CurrentYearCapitalLossesDiscounted = 0.00m;
            else
                responce.CurrentYearCapitalLossesDiscounted = responce.CurrentYearCapitalLossesTotal - responce.CurrentYearCapitalGainsOther;

            responce.GrossCapitalGainOther = responce.CurrentYearCapitalGainsOther - responce.CurrentYearCapitalLossesOther;
            responce.GrossCapitalGainDiscounted = responce.CurrentYearCapitalGainsDiscounted - responce.CurrentYearCapitalLossesDiscounted;
            responce.GrossCapitalGainTotal = responce.GrossCapitalGainOther + responce.GrossCapitalGainDiscounted;
            if (responce.GrossCapitalGainDiscounted > 0)
                responce.Discount = (responce.GrossCapitalGainDiscounted / 2).ToCurrency(RoundingRule.Round);
            else
                responce.Discount = 0.00m;
            responce.NetCapitalGainOther = responce.GrossCapitalGainOther;
            responce.NetCapitalGainDiscounted = responce.GrossCapitalGainDiscounted - responce.Discount;
            responce.NetCapitalGainTotal = responce.NetCapitalGainOther + responce.NetCapitalGainDiscounted;

            responce.SetStatusToSuccessfull();

            return Task.FromResult<CGTLiabilityResponce>(responce);
        }

    }


}
