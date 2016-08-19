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
    class UnitCountAdjustmentViewModel : TransactionViewModel
    {
        public UnitCountAdjustmentViewModel(UnitCountAdjustment unitCostAdjustment, StockService stockService)
            : base(unitCostAdjustment, stockService)
        {

        }
    }
}
