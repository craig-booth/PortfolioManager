using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Utils
{
    static class StockPriceHistoryTest
    {
        public static void Test()
        {
         /*   var prices = new StockPriceHistory();

            prices.UpdatePrice(new DateTime(2019, 06, 01), 1.00m);
            prices.UpdatePrice(new DateTime(2019, 06, 02), 2.00m);
            prices.UpdatePrice(new DateTime(2019, 06, 03), 3.00m);
            prices.UpdatePrice(new DateTime(2019, 06, 10), 10.00m);
            prices.UpdatePrice(new DateTime(2019, 06, 11), 11.00m);
            prices.UpdatePrice(new DateTime(2019, 06, 15), 15.00m);
            prices.UpdatePrice(new DateTime(2019, 06, 16), 16.00m);
            prices.UpdatePrice(new DateTime(2019, 06, 17), 17.00m);
            prices.UpdatePrice(new DateTime(2019, 06, 20), 20.00m);


            Console.WriteLine("Earliest Date : {0:d}", prices.EarliestDate);
            Console.WriteLine("Latest Date : {0:d}", prices.LatestDate);

            Console.WriteLine("Closing Prices:");
            var date = new DateTime(2019, 05, 30);
            while (date < new DateTime(2019, 06, 22))
            {
                Console.WriteLine("{0:d} : {1}", date, prices.GetPrice(date));
                date = date.AddDays(1);
            }

            TestRange(prices, new DateTime(2019, 05, 20), new DateTime(2019, 06, 22));
            TestRange(prices, new DateTime(2019, 06, 01), new DateTime(2019, 06, 22));
            TestRange(prices, new DateTime(2019, 06, 03), new DateTime(2019, 06, 22));
            TestRange(prices, new DateTime(2019, 06, 05), new DateTime(2019, 06, 22));
            TestRange(prices, new DateTime(2019, 06, 30), new DateTime(2019, 07, 22));

            TestRange(prices, new DateTime(2019, 05, 20), new DateTime(2019, 06, 19));
            TestRange(prices, new DateTime(2019, 06, 01), new DateTime(2019, 06, 20));
            TestRange(prices, new DateTime(2019, 06, 03), new DateTime(2019, 06, 21));
            TestRange(prices, new DateTime(2019, 06, 05), new DateTime(2019, 06, 22));

            Console.ReadKey(); */
        }

        private static void TestRange(StockPriceHistory prices, DateTime from, DateTime to)
        {
            var range = prices.GetPrices(new DateRange(from, to));

            Console.WriteLine("Entries: {0}, First {1:d}, Last {2:d}", range.Count(), range.FirstOrDefault().Key, range.LastOrDefault().Key);
        }
    }
}
