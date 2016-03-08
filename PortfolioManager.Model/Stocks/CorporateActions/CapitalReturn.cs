using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Stocks
{
    public class CapitalReturn : CorporateAction
    {
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }

        public CapitalReturn(Guid stock, DateTime actionDate, DateTime paymentDate, decimal amount, string description)
            : this(Guid.NewGuid(), stock, actionDate, paymentDate, amount, description)
        {
        }

        public CapitalReturn(Guid id, Guid stock, DateTime actionDate, DateTime paymentDate, decimal amount, string description)
            : base(id, CorporateActionType.CapitalReturn, stock, actionDate)
        {
            PaymentDate = paymentDate;
            Amount = amount;
            if (description != "")
                Description = description;
            else
                Description = "Capital Return " + Amount.ToString("$#,##0.00###");
        }

    }
}
