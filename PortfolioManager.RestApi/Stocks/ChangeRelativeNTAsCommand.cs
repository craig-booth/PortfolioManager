using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Stocks
{
    public class ChangeRelativeNTAsCommand
    {
        public Guid Id { get; set; }
        public DateTime ChangeDate { get; set; }
        public List<RelativeNTA> RelativeNTAs { get; set; }

        public class RelativeNTA
        {
            public string ChildSecurity { get; set; }
            public decimal Percentage { get; set; }
        }

        public ChangeRelativeNTAsCommand()
        {
            RelativeNTAs = new List<RelativeNTA>();
        }
    }
}
