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


    public class CashAccountService
    {
        public ICashAccount CashAccount { get; }
        private IMapper _Mapper;

        public CashAccountService(ICashAccount cashAccount, IMapper mapper)
        {
            CashAccount = cashAccount;
            _Mapper = mapper;
        }

        public CashAccountTransactionsResponse GetCashAccountTransactions(DateRange dateRange)
        {
        var response = new CashAccountTransactionsResponse();

            var transactions = CashAccount.Transactions.InDateRange(dateRange);

            response.OpeningBalance = CashAccount.Balance(dateRange.FromDate);
            response.ClosingBalance = CashAccount.Balance(dateRange.ToDate);

            response.Transactions.AddRange(_Mapper.Map<IEnumerable<CashAccountTransactionsResponse.CashTransactionItem>>(transactions));

            return response;
        }
    }
}
