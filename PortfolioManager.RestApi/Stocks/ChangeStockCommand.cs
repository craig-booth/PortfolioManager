﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.RestApi.Stocks
{
    public class ChangeStockCommand
    {
        public Guid Id { get; set; }
        public DateTime ChangeDate { get; set; }
        public string AsxCode { get; set; }
        public string Name { get; set; }
        public AssetCategory Category { get; set; }
    }
}
