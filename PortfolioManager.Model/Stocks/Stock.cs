using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Stocks
{
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


    public class Stock: EffectiveDatedEntity, IEditableEffectiveDatedEntity<Stock>  
    {
        public string ASXCode { get; set; }
        public string Name { get; set; }
        public StockType Type { get; set; }
        public Guid ParentId { get; set; }
        public RoundingRule DividendRoundingRule { get; set; }
        public DRPMethod DRPMethod { get; set; }
        public AssetCategory Category { get; set; }

        public Stock(DateTime fromDate, string asxCode, string name, StockType type, Guid parent)
            : this(Guid.NewGuid(), fromDate, DateUtils.NoEndDate, asxCode, name, type, parent)

        {
        }

        public Stock(Guid id, DateTime fromDate, DateTime toDate, string asxCode, string name, StockType type, Guid parent)
            : this(id, fromDate, toDate, asxCode, name, type, parent, RoundingRule.Round, DRPMethod.Round, AssetCategory.AustralianStocks)
        {
        }

        public Stock(Guid id, DateTime fromDate, DateTime toDate, string asxCode, string name, StockType type, Guid parent, RoundingRule dividendRoundingRule, DRPMethod drpMethod, AssetCategory category)
            : base(id, fromDate, toDate)
        {
            ASXCode = asxCode;
            Name = name;
            Type = type;
            ParentId = parent;
            DividendRoundingRule = dividendRoundingRule;
            DRPMethod = drpMethod;
            Category = category;
        }

        public override string ToString()
        {
            return ASXCode + " - " + Name;
        }

        public Stock CreateNewEffectiveEntity(DateTime atDate)
        {
            return new Stock(this.Id, atDate, this.ToDate, this.ASXCode, this.Name, this.Type, this.ParentId, this.DividendRoundingRule, this.DRPMethod, this.Category);
        }

    }
 
}
