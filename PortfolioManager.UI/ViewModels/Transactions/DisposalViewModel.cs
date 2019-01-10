﻿using System;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Client;
using PortfolioManager.UI.Models;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    class DisposalViewModel : TransactionViewModel
    {

        private int _Units;
        public int Units
        {
            get
            {
                return _Units;
            }
            set
            {
                _Units = value;

                ClearErrors();

                if (_Units <= 0)
                    AddError("Units must be greater than 0");

                if (Stock.Id != Guid.Empty)
                    CheckEnoughUnitsOwned();
            }
        }

        private async void CheckEnoughUnitsOwned()
        {
            var responce = await _RestWebClient.GetPortfolioHoldingsAsync(Stock.Id, RecordDate);

            var availableUnits = responce.Holding.Units;
            if (Transaction != null)
                availableUnits += ((DisposalTransaction)Transaction).Units;

            if (_Units > availableUnits)
                AddError(String.Format("Only {0} units available", availableUnits), "Units");

        }

        private decimal _AveragePrice;
        public decimal AveragePrice
        {
            get
            {
                return _AveragePrice;
            }
            set
            {
                _AveragePrice = value;

                ClearErrors();

                if (_AveragePrice < 0.00m)
                    AddError("Average Price must not be less than 0");
            }
        }

        private decimal _TransactionCosts;
        public decimal TransactionCosts
        {
            get
            {
                return _TransactionCosts;
            }
            set
            {
                _TransactionCosts = value;

                ClearErrors();

                if (_TransactionCosts < 0.00m)
                    AddError("Transaction Costs must not be less than 0");
            }
        }

        public CGTCalculationMethod CGTMethod { get; set; }

        public bool CreateCashTransaction { get; set; }

        public DisposalViewModel(DisposalTransaction disposal, RestWebClient restWebClient, RestClient restClient)
            : base(disposal, TransactionStockSelection.TradeableHoldings, restWebClient, restClient)
        {
            
        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
            {
                Units = ((DisposalTransaction)Transaction).Units;
                AveragePrice = ((DisposalTransaction)Transaction).AveragePrice;
                TransactionCosts = ((DisposalTransaction)Transaction).TransactionCosts;
                CGTMethod = ((DisposalTransaction)Transaction).CGTMethod;
                CreateCashTransaction = ((DisposalTransaction)Transaction).CreateCashTransaction;
            }
            else
            {
                Units = 0;
                AveragePrice = 0.00m;
                TransactionCosts = 0.00m;
                CGTMethod = CGTCalculationMethod.MinimizeGain;
                CreateCashTransaction = true;
            }
        }

        protected override void CopyFieldsToTransaction()
        {
            if (Transaction == null)
                Transaction = new DisposalTransaction();

            base.CopyFieldsToTransaction();

            var disposal = (DisposalTransaction)Transaction;
            disposal.TransactionDate = RecordDate;
            disposal.Units = Units;
            disposal.AveragePrice = AveragePrice;
            disposal.TransactionCosts = TransactionCosts;
            disposal.CGTMethod = CGTMethod;
            disposal.CreateCashTransaction = CreateCashTransaction;
        }

    }
}
