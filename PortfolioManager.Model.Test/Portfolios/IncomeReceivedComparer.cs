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
            return false;
        }

        public void Write(MessageWriter writer, IncomeReceived income)
        {
            writer.Write("<IncomeReceived:- >");
        }
    }

}
