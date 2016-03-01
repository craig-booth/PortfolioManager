using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Stocks
{
    public enum StockType {Ordinary, StapledSecurity, Trust}

    [Serializable]
    public class NotStapledSecurityException : Exception
    {
        public NotStapledSecurityException(string asxCode)
            : base(asxCode + " is not a stapled security.")
        {
        }
    }

    [Serializable]
    public class NotStapledSecurityComponentException : Exception
    {
        public NotStapledSecurityComponentException(string asxCode)
            : base(asxCode + " is not a component stock of a stapled security.")
        {
        }
    }


    public class Stock: IEffectiveDatedEntity 
    {
        public Guid Id { get; private set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ASXCode { get; set; }
        public string Name { get; set; }
        public StockType Type { get; set; }
        public Guid ParentId { get; set; }
        public RoundingRule DividendRoundingRule { get; set; }

        public Stock(DateTime fromDate, string asxCode, string name, StockType type, Guid parent)
            : this(Guid.NewGuid(), fromDate, DateTimeConstants.NoEndDate, asxCode, name, type, parent)

        {
        }

        public Stock(Guid id, DateTime fromDate, DateTime toDate, string asxCode, string name, StockType type, Guid parent)
            : this(id, fromDate, toDate, asxCode, name, type, parent, RoundingRule.Round)
        {
        }

        public Stock(Guid id, DateTime fromDate, DateTime toDate, string asxCode, string name, StockType type, Guid parent, RoundingRule dividendRoundingRule)
        {
            Id = id;
            FromDate = fromDate;
            ToDate = toDate;
            ASXCode = asxCode;
            Name = name;
            FromDate = fromDate;
            Type = type;
            ParentId = parent;
            DividendRoundingRule = dividendRoundingRule;
        }

        public override string ToString()
        {
            return ASXCode + " - " + Name;
        }

        public Stock Clone()
        {
            return new Stock(this.Id, this.FromDate, this.ToDate, this.ASXCode, this.Name, this.Type, this.ParentId, this.DividendRoundingRule);
        }

    }
 
}
