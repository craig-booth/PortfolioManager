﻿using System;

using PortfolioManager.Common;

namespace PortfolioManager.Data.Stocks
{
    public class SplitConsolidation : CorporateAction
    {
        public int OldUnits { get; set; }
        public int NewUnits { get; set; }

        public SplitConsolidation(Guid stock, DateTime actionDate, int oldUnits, int newUnits, string description)
            : this(Guid.NewGuid(), stock, actionDate, oldUnits, newUnits, description)
        {
        }

        public SplitConsolidation(Guid id, Guid stock, DateTime actionDate, int oldUnits, int newUnits, string description)
            : base(id, CorporateActionType.SplitConsolidation, stock, actionDate)
        {
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

