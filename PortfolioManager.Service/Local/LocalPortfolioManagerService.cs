using System;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;

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

        public Task<bool> Connect(string portfolioDatabasePath, string stockDatabasePath)
        {
            IStockDatabase stockDatabase = new SQLiteStockDatabase(stockDatabasePath);
            IPortfolioDatabase portfolioDatabase = new SQLitePortfolioDatabase(portfolioDatabasePath);
           
            _ServiceFactory.Clear();
            _ServiceFactory.Register<IPortfolioSummaryService>(() => new PortfolioSummaryService(portfolioDatabase, stockDatabase));
            _ServiceFactory.Register<IPortfolioPerformanceService>(() => new PortfolioPerformanceService(portfolioDatabase, stockDatabase));
            _ServiceFactory.Register<ICapitalGainService>(() => new CapitalGainService(portfolioDatabase, stockDatabase));
            _ServiceFactory.Register<IPortfolioValueService>(() => new PortfolioValueService(portfolioDatabase, stockDatabase));
            _ServiceFactory.Register<ICorporateActionService>(() => new CorporateActionService(portfolioDatabase, stockDatabase));
            _ServiceFactory.Register<ITransactionService>(() => new TransactionService(portfolioDatabase, stockDatabase));
            _ServiceFactory.Register<IHoldingService>(() => new HoldingService(portfolioDatabase, stockDatabase));
            _ServiceFactory.Register<ICashAccountService>(() => new CashAccountService(portfolioDatabase));
            _ServiceFactory.Register<IIncomeService>(() => new IncomeService(portfolioDatabase, stockDatabase));
            _ServiceFactory.Register<IStockService>(() => new StockService(stockDatabase));
            _ServiceFactory.Register<IAttachmentService>(() => new AttachmentService(portfolioDatabase)); 

      /*      SetMapping(stockQuery); 
              Mapper.AssertConfigurationIsValid();*/

           // LoadTransactions(portfolioDatabase);

            return Task.FromResult<bool>(true);
        }

        public T GetService<T>() where T : IPortfolioService
        {
            return (T)_ServiceFactory.GetService<T>();
        }

        private void SetMapping(IStockQuery stockQuery)
        {
            Mapper.Initialize(cfg => cfg.AddProfile(new ModelToServiceMapping()));
        }

    }


    class ModelToServiceMapping : Profile
    {
     //   private readonly IStockQuery _StockQuery;

        public ModelToServiceMapping()//IStockQuery stockQuery)
        {
    //        _StockQuery = stockQuery;

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
       //     try
       //     {
       //         var stock = _StockQuery.GetByASXCode(transaction.ASXCode, transaction.TransactionDate);
       //         return new StockItem(stock);
       //     }
       //     catch
       //     {
                return new StockItem(Guid.Empty, transaction.ASXCode, "");
        //    }
             
            
        } 
    }
}
