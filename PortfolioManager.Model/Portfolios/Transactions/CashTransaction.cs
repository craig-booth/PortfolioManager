using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Portfolios
{
    class CashTransaction : ITransaction
    {
        public Guid Id { get; private set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public string ASXCode { get; private set; }
        public decimal Amount { get; set; }

        public TransactionType Type
        {
            get
            {
                return TransactionType.Deposit;
            }
        }
    }
}
