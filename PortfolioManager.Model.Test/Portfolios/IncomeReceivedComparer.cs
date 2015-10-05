using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework.Constraints;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Test.Portfolios
{
    public class IncomeReceivedComparer : IEntityComparer<IncomeReceived>
    {
        public bool Equals(IncomeReceived income1, IncomeReceived income2)
        {
            return ((income1.TransactionDate == income2.TransactionDate) &&
                (income1.ASXCode == income2.ASXCode) &&
                (income1.FrankedAmount == income2.FrankedAmount) &&
                (income1.UnfrankedAmount == income2.UnfrankedAmount) &&
                (income1.FrankingCredits == income2.FrankingCredits) &&
                (income1.Interest == income2.Interest) &&
                (income1.TaxDeferred == income2.TaxDeferred)); 
        }

        public void Write(MessageWriter writer, IncomeReceived income)
        {
            writer.Write("<IncomeReceived:- TransactionDate: {0:d}, ASXCode: {1}, FrankedAmount: {2}, UnfrankedAmount: {3}, FrankingCredits: {4}, Interest: {5}, TaxDeferred: {6}>",
                        new object[] { income.TransactionDate, income.ASXCode, income.FrankedAmount, income.UnfrankedAmount, income.FrankingCredits, income.Interest, income.TaxDeferred });
        }
    }

}
