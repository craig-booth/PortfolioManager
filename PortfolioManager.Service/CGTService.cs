using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Service.Utils;

namespace PortfolioManager.Service
{

    public class CGTService
    {
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly TransactionService _TransactionService;
        private readonly StockService _StockService;

        public CGTService(IPortfolioQuery portfolioQuery, StockService stockService, TransactionService transactionService)
        {
            _PortfolioQuery = portfolioQuery;
            _TransactionService = transactionService;
            _StockService = stockService;
        }

        public Task<SimpleCGTResponce> GetSimpleCGT(DateTime date)
        {
            var parcels = _PortfolioQuery.GetAllParcels(date, date);

            var responce = GetSimpleCGT(parcels, date);
            return Task.FromResult<SimpleCGTResponce>(responce);
        }

        public Task<SimpleCGTResponce> GetSimpleCGT(Guid stockId, DateTime date)
        {
            var parcels = _PortfolioQuery.GetParcelsForStock(stockId, date, date);

            var responce = GetSimpleCGT(parcels, date);
            return Task.FromResult<SimpleCGTResponce>(responce);
        }

        private SimpleCGTResponce GetSimpleCGT(IEnumerable<ShareParcel> parcels, DateTime date)
        {
            var responce = new SimpleCGTResponce();

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

                var item = new SimpleCGTResponceItem()
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


        public Task<DetailedCGTResponce> GetDetailedCGT(DateTime date)
        {
            var parcels = _PortfolioQuery.GetAllParcels(date, date);

            var responce = GetDetailedCGT(parcels, date);
            return Task.FromResult<DetailedCGTResponce>(responce);
        }

        public Task<DetailedCGTResponce> GetDetailedCGT(Guid stockId, DateTime date)
        {
            var parcels = _PortfolioQuery.GetParcelsForStock(stockId, date, date);
            var responce = GetDetailedCGT(parcels, date);
            return Task.FromResult<DetailedCGTResponce>(responce);
        }

        private DetailedCGTResponce GetDetailedCGT(IEnumerable<ShareParcel> parcels, DateTime date)
        {
            var responce = new DetailedCGTResponce();

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

                var item = new DetailedCGTResponceItem()
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

        private void AddParcelHistory(DetailedCGTResponceItem parcelItem, DateTime date)
        {
            var parcelAudit = _PortfolioQuery.GetParcelAudit(parcelItem.Id, parcelItem.AquisitionDate, date);

            decimal costBase = 0.00m;
            foreach (var auditRecord in parcelAudit)
            {
                costBase += auditRecord.CostBaseChange;

                var transaction = _TransactionService.GetTransaction(auditRecord.Transaction);
          
                var historyItem = new DetailedCGTParcelHistoryItem()
                {
                    TransactionType = transaction.Type,
                    Date = auditRecord.Date,
                    Units = auditRecord.UnitCount,
                    Amount = auditRecord.CostBaseChange,
                    CostBase = costBase,
                    Comment = transaction.Description
                };

                parcelItem.History.Add(historyItem);
            }

        }


    }

    public class SimpleCGTResponce
    {
        public List<SimpleCGTResponceItem> CGTItems;

        public SimpleCGTResponce()
        {
            CGTItems = new List<SimpleCGTResponceItem>();
        }
    }

    public class SimpleCGTResponceItem
    {
        public Guid Id { get; set; }
        public string ASXCode { get; set; }
        public string CompanyName { get; set; }

        public DateTime AquisitionDate { get; set; }
        public int Units { get; set; }
        public decimal CostBase { get; set; }
        public decimal MarketValue { get; set; }
        public decimal CapitalGain { get; set; }
        public decimal DiscoutedGain { get; set; }
        public CGTMethod DiscountMethod { get; set; }
    }


    public class DetailedCGTResponce
    {
        public List<DetailedCGTResponceItem> CGTItems;

        public DetailedCGTResponce()
        {
            CGTItems = new List<DetailedCGTResponceItem>();
        }
    }

    public class DetailedCGTResponceItem : SimpleCGTResponceItem
    {
        public List<DetailedCGTParcelHistoryItem> History;

        public DetailedCGTResponceItem()
        {
            History = new List<DetailedCGTParcelHistoryItem>();
        }

    }


    public class DetailedCGTParcelHistoryItem
    {
        public DateTime Date { get; set; }
        public TransactionType TransactionType { get; set; }
        public int Units { get; set; }
        public decimal Amount { get; set; }
        public decimal CostBase { get; set; }
        public string Comment { get; set; }
    }

    public class CGTService2
    {

        private readonly IPortfolioQuery _PortfolioQuery;

        internal CGTService2(IPortfolioQuery portfolioQuery)
        {
            _PortfolioQuery = portfolioQuery;
        }


        public IReadOnlyCollection<CGTEvent> GetEvents(DateTime fromDate, DateTime toDate)
        {
            return _PortfolioQuery.GetCGTEvents(fromDate, toDate);
        }

    }
}
