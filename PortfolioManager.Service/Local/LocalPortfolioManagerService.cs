using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;
using StockManager.Service;

using PortfolioManager.Service.Interface;

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

            SetMapping();
        }

        private void SetMapping()
        {
            Mapper.Initialize(cfg => cfg.AddProfile(new ModelToServiceMapping()));
        }

    }


    class ModelToServiceMapping : Profile
    {
        public ModelToServiceMapping()
        {
            CreateMap<Transaction, TransactionItem>()
                .Include<Aquisition, AquisitionTransactionItem>()
                .Include<CashTransaction, CashTransactionItem>()
                .Include<CostBaseAdjustment, CostBaseAdjustmentTransactionItem>()
                .Include<Disposal, DisposalTransactionItem>()
                .Include<IncomeReceived, IncomeTransactionItem>()
                .Include<OpeningBalance, OpeningBalanceTransactionItem>()
                .Include<ReturnOfCapital, ReturnOfCapitalTransactionItem>()
                .Include<UnitCountAdjustment, UnitCountAdjustmentTransactionItem>();
            CreateMap<Aquisition, AquisitionTransactionItem>();
            CreateMap<CashTransaction, CashTransactionItem>();
            CreateMap<CostBaseAdjustment, CostBaseAdjustmentTransactionItem>();
            CreateMap<Disposal, DisposalTransactionItem>();
            CreateMap<IncomeReceived, IncomeTransactionItem>();
            CreateMap<OpeningBalance, OpeningBalanceTransactionItem>();
            CreateMap<ReturnOfCapital, ReturnOfCapitalTransactionItem>();
            CreateMap<UnitCountAdjustment, UnitCountAdjustmentTransactionItem>();

            CreateMap<TransactionItem, Transaction>()
                .Include<AquisitionTransactionItem, Aquisition>()
                .Include<CashTransactionItem, CashTransaction>()
                .Include<CostBaseAdjustmentTransactionItem, CostBaseAdjustment>()
                .Include<DisposalTransactionItem, Disposal>()
                .Include<IncomeTransactionItem, IncomeReceived>()
                .Include<OpeningBalanceTransactionItem, OpeningBalance>()
                .Include<ReturnOfCapitalTransactionItem, ReturnOfCapital>()
                .Include<UnitCountAdjustmentTransactionItem, UnitCountAdjustment>();
            CreateMap<AquisitionTransactionItem, Aquisition>();
            CreateMap<CashTransactionItem, CashTransaction>();
            CreateMap<CostBaseAdjustmentTransactionItem, CostBaseAdjustment>();
            CreateMap<DisposalTransactionItem, Disposal>();
            CreateMap<IncomeTransactionItem, IncomeReceived>();
            CreateMap<OpeningBalanceTransactionItem, OpeningBalance>();
            CreateMap<ReturnOfCapitalTransactionItem, ReturnOfCapital>();
            CreateMap<UnitCountAdjustmentTransactionItem, UnitCountAdjustment>();
        }
    }
}
