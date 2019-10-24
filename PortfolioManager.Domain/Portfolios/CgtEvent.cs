using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Utils;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.Portfolios
{
    public class CgtEvent : ITransaction
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Stock Stock { get; set; }
        public int Units { get; set; }
       
        public decimal CostBase { get; set; }
        public decimal AmountReceived { get; set; }
        public decimal CapitalGain { get; set; }
        public CGTMethod CgtMethod { get; set; }
    }
}
