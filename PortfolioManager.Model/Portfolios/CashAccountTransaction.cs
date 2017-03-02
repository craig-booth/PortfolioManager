using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{

    public class CashAccountTransaction  : Entity
    {
        public BankAccountTransactionType Type { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        public CashAccountTransaction()
            : this(Guid.NewGuid())
        {
        }

        public CashAccountTransaction(Guid id)
            :base(id)
        {

        }
    }
}