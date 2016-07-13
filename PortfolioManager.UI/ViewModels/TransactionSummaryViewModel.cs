﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class TransactionSummaryViewModel : ViewModel
    {

        public TransactionSummaryViewModel(string label)
            : base(label)
        {
            Options.AllowStockSelection = true;
            Options.DateSelection = DateSelectionType.Range;
        }

        public override void SetData(object data)
        {

        }
    }
}
