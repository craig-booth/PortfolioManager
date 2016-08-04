using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Service
{
    public class ShareHoldingService
    {
        private readonly ParcelService _ParcelService;
        private readonly StockService _StockService;
        private readonly StockPriceService _StockPriceService;

        internal ShareHoldingService(ParcelService parcelService, StockService stockService, StockPriceService stockPriceService)
        {
            _ParcelService = parcelService;
            _StockService = stockService;
            _StockPriceService = stockPriceService;
        }

        public ShareHolding GetHolding(Stock stock, DateTime date)
        {            
            var parcels = _ParcelService.GetParcels(stock, date);

            var holding = new ShareHolding();
            holding.Stock = stock;
            holding.UnitValue = _StockPriceService.GetPrice(stock, date);

            foreach (var parcel in parcels)
            {
                holding.Units += parcel.Units;
                holding.TotalCostBase += parcel.CostBase;
                holding.TotalCost += parcel.Units * parcel.UnitPrice;
                
            }

            return holding;
        }

        public IReadOnlyCollection<ShareHolding> GetHoldings(DateTime date)
        { 
            var allParcels = _ParcelService.GetParcels(date).OrderBy(x => x.Stock);

            var holdings = new List<ShareHolding>();
            ShareHolding holding = null;
            foreach (var parcel in allParcels)
            {
                if (parcel.Units <= 0)
                    continue;

                if ((holding == null) || (parcel.Stock != holding.Stock.Id))
                {
                    var stock = _StockService.Get(parcel.Stock, date);

                    // If a stapled security then get the parent stock
                    if (stock.ParentId != Guid.Empty)
                    {
                        holding = holdings.FirstOrDefault(x => x.Stock.Id == stock.ParentId);
                        if (holding != null)
                        {
                            holding.TotalCostBase += parcel.CostBase;
                            holding.TotalCost += parcel.Units * parcel.UnitPrice;
                        }
                        else
                        {
                            stock = _StockService.Get(stock.ParentId, date);

                            holding = new ShareHolding();
                            holdings.Add(holding);

                            holding.Stock = stock;
                            holding.Units = parcel.Units;
                            holding.TotalCostBase = parcel.CostBase;
                            holding.TotalCost = parcel.Units * parcel.UnitPrice;
                        }

                    }
                    else
                    {
                        holding = new ShareHolding();
                        holdings.Add(holding);

                        holding.Stock = stock;
                        holding.Units = parcel.Units;
                        holding.TotalCostBase = parcel.CostBase;
                        holding.TotalCost = parcel.Units * parcel.UnitPrice;
   
                    }
                }
                else
                {
                    holding.Units += parcel.Units;
                    holding.TotalCostBase += parcel.CostBase;
                    holding.TotalCost += parcel.Units * parcel.UnitPrice;
                }

            }

            /* Retrieve the stock price information */
            if (date == DateTime.Today)
            {
                var asxCodes = new string[holdings.Count];
                for (var i = 0; i < holdings.Count; i++)
                    asxCodes[i] = holdings[i].Stock.ASXCode;
                var stockPrices = _StockPriceService.GetCurrentPrice(asxCodes);

                /* Add prices to the Holdings list */
                foreach (var shareHolding in holdings)
                {
                    var stockPrice = stockPrices.FirstOrDefault(x => x.ASXCode == shareHolding.Stock.ASXCode);

                    if (stockPrice != null)
                        shareHolding.UnitValue = stockPrice.Price;
                } 
            }
            else
            {
                foreach (var shareHolding in holdings)
                {
                    shareHolding.UnitValue = _StockPriceService.GetClosingPrice(shareHolding.Stock, date);
                }
            }

            return holdings.AsReadOnly();

        }

        public DateTime GetPortfolioStartDate()
        {
            var firstParcel = _ParcelService.GetParcels(new DateTime(0001, 01, 01), DateTime.Today).OrderBy(x => x.FromDate).FirstOrDefault();

            return firstParcel.FromDate;
        }

        public IReadOnlyCollection<Stock> GetOwnedStocks(DateTime date)
        {
            return GetOwnedStocks(date, date);
        }

        public IReadOnlyCollection<Stock> GetOwnedStocks(DateTime fromDate, DateTime toDate)
        {
            var ownedStocks = new List<Stock>();

            var parcels = _ParcelService.GetParcels(fromDate, toDate).OrderBy(x => x.Stock);

            Stock currentStock = null;
            foreach (var shareParcel in parcels)
            {
                if ((currentStock == null) || (shareParcel.Stock != currentStock.Id))
                {
                    var stock =_StockService.Get(shareParcel.Stock, shareParcel.FromDate);
                    ownedStocks.Add(stock);

                    currentStock = stock;
                }
            } 

            return ownedStocks.AsReadOnly();
        }

        public IReadOnlyCollection<OwnedStockId> GetOwnedStockIds(DateTime date)
        {
            return GetOwnedStockIds(date, date);
        }

        public IReadOnlyCollection<OwnedStockId> GetOwnedStockIds(DateTime fromDate, DateTime toDate)
        {
              var ownedStocks = new List<OwnedStockId>();

              var parcels = _ParcelService.GetParcels(fromDate, toDate).OrderBy(x => x.Stock).ThenBy(x => x.FromDate);

              OwnedStockId currentStock = null;
              foreach (var shareParcel in parcels)
              {
                  if ((currentStock != null) && (shareParcel.Stock == currentStock.Id) && (shareParcel.FromDate < currentStock.ToDate))
                  {
                      if (shareParcel.ToDate > currentStock.ToDate)
                          currentStock.ToDate = shareParcel.ToDate;
                  }
                  else
                  {
                      currentStock = new OwnedStockId()
                      {
                          Id = shareParcel.Stock,
                          FromDate = shareParcel.FromDate,
                          ToDate = shareParcel.ToDate
                      };
                      ownedStocks.Add(currentStock);
                  }
              }

              return ownedStocks.AsReadOnly(); 
        }

    }

    public class OwnedStockId
    {
        public Guid Id;
        public DateTime FromDate;
        public DateTime ToDate;
    }

}
