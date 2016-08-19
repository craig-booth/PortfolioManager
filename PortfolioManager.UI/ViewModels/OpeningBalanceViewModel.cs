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
                Units = ((OpeningBalance)Transaction).Units;
                CostBase = ((OpeningBalance)Transaction).CostBase;
                AquisitionDate = ((OpeningBalance)Transaction).AquisitionDate;
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

        }

    }
}
