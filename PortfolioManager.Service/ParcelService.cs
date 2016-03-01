using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service
{
    public class ParcelService
    {
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly StockService _StockService;

        internal ParcelService(IPortfolioQuery portfolioQuery, StockService stockService)
        {
            _PortfolioQuery = portfolioQuery;
            _StockService = stockService;
        }

        public IReadOnlyCollection<ShareParcel> GetParcels(DateTime date)
        {
            return _PortfolioQuery.GetAllParcels(date);
        }

        public IReadOnlyCollection<ShareParcel> GetParcels(Stock stock, DateTime date)
        {
            return _PortfolioQuery.GetParcelsForStock(stock.Id, date);
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

    }
}
