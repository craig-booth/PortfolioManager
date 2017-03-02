using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Service.Interface;

using PortfolioManager.Service.Obsolete;

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

        public AquisitionViewModel(Aquisition aquisition, StockService stockService, ShareHoldingService obsoleteHoldingService, IHoldingService holdingService)
            : base(aquisition, TransactionStockSelection.TradeableStocks(false), stockService, obsoleteHoldingService, holdingService)
        {

        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
            {
                Units = ((Aquisition)Transaction).Units;
                AveragePrice = ((Aquisition)Transaction).AveragePrice;
                TransactionCosts = ((Aquisition)Transaction).TransactionCosts;
                CreateCashTransaction = ((Aquisition)Transaction).CreateCashTransaction;
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
                Transaction = new Aquisition();

            base.CopyFieldsToTransaction();

            var aquisition = (Aquisition)Transaction;
            aquisition.TransactionDate = RecordDate;
            aquisition.Units = Units;
            aquisition.AveragePrice = AveragePrice;
            aquisition.TransactionCosts = TransactionCosts;
            aquisition.CreateCashTransaction = CreateCashTransaction;
        }

    }
}
