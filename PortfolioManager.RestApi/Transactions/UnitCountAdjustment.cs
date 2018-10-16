﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Transactions
{
    public class UnitCountAdjustment : Transaction
    {
        public override string Type
        {
            get { return "unitcountadjustment"; }
        }
        public int OriginalUnits { get; set; }
        public int NewUnits { get; set; }
    }
}