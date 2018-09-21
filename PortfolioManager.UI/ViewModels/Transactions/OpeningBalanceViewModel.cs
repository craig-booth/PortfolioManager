using System;

using PortfolioManager.RestApi.Client;
using PortfolioManager.Service.Interface;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    class OpeningBalanceViewModel : TransactionViewModel
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
            }
        }

        private decimal _CostBase;
        public decimal CostBase
        {
            get
            {
                return _CostBase;
            }
            set
            {
                _CostBase = value;

                ClearErrors();

                if (_CostBase < 0.00m)
                    AddError("Cost Base must not be less than 0");
            }
        }

        private DateTime _AquisitionDate;
        public DateTime AquisitionDate
        {
            get
            {
                return _AquisitionDate;
            }
            set
            {
                _AquisitionDate = value;

                ClearErrors();

                if (_AquisitionDate > RecordDate)
                    AddError("Aquisition Date must not be after the Opening Balance date");
            }
        }

        public OpeningBalanceViewModel(OpeningBalanceTransactionItem openingBalance, RestWebClient restWebClient, RestClient restClient)
            : base(openingBalance, TransactionStockSelection.Stocks, restWebClient, restClient)
        {
        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
            {
                var openingBalance = (OpeningBalanceTransactionItem)Transaction;

                Units = openingBalance.Units;
                CostBase = openingBalance.CostBase;
                AquisitionDate = openingBalance.AquisitionDate;
            }
            else
            {
                Units = 0;
                CostBase = 0.00m;
                AquisitionDate = DateTime.Today;
            }
        }

        protected override void CopyFieldsToTransaction()
        {
            if (Transaction == null)
                Transaction = new OpeningBalanceTransactionItem();

            base.CopyFieldsToTransaction();

            var openingBalance = (OpeningBalanceTransactionItem)Transaction;
            openingBalance.TransactionDate = RecordDate;
            openingBalance.Units = Units;
            openingBalance.CostBase = CostBase;
            openingBalance.AquisitionDate = AquisitionDate;
        }

    }
}
