using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{

    class CashTransactionHandler : ITransactionHandler
    {
        public CashTransactionHandler()
        {

        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, ITransaction transaction)
        {
            var cashTransaction = transaction as CashTransaction;

            /*            if (cashTransaction.Type == TransactionType.Withdrawl)
                            CashAccount.AddTransaction(CashAccountTransactionType.Withdrawl, cashTransaction.TransactionDate, cashTransaction.Description, cashTransaction.Amount);
                        else if (cashTransaction.Type == TransactionType.Deposit)
                            CashAccount.AddTransaction(CashAccountTransactionType.Deposit, cashTransaction.TransactionDate, cashTransaction.Description, cashTransaction.Amount);
                        else if (cashTransaction.Type == TransactionType.Fee)
                            CashAccount.AddTransaction(CashAccountTransactionType.Fee, cashTransaction.TransactionDate, cashTransaction.Description, cashTransaction.Amount);
                        else if (cashTransaction.Type == TransactionType.Interest)
                            CashAccount.AddTransaction(CashAccountTransactionType.Interest, cashTransaction.TransactionDate, cashTransaction.Description, cashTransaction.Amount); */
        }


    }
}
