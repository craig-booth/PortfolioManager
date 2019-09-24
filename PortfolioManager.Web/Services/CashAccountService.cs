using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.Services
{

    public interface ICashAccountService
    {
        CashAccountTransactionsResponse GetCashAccountTransactions(Guid portfolioId, DateRange dateRange);
    }

    public class CashAccountService : ICashAccountService
    {
        private readonly IPortfolioCache _PortfolioCache;
        private readonly IMapper _Mapper;

        public CashAccountService(IPortfolioCache portfolioCache, IMapper mapper)
        {
            _PortfolioCache = portfolioCache;
            _Mapper = mapper;          
        }

        public CashAccountTransactionsResponse GetCashAccountTransactions(Guid portfolioId, DateRange dateRange)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var response = new CashAccountTransactionsResponse();

            var transactions = portfolio.CashAccount.Transactions.InDateRange(dateRange);

            response.OpeningBalance = portfolio.CashAccount.Balance(dateRange.FromDate);
            response.ClosingBalance = portfolio.CashAccount.Balance(dateRange.ToDate);

            response.Transactions.AddRange(_Mapper.Map<IEnumerable<CashAccountTransactionsResponse.CashTransactionItem>>(transactions));

            return response;
        }
    }
}
