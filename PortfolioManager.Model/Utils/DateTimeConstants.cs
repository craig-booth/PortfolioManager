using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Utils
{
    public class DateTimeConstants
    {
        public static DateTime NoStartDate()
        {
            return new DateTime(0001, 01, 01);
        }

        public static DateTime NoEndDate()
        {
            return new DateTime(9999, 12, 31);
        }
    }
}
