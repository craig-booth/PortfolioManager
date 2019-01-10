using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.RestApi.Portfolios;

namespace PortfolioManager.Web.Services
{
    public class PortfolioTransactionService
    {
        public Portfolio Portfolio { get; }
        private IMapper _Mapper;

        public PortfolioTransactionService(Portfolio portfolio, IMapper mapper)
        {
            Portfolio = portfolio;
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
    }
}
