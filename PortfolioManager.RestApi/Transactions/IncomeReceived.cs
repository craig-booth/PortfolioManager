using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.RestApi.Transactions
{
    public class IncomeReceived : Transaction
    {
        public override string Type => TransactionType.Income.ToRestName();
        public DateTime RecordDate { get; set; }
        public decimal FrankedAmount { get; set; }
        public decimal UnfrankedAmount { get; set; }
        public decimal FrankingCredits { get; set; }
        public decimal Interest { get; set; }
        public decimal TaxDeferred { get; set; }
        public bool CreateCashTransaction { get; set; }
        public decimal DRPCashBalance { get; set; }
    }
}
