using PortfolioManager.RestApi.Client;
using PortfolioManager.UI.Models;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    class AquisitionViewModel : TransactionViewModel
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

        public bool CreateCashTransaction { get; set; }

        public AquisitionViewModel(AquisitionTransaction aquisition, RestWebClient restWebClient, RestClient restClient)
            : base(aquisition, TransactionStockSelection.TradeableStocks, restWebClient, restClient)
        {

        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
            {
                Units = ((AquisitionTransaction)Transaction).Units;
                AveragePrice = ((AquisitionTransaction)Transaction).AveragePrice;
                TransactionCosts = ((AquisitionTransaction)Transaction).TransactionCosts;
                CreateCashTransaction = ((AquisitionTransaction)Transaction).CreateCashTransaction;
            }
            else
            {
                Units = 0;
                AveragePrice = 0.00m;
                TransactionCosts = 0.00m;
                CreateCashTransaction = true;
            }
        }

        protected override void CopyFieldsToTransaction()
        {
            if (Transaction == null)
                Transaction = new AquisitionTransaction();

            base.CopyFieldsToTransaction();

            var aquisition = (AquisitionTransaction)Transaction;
            aquisition.TransactionDate = RecordDate;
            aquisition.Units = Units;
            aquisition.AveragePrice = AveragePrice;
            aquisition.TransactionCosts = TransactionCosts;
            aquisition.CreateCashTransaction = CreateCashTransaction;
        }

    }
}
