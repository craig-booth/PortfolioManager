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
                (income1.PaymentDate == income2.PaymentDate) &&
                (income1.FrankedAmount == income2.FrankedAmount) &&
                (income1.UnfrankedAmount == income2.UnfrankedAmount) &&
                (income1.FrankingCredits == income2.FrankingCredits) &&
                (income1.Interest == income2.Interest) &&
                (income1.TaxDeferred == income2.TaxDeferred)); 
        }

        public void Write(MessageWriter writer, IncomeReceived income)
        {
            writer.Write("<IncomeReceived:- TransactionDate: {0:d}, ASXCode: {1}, PaymentDate: {2: d}, FrankedAmount: {3}, UnfrankedAmount: {4}, FrankingCredits: {5}, Interest: {6}, TaxDeferred: {7}>",
                        new object[] { income.TransactionDate, income.ASXCode, income.PaymentDate, income.FrankedAmount, income.UnfrankedAmount, income.FrankingCredits, income.Interest, income.TaxDeferred });
        }
    }

}
