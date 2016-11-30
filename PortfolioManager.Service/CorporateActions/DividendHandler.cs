using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Service.CorporateActions
{
    class DividendHandler : ICorporateActionHandler 
    {
        private readonly StockService _StockService;
        private readonly ParcelService _ParcelService;
        private readonly IncomeService _IncomeService;

        public DividendHandler(StockService stockService, ParcelService parcelService, IncomeService incomeService)
        {
            _StockService = stockService;
            _ParcelService = parcelService;
            _IncomeService = incomeService;
        }

        public IReadOnlyCollection<Transaction> CreateTransactionList(CorporateAction corporateAction)
        {
            var dividend = corporateAction as Dividend;

            var transactions = new List<Transaction>();

            /* locate parcels that the dividend applies to */
            var dividendStock = _StockService.Get(dividend.Stock, dividend.ActionDate);
            var parcels = _ParcelService.GetParcels(dividendStock, dividend.ActionDate);
            if (parcels.Count == 0)
                return transactions;

            var stock = _StockService.Get(dividend.Stock, dividend.PaymentDate);

            /* Assume that DRP applies */
            bool applyDRP = false;
            if ((dividend.DRPPrice != 0.00m) && (_IncomeService.DRPActive(stock)))
                applyDRP = true;

            var unitsHeld = parcels.Sum(x => x.Units);
            var amountPaid = unitsHeld * dividend.DividendAmount;
            var franked = (amountPaid * dividend.PercentFranked).ToCurrency(stock.DividendRoundingRule);
            var unFranked = (amountPaid * (1 - dividend.PercentFranked)).ToCurrency(stock.DividendRoundingRule);
            var frankingCredits = (((amountPaid / (1 - dividend.CompanyTaxRate)) - amountPaid) * dividend.PercentFranked).ToCurrency(stock.DividendRoundingRule);

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = dividend.PaymentDate,
                ASXCode = stock.ASXCode,
                RecordDate = dividend.ActionDate,
                FrankedAmount = franked,
                UnfrankedAmount = unFranked,
                FrankingCredits = frankingCredits,
                CreateCashTransaction = true,
                DRPCashBalance = 0.00m,
                Comment = dividend.Description
            };
            transactions.Add(incomeReceived);

            /* add drp shares */
            if (applyDRP)
            {
                incomeReceived.CreateCashTransaction = false;

                int drpUnits;
                if (stock.DRPMethod == DRPMethod.RoundUp)
                    drpUnits = (int)Math.Ceiling(amountPaid / dividend.DRPPrice);
                else if (stock.DRPMethod == DRPMethod.RoundDown)
                    drpUnits = (int)Math.Floor(amountPaid / dividend.DRPPrice);
                else if (stock.DRPMethod == DRPMethod.RetainCashBalance)
                {
                    var drpCashBalance = _IncomeService.GetDRPCashBalance(stock, dividend.PaymentDate);
                    drpUnits = (int)Math.Floor((amountPaid + drpCashBalance) / dividend.DRPPrice);
                    incomeReceived.DRPCashBalance = amountPaid - (drpUnits * dividend.DRPPrice);
                }
                else
                    drpUnits = (int)Math.Round(amountPaid / dividend.DRPPrice);

                if (drpUnits > 0)
                {
                    transactions.Add(new OpeningBalance()
                    {
                        TransactionDate = dividend.PaymentDate,
                        ASXCode = stock.ASXCode,
                        Units = drpUnits,
                        CostBase = amountPaid - incomeReceived.DRPCashBalance,
                        AquisitionDate = dividend.PaymentDate,
                        RecordDate = dividend.PaymentDate,
                        Comment = "DRP " + MathUtils.FormatCurrency(dividend.DRPPrice, false, true)
                    }
                    );
                }
            }         

            return transactions;
        }

        public bool HasBeenApplied(CorporateAction corporateAction, TransactionService transactionService)
        {
            Dividend dividend = corporateAction as Dividend;
            string asxCode = _StockService.Get(dividend.Stock, dividend.PaymentDate).ASXCode;
           
            var transactions = transactionService.GetTransactions(asxCode, TransactionType.Income, dividend.PaymentDate, dividend.PaymentDate);
            return (transactions.Count() > 0); 
        }

    }


}
