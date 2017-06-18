using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Data;

namespace PortfolioManager.Data.Portfolios
{ 

    public abstract class Transaction : Entity
    {
        public TransactionType Type { get; protected set; }
        public DateTime TransactionDate { get; set; }
        public string ASXCode { get; set; }
        public DateTime RecordDate { get; set; }
        public Guid Attachment { get; set; }
        public string Comment { get; set; }

        public string Description
        {
            get
            {
                return GetDescription();
            }
        }

        public Transaction(Guid id)
            : base(id)
        {
        }

        protected abstract string GetDescription();
    }

}
