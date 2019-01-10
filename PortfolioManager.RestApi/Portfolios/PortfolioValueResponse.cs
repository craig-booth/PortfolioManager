using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Portfolios
{
    public enum ValueFrequency { Daily, Weekly, Monthly };

    public class PortfolioValueResponse
    {
        public List<ValueItem> Values { get; } = new List<ValueItem>();

        public class ValueItem
        {
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
        }     
    }
}
