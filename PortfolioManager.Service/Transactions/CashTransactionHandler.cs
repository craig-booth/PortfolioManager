using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Data;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Transactions;

namespace PortfolioManager.Service
{

    class CashTransactionHandler : TransacactionHandler, ITransactionHandler
    {
        public CashTransactionHandler()
        {

        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var cashTransaction = transaction as CashTransaction;

            CashAccountTransaction(unitOfWork, cashTransaction.CashTransactionType, cashTransaction.TransactionDate, cashTransaction.Description, cashTransaction.Amount);
        }
    }
}
