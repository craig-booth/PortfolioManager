using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    public class Disposal : ITransaction
    {
        public Guid Id { get; private set; }
        public DateTime TransactionDate { get; set; }
        public string ASXCode { get; set; }
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public CGTCalculationMethod CGTMethod { get; set; }
        public string Comment { get; set; }

        public string Description
        {
            get
            {
                return "Disposed of " + Units.ToString("n0") + " shares @ " + AveragePrice.ToString("c");
            }
        }

        public Disposal()
            : this (Guid.NewGuid())
        {

        }

        public Disposal(Guid id)
        {
            Id = id;
        }

        public TransactionType Type
        {
            get
            {
                return TransactionType.Disposal;
            }
        }

    }
}
