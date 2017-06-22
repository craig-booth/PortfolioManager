using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Common;
using PortfolioManager.Service.Interface;
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

        public AquisitionViewModel(AquisitionTransactionItem aquisition, RestWebClient restWebClient)
            : base(aquisition, TransactionStockSelection.TradeableStocks, restWebClient)
        {

        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
            {
                Units = ((AquisitionTransactionItem)Transaction).Units;
                AveragePrice = ((AquisitionTransactionItem)Transaction).AveragePrice;
                TransactionCosts = ((AquisitionTransactionItem)Transaction).TransactionCosts;
                CreateCashTransaction = ((AquisitionTransactionItem)Transaction).CreateCashTransaction;
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
                Transaction = new AquisitionTransactionItem();

            base.CopyFieldsToTransaction();

            var aquisition = (AquisitionTransactionItem)Transaction;
            aquisition.TransactionDate = RecordDate;
            aquisition.Units = Units;
            aquisition.AveragePrice = AveragePrice;
            aquisition.TransactionCosts = TransactionCosts;
            aquisition.CreateCashTransaction = CreateCashTransaction;
        }

    }
}
