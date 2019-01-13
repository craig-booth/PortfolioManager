using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.CorporateActions
{
    public class Transformation : CorporateAction
    {
        public override string Type
        {
            get { return "capitalreturn"; }
        }
        public DateTime ImplementationDate { get; set; }
        public decimal CashComponent { get; set; }
        public bool RolloverRefliefApplies { get; set; }

        public List<ResultingStock> ResultingStocks = new List<ResultingStock>();

        public class ResultingStock
        {
            public Guid Stock { get; set; }
            public int OriginalUnits { get; set; }
            public int NewUnits { get; set; }
            public decimal CostBase { get; set; }
            public DateTime AquisitionDate { get; set; }

            public ResultingStock()
            {

            }

            public ResultingStock(Guid stock, int originalUnits, int newUnits, decimal costBase, DateTime aquisitionDate)
            {
                Stock = stock;
                OriginalUnits = originalUnits;
                NewUnits = newUnits;
                CostBase = costBase;
                AquisitionDate = aquisitionDate;
            }
        }
    }
}
