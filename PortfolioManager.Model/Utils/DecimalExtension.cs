using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Utils
{
    public class MathUtils
    {
        public static decimal Truncate(decimal value)
        {
            int cents = (int)(value * 100);

            return ((decimal)cents / 100);
        }

    }
}
