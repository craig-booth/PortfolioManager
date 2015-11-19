using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    class OpeningBalanceHandler : ITransactionHandler
    {
        public readonly ParcelService _ParcelService;
        public readonly StockService _StockService;

        public OpeningBalanceHandler(ParcelService parcelService, StockService stockService)
        {
            _ParcelService = parcelService;
            _StockService = stockService;
        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, ITransaction transaction)
        {
            var openingBalance = transaction as OpeningBalance;

            var stock = _StockService.Get(openingBalance.ASXCode, openingBalance.TransactionDate);

            if (stock.ParentId != Guid.Empty)
                throw new TransctionNotSupportedForChildSecurity(openingBalance, "Cannot aquire child securities. Aquire stapled security instead");
            
            _ParcelService.AddParcel(unitOfWork, openingBalance.TransactionDate, stock, openingBalance.Units, openingBalance.CostBase / openingBalance.Units, openingBalance.CostBase, openingBalance.CostBase, ParcelEvent.OpeningBalance);
        }
    }
}
