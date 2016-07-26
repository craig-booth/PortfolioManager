using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Portfolios
{

    public class IncomeReceived: Transaction
    {
        public decimal FrankedAmount { get;  set; }
        public decimal UnfrankedAmount { get; set; }
        public decimal FrankingCredits { get;  set; }
        public decimal Interest { get;  set; }
        public decimal TaxDeferred { get; set; }
        public bool CreateCashTransaction { get; set; }

        public IncomeReceived()
            : this(Guid.NewGuid())
        {

        }

        public IncomeReceived(Guid id)
            : base(id)
        {
            Type = TransactionType.Income;
        }

        protected override string GetDescription()
        {
            return "Income received " + MathUtils.FormatCurrency(CashIncome, false, true);
        }

        public decimal CashIncome
        {
            get { return FrankedAmount + UnfrankedAmount + Interest; }
        }

        public decimal NonCashIncome
        {
            get { return FrankingCredits + TaxDeferred; }
        }

        public decimal TotalIncome
        {
            get { return CashIncome + NonCashIncome; }
        }

    }
    
}
