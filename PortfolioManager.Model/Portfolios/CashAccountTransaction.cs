using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{

    public enum CashAccountTransactionType
    {
        Deposit,
        Withdrawl,
        Transfer,
        Fee,
        Interest
    }

    public class CashAccountTransaction  : Entity
    {
        public CashAccountTransactionType Type { get; set; }
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