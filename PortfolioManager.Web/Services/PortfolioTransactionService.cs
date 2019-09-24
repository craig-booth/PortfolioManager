using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Transactions;

using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.Services
{

    public interface IPortfolioTransactionService
    {
        RestApi.Transactions.Transaction GetTransaction(Guid portfolioId, Guid id);

        RestApi.Portfolios.TransactionsResponse GetTransactions(Guid portfolioId, DateRange dateRange);
        RestApi.Portfolios.TransactionsResponse GetTransactions(Guid portfolioId, Guid stockId, DateRange dateRange);
   
        void ApplyTransaction(Guid portfolioId, RestApi.Transactions.Aquisition aquisition);
        void ApplyTransaction(Guid portfolioId, RestApi.Transactions.CashTransaction cashTransaction);
        void ApplyTransaction(Guid portfolioId, RestApi.Transactions.CostBaseAdjustment costBaseAdjustment);
        void ApplyTransaction(Guid portfolioId, RestApi.Transactions.Disposal disposal);
        void ApplyTransaction(Guid portfolioId, RestApi.Transactions.IncomeReceived incomeReceived);
        void ApplyTransaction(Guid portfolioId, RestApi.Transactions.OpeningBalance openingBalance);
        void ApplyTransaction(Guid portfolioId, RestApi.Transactions.ReturnOfCapital returnOfCapital);
        void ApplyTransaction(Guid portfolioId, RestApi.Transactions.UnitCountAdjustment unitCountAdjustment);
    }

    public class PortfolioTransactionService : IPortfolioTransactionService
    {
        private readonly IStockQuery _StockQuery;
        private readonly IRepository<Portfolio> _PortfolioRepository;
        private readonly IMapper _Mapper;
        private readonly IPortfolioCache _PortfolioCache;

        public PortfolioTransactionService(IPortfolioCache portfolioCache, IRepository<Portfolio> portfolioRepository, IStockQuery stockQuery, IMapper mapper)
        {
            _PortfolioCache = portfolioCache;
            _PortfolioRepository = portfolioRepository;
            _StockQuery = stockQuery;
            _Mapper = mapper;
        }

        public RestApi.Transactions.Transaction GetTransaction(Guid portfolioId, Guid id)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var transaction = portfolio.Transactions[id];
            if (transaction == null)
                throw new TransactionNotFoundException(id);

            return _Mapper.Map<RestApi.Transactions.Transaction>(transaction);
        }

        public RestApi.Portfolios.TransactionsResponse GetTransactions(Guid portfolioId, DateRange dateRange)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            return GetTransactions(portfolio.Transactions.InDateRange(dateRange), dateRange.ToDate);
        }

        public RestApi.Portfolios.TransactionsResponse GetTransactions(Guid portfolioId, Guid stockId, DateRange dateRange)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var holding = portfolio.Holdings.Get(stockId);
            if (holding == null)
                throw new HoldingNotFoundException(stockId);

            return GetTransactions(portfolio.Transactions.ForHolding(holding.Stock.Id, dateRange), dateRange.ToDate);
        }

        private RestApi.Portfolios.TransactionsResponse GetTransactions(IEnumerable<Transaction> transactions, DateTime date)
        {
            var response = new RestApi.Portfolios.TransactionsResponse();

            foreach (var transaction in transactions)
            {
                var t = _Mapper.Map<RestApi.Portfolios.TransactionsResponse.TransactionItem>(transaction, opts => opts.Items["date"] = date);
                response.Transactions.Add(t);
            }

            return response;
        }

        public void ApplyTransaction(Guid portfolioId, RestApi.Transactions.Aquisition aquisition)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var stock = _StockQuery.Get(aquisition.Stock);
            portfolio.AquireShares(aquisition.TransactionDate, stock, aquisition.Units, aquisition.AveragePrice, aquisition.TransactionCosts, aquisition.CreateCashTransaction, aquisition.Comment, aquisition.Id);

            _PortfolioRepository.Update(portfolio);
        }

        public void ApplyTransaction(Guid portfolioId, RestApi.Transactions.CashTransaction cashTransaction)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            portfolio.MakeCashTransaction(cashTransaction.TransactionDate, PortfolioManager.RestApi.Transactions.RestApiNameMapping.ToBankAccountTransactionType(cashTransaction.CashTransactionType), cashTransaction.Amount, cashTransaction.Comment, cashTransaction.Id);

            _PortfolioRepository.Update(portfolio);
        }

        public void ApplyTransaction(Guid portfolioId, RestApi.Transactions.CostBaseAdjustment costBaseAdjustment)
        {

        }

        public void ApplyTransaction(Guid portfolioId, RestApi.Transactions.Disposal disposal)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var stock = _StockQuery.Get(disposal.Stock);
            portfolio.DisposeOfShares(disposal.TransactionDate, stock, disposal.Units, disposal.AveragePrice, disposal.TransactionCosts, RestApi.Transactions.RestApiNameMapping.ToCGTCalculationMethod(disposal.CGTMethod), disposal.CreateCashTransaction, disposal.Comment, disposal.Id);

            _PortfolioRepository.Update(portfolio);
        }

        public void ApplyTransaction(Guid portfolioId, RestApi.Transactions.IncomeReceived incomeReceived)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var stock = _StockQuery.Get(incomeReceived.Stock);
            portfolio.IncomeReceived(incomeReceived.RecordDate, incomeReceived.TransactionDate, stock, incomeReceived.FrankedAmount, incomeReceived.UnfrankedAmount, incomeReceived.FrankingCredits, incomeReceived.Interest, incomeReceived.TaxDeferred, incomeReceived.DRPCashBalance, incomeReceived.CreateCashTransaction, incomeReceived.Comment, incomeReceived.Id);

            _PortfolioRepository.Update(portfolio);
        }

        public void ApplyTransaction(Guid portfolioId, RestApi.Transactions.OpeningBalance openingBalance)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var stock = _StockQuery.Get(openingBalance.Stock);
            portfolio.AddOpeningBalance(openingBalance.TransactionDate, openingBalance.AquisitionDate, stock, openingBalance.Units, openingBalance.CostBase, openingBalance.Comment, openingBalance.Id);

            _PortfolioRepository.Update(portfolio);
        }

        public void ApplyTransaction(Guid portfolioId, RestApi.Transactions.ReturnOfCapital returnOfCapital)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var stock = _StockQuery.Get(returnOfCapital.Stock);
            portfolio.ReturnOfCapitalReceived(returnOfCapital.TransactionDate, returnOfCapital.RecordDate, stock, returnOfCapital.Amount, returnOfCapital.CreateCashTransaction, returnOfCapital.Comment, returnOfCapital.Id);

            _PortfolioRepository.Update(portfolio);
        }

        public void ApplyTransaction(Guid portfolioId, RestApi.Transactions.UnitCountAdjustment unitCountAdjustment)
        {

        }
    }
}
