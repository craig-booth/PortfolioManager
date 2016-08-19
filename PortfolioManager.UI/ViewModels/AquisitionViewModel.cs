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
    class AquisitionViewModel : TransactionViewModel
    {
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public bool CreateCashTransaction { get; set; }

        public AquisitionViewModel(Aquisition aquisition, StockService stockService)
            : base(aquisition, stockService)
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
                CreateCashTransaction = false;
            }
        }

        protected override void CopyFieldsToTransaction()
        {

        }

    }
}
