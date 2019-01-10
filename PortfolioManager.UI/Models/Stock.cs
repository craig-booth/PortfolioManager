using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.UI.Models
{
    public class Stock
    {
        public Guid Id { get; set; }
        public string AsxCode { get; set; }
        public string Name { get; set; }

        public Stock(Guid id, string asxCode, string name)
        {
            Id = id;
            AsxCode = asxCode;
            Name = name;
        }

        public Stock(RestApi.Portfolios.Stock stock)
        {
            Id = stock.Id;
            AsxCode = stock.AsxCode;
            Name = stock.Name;
        }

        public Stock(Service.Interface.StockItem stock)
        {
            Id = stock.Id;
            AsxCode = stock.ASXCode;
            Name = stock.Name;
        }

        public string FormattedCompanyName
        {
            get
            {
                if (AsxCode != "")
                    return string.Format("{0} ({1})", Name, AsxCode);
                else
                    return Name;
            }
        }

        public string FormattedAsxCode
        {
            get
            {
                if (Name != "")
                    return string.Format("{0} ({1})", AsxCode, Name);
                else
                    return AsxCode;
            }
        }
    }
}
