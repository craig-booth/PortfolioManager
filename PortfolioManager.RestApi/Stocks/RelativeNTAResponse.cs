using System;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioManager.RestApi.Stocks
{
    public class RelativeNTAResponse
    {
        public Guid Id { get; set; }
        public string ASXCode { get; set; }
        public string Name { get; set; }

        public List<RelativeNTA> RelativeNTAs { get; }

        public class RelativeNTA
        {
            public DateTime FromDate { get; }
            public DateTime ToDate { get; }
            public ChildSecurityNTA[] RelativeNTAs { get; }

            public RelativeNTA(DateTime fromDate, DateTime toDate, IEnumerable<ChildSecurityNTA> relativeNTAs)
            {
                FromDate = fromDate;
                ToDate = toDate;
                RelativeNTAs = relativeNTAs.ToArray();
            }
        }

        public struct ChildSecurityNTA
        {
            public string ChildSecurity { get; set; }
            public decimal Percentage { get; set;  }

            public ChildSecurityNTA(string childSecurity, decimal percentage)
            {
                ChildSecurity = childSecurity;
                Percentage = percentage;
            }

        }   

        public RelativeNTAResponse()
        {
            RelativeNTAs = new List<RelativeNTA>();
        }

    }
}
