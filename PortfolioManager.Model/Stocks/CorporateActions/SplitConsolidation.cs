using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Stocks
{
    public class SplitConsolidation : ICorporateAction
    {
        public Guid Id { get; private set; }
        public Guid Stock { get; private set; }
        public DateTime ActionDate { get; set; }
        public int OldUnits { get; set; }
        public int NewUnits { get; set; }
        public string Description { get; set; }

        public CorporateActionType Type
        {
            get
            {
                return CorporateActionType.SplitConsolidation;
            }
        }

        public SplitConsolidation(Guid stock, DateTime actionDate, int oldUnits, int newUnits, string description)
            : this(Guid.NewGuid(), stock, actionDate, oldUnits, newUnits, description)
        {
        }

        public SplitConsolidation(Guid id, Guid stock, DateTime actionDate, int oldUnits, int newUnits, string description)
        {
            Id = id;
            ActionDate = actionDate;
            Stock = stock;
            OldUnits = oldUnits;
            NewUnits = newUnits;
            if (description != "")
                Description = description;
            else
            {
                if (NewUnits > OldUnits)
                    Description = string.Format("Stock split ratio {0}:{1}", OldUnits, NewUnits);
                else
                    Description = string.Format("Stock consolidation ratio {0}:{1}", OldUnits, NewUnits);
            }
        }
    }
}

