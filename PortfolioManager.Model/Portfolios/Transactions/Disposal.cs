using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    public class Disposal : ITransaction
    {
        public Guid Id { get; private set; }
        public DateTime TransactionDate { get; set; }
        public Guid Stock { get; set; }
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public CGTCalculationMethod CGTMethod { get; set; }
        public string Comment { get; set; }

        public string Description
        {
            get
            {
                return "Disposed of " + Units.ToString("n0") + " shares @ " + AveragePrice.ToString("c");
            }
        }

        public Disposal()
        {
            Id = Guid.NewGuid();
        }

        public Disposal(DateTime transactionDate, Guid stock, int units, decimal averagePrice, decimal transactionCosts, CGTCalculationMethod cgtMethod, string comment)
        {
            TransactionDate = transactionDate;
            Stock = stock;
            Units = units;
            AveragePrice = averagePrice;
            TransactionCosts = transactionCosts;
            Comment = comment;
            CGTMethod = cgtMethod;
        }

        public TransactionType Type
        {
            get
            {
                return TransactionType.Disposal;
            }
        }

    }
}
