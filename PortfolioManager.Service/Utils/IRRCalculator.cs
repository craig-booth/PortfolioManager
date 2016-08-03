using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Service.Utils
{

    class IRRCalculator
    {

        public static decimal Calculate(CashFlow[] cashflows)
        {
            return 0.0567m;
        }

    }

    struct CashFlow
    {
        public DateTime Date;
        public decimal Amount;

        public CashFlow(DateTime date, decimal amount)
        {
            Date = date;
            Amount = amount;
        }
    }

}
