using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.RestApi.Responses
{
    public class StockHistoryResponse 
    {
        public Guid Id { get; set; }
        public string ASXCode { get; set; }
        public string Name { get; set; }

        public class HistoricProperties
        {
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string ASXCode { get; set; }
            public string Name { get; set; }

            public AssetCategory Category { get; set; }
        }

        public List<HistoricProperties> History { get; set; }

        public class HistoricDRP
        {
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public bool Active { get; set; }
            public DRPMethod DRPMethod { get; set; }
            public RoundingRule DividendRoundingRule { get; set; }
        }

        public List<HistoricDRP> DRP { get; set; }

        public StockHistoryResponse()
        {
            History = new List<HistoricProperties>();
            DRP = new List<HistoricDRP>();
        }

    }
}
