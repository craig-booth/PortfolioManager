using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service
{

    public class StockSetting
    {
        public string ASXCode { get; private set; }
        public bool DRPActive { get; set; }

        public StockSetting(string asxCode)
        {
            ASXCode = asxCode;
            DRPActive = false;
        }
    }

    public class Portfolio: IEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public CashAccount CashAccount { get; private set; }

        public Dictionary<string, StockSetting> StockSetting { get; private set; }

        public ParcelService ParcelService { get; private set; }
        public ShareHoldingService ShareHoldingService { get; private set; }
        public TransactionService TransactionService { get; private set; }
        public IncomeService IncomeService { get; private set; }
        public CGTService CGTService { get; private set; }
        public CorporateActionService CorporateActionService { get; private set; }

        protected internal Portfolio(string name, IPortfolioDatabase database, StockService stockService, StockPriceService stockPriceService, ICorporateActionQuery corporateActionQuery)
        {
            Id = Guid.NewGuid();
            Name = name;

            StockSetting = new Dictionary<string, StockSetting>();
            CashAccount = new CashAccount();

            ParcelService = new ParcelService(database.PortfolioQuery, stockService);
            ShareHoldingService = new ShareHoldingService(ParcelService, stockService, stockPriceService);
            TransactionService = new TransactionService(database, ParcelService, stockService);
            IncomeService = new IncomeService(database.PortfolioQuery);
            CGTService = new CGTService(database.PortfolioQuery);
            CorporateActionService = new CorporateActionService(corporateActionQuery, ParcelService, stockService, TransactionService);
        }
    }
}


