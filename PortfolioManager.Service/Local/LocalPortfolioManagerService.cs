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

            var settingsService = new Obsolete.PortfolioSettingsService();

            var stockService = new Obsolete.StockService(stockServiceRepository);
            var parcelService = new Obsolete.ParcelService(portfolioDatabase.PortfolioQuery, stockService);
            var attachmentService = new Obsolete.AttachmentService(portfolioDatabase);
            var transactionService = new Obsolete.TransactionService(portfolioDatabase, parcelService, stockService, attachmentService);
            var cashAccountService = new Obsolete.CashAccountService(portfolioDatabase);
            var shareHoldingService = new Obsolete.ShareHoldingService(parcelService, stockService, transactionService, cashAccountService);
            var incomeService = new Obsolete.IncomeService(portfolioDatabase.PortfolioQuery, stockService, settingsService);
            var cgtService = new Obsolete.CGTService(portfolioDatabase.PortfolioQuery);
            var corporateActionService = new Obsolete.CorporateActionService(corporateActionQuery, parcelService, stockService, transactionService, shareHoldingService, incomeService);

            Register<IPortfolioSummaryService>(() => new PortfolioSummaryService(shareHoldingService, cashAccountService));
            Register<IPortfolioPerformanceService>(() => new PortfolioPerformanceService(shareHoldingService, cashAccountService, transactionService, stockService, incomeService));
            Register<ICapitalGainService>(() => new CapitalGainService(portfolioDatabase.PortfolioQuery, stockService, transactionService, cgtService));
            Register<IPortfolioValueService>(() => new PortfolioValueService(portfolioDatabase.PortfolioQuery, stockService, cashAccountService));
            Register<ICorporateActionService>(() => new CorporateActionService(corporateActionService, corporateActionQuery, stockService));
            Register<ITransactionService>(() => new TransactionService(transactionService));
            Register<IHoldingService>(() => new HoldingService(shareHoldingService));
            Register<ICashAccountService>(() => new CashAccountService(cashAccountService));
        }

    }
}
