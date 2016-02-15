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
    public class IncomeComparer : IEntityComparer<Income>
    {
        public bool Equals(Income income1, Income income2)
        {
            return ((income1.ASXCode == income2.ASXCode) &&
                (income1.FrankedAmount == income2.FrankedAmount) &&
                (income1.UnfrankedAmount == income2.UnfrankedAmount) &&
                (income1.FrankingCredits == income2.FrankingCredits) &&
                (income1.Interest == income2.Interest) &&
                (income1.TaxDeferred == income2.TaxDeferred)); 
        }

        public void Write(MessageWriter writer, Income income)
        {
            writer.Write("<IncomeReceived:- ASXCode: {0}, FrankedAmount: {1}, UnfrankedAmount: {2}, FrankingCredits: {3}, Interest: {4}, TaxDeferred: {5}>",
                        new object[] { income.ASXCode, income.FrankedAmount, income.UnfrankedAmount, income.FrankingCredits, income.Interest, income.TaxDeferred });
        }
    }

}
