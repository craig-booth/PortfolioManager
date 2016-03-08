using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;


namespace PortfolioManager.Model.Stocks
{

    public class RelativeNTA: Entity
    {
        public DateTime Date { get; private set; }
        public Guid Parent { get; private set; }
        public Guid Child { get; private set; }
        public decimal Percentage { get; set; }

        public RelativeNTA(DateTime date, Guid parent, Guid child, decimal percentage)
            : this(Guid.NewGuid(), date, parent, child, percentage)
        {
        }

        public RelativeNTA(Guid id, DateTime date, Guid parent, Guid child, decimal percentage)
            : base(id)
        {
            Date = date;
            Parent = parent;
            Child = child;
            Percentage = percentage;
        }

    }


}
