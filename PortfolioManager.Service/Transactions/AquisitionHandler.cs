using System;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Domain.Stocks;

using PortfolioManager.Service.Utils;

namespace PortfolioManager.Service.Transactions
{
    class AquisitionHandler : TransacactionHandler, ITransactionHandler
    {
        public AquisitionHandler(IPortfolioQuery portfolioQuery, StockExchange stockExchange)
            : base (portfolioQuery, stockExchange)
        {
        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var aquisition = transaction as Aquisition;

            var stock = _StockExchange.Stocks.Get(aquisition.ASXCode, aquisition.TransactionDate);

            if (stock.HasParent(aquisition.TransactionDate))
                throw new TransctionNotSupportedForChildSecurity(aquisition, "Cannot aquire child securities. Aquire stapled security instead");

            decimal amountPaid = (aquisition.Units * aquisition.AveragePrice) + aquisition.TransactionCosts;
            decimal costBase = amountPaid;

            AddParcel(unitOfWork, aquisition.TransactionDate, aquisition.TransactionDate, stock, aquisition.Units, aquisition.AveragePrice, amountPaid, costBase, transaction.Id, transaction.Id);

            if (aquisition.CreateCashTransaction)
            {
                CashAccountTransaction(unitOfWork, BankAccountTransactionType.Transfer, aquisition.TransactionDate, String.Format("Purchase of {0}", aquisition.ASXCode), -1 * (aquisition.Units * aquisition.AveragePrice));
                CashAccountTransaction(unitOfWork, BankAccountTransactionType.Fee, aquisition.TransactionDate, String.Format("Brokerage for purchase of {0}", aquisition.ASXCode), -1 * aquisition.TransactionCosts);                
            }
        }
    }
}
