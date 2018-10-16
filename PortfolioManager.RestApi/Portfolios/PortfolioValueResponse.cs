using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Portfolios
{
    public class PortfolioValueResponse
    {
        public struct ValueItem
        {
            public DateTime Date;
            public decimal Amount;

            public ValueItem(DateTime date, decimal amount)
            {
                Date = date;
                Amount = amount;
            }
        }

        public ValueItem[] Values { get; set; }
    }
}
