using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public enum TransactionType { Aquisition, Disposal, CostBaseAdjustment, OpeningBalance, ReturnOfCapital, Income, UnitCountAdjustment, Deposit, Withdrawl, Interest, Fee }

    public abstract class Transaction : IEntity
    {
        public Guid Id { get; private set; }
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

        public TransactionType Type
        {
            get
            {
                return GetTransactionType();
            }
        }     

        public Transaction()
            : this (Guid.NewGuid())
        {

        }

        public Transaction(Guid id)
        {
            Id = id;
        }

        protected abstract string GetDescription();
        protected abstract TransactionType GetTransactionType();

    }

}
