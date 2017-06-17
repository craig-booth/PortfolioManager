using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public class StockSetting : EffectiveDatedEntity, IEditableEffectiveDatedEntity<StockSetting>
    {
        public bool ParticipateinDRP { get; set; }

        public StockSetting(Guid stockId, DateTime fromDate, DateTime toDate)
            : base(stockId, fromDate, toDate)
        {
            ParticipateinDRP = false;
        }

        public StockSetting CreateNewEffectiveEntity(DateTime atDate)
        {
            return new StockSetting(Id, atDate, ToDate)
            {
                ParticipateinDRP = ParticipateinDRP
            };
        }
    }
}
