
using PortfolioManager.RestApi.Client;
using PortfolioManager.UI.Models;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    class CostBaseAdjustmentViewModel : TransactionViewModel
    {
        public decimal _Percentage;
        public decimal Percentage
        {
            get
            {
                return _Percentage;
            }
            set
            {
                _Percentage = value;

                ClearErrors();

                if ((_Percentage <= 0) || (_Percentage > 100))
                    AddError("Percentage must be greater than 0 and less than or equal to 100");
            }
        }

        public CostBaseAdjustmentViewModel(CostBaseAdjustmentTransaction costBaseAdjustment, RestWebClient restWebClient, RestClient restClient)
            : base(costBaseAdjustment, TransactionStockSelection.Holdings, restWebClient, restClient)
        {

        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
                Percentage = ((CostBaseAdjustmentTransaction)Transaction).Percentage;
            else
                Percentage = 0.00m;
        }

        protected override void CopyFieldsToTransaction()
        {
            if (Transaction == null)
                Transaction = new CostBaseAdjustmentTransaction();

            base.CopyFieldsToTransaction();

            var costBaseAdjustment = (CostBaseAdjustmentTransaction)Transaction;
            costBaseAdjustment.TransactionDate = RecordDate;
            costBaseAdjustment.Percentage = Percentage;
        }
    }
}
