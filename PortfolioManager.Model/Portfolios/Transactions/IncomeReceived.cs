using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{

    public class IncomeReceived: ITransaction  
    {
        public Guid Id { get; private set; }
        public DateTime TransactionDate { get; set; }
        public string ASXCode { get; set; }
        public decimal FrankedAmount { get;  set; }
        public decimal UnfrankedAmount { get; set; }
        public decimal FrankingCredits { get;  set; }
        public decimal Interest { get;  set; }
        public decimal TaxDeferred { get; set; }
        public string Comment { get; set; }

        public string Description
        { 
            get 
            {
                return "Income received " + CashIncome.ToString("c");
            } 
        }

        public TransactionType Type
        {
            get
            {
                return TransactionType.Income;
            }
        } 

        public decimal CashIncome
        {
            get { return FrankedAmount + UnfrankedAmount + Interest + TaxDeferred; }
        }

        public decimal NonCashIncome
        {
            get { return FrankingCredits; }
        }

        public decimal TotalIncome
        {
            get { return CashIncome + NonCashIncome; }
        }

        public IncomeReceived()
        {
            Id = Guid.NewGuid();
        }

    }
    
}
