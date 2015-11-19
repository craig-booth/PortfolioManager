﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    class AquisitionHandler : ITransactionHandler
    {
        public readonly ParcelService _ParcelService;
        public readonly StockService _StockService;

        public AquisitionHandler(ParcelService parcelService, StockService stockService)
        {
            _ParcelService = parcelService;
            _StockService = stockService;
        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, ITransaction transaction)
        {
            var aquisition = transaction as Aquisition;

            Stock stock = _StockService.Get(aquisition.ASXCode, aquisition.TransactionDate);

            if (stock.ParentId != Guid.Empty)
                throw new TransctionNotSupportedForChildSecurity(aquisition, "Cannot aquire child securities. Aquire stapled security instead");

            decimal amountPaid = (aquisition.Units * aquisition.AveragePrice) + aquisition.TransactionCosts;
            decimal costBase = amountPaid;

            _ParcelService.AddParcel(unitOfWork, aquisition.TransactionDate, stock, aquisition.Units, aquisition.AveragePrice, amountPaid, costBase, ParcelEvent.Aquisition);

            //CashAccount.AddTransaction(CashAccountTransactionType.Transfer, aquisition.TransactionDate, String.Format("Purchase of {0}", aquisition.ASXCode), amountPaid);
        }
    }
}
