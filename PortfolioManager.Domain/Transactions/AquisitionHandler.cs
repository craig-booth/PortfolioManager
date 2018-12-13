﻿using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Portfolios;

namespace PortfolioManager.Domain.Transactions
{
    public class AquisitionHandler : ITransactionHandler
    {
        private HoldingCollection _Holdings;
        private CashAccount _CashAccount;

        public AquisitionHandler(HoldingCollection holdings, CashAccount cashAccount)
        {
            _Holdings = holdings;
            _CashAccount = cashAccount;
        }

        public void ApplyTransaction(Transaction transaction)
        {
            var aquisition = transaction as Aquisition;

            var holding = _Holdings.Get(aquisition.Stock.Id);
            if (holding == null)
            {
                holding = _Holdings.Add(aquisition.Stock, aquisition.TransactionDate);
            }

            decimal cost = aquisition.Units * aquisition.AveragePrice;
            decimal amountPaid = cost + aquisition.TransactionCosts;
            decimal costBase = amountPaid;

            holding.AddParcel(aquisition.TransactionDate, aquisition.TransactionDate, aquisition.Units, amountPaid, costBase);

            if (aquisition.CreateCashTransaction)
            {
                var asxCode = aquisition.Stock.Properties[aquisition.TransactionDate].ASXCode;
                _CashAccount.Transfer(aquisition.TransactionDate, -cost, String.Format("Purchase of {0}", asxCode));

                if (aquisition.TransactionCosts > 0.00m)
                    _CashAccount.FeeDeducted(aquisition.TransactionDate, aquisition.TransactionCosts, String.Format("Brokerage for purchase of {0}", asxCode));
            }
        }


    }
}
