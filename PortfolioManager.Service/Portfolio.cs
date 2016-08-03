using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Service.Utils;


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

    public class Portfolio
    {
        public Dictionary<string, StockSetting> StockSetting { get; private set; }

        public ParcelService ParcelService { get; private set; }
        public ShareHoldingService ShareHoldingService { get; private set; }
        public TransactionService TransactionService { get; private set; }
        public IncomeService IncomeService { get; private set; }
        public CGTService CGTService { get; private set; }
        public CorporateActionService CorporateActionService { get; private set; }
        public AttachmentService AttachmentService { get; private set; }

        public StockService StockService { get; private set; }
        public StockPriceService StockPriceService { get; private set; }

        public Portfolio(IPortfolioDatabase database, IStockQuery stockQuery, ICorporateActionQuery corporateActionQuery)
        {
            StockSetting = new Dictionary<string, StockSetting>();

            StockService = new StockService(stockQuery);

            var stockPriceDownloader = new GoogleStockPriceDownloader();
            StockPriceService = new StockPriceService(stockQuery, stockPriceDownloader);

            ParcelService = new ParcelService(database.PortfolioQuery, StockService);
            ShareHoldingService = new ShareHoldingService(ParcelService, StockService);
            AttachmentService = new AttachmentService(database);
            TransactionService = new TransactionService(database, ParcelService, StockService, AttachmentService);
            IncomeService = new IncomeService(database.PortfolioQuery, StockService);
            CGTService = new CGTService(database.PortfolioQuery);
            CorporateActionService = new CorporateActionService(corporateActionQuery, ParcelService, StockService, TransactionService, ShareHoldingService);

            /* Load transactions */
            var allTransactions = TransactionService.GetTransactions(DateTime.MinValue, DateTime.MaxValue);
            using (IPortfolioUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                foreach (var transaction in allTransactions)
                    TransactionService.ApplyTransaction(unitOfWork, transaction);
                unitOfWork.Save();
            }
        }
    }
}


