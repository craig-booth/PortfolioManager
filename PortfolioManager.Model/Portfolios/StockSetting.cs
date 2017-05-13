using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public class StockSetting : EffectiveDatedEntity
    {
        public bool ParticipateinDRP { get; set; }

        public StockSetting(Guid stockId, DateTime fromDate)
            : base(stockId, fromDate, DateUtils.NoEndDate)
        {
            ParticipateinDRP = false;
        }
    }
}
