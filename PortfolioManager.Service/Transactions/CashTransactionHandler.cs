using PortfolioManager.Data.Portfolios;
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
