using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.UI.Models
{
    public class ReturnOfCapitalTransaction : Transaction
    {
        public decimal Amount { get; set; }
        public bool CreateCashTransaction { get; set; }
    }
}
