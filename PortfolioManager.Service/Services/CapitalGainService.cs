using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Utils;
using PortfolioManager.Service.Interface;

using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Service.Services
{
    public class CapitalGainService : ICapitalGainService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly StockExchange _StockExchange;

        public CapitalGainService(IPortfolioDatabase portfolioDatabase, StockExchange stockExchange)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockExchange = stockExchange;
        }

        public Task<SimpleUnrealisedGainsResponce> GetSimpleUnrealisedGains(DateTime date)
        {
            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                var parcels = portfolioUnitOfWork.PortfolioQuery.GetAllParcels(date, date);

                var responce = GetSimpleCGT(parcels, date);
                return Task.FromResult<SimpleUnrealisedGainsResponce>(responce);
            }
        }

        public Task<SimpleUnrealisedGainsResponce> GetSimpleUnrealisedGains(Guid stockId, DateTime date)
        {
            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                var parcels = portfolioUnitOfWork.PortfolioQuery.GetParcelsForStock(stockId, date, date);

                var responce = GetSimpleCGT(parcels, date);
                return Task.FromResult<SimpleUnrealisedGainsResponce>(responce);
            }
        }

        private SimpleUnrealisedGainsResponce GetSimpleCGT(IEnumerable<ShareParcel> parcels, DateTime date)
        {
            var responce = new SimpleUnrealisedGainsResponce();

            StockItem currentStock = null;
            Guid previousStock = Guid.Empty;
            decimal unitPrice = 0.00m;

            foreach (var parcel in parcels)
            {
                if (parcel.Stock != previousStock)
                {
                    var stock = _StockExchange.Stocks.Get(parcel.Stock);
                    currentStock = stock.ToStockItem(date);
                    unitPrice = stock.GetPrice(date);

                    previousStock = parcel.Stock;
                }

                var item = new SimpleUnrealisedGainsItem()
                {
                    Stock = currentStock,
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
            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                var parcels = portfolioUnitOfWork.PortfolioQuery.GetAllParcels(date, date);

                var responce = GetDetailedCGT(parcels, date, portfolioUnitOfWork.PortfolioQuery);

                responce.SetStatusToSuccessfull();

                return Task.FromResult<DetailedUnrealisedGainsResponce>(responce);
            }
        }

        public Task<DetailedUnrealisedGainsResponce> GetDetailedUnrealisedGains(Guid stockId, DateTime date)
        {
            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                var parcels = portfolioUnitOfWork.PortfolioQuery.GetParcelsForStock(stockId, date, date);
                var responce = GetDetailedCGT(parcels, date, portfolioUnitOfWork.PortfolioQuery);

                responce.SetStatusToSuccessfull();

                return Task.FromResult<DetailedUnrealisedGainsResponce>(responce);
            }
        }

        private DetailedUnrealisedGainsResponce GetDetailedCGT(IEnumerable<ShareParcel> parcels, DateTime date, IPortfolioQuery portfolioQuery)
        {
            var responce = new DetailedUnrealisedGainsResponce();

            StockItem currentStock = null;
            Guid previousStock = Guid.Empty;
            decimal unitPrice = 0.00m;

            foreach (var parcel in parcels)
            {
                if (parcel.Stock != previousStock)
                {
                    var stock = _StockExchange.Stocks.Get(parcel.Stock);
                    currentStock = stock.ToStockItem(date);
                    unitPrice = stock.GetPrice(date);

                    previousStock = parcel.Stock;
                }

                var item = new DetailedUnrealisedGainsItem()
                {
                    Stock = currentStock,
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

                AddParcelHistory(item, parcel.Id, date, portfolioQuery);

                responce.CGTItems.Add(item);
            }

            responce.CGTItems.OrderBy(x => x.Stock.Name).ThenBy(x => x.AquisitionDate);

            return responce;
        }

        private void AddParcelHistory(DetailedUnrealisedGainsItem parcelItem, Guid parcelId, DateTime date, IPortfolioQuery portfolioQuery)
        {
            var parcelAudit = portfolioQuery.GetParcelAudit(parcelId, parcelItem.AquisitionDate, date);

            decimal costBase = 0.00m;
            foreach (var auditRecord in parcelAudit)
            {
                costBase += auditRecord.CostBaseChange;

                var transaction = portfolioQuery.GetTransaction(auditRecord.Transaction);

                var cgtEvent = new CGTEventItem()
                {
                    TransactionType = transaction.Type,
                    Date = auditRecord.Date,
                    Units = auditRecord.UnitCount,
                    CostBaseChange = auditRecord.CostBaseChange,
                    CostBase = costBase,
                    Comment = transaction.Description
                };

                parcelItem.CGTEvents.Add(cgtEvent);
            }

        }

        public Task<CGTLiabilityResponce> GetCGTLiability(DateTime fromDate, DateTime toDate)
        {
            var responce = new CGTLiabilityResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                responce.CurrentYearCapitalGainsOther = 0.00m;
                responce.CurrentYearCapitalGainsDiscounted = 0.00m;
                responce.CurrentYearCapitalLossesTotal = 0.00m;

                // Get a list of all the cgt events for the year
                var cgtEvents = portfolioUnitOfWork.PortfolioQuery.GetCGTEvents(fromDate, toDate);
                foreach (var cgtEvent in cgtEvents)
                {
                    var item = new CGTLiabilityItem()
                    {
                        Stock = _StockExchange.Stocks.Get(cgtEvent.Stock).ToStockItem(cgtEvent.EventDate),
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
            }

            return Task.FromResult<CGTLiabilityResponce>(responce);
        }

    }

}
