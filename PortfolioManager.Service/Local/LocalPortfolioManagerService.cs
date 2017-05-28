using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;

using PortfolioManager.Common;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;
using StockManager.Service;

using PortfolioManager.Service.Interface;
using PortfolioManager.Service.CorporateActions;

namespace PortfolioManager.Service.Local
{
    public class LocalPortfolioManagerService : IPortfolioManagerService
    {
        private ServiceFactory<IPortfolioService> _ServiceFactory = new ServiceFactory<IPortfolioService>();

        public LocalPortfolioManagerService()
        {

        }

        public async Task<bool> Connect(string portfolioDatabasePath, string stockDatabasePath)
        {
            IStockDatabase stockDatabase = new SQLiteStockDatabase(stockDatabasePath);
            IPortfolioDatabase portfolioDatabase = new SQLitePortfolioDatabase(portfolioDatabasePath);

            var stockServiceRepository = new StockServiceRepository(stockDatabase);
            var stockQuery = stockDatabase.StockQuery;
            var corporateActionQuery = stockDatabase.CorporateActionQuery;

            var stockService = new Obsolete.StockService(stockServiceRepository);
            var attachmentService = new Obsolete.AttachmentService(portfolioDatabase);

            _ServiceFactory.Register<IPortfolioSummaryService>(() => new PortfolioSummaryService(portfolioDatabase.PortfolioQuery, stockDatabase.StockQuery, stockDatabase, stockService));
            _ServiceFactory.Register<IPortfolioPerformanceService>(() => new PortfolioPerformanceService(portfolioDatabase.PortfolioQuery, stockDatabase.StockQuery, stockDatabase, stockService));
            _ServiceFactory.Register<ICapitalGainService>(() => new CapitalGainService(portfolioDatabase.PortfolioQuery, stockService));
            _ServiceFactory.Register<IPortfolioValueService>(() => new PortfolioValueService(portfolioDatabase.PortfolioQuery, stockService));
            _ServiceFactory.Register<ICorporateActionService>(() => new CorporateActionService(portfolioDatabase.PortfolioQuery, corporateActionQuery, stockService, new CorporateActionHandlerFactory(portfolioDatabase.PortfolioQuery, stockService)));
            _ServiceFactory.Register<ITransactionService>(() => new TransactionService(portfolioDatabase, stockDatabase, stockService));
            _ServiceFactory.Register<IHoldingService>(() => new HoldingService(portfolioDatabase.PortfolioQuery, stockDatabase.StockQuery, stockDatabase, stockService));
            _ServiceFactory.Register<ICashAccountService>(() => new CashAccountService(portfolioDatabase.PortfolioQuery));
            _ServiceFactory.Register<IIncomeService>(() => new IncomeService(portfolioDatabase.PortfolioQuery, stockService));
            _ServiceFactory.Register<IStockService>(() => new StockService(stockService));

            SetMapping(stockService);
            Mapper.AssertConfigurationIsValid();

            LoadTransactions(portfolioDatabase);

            await stockServiceRepository.DownloadUpdatedData();

            return true;
        }

        public T GetService<T>() where T : IPortfolioService
        {
            return (T)_ServiceFactory.GetService<T>();
        }

        private void LoadTransactions(IPortfolioDatabase database)
        {
            var transactionService = _ServiceFactory.GetService<ITransactionService>() as TransactionService;

            var allTransactions = database.PortfolioQuery.GetTransactions(DateTime.MinValue, DateTime.MaxValue);
            using (IPortfolioUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                foreach (var transaction in allTransactions)
                    transactionService.LoadTransaction(unitOfWork, transaction);
                unitOfWork.Save();
            }
        }

        private void SetMapping(Obsolete.StockService stockService)
        {
            Mapper.Initialize(cfg => cfg.AddProfile(new ModelToServiceMapping(stockService)));
        }

    }


    class ModelToServiceMapping : Profile
    {
        private readonly Obsolete.StockService _StockService;

        public ModelToServiceMapping(Obsolete.StockService stockService)
        {
            _StockService = stockService;

            CreateMap<Transaction, TransactionItem>()
                .ForMember(dest => dest.Stock, opts => opts.MapFrom(src => StockForTransaction(src)))
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
                .ForMember(dest => dest.ASXCode, opts => opts.MapFrom(src => src.Stock.ASXCode))
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

        private StockItem StockForTransaction(Transaction transaction)
        {

            if (transaction.ASXCode == "")
                return new StockItem(Guid.Empty, "", "");
            try
            {
                var stock = _StockService.Get(transaction.ASXCode, transaction.TransactionDate);
                return new StockItem(stock);
            }
            catch
            {
                return new StockItem(Guid.Empty, transaction.ASXCode, "");
            }
             
            
        } 
    }
}
