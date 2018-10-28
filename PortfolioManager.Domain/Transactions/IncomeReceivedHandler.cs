using System;
using System.Collections.Generic;
using System.Text;
using PortfolioManager.Domain.Portfolios;

namespace PortfolioManager.Domain.Transactions
{
    public class IncomeReceivedHandler : ITransactionHandler
    {
        public void ApplyTransaction(Transaction transaction, Portfolio portfolio)
        {
            var incomeReceived = transaction as IncomeReceived;

            var holding = portfolio.Holdings.Get(incomeReceived.Stock.Id);
            if ((holding == null) || (!holding.IsEffectiveAt(incomeReceived.RecordDate)))
                throw new NoParcelsForTransaction(incomeReceived, "No parcels found for transaction");

            var holdingProperties = holding.Properties[incomeReceived.RecordDate].Units;
            
            // Handle any tax deferred amount recieved 
            if (incomeReceived.TaxDeferred > 0)
            {
                // Apportion amount between parcels 
             /*   ApportionedCurrencyValue[] apportionedAmounts = new ApportionedCurrencyValue[parcels.Count()];
                int i = 0;
                foreach (ShareParcel parcel in parcels)
                    apportionedAmounts[i++].Units = parcel.Units;
                MathUtils.ApportionAmount(incomeReceived.TaxDeferred, apportionedAmounts); */

                // Reduce cost base of parcels 
             /*   i = 0;
                foreach (ShareParcel parcel in parcels)
                {
                    decimal costBaseReduction = apportionedAmounts[i++].Amount;

                    ReduceParcelCostBase(unitOfWork, parcel, incomeReceived.RecordDate, costBaseReduction, transaction.Id);
                } */
            }

            if (incomeReceived.CreateCashTransaction)
                portfolio.CashAccount.Transfer(incomeReceived.TransactionDate, incomeReceived.CashIncome, incomeReceived.Stock.Properties[incomeReceived.RecordDate].Name);
                

           // UpdateDRPCashBalance(unitOfWork, stock, incomeReceived.TransactionDate, incomeReceived.DRPCashBalance);
        }
    }
}
