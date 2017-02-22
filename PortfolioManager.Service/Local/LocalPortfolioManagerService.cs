using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using StockManager.Service;

using PortfolioManager.Service.Interface;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.Local
{
    public class LocalPortfolioManagerService : PortfolioManagerService
    {

        public LocalPortfolioManagerService(IPortfolioDatabase portfolioDatabase, IStockDatabase stockDatabase)
        {
            var stockServiceRepository = new StockServiceRepository(stockDatabase);
            var stockQuery = stockDatabase.StockQuery;
            var corporateActionQuery = stockDatabase.CorporateActionQuery;

            var settingsService = new PortfolioSettingsService();

            var stockService = new PortfolioManager.Service.Obsolete.StockService(stockServiceRepository);
            var parcelService = new ParcelService(portfolioDatabase.PortfolioQuery, stockService);
            var attachmentService = new AttachmentService(portfolioDatabase);
            var transactionService = new TransactionService(portfolioDatabase, parcelService, stockService, attachmentService);
            var cashAccountService = new CashAccountService(portfolioDatabase);
            var shareHoldingService = new ShareHoldingService(parcelService, stockService, transactionService, cashAccountService);
            var incomeService = new IncomeService(portfolioDatabase.PortfolioQuery, stockService, settingsService);
            var cgtService = new CGTService(portfolioDatabase.PortfolioQuery);
            var corporateActionService = new PortfolioManager.Service.Obsolete.CorporateActionService(corporateActionQuery, parcelService, stockService, transactionService, shareHoldingService, incomeService);

            Register<IPortfolioSummaryService>(() => new PortfolioSummaryService(shareHoldingService, cashAccountService));
            Register<IPortfolioPerformanceService>(() => new PortfolioPerformanceService(shareHoldingService, cashAccountService, transactionService, stockService, incomeService));
            Register<ICapitalGainService>(() => new CapitalGainService(portfolioDatabase.PortfolioQuery, stockService, transactionService, cgtService));
            Register<IPortfolioValueService>(() => new PortfolioValueService(portfolioDatabase.PortfolioQuery, stockService, cashAccountService));
            Register<ICorporateActionService>(() => new CorporateActionService(corporateActionService, corporateActionQuery, stockService));
        }

    }
}
