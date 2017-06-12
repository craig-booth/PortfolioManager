using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Service.CorporateActions
{
    class CapitalReturnHandler : ICorporateActionHandler 
    {
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly IStockQuery _StockQuery;

        public CapitalReturnHandler(IPortfolioQuery portfolioQuery, IStockQuery stockQuery)
        {
            _PortfolioQuery = portfolioQuery;
            _StockQuery = stockQuery;
        }

        public IReadOnlyCollection<Transaction> CreateTransactionList(CorporateAction corporateAction)
        {
            var capitalReturn = corporateAction as CapitalReturn;

            var transactions = new List<Transaction>();

            var stock = _StockQuery.Get(capitalReturn.Stock, capitalReturn.ActionDate);

            /* locate parcels that the capital return applies to */
            var parcels = _PortfolioQuery.GetParcelsForStock(stock.Id, capitalReturn.ActionDate, capitalReturn.ActionDate);
            if (! parcels.Any())
                return transactions;

            transactions.Add(new ReturnOfCapital()
                {
                    ASXCode = stock.ASXCode,
                    TransactionDate = capitalReturn.PaymentDate,
                    RecordDate = capitalReturn.ActionDate,
                    Amount = capitalReturn.Amount,
                    CreateCashTransaction = true,
                    Comment = capitalReturn.Description,
                }
            );

            return transactions.AsReadOnly();
        }

        public bool HasBeenApplied(CorporateAction corporateAction)
        {
            CapitalReturn capitalReturn = corporateAction as CapitalReturn;

            string asxCode = _StockQuery.Get(capitalReturn.Stock, capitalReturn.PaymentDate).ASXCode;

            var transactions = _PortfolioQuery.GetTransactions(asxCode, TransactionType.Income, capitalReturn.PaymentDate, capitalReturn.PaymentDate);
            return (transactions.Count() > 0);
        }

    }
}
