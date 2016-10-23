using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.Transactions
{
    class AquisitionHandler : TransacactionHandler, ITransactionHandler
    {
        public AquisitionHandler(ParcelService parcelService, StockService stockService)
            : base (parcelService, stockService)
        {
        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var aquisition = transaction as Aquisition;

            var stock = _StockService.Get(aquisition.ASXCode, aquisition.TransactionDate);

            if (stock.ParentId != Guid.Empty)
                throw new TransctionNotSupportedForChildSecurity(aquisition, "Cannot aquire child securities. Aquire stapled security instead");

            decimal amountPaid = (aquisition.Units * aquisition.AveragePrice) + aquisition.TransactionCosts;
            decimal costBase = amountPaid;

            AddParcel(unitOfWork, aquisition.TransactionDate, aquisition.TransactionDate, stock, aquisition.Units, aquisition.AveragePrice, amountPaid, costBase, transaction.Id, transaction.Id);

            if (aquisition.CreateCashTransaction)
            {
                CashAccountTransaction(unitOfWork, CashAccountTransactionType.Transfer, aquisition.TransactionDate, String.Format("Purchase of {0}", aquisition.ASXCode), -1 * (aquisition.Units * aquisition.AveragePrice));
                CashAccountTransaction(unitOfWork, CashAccountTransactionType.Fee, aquisition.TransactionDate, String.Format("Brokerage for purchase of {0}", aquisition.ASXCode), -1 * aquisition.TransactionCosts);                
            }
        }
    }
}
