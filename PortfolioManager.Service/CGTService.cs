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
        //     private readonly ShareHoldingService _ShareHoldingService;
        //     private readonly CashAccountService _CashAccountService;
        //     private readonly TransactionService _TransactionService;
        private readonly StockService _StockService;
        //     private readonly IncomeService _IncomeService;

        public CGTService(IPortfolioQuery portfolioQuery, StockService stockService) //(ShareHoldingService shareHoldingService, CashAccountService cashAccountService, TransactionService transactionService, , IncomeService incomeService)
        {
            _PortfolioQuery = portfolioQuery;
     //       _ShareHoldingService = shareHoldingService;
     //       _CashAccountService = cashAccountService;
      //      _TransactionService = transactionService;
            _StockService = stockService;
      //      _IncomeService = incomeService;
        }

        public Task<SimpleCGTResponce> GetSimpleCGT(DateTime date)
        {
            var parcels = _PortfolioQuery.GetAllParcels(date, date);
            return GetSimpleCGT(parcels, date);
        }

        public Task<SimpleCGTResponce> GetSimpleCGT(Guid stockId, DateTime date)
        {
            var parcels = _PortfolioQuery.GetParcelsForStock(stockId, date, date);
            return GetSimpleCGT(parcels, date);
        }

        private Task<SimpleCGTResponce> GetSimpleCGT(IEnumerable<ShareParcel> parcels, DateTime date)
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

            return Task.FromResult<SimpleCGTResponce>(responce);
        }

        public Task<DetailedCGTResponce> GetDetailedCGT(DateTime date)
        {
            var responce = new DetailedCGTResponce();

            return Task.FromResult<DetailedCGTResponce>(responce);
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
