using System;


namespace PortfolioManager.Data.Stocks
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
