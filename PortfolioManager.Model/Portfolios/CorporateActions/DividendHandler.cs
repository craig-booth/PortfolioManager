using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Portfolios
{
    class DividendHandler : ICorporateActionHandler 
    {
        private readonly StockService _StockService;
        private readonly ParcelService _ParcelService;

        public DividendHandler(StockService stockService, ParcelService parcelService)
        {
            _StockService = stockService;
            _ParcelService = parcelService;
        }

        public IReadOnlyCollection<ITransaction> CreateTransactionList(ICorporateAction corporateAction)
        {
            var dividend = corporateAction as Dividend;

            var transactions = new List<ITransaction>();

            /* locate parcels that the dividend applies to */
            var dividendStock = _StockService.Get(dividend.Stock, dividend.ActionDate);
            var parcels = _ParcelService.GetParcels(dividendStock, dividend.ActionDate);
            if (parcels.Count == 0)
                return transactions;

            var stock = _StockService.Get(dividend.Stock, dividend.PaymentDate);

            var unitsHeld = parcels.Sum(x => x.Units);
            var amountPaid = unitsHeld * dividend.DividendAmount;
            var franked = MathUtils.ToCurrency(amountPaid * dividend.PercentFranked, stock.DividendRoundingRule);
            var unFranked = MathUtils.ToCurrency(amountPaid * (1 - dividend.PercentFranked), stock.DividendRoundingRule);
            var frankingCredits = MathUtils.ToCurrency(((amountPaid / (1 - dividend.CompanyTaxRate)) - amountPaid) * dividend.PercentFranked, stock.DividendRoundingRule);

            /* add drp shares */
            if (dividend.DRPPrice != 0.00M)
            {
                int drpUnits = (int)Math.Round(amountPaid / dividend.DRPPrice);

                transactions.Add(new OpeningBalance()
                {
                    TransactionDate = dividend.PaymentDate,
                    ASXCode = stock.ASXCode,
                    Units = drpUnits,
                    CostBase = amountPaid,
                    AquisitionDate = dividend.PaymentDate,
                    Comment = "DRP " + MathUtils.FormatCurrency(dividend.DRPPrice, false, true)
                }
                );
            }

            transactions.Add(new IncomeReceived()
            {
                TransactionDate = dividend.PaymentDate,
                ASXCode = stock.ASXCode,
                RecordDate = dividend.ActionDate,
                FrankedAmount = franked,
                UnfrankedAmount = unFranked,
                FrankingCredits = frankingCredits,
                Comment = dividend.Description
            });

            return transactions;
        }

        public bool HasBeenApplied(ICorporateAction corporateAction, TransactionService transactionService)
        {
            Dividend dividend = corporateAction as Dividend;
            string asxCode = _StockService.Get(dividend.Stock, dividend.PaymentDate).ASXCode;
           
            var transactions = transactionService.GetTransactions(asxCode, TransactionType.Income, dividend.PaymentDate, dividend.PaymentDate);
            return (transactions.Count() > 0); 
        }

    }


}
