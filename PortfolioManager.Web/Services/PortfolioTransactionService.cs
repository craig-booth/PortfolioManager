using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.RestApi.Transactions;
using PortfolioManager.RestApi.Portfolios;

namespace PortfolioManager.Web.Services
{
    public class PortfolioTransactionService
    {
        public Portfolio Portfolio { get; }
        private IStockQuery _StockQuery;
        private IRepository<Portfolio> _PortfolioRepository;
        private IMapper _Mapper;

        public PortfolioTransactionService(Portfolio portfolio, IRepository<Portfolio> portfolioRepository, IStockQuery stockQuery, IMapper mapper)
        {
            Portfolio = portfolio;

            _PortfolioRepository = portfolioRepository;
            _StockQuery = stockQuery;
            _Mapper = mapper;
        }

        public TransactionsResponse GetTransactions(DateRange dateRange)
        {
            return GetTransactions(Portfolio.Transactions.InDateRange(dateRange), dateRange.ToDate);
        }

        public TransactionsResponse GetTransactions(Domain.Portfolios.Holding holding, DateRange dateRange)
        {
            return GetTransactions(Portfolio.Transactions.InDateRange(dateRange).Where(x => x.Stock.Id == holding.Stock.Id), dateRange.ToDate);
        }

        private TransactionsResponse GetTransactions(IEnumerable<Domain.Transactions.Transaction> transactions, DateTime date)
        {
            var response = new TransactionsResponse();

            foreach (var transaction in transactions)
            {
                var t = _Mapper.Map<TransactionsResponse.TransactionItem>(transaction, opts => opts.Items["date"] = date);
                response.Transactions.Add(t);
            }

            return response;
        }

        public void ApplyTransaction(RestApi.Transactions.Aquisition aquisition)
        {
            var stock = _StockQuery.Get(aquisition.Stock);
            Portfolio.AquireShares(aquisition.TransactionDate, stock, aquisition.Units, aquisition.AveragePrice, aquisition.TransactionCosts, aquisition.CreateCashTransaction, aquisition.Comment, aquisition.Id);

            _PortfolioRepository.Update(Portfolio);
        }

        public void ApplyTransaction(RestApi.Transactions.CashTransaction cashTransaction)
        {
            Portfolio.MakeCashTransaction(cashTransaction.TransactionDate, RestApiNameMapping.ToBankAccountTransactionType(cashTransaction.CashTransactionType), cashTransaction.Amount, cashTransaction.Comment, cashTransaction.Id);

            _PortfolioRepository.Update(Portfolio);
        }

        public void ApplyTransaction(RestApi.Transactions.CostBaseAdjustment costBaseAdjustment)
        {

        }

        public void ApplyTransaction(RestApi.Transactions.Disposal disposal)
        { 
            var stock = _StockQuery.Get(disposal.Stock);
            Portfolio.DisposeOfShares(disposal.TransactionDate, stock, disposal.Units, disposal.AveragePrice, disposal.TransactionCosts, RestApiNameMapping.ToCGTCalculationMethod(disposal.CGTMethod), disposal.CreateCashTransaction, disposal.Comment, disposal.Id);

            _PortfolioRepository.Update(Portfolio);
        }

        public void ApplyTransaction(RestApi.Transactions.IncomeReceived incomeReceived)
        {
            var stock = _StockQuery.Get(incomeReceived.Stock);
            Portfolio.IncomeReceived(incomeReceived.RecordDate, incomeReceived.TransactionDate, stock, incomeReceived.FrankedAmount, incomeReceived.UnfrankedAmount, incomeReceived.FrankingCredits, incomeReceived.Interest, incomeReceived.TaxDeferred, incomeReceived.DRPCashBalance, incomeReceived.CreateCashTransaction, incomeReceived.Comment, incomeReceived.Id);

            _PortfolioRepository.Update(Portfolio);
        }

        public void ApplyTransaction(RestApi.Transactions.OpeningBalance openingBalance)
        {
            var stock = _StockQuery.Get(openingBalance.Stock);
            Portfolio.AddOpeningBalance(openingBalance.TransactionDate, openingBalance.AquisitionDate, stock, openingBalance.Units, openingBalance.CostBase, openingBalance.Comment, openingBalance.Id);

            _PortfolioRepository.Update(Portfolio);
        }

        public void ApplyTransaction(RestApi.Transactions.ReturnOfCapital returnOfCapital)
        {
            var stock = _StockQuery.Get(returnOfCapital.Stock);
            Portfolio.ReturnOfCapitalReceived(returnOfCapital.TransactionDate, returnOfCapital.RecordDate, stock, returnOfCapital.Amount, returnOfCapital.CreateCashTransaction, returnOfCapital.Comment, returnOfCapital.Id);

            _PortfolioRepository.Update(Portfolio);
        }

        public void ApplyTransaction(RestApi.Transactions.UnitCountAdjustment unitCountAdjustment)
        {

        }
    }
}
