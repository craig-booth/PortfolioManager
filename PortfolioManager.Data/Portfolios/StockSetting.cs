using System;

namespace PortfolioManager.Data.Portfolios
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
