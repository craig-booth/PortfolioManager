using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    class CapitalReturnHandler : ICorporateActionHandler 
    {

        private readonly StockService _StockService;
        private readonly ParcelService _ParcelService;

        public CapitalReturnHandler(StockService stockService, ParcelService parcelService)
        {
            _StockService = stockService;
            _ParcelService = parcelService;
        }

        public IReadOnlyCollection<ITransaction> CreateTransactionList(ICorporateAction corporateAction)
        {
            var capitalReturn = corporateAction as CapitalReturn;

            var transactions = new List<ITransaction>();

            var stock = _StockService.Get(capitalReturn.Stock, capitalReturn.PaymentDate);

            transactions.Add(new ReturnOfCapital()
                {
                    ASXCode = stock.ASXCode,
                    TransactionDate = capitalReturn.PaymentDate,
                    Amount = capitalReturn.Amount,
                    Comment = capitalReturn.Description
                }
            );

            return transactions.AsReadOnly();
        }

    }
}
