
using PortfolioManager.Service.Interface;
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

        public CostBaseAdjustmentViewModel(CostBaseAdjustmentTransactionItem costBaseAdjustment, RestWebClient restWebClient)
            : base(costBaseAdjustment, TransactionStockSelection.Holdings, restWebClient)
        {

        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
                Percentage = ((CostBaseAdjustmentTransactionItem)Transaction).Percentage;
            else
                Percentage = 0.00m;
        }

        protected override void CopyFieldsToTransaction()
        {
            if (Transaction == null)
                Transaction = new CostBaseAdjustmentTransactionItem();

            base.CopyFieldsToTransaction();

            var costBaseAdjustment = (CostBaseAdjustmentTransactionItem)Transaction;
            costBaseAdjustment.TransactionDate = RecordDate;
            costBaseAdjustment.Percentage = Percentage;
        }
    }
}
