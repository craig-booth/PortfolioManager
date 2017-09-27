using System;

namespace PortfolioManager.Data.SQLite
{
    static class SQLiteUtils
    {
        public static long DecimalToDB(decimal value)
        {
            return (long)Math.Floor(value * 100000);
        }

        public static decimal DBToDecimal(long value)
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
