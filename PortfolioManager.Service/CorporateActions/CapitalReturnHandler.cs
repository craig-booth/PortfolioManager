using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Domain.CorporateActions;

namespace PortfolioManager.Service.CorporateActions
{
    class CapitalReturnHandler : ICorporateActionHandler 
    {
        private readonly IPortfolioQuery _PortfolioQuery;

        public CapitalReturnHandler(IPortfolioQuery portfolioQuery)
        {
            _PortfolioQuery = portfolioQuery;
        }

        public IReadOnlyCollection<Transaction> CreateTransactionList(CorporateAction corporateAction)
        {
            var capitalReturn = corporateAction as CapitalReturn;

            var transactions = new List<Transaction>();

            var stock = capitalReturn.Stock;

            /* locate parcels that the capital return applies to */
            var parcels = _PortfolioQuery.GetParcelsForStock(stock.Id, capitalReturn.ActionDate, capitalReturn.ActionDate);
            if (! parcels.Any())
                return transactions;

            transactions.Add(new ReturnOfCapital()
                {
                    ASXCode = stock.Properties[capitalReturn.ActionDate].ASXCode,
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

            var asxCode = capitalReturn.Stock.Properties[capitalReturn.ActionDate].ASXCode;

            var transactions = _PortfolioQuery.GetTransactions(asxCode, TransactionType.Income, capitalReturn.PaymentDate, capitalReturn.PaymentDate);
            return (transactions.Count() > 0);
        }

    }
}
