using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Service.Interface;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    class UnitCountAdjustmentViewModel : TransactionViewModel
    {

        private int _OriginalUnits;
        public int OriginalUnits
        {
            get
            {
                return _OriginalUnits;
            }
            set
            {
                _OriginalUnits = value;

                ClearErrors();

                if (_OriginalUnits <= 0)
                    AddError("Original Units must be greater than 0");
            }
        }

        private int _NewUnits;
        public int NewUnits
        {
            get
            {
                return _NewUnits;
            }
            set
            {
                _NewUnits = value;

                ClearErrors();

                if (_NewUnits <= 0)
                    AddError("New Units must be greater than 0");
            }
        }

        public bool CreateCashTransaction { get; set; }

        public UnitCountAdjustmentViewModel(UnitCountAdjustmentTransactionItem unitCostAdjustment, RestWebClient restWebClient)
            : base(unitCostAdjustment, TransactionStockSelection.Holdings, restWebClient)
        {

        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
            {
                OriginalUnits = ((UnitCountAdjustmentTransactionItem)Transaction).OriginalUnits;
                NewUnits = ((UnitCountAdjustmentTransactionItem)Transaction).NewUnits;
            }
            else
            {
                OriginalUnits = 0;
                NewUnits = 0;
            }
        }

        protected override void CopyFieldsToTransaction()
        {
            if (Transaction == null)
                Transaction = new UnitCountAdjustmentTransactionItem();

            base.CopyFieldsToTransaction();

            var unitCountAdjustment = (UnitCountAdjustmentTransactionItem)Transaction;
            unitCountAdjustment.TransactionDate = RecordDate;
            unitCountAdjustment.OriginalUnits = OriginalUnits;
            unitCountAdjustment.NewUnits = NewUnits;
        }



    }
}
