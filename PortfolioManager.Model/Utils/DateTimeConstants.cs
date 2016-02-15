using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Utils
{
    public class DateTimeConstants
    {
        private readonly static DateTime _NoStartDate = new DateTime(0001, 01, 01);
        private readonly static DateTime _NoEndDate = new DateTime(9999, 12, 31);

        public static DateTime NoDate
        {
            get { return _NoStartDate; }
        }

        public static DateTime NoStartDate 
        {
            get { return _NoStartDate; }
        }

        public static DateTime NoEndDate
        {
            get { return _NoEndDate; }
        }
    }
}
