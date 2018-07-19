﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.CorporateActions
{
    public class AddCapitalReturnCommand : AddCorporateActionCommand
    {
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
    }
}