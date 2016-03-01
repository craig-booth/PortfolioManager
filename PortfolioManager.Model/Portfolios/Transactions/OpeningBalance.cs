using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    public class OpeningBalance : ITransaction
    {
        public Guid Id { get; private set; }
        public DateTime TransactionDate { get; set; }
        public string ASXCode { get; set; }
        public int Units { get; set; }
        public decimal CostBase { get; set; }
        public DateTime AquisitionDate { get; set; }
        public string Comment { get; set; }

        public string Description
        {
            get
            {
                return "Opening balance of " + Units.ToString("n0") + " shares";
            }
        }

        public TransactionType Type
        {
            get
            {
                return TransactionType.OpeningBalance;
            }
        }

        public OpeningBalance()
            : this (Guid.NewGuid())
        {

        }

        public OpeningBalance(Guid id)
        {
            Id = id;
        }
    }
}
