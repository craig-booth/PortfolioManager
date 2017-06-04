using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Service.Utils;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.Transactions
{
    class IncomeReceivedHandler : TransacactionHandler, ITransactionHandler
    {

        public IncomeReceivedHandler(IPortfolioQuery portfolioQuery, IStockQuery stockQuery)
            : base (portfolioQuery, stockQuery)
        {
        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var incomeReceived = transaction as IncomeReceived;

            var stock = _StockQuery.GetByASXCode(incomeReceived.ASXCode, incomeReceived.RecordDate);

            if (stock.Type == StockType.StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(incomeReceived, "Cannot have a income for stapled securities. Income should be recorded against child securities instead");

            /* locate parcels that the dividend applies to */
            var parcels = _PortfolioQuery.GetParcelsForStock(stock.Id, incomeReceived.RecordDate, incomeReceived.RecordDate);

            if (parcels.Count == 0)
                throw new NoParcelsForTransaction(incomeReceived, "No parcels found for transaction");

            /* Handle any tax deferred amount recieved */
            if (incomeReceived.TaxDeferred > 0)
            {
                /* Apportion amount between parcels */
                ApportionedCurrencyValue[] apportionedAmounts = new ApportionedCurrencyValue[parcels.Count];
                int i = 0;
                foreach (ShareParcel parcel in parcels)
                    apportionedAmounts[i++].Units = parcel.Units;
                MathUtils.ApportionAmount(incomeReceived.TaxDeferred, apportionedAmounts);

                /* Reduce cost base of parcels */
                i = 0;
                foreach (ShareParcel parcel in parcels)
                {
                    decimal costBaseReduction = apportionedAmounts[i++].Amount;

                    ReduceParcelCostBase(unitOfWork, parcel, incomeReceived.RecordDate, costBaseReduction, transaction.Id);
                }
            }

            if (incomeReceived.CreateCashTransaction)
                CashAccountTransaction(unitOfWork, BankAccountTransactionType.Transfer, incomeReceived.TransactionDate, String.Format("Distribution for {0}", incomeReceived.ASXCode), incomeReceived.CashIncome);

            UpdateDRPCashBalance(unitOfWork, stock, incomeReceived.TransactionDate, incomeReceived.DRPCashBalance);
        }
    }
}
