using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Domain.CorporateActions;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Service.CorporateActions
{
    class DividendHandler : ICorporateActionHandler 
    {
        private readonly IPortfolioQuery _PortfolioQuery;

        public DividendHandler(IPortfolioQuery portfolioQuery)
        {
            _PortfolioQuery = portfolioQuery;
        }

        public IReadOnlyCollection<Transaction> CreateTransactionList(CorporateAction corporateAction)
        {
            var dividend = corporateAction as Dividend;

            var transactions = new List<Transaction>();

            /* locate parcels that the dividend applies to */
            var parcels = _PortfolioQuery.GetParcelsForStock(dividend.Stock.Id, dividend.ActionDate, dividend.ActionDate);
            if (!parcels.Any())
                return transactions;

            var dividendRoundingRule = dividend.Stock.DividendRules[dividend.ActionDate].DividendRoundingRule;

            var unitsHeld = parcels.Sum(x => x.Units);
            var amountPaid = (unitsHeld * dividend.DividendAmount).ToCurrency(dividendRoundingRule);
            var franked = (amountPaid * dividend.PercentFranked).ToCurrency(dividendRoundingRule);
            var unFranked = (amountPaid * (1 - dividend.PercentFranked)).ToCurrency(dividendRoundingRule);
            var frankingCredits = (((amountPaid / (1 - dividend.CompanyTaxRate)) - amountPaid) * dividend.PercentFranked).ToCurrency(dividendRoundingRule);

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = dividend.PaymentDate,
                ASXCode = dividend.Stock.Properties[dividend.PaymentDate].ASXCode,
                RecordDate = dividend.ActionDate,
                FrankedAmount = franked,
                UnfrankedAmount = unFranked,
                FrankingCredits = frankingCredits,
                CreateCashTransaction = true,
                DRPCashBalance = 0.00m,
                Comment = dividend.Description
            };
            transactions.Add(incomeReceived);

            /* Handle Dividend Reinvestment Plan */
            if (dividend.DRPPrice != 0.00m)
            {
                var stockSetting = _PortfolioQuery.GetStockSetting(dividend.Stock.Id, dividend.ActionDate);
                if (stockSetting.ParticipateinDRP)
                {
                    incomeReceived.CreateCashTransaction = false;

                    int drpUnits;
                    decimal costBase;

                    var drpMethod = dividend.Stock.DividendRules[dividend.ActionDate].DRPMethod;
                    if (drpMethod == DRPMethod.RoundUp)
                    {
                        drpUnits = (int)Math.Ceiling(amountPaid / dividend.DRPPrice);
                        costBase = amountPaid;
                    }
                    else if (drpMethod == DRPMethod.RoundDown)
                    {
                        drpUnits = (int)Math.Floor(amountPaid / dividend.DRPPrice);
                        costBase = amountPaid;
                    }
                    else if (drpMethod == DRPMethod.RetainCashBalance)
                    {
                        var drpCashBalance = _PortfolioQuery.GetDRPBalance(dividend.Stock.Id, dividend.PaymentDate);
                        var availableAmount = amountPaid + drpCashBalance;
                        drpUnits = (int)Math.Floor(availableAmount / dividend.DRPPrice);
                        costBase = (drpUnits * dividend.DRPPrice).ToCurrency(dividendRoundingRule);
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
                            ASXCode = dividend.Stock.Properties[dividend.PaymentDate].ASXCode,
                            Units = drpUnits,
                            CostBase = costBase,
                            AquisitionDate = dividend.PaymentDate,
                            RecordDate = dividend.PaymentDate,
                            Comment = "DRP " + MathUtils.FormatCurrency(dividend.DRPPrice, false, true)
                        }
                        );
                    }
                }
            }
                  

            return transactions;
        }

        public bool HasBeenApplied(CorporateAction corporateAction)
        {
            Dividend dividend = corporateAction as Dividend;

            var asxCode = dividend.Stock.Properties[dividend.PaymentDate].ASXCode;
           
            var transactions = _PortfolioQuery.GetTransactions(asxCode, TransactionType.Income, dividend.PaymentDate, dividend.PaymentDate);
            return (transactions.Count() > 0); 
        }

    }


}
