using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;


namespace PortfolioManager.Model.Portfolios
{
    public class Aquisition : ITransaction
    {
        public Guid Id { get; private set; }
        public DateTime TransactionDate { get; set; }
        public int Sequence { get; private set; }
        public string ASXCode { get; set; }
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public string Comment { get; set; }

        public string Description
        {
            get
            {
                return "Aquired " + Units.ToString("n0") + " shares @ " + MathUtils.FormatCurrency(AveragePrice, false, true);
            }
        }

        public Aquisition()
            : this (Guid.NewGuid())
        {

        }

        public Aquisition(Guid id)
        {
            Id = id;
        }

        public TransactionType Type
        {
            get
            {
                return TransactionType.Aquisition;
            }
        }

    }
}
