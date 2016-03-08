﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public enum TransactionType { Aquisition, Disposal, CostBaseAdjustment, OpeningBalance, ReturnOfCapital, Income, UnitCountAdjustment, Deposit, Withdrawl, Interest, Fee }

    public abstract class Transaction : Entity
    {
        public TransactionType Type { get; protected set; }
        public DateTime TransactionDate { get; set; }
        public string ASXCode { get; set; }
        public Guid Attachment { get; set; }
        

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
