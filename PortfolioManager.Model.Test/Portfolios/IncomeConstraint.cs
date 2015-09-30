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
    public class IncomeReceivedComparer : IEqualityComparer<IncomeReceived>
    {
        public bool Equals(IncomeReceived income1, IncomeReceived income2)
        {
         /*   return income1.FromDate == income2.FromDate &&
                   income1.ToDate == income2.ToDate &&
                   income1.Stock == income2.Stock &&
                   income1.AquisitionDate == income2.AquisitionDate &&
                   income1.Units == income2.Units &&
                   income1.UnitPrice == income2.UnitPrice &&
                   income1.CostBase == income2.CostBase &&
                   income1.Event == income2.Event; */
            return false;
        }

        public int GetHashCode(IncomeReceived income)
        {
            return income.Id.GetHashCode();
        }
    }

    public class IncomeReceviedWriter : IEntityWriter<IncomeReceived>
    {
        public void Write(MessageWriter writer, IncomeReceived income)
        {
            writer.Write("<IncomeReceived>");
        }
    }
}
