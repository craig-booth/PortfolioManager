using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.Transactions
{ 

    class OpeningBalanceHandler : TransacactionHandler, ITransactionHandler
    {
        public OpeningBalanceHandler(ParcelService parcelService, StockService stockService)
            : base (parcelService, stockService)
        {

        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var openingBalance = transaction as OpeningBalance;

            var stock = _StockService.Get(openingBalance.ASXCode, openingBalance.TransactionDate);
            
            AddParcel(unitOfWork, openingBalance.AquisitionDate, stock, openingBalance.Units, openingBalance.CostBase / openingBalance.Units, openingBalance.CostBase, openingBalance.CostBase, ParcelEvent.OpeningBalance);
        }
    }
}
