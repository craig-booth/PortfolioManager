using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.CorporateActions
{
    class DividendHandler : ICorporateActionHandler 
    {
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly StockService _StockService;
        private readonly IncomeService _IncomeService;

        public DividendHandler(IPortfolioQuery portfolioQuery, StockService stockService, IncomeService incomeService)
        {
            _PortfolioQuery = portfolioQuery;
            _StockService = stockService;
            _IncomeService = incomeService;
        }

        public IReadOnlyCollection<Transaction> CreateTransactionList(CorporateAction corporateAction)
        {
            var dividend = corporateAction as Dividend;

            var transactions = new List<Transaction>();

            /* locate parcels that the dividend applies to */
            var dividendStock = _StockService.Get(dividend.Stock, dividend.ActionDate);
            var parcels = _PortfolioQuery.GetParcelsForStock(dividendStock.Id, dividend.ActionDate, dividend.ActionDate);
            if (parcels.Count == 0)
                return transactions;

            var stock = _StockService.Get(dividend.Stock, dividend.PaymentDate);

            /* Assume that DRP applies */
            bool applyDRP = false;
            if ((dividend.DRPPrice != 0.00m) && (_IncomeService.DRPActive(stock)))
                applyDRP = true;

            var unitsHeld = parcels.Sum(x => x.Units);
            var amountPaid = (unitsHeld * dividend.DividendAmount).ToCurrency(stock.DividendRoundingRule);
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
                decimal costBase;

                if (stock.DRPMethod == DRPMethod.RoundUp)
                {
                    drpUnits = (int)Math.Ceiling(amountPaid / dividend.DRPPrice);
                    costBase = amountPaid;
                }
                else if (stock.DRPMethod == DRPMethod.RoundDown)
                { 
                    drpUnits = (int)Math.Floor(amountPaid / dividend.DRPPrice);
                    costBase = amountPaid;
                }
                else if (stock.DRPMethod == DRPMethod.RetainCashBalance)
                {
                    var drpCashBalance = _IncomeService.GetDRPCashBalance(stock, dividend.PaymentDate);
                    var availableAmount = amountPaid + drpCashBalance;
                    drpUnits = (int)Math.Floor(availableAmount / dividend.DRPPrice);
                    costBase = (drpUnits * dividend.DRPPrice).ToCurrency(stock.DividendRoundingRule);
                    incomeReceived.DRPCashBalance = availableAmount - costBase;
                }
                else
                { 
                    drpUnits = (int)Math.Round(amountPaid / dividend.DRPPrice);
                    costBase = amountPaid;
                }

            if (drpUnits > 0)
                {
                    transactions.Add(new OpeningBalance()
                    {
                        TransactionDate = dividend.PaymentDate,
                        ASXCode = stock.ASXCode,
                        Units = drpUnits,
                        CostBase = costBase,
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
