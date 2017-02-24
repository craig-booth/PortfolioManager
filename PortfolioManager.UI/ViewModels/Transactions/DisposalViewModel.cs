using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Service.Interface;

using PortfolioManager.UI.Utilities;

using PortfolioManager.Service.Obsolete;

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

                if (Stock != null)
                {
                    var stock = _StockService.Get(Stock.Id, RecordDate);
                    var holding = _ObsoleteHoldingService.GetHolding(stock, RecordDate);

                    var availableUnits = holding.Units;
                    if (Transaction != null)
                        availableUnits += ((Disposal)Transaction).Units;

                    if (_Units > availableUnits)
                        AddError(String.Format("Only {0} units available", availableUnits));
                }
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

        public CGTCalculationMethod CGTMethod { get; set; }

        public bool CreateCashTransaction { get; set; }

        public DisposalViewModel(Disposal disposal, StockService stockService, ShareHoldingService obsoleteHoldingService, IHoldingService holdingService)
            : base(disposal, TransactionStockSelection.TradeableStocks(true), stockService, obsoleteHoldingService, holdingService)
        {
            
        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
            {
                Units = ((Disposal)Transaction).Units;
                AveragePrice = ((Disposal)Transaction).AveragePrice;
                TransactionCosts = ((Disposal)Transaction).TransactionCosts;
                CGTMethod = ((Disposal)Transaction).CGTMethod;
                CreateCashTransaction = ((Disposal)Transaction).CreateCashTransaction;
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
                Transaction = new Disposal();

            base.CopyFieldsToTransaction();

            var disposal = (Disposal)Transaction;
            disposal.TransactionDate = RecordDate;
            disposal.Units = Units;
            disposal.AveragePrice = AveragePrice;
            disposal.TransactionCosts = TransactionCosts;
            disposal.CGTMethod = CGTMethod;
            disposal.CreateCashTransaction = CreateCashTransaction;
        }

    }
}
