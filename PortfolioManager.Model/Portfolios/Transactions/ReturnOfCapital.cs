using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Portfolios
{

    public class ReturnOfCapital : ITransaction
    {
        public Guid Id { get; private set; }
        public DateTime TransactionDate { get; set; }
        public string ASXCode { get; set; }
        public DateTime RecordDate { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
        public Guid Attachment { get; set; }

        public string Description
        {
            get
            {
                return "Return of Capital of " + MathUtils.FormatCurrency(Amount, false, true);
            }
        }

        public TransactionType Type
        {
            get
            {
                return TransactionType.ReturnOfCapital;
            }
        }

        public ReturnOfCapital()
            : this (Guid.NewGuid())
        {

        }

        public ReturnOfCapital(Guid id)
        {
            Id = id;
        }

    }
}
