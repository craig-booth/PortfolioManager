using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Transactions;
using PortfolioManager.Service.Utils;
using PortfolioManager.Domain.Stocks;


namespace PortfolioManager.Service.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly StockExchange _StockExchange;

        public TransactionService(IPortfolioDatabase portfolioDatabase, StockExchange stockExchange)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockExchange = stockExchange;

            Mapper.Initialize(cfg => cfg.AddProfile(new ModelToServiceMapping(_StockExchange)));
            Mapper.AssertConfigurationIsValid();
        }

        public Task<ServiceResponce> AddTransaction(TransactionItem transactionItem)
        {
            var responce = new ServiceResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                transactionItem.Id = Guid.NewGuid();
                var transaction = Mapper.Map<Transaction>(transactionItem);

                var transactionHandlerFactory = new TransactionHandlerFactory(portfolioUnitOfWork.PortfolioQuery, _StockExchange);

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

            return Task.FromResult<ServiceResponce>(responce); 
        }

        public Task<ServiceResponce> AddTransactions(IEnumerable<TransactionItem> transactionItems)
        {
            var responce = new ServiceResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                var transactionHandlerFactory = new TransactionHandlerFactory(portfolioUnitOfWork.PortfolioQuery, _StockExchange);

                foreach (var transactionItem in transactionItems)
                {
                    transactionItem.Id = Guid.NewGuid();
                    var transaction = Mapper.Map<Transaction>(transactionItem);

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
                }

                portfolioUnitOfWork.Save();

                responce.SetStatusToSuccessfull();

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
                var stock = _StockExchange.Stocks.Get(stockId);

                var transactions = portfolioUnitOfWork.PortfolioQuery.GetTransactions(stock.Properties[toDate].ASXCode, fromDate, toDate);
                responce.Transactions.AddRange(Mapper.Map<IEnumerable<TransactionItem>>(transactions));

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<GetTransactionsResponce>(responce); 
        }

        public Task<ServiceResponce> ImportTransactions(string fileName)
        {
            var responce = new ServiceResponce();

            var importer = new TransactionImporter();
            var transactions = importer.ImportTransactions(fileName);

            ImportTransactions(transactions);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<ServiceResponce>(responce); 
        }

        public Task<ServiceResponce> ImportTransactions(TextReader textReader)
        {
            var responce = new ServiceResponce();

            var importer = new TransactionImporter();
            var transactions = importer.ImportTransactions(textReader);

            ImportTransactions(transactions);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<ServiceResponce>(responce); 
        }

        private void ImportTransactions(IEnumerable<Transaction> transactions)
        {
            using (IPortfolioUnitOfWork portfolioUnitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                var transactionHandlerFactory = new TransactionHandlerFactory(portfolioUnitOfWork.PortfolioQuery, _StockExchange);

                foreach (var transaction in transactions)
                {
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
                }

                portfolioUnitOfWork.Save();  
            } 
        }

        public Task<ServiceResponce> ExportTransactions(string fileName, DateTime fromDate, DateTime toDate)
        {
            var responce = new ServiceResponce();

            using (IPortfolioReadOnlyUnitOfWork portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                var exporter = new TransactionExporter();

                var transactions = portfolioUnitOfWork.PortfolioQuery.GetTransactions(fromDate, toDate);

                exporter.ExportTransactions(fileName, transactions);

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<ServiceResponce>(responce); 
        }

        public Task<ServiceResponce> ExportTransactions(TextWriter textWriter, DateTime fromDate, DateTime toDate)
        {
            var responce = new ServiceResponce();

            using (IPortfolioReadOnlyUnitOfWork portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                var exporter = new TransactionExporter();

                var transactions = portfolioUnitOfWork.PortfolioQuery.GetTransactions(fromDate, toDate);

                exporter.ExportTransactions(textWriter, transactions);

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<ServiceResponce>(responce); 
        }
    }

    class TransactionHandlerFactory
    {
        private readonly ServiceFactory<ITransactionHandler> _TransactionHandlers;

        public TransactionHandlerFactory(IPortfolioQuery portfolioQuery, StockExchange stockExchange)
        {
            _TransactionHandlers = new ServiceFactory<ITransactionHandler>();

            _TransactionHandlers.Register<Aquisition>(() => new AquisitionHandler(portfolioQuery, stockExchange))
                .Register<Disposal>(() => new DisposalHandler(portfolioQuery, stockExchange))
                .Register<CostBaseAdjustment>(() => new CostBaseAdjustmentHandler(portfolioQuery, stockExchange))
                .Register<IncomeReceived>(() => new IncomeReceivedHandler(portfolioQuery, stockExchange))
                .Register<OpeningBalance>(() => new OpeningBalanceHandler(portfolioQuery, stockExchange))
                .Register<ReturnOfCapital>(() => new ReturnOfCapitalHandler(portfolioQuery, stockExchange))
                .Register<UnitCountAdjustment>(() => new UnitCountAdjustmentHandler(portfolioQuery, stockExchange))
                .Register<CashTransaction>(() => new CashTransactionHandler());
        }

        public ITransactionHandler GetHandler(Transaction transaction)
        {
            return _TransactionHandlers.GetService(transaction);
        }
    }

    class ModelToServiceMapping : Profile
    {
        public ModelToServiceMapping(StockExchange stockExchange)
        {
            var stockResolver = new StockResolver(stockExchange);

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
        private StockExchange _StockExchange;

        public StockResolver(StockExchange stockExchange)
        {
            _StockExchange = stockExchange;
        }

        public StockItem Resolve(Transaction source, TransactionItem destination, StockItem member, ResolutionContext context)
        {
            if (source.ASXCode == "")
                return new StockItem(Guid.Empty, "", "");

            try
            {
                return _StockExchange.Stocks.Get(source.ASXCode, source.TransactionDate).ToStockItem(source.TransactionDate);
            }
            catch
            {
                return new StockItem(Guid.Empty, source.ASXCode, "");
            }

        }
    }
}
