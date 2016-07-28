using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Data.SQLite
{
    class SQLiteUtils
    {
        public static int DecimalToDB(decimal value)
        {
            return (int)Math.Floor(value * 100000);
        }

        public static decimal DBToDecimal(int value)
        {
            return (decimal)value / 100000;
        }

        public static string BoolToDb(bool value)
        {
            return value ? "Y" : "N";
        }

        public static bool DBToBool(string value)
        {
            return (value == "Y");
        }
    }
}
