using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.Transactions
{ 

    class OpeningBalanceHandler : TransacactionHandler, ITransactionHandler
    {
        public OpeningBalanceHandler(IPortfolioQuery portfolioQuery, IStockQuery stockQuery, ILiveStockPriceQuery livePriceQuery)
            : base (portfolioQuery, stockQuery, livePriceQuery)
        {

        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var openingBalance = transaction as OpeningBalance;

            var stock = _StockQuery.GetByASXCode(openingBalance.ASXCode, openingBalance.TransactionDate);

            Guid purchaseId = openingBalance.PurchaseId;
            if (purchaseId == Guid.Empty)
                purchaseId = transaction.Id;

            var unitPrice = Math.Round(openingBalance.CostBase / openingBalance.Units, 5, MidpointRounding.AwayFromZero);

            AddParcel(unitOfWork, openingBalance.AquisitionDate, transaction.TransactionDate, stock, openingBalance.Units, unitPrice, openingBalance.CostBase, openingBalance.CostBase, transaction.Id, purchaseId);
        }
    }
}
