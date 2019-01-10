using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.UI.Models
{
    public abstract class Transaction
    {
        public Guid Id { get; set; }
        public Stock Stock { get; set; }
        public TransactionType Type { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime RecordDate { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public Guid Attachment { get; set; }
    }

}
