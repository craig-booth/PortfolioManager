using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Transactions;

namespace PortfolioManager.Service.Local
{
    public class TransactionService : ITransactionService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly IStockDatabase _StockDatabase;

        public TransactionService(IPortfolioDatabase portfolioDatabase, IStockDatabase stockDatabase)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockDatabase = stockDatabase;

            Mapper.Initialize(cfg => cfg.AddProfile(new ModelToServiceMapping(stockDatabase)));
            Mapper.AssertConfigurationIsValid();
        }

        public Task<ServiceResponce> AddTransaction(TransactionItem transactionItem)
        {
            var responce = new ServiceResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                using (var stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
                {
                    transactionItem.Id = Guid.NewGuid();
                    var transaction = Mapper.Map<Transaction>(transactionItem);

                    var transactionHandlerFactory = new TransactionHandlerFactory(portfolioUnitOfWork.PortfolioQuery, stockUnitOfWork.StockQuery);

                    var handler = transactionHandlerFactory.GetHandler(transaction);
                    if (handler != null)
                    {
                        handler.ApplyTransaction(portfolioUnitOfWork, transaction);
                        portfolioUnitOfWork.TransactionRepository.Add(transaction);
                    }
                    else
                    {
                        throw new NotSupportedException("Transaction type not supported");
                    }

                    portfolioUnitOfWork.Save();

                    responce.SetStatusToSuccessfull();
                }

            }

            return Task.FromResult<ServiceResponce>(responce); 
        }

        public Task<ServiceResponce> DeleteTransaction(Guid id)
        {
            var responce = new ServiceResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                portfolioUnitOfWork.TransactionRepository.Delete(id);
                portfolioUnitOfWork.Save();

                responce.SetStatusToSuccessfull();               
            }

            return Task.FromResult<ServiceResponce>(responce); 
        }

        public Task<ServiceResponce> UpdateTransaction(TransactionItem transactionItem)
        {
            var responce = new ServiceResponce();

            var transaction = Mapper.Map<Transaction>(transactionItem);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Update(transaction);
                unitOfWork.Save();
            }

            responce.SetStatusToSuccessfull();

            return Task.FromResult<ServiceResponce>(responce); 
        }

        public Task<GetTransactionResponce> GetTransaction(Guid id)
        {
            var responce = new GetTransactionResponce();

            using (IPortfolioReadOnlyUnitOfWork unitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                responce.Transaction = Mapper.Map<TransactionItem>(unitOfWork.PortfolioQuery.GetTransaction(id));

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<GetTransactionResponce>(responce);
        }

        public Task<GetTransactionsResponce> GetTransactions(DateTime fromDate, DateTime toDate)
        {       
            var responce = new GetTransactionsResponce();

            using (IPortfolioReadOnlyUnitOfWork unitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                var transactions = unitOfWork.PortfolioQuery.GetTransactions(fromDate, toDate);
                responce.Transactions.AddRange(Mapper.Map<IEnumerable<TransactionItem>>(transactions));

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<GetTransactionsResponce>(responce); 
        }

        public Task<GetTransactionsResponce> GetTransactions(Guid stockId, DateTime fromDate, DateTime toDate)
        {
            var responce = new GetTransactionsResponce();

            using (IPortfolioReadOnlyUnitOfWork portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                using (IStockReadOnlyUnitOfWork stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
                {
                    var asxCode = stockUnitOfWork.StockQuery.GetASXCode(stockId, fromDate);

                    var transactions = portfolioUnitOfWork.PortfolioQuery.GetTransactions(asxCode, fromDate, toDate);
                    responce.Transactions.AddRange(Mapper.Map<IEnumerable<TransactionItem>>(transactions));

                    responce.SetStatusToSuccessfull();
                }
            }

            return Task.FromResult<GetTransactionsResponce>(responce);
        }

    }

    class TransactionHandlerFactory
    {
        private readonly ServiceFactory<ITransactionHandler> _TransactionHandlers;
   
        public TransactionHandlerFactory(IPortfolioQuery portfolioQuery, IStockQuery stockQuery)
        {
            _TransactionHandlers = new ServiceFactory<ITransactionHandler>();
            _TransactionHandlers.Register<Aquisition>(() => new AquisitionHandler(portfolioQuery, stockQuery));
            _TransactionHandlers.Register<Disposal>(() => new DisposalHandler(portfolioQuery, stockQuery));
            _TransactionHandlers.Register<CostBaseAdjustment>(() => new CostBaseAdjustmentHandler(portfolioQuery, stockQuery));
            _TransactionHandlers.Register<IncomeReceived>(() => new IncomeReceivedHandler(portfolioQuery, stockQuery));
            _TransactionHandlers.Register<OpeningBalance>(() => new OpeningBalanceHandler(portfolioQuery, stockQuery));
            _TransactionHandlers.Register<ReturnOfCapital>(() => new ReturnOfCapitalHandler(portfolioQuery, stockQuery));
            _TransactionHandlers.Register<UnitCountAdjustment>(() => new UnitCountAdjustmentHandler(portfolioQuery, stockQuery));
            _TransactionHandlers.Register<CashTransaction>(() => new CashTransactionHandler());
        }

        public ITransactionHandler GetHandler(Transaction transaction)
        {
            return _TransactionHandlers.GetService(transaction);
        }
    }

    class ModelToServiceMapping : Profile
    {
        public ModelToServiceMapping(IStockDatabase stockDatabase)
        {
            var stockResolver = new StockResolver(stockDatabase);

            CreateMap<Transaction, TransactionItem>()
                .ForMember(dest => dest.Stock, opts => opts.ResolveUsing(stockResolver))
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
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .Include<AquisitionTransactionItem, Aquisition>()
                .Include<CashTransactionItem, CashTransaction>()
                .Include<CostBaseAdjustmentTransactionItem, CostBaseAdjustment>()
                .Include<DisposalTransactionItem, Disposal>()
                .Include<IncomeTransactionItem, IncomeReceived>()
                .Include<OpeningBalanceTransactionItem, OpeningBalance>()
                .Include<ReturnOfCapitalTransactionItem, ReturnOfCapital>()
                .Include<UnitCountAdjustmentTransactionItem, UnitCountAdjustment>();
            CreateMap<AquisitionTransactionItem, Aquisition>()
                .ConstructUsing(src => new Aquisition(src.Id));
            CreateMap<CashTransactionItem, CashTransaction>()
                .ConstructUsing(src => new CashTransaction(src.Id));
            CreateMap<CostBaseAdjustmentTransactionItem, CostBaseAdjustment>()
                .ConstructUsing(src => new CostBaseAdjustment(src.Id));
            CreateMap<DisposalTransactionItem, Disposal>()
                .ConstructUsing(src => new Disposal(src.Id));
            CreateMap<IncomeTransactionItem, IncomeReceived>()
                .ConstructUsing(src => new IncomeReceived(src.Id));
            CreateMap<OpeningBalanceTransactionItem, OpeningBalance>()
                .ConstructUsing(src => new OpeningBalance(src.Id));
            CreateMap<ReturnOfCapitalTransactionItem, ReturnOfCapital>()
                .ConstructUsing(src => new ReturnOfCapital(src.Id));
            CreateMap<UnitCountAdjustmentTransactionItem, UnitCountAdjustment>()
                .ConstructUsing(src => new UnitCountAdjustment(src.Id));
        }
    }

    public class StockResolver : IValueResolver<Transaction, TransactionItem, StockItem>
    {
        private IStockDatabase _StockDatabase;

        public StockResolver(IStockDatabase stockDatabase)
        {
            _StockDatabase = stockDatabase;
        }

        public StockItem Resolve(Transaction source, TransactionItem destination, StockItem member, ResolutionContext context)
        {
            if (source.ASXCode == "")
                return new StockItem(Guid.Empty, "", "");

            using (var unitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
            {
                try
                {
                    var stock = unitOfWork.StockQuery.GetByASXCode(source.ASXCode, source.TransactionDate);
                    return new StockItem(stock);
                }
                catch
                {
                    return new StockItem(Guid.Empty, source.ASXCode, "");
                }
            }
               
        }
    }
}
