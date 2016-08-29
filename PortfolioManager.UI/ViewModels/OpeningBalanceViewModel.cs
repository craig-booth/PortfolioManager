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
        public int Units { get; set; }
        public decimal CostBase { get; set; }
        public DateTime AquisitionDate { get; set; }

        public OpeningBalanceViewModel(OpeningBalance openingBalance, StockService stockService)
            : base(openingBalance, stockService)
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
            openingBalance.Units = Units;
            openingBalance.CostBase = CostBase;
            openingBalance.AquisitionDate = AquisitionDate;
        }

    }
}
