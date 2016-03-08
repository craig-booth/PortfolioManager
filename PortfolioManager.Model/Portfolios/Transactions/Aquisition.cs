using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;


namespace PortfolioManager.Model.Portfolios
{
    public class Aquisition : Transaction 
    {
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public string Comment { get; set; }

        public Aquisition()
            : base ()
        {

        }

        public Aquisition(Guid id)
            : base(id)
        {
        }

        protected override string GetDescription()
        {
            return "Aquired " + Units.ToString("n0") + " shares @ " + MathUtils.FormatCurrency(AveragePrice, false, true);
        }

        protected override TransactionType GetTransactionType()
        {
            return TransactionType.Aquisition;
        } 

    }
}
