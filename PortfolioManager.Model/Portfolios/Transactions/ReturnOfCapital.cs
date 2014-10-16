using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{

    public class ReturnOfCapital : ITransaction
    {
        public Guid Id { get; private set; }
        public DateTime TransactionDate { get; set; }
        public Guid Stock { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }

        public string Description
        {
            get
            {
                return "Return of Capital of " + Amount.ToString("c");
            }
        }

        public TransactionType Type
        {
            get
            {
                return TransactionType.ReturnOfCapital;
            }
        }
    }
}
