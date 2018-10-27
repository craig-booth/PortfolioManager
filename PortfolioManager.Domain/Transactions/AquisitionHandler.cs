using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Portfolios;

namespace PortfolioManager.Domain.Transactions
{
    public class AquisitionHandler : ITransactionHandler
    {
        public void ApplyTransaction(Transaction transaction, Portfolio portfolio)
        {
            var aquisition = transaction as Aquisition;
           
            var holding = portfolio.Holdings.Get(aquisition.Stock.Id);
            if (holding == null)
            {
                holding = portfolio.Holdings.Add(aquisition.Stock, aquisition.TransactionDate);      
            }

            decimal cost = aquisition.Units * aquisition.AveragePrice;
            decimal amountPaid = cost + aquisition.TransactionCosts;
            decimal costBase = amountPaid;

            holding.AddParcel(aquisition.TransactionDate, aquisition.TransactionDate, aquisition.Units, aquisition.AveragePrice, amountPaid, costBase);
        
            if (aquisition.CreateCashTransaction)
            {
                var asxCode = aquisition.Stock.Properties[aquisition.TransactionDate].ASXCode;
                portfolio.CashAccount.Transfer(aquisition.TransactionDate, cost, String.Format("Purchase of {0}", asxCode));
                portfolio.CashAccount.FeeDeducted(aquisition.TransactionDate, aquisition.TransactionCosts, String.Format("Brokerage for purchase of {0}", asxCode));
            }

            portfolio.Transactions.Add(aquisition);
        }


    }
}
