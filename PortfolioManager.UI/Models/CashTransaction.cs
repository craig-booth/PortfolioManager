using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.UI.Models
{
    public class CashTransaction : Transaction
    {
        public BankAccountTransactionType CashTransactionType { get; set; }
        public decimal Amount { get; set; }
    }
}
