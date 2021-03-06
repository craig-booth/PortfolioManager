﻿using System;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Service.Transactions
{

    class OpeningBalanceHandler : TransacactionHandler, ITransactionHandler
    {
        public OpeningBalanceHandler(IPortfolioQuery portfolioQuery, StockExchange stockExchange)
            : base (portfolioQuery, stockExchange)
        {

        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var openingBalance = transaction as OpeningBalance;

            var stock = _StockExchange.Stocks.Get(openingBalance.ASXCode, openingBalance.TransactionDate);

            Guid purchaseId = openingBalance.PurchaseId;
            if (purchaseId == Guid.Empty)
                purchaseId = transaction.Id;

            var unitPrice = Math.Round(openingBalance.CostBase / openingBalance.Units, 5, MidpointRounding.AwayFromZero);

            AddParcel(unitOfWork, openingBalance.AquisitionDate, transaction.TransactionDate, stock, openingBalance.Units, unitPrice, openingBalance.CostBase, openingBalance.CostBase, transaction.Id, purchaseId);
        }
    }
}
