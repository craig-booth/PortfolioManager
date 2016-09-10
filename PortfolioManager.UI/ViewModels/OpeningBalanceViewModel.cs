using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Service;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
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

        public OpeningBalanceViewModel(OpeningBalance openingBalance, StockService stockService)
            : base(openingBalance, TransactionStockSelection.Any, stockService)
        {
        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
            {
                var openingBalance = (OpeningBalance)Transaction;

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
                Transaction = new OpeningBalance();

            base.CopyFieldsToTransaction();

            var openingBalance = (OpeningBalance)Transaction;
            openingBalance.TransactionDate = RecordDate;
            openingBalance.Units = Units;
            openingBalance.CostBase = CostBase;
            openingBalance.AquisitionDate = AquisitionDate;
        }

    }
}
