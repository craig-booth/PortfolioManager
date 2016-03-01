using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Stocks
{
    public class CapitalReturn : ICorporateAction
    {
        public Guid Id { get; private set; }
        public Guid Stock { get; private set; }
        public DateTime ActionDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

        public CorporateActionType Type
        {
            get
            {
                return CorporateActionType.CapitalReturn;
            }
        }

        public CapitalReturn(Guid stock, DateTime actionDate, DateTime paymentDate, decimal amount, string description)
            : this(Guid.NewGuid(), stock, actionDate, paymentDate, amount, description)
        {
        }

        public CapitalReturn(Guid id, Guid stock, DateTime actionDate, DateTime paymentDate, decimal amount, string description)
        {
            Id = id;
            ActionDate = actionDate;
            Stock = stock;
            PaymentDate = paymentDate;
            Amount = amount;
            if (description != "")
                Description = description;
            else
                Description = "Capital Return " + Amount.ToString("$#,##0.00###");
        }

    }
}
