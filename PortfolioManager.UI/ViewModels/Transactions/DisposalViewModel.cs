using System;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Client;
using PortfolioManager.RestApi.Transactions;
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

                if ((Stock != null) && (Stock.Id != Guid.Empty))
                    CheckEnoughUnitsOwned();
            }
        }

        private async void CheckEnoughUnitsOwned()
        {
            var holding = await _RestClient.Holdings.Get(Stock.Id, RecordDate);

            var availableUnits = holding.Units;
            if (_Transaction != null)
                availableUnits += ((Disposal)_Transaction).Units;

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

        public DisposalViewModel(Disposal disposal, RestClient restClient)
            : base(disposal, "Disposal", TransactionStockSelection.TradeableHoldings, restClient)
        {
            
        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (_Transaction != null)
            {
                Units = ((Disposal)_Transaction).Units;
                AveragePrice = ((Disposal)_Transaction).AveragePrice;
                TransactionCosts = ((Disposal)_Transaction).TransactionCosts;
                CGTMethod = RestApiNameMapping.ToCGTCalculationMethod(((Disposal)_Transaction).CGTMethod);
                CreateCashTransaction = ((Disposal)_Transaction).CreateCashTransaction;
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
            if (_Transaction == null)
                _Transaction = new Disposal();

            base.CopyFieldsToTransaction();

            var disposal = (Disposal)_Transaction;
            disposal.TransactionDate = RecordDate;
            disposal.Units = Units;
            disposal.AveragePrice = AveragePrice;
            disposal.TransactionCosts = TransactionCosts;
            disposal.CGTMethod = CGTMethod.ToRestName();
            disposal.CreateCashTransaction = CreateCashTransaction;
        }

    }
}
