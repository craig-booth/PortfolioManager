﻿using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.CorporateActions
{
    public class CompositeAction : ICorporateAction
    {
        public Guid Id => throw new NotImplementedException();

        public DateTime ActionDate => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public Stock Stock => throw new NotImplementedException();
    }
}