using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Portfolios
{
    class IncomeReceivedHandler : TransacactionHandler, ITransactionHandler
    {
        public IncomeReceivedHandler(ParcelService parcelService, StockService stockService)
            : base (parcelService, stockService)
        {

        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, ITransaction transaction)
        {
            var incomeReceived = transaction as IncomeReceived;

            var stock = _StockService.Get(incomeReceived.ASXCode, incomeReceived.RecordDate);

            if (stock.Type == StockType.StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(incomeReceived, "Cannot have a income for stapled securities. Income should be recorded against child securities instead");

            /* locate parcels that the dividend applies to */
            var parcels = _ParcelService.GetParcels(stock, incomeReceived.RecordDate);

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
                foreach (ShareParcel parcelAtRecordDate in parcels)
                {
                    ShareParcel parcelAtPaymentDate;
                    if (incomeReceived.TransactionDate <= parcelAtRecordDate.ToDate)
                        parcelAtPaymentDate = parcelAtRecordDate;
                    else
                        parcelAtPaymentDate = _ParcelService.GetParcel(parcelAtRecordDate.Id, incomeReceived.TransactionDate);

                    decimal costBaseReduction = apportionedAmounts[i++].Amount;

                    if (costBaseReduction <= parcelAtPaymentDate.CostBase)
                        ModifyParcel(unitOfWork, parcelAtPaymentDate, incomeReceived.TransactionDate, ParcelEvent.CostBaseReduction, x => { x.CostBase -= costBaseReduction; });
                    else
                    {
                        ModifyParcel(unitOfWork, parcelAtPaymentDate, incomeReceived.TransactionDate, ParcelEvent.CostBaseReduction, x => { x.CostBase = 0.00m; });

                        var cgtEvent = new CGTEvent(parcelAtPaymentDate.Stock, incomeReceived.TransactionDate, parcelAtPaymentDate.Units, parcelAtPaymentDate.CostBase, costBaseReduction - parcelAtPaymentDate.CostBase);
                        unitOfWork.CGTEventRepository.Add(cgtEvent);
                    }
                }
              //  CashAccount.AddTransaction(CashAccountTransactionType.Transfer, incomeReceived.TransactionDate, String.Format("Distribution for {0}", incomeReceived.ASXCode), incomeReceived.CashIncome);
            }
          //  else
          //      CashAccount.AddTransaction(CashAccountTransactionType.Transfer, incomeReceived.TransactionDate, String.Format("Distribution for {0}", incomeReceived.ASXCode), incomeReceived.CashIncome);

        }
    }
}
