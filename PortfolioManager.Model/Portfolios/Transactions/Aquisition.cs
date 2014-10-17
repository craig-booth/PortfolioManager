using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;


namespace PortfolioManager.Model.Portfolios
{
    public class Aquisition : ITransaction
    {
        public Guid Id { get; private set; }
        public DateTime TransactionDate { get; set; }
        public string ASXCode { get; set; }
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public string Comment { get; set; }

        public string Description
        {
            get
            {
                return "Aquired " + Units.ToString("n0") + " shares @ " + AveragePrice.ToString("c");
            }
        }

        public Aquisition()
        {
            Id = Guid.NewGuid();
        }

        public Aquisition(DateTime transactionDate, string asxCode, int units, decimal averagePrice, decimal transactionCosts, string comment)
        {
            TransactionDate = transactionDate;
            ASXCode = asxCode;
            Units = units;
            AveragePrice = averagePrice;
            TransactionCosts = transactionCosts;
            Comment = comment;
        }

        public TransactionType Type
        {
            get
            {
                return TransactionType.Aquisition;
            }
        }


    }
}
