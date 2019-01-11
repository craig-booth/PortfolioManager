using PortfolioManager.RestApi.Client;
using PortfolioManager.RestApi.Transactions;
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

        public AquisitionViewModel(Aquisition aquisition, RestClient restClient)
            : base(aquisition, TransactionStockSelection.TradeableStocks, restClient)
        {

        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (_Transaction != null)
            {
                Units = ((Aquisition)_Transaction).Units;
                AveragePrice = ((Aquisition)_Transaction).AveragePrice;
                TransactionCosts = ((Aquisition)_Transaction).TransactionCosts;
                CreateCashTransaction = ((Aquisition)_Transaction).CreateCashTransaction;
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
            if (_Transaction == null)
                _Transaction = new Aquisition();

            base.CopyFieldsToTransaction();

            var aquisition = (Aquisition)_Transaction;
            aquisition.TransactionDate = RecordDate;
            aquisition.Units = Units;
            aquisition.AveragePrice = AveragePrice;
            aquisition.TransactionCosts = TransactionCosts;
            aquisition.CreateCashTransaction = CreateCashTransaction;
        }

    }
}
