﻿using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Transactions;
using PortfolioManager.Domain.Utils;

namespace PortfolioManager.Domain.CorporateActions
{
    public class Dividend : CorporateAction
    {
        public DateTime PaymentDate { get; private set; }
        public decimal DividendAmount { get; private set; }
        public decimal PercentFranked { get; private set; }
        public decimal DRPPrice { get; private set; }

        internal Dividend(Guid id, Stock stock, DateTime actionDate, string description, DateTime paymentDate, decimal dividendAmount, decimal percentFranked, decimal drpPrice)
            : base(id, stock, CorporateActionType.Dividend, actionDate, description)
        {
            PaymentDate = paymentDate;
            DividendAmount = dividendAmount;
            PercentFranked = percentFranked;
            DRPPrice = drpPrice;
        }

        public override IEnumerable<Transaction> GetTransactionList(Holding holding)
        {
            var transactions = new List<Transaction>();

            var holdingProperties = holding.Properties[Date];        
            if (holdingProperties.Units == 0)
                return transactions;

            var dividendRules = Stock.DividendRules[Date];

            var amountPaid = (holdingProperties.Units * DividendAmount).ToCurrency(dividendRules.DividendRoundingRule);
            var franked = (amountPaid * PercentFranked).ToCurrency(dividendRules.DividendRoundingRule);
            var unFranked = (amountPaid * (1 - PercentFranked)).ToCurrency(dividendRules.DividendRoundingRule);
            var frankingCredits = (((amountPaid / (1 - dividendRules.CompanyTaxRate)) - amountPaid) * PercentFranked).ToCurrency(dividendRules.DividendRoundingRule);
            
            var incomeReceived = new IncomeReceived()
            {
                Id = Guid.NewGuid(),
                Date = PaymentDate,
                Stock = Stock,
                RecordDate = Date,
                FrankedAmount = franked,
                UnfrankedAmount = unFranked,
                FrankingCredits = frankingCredits,
                Interest = 0.00m,
                TaxDeferred = 0.00m,
                CreateCashTransaction = true,
                DRPCashBalance = 0.00m,
                Comment = Description
            };
            transactions.Add(incomeReceived);

            /* Handle Dividend Reinvestment Plan */
            var holdingSettings = holding.Settings;
            if (dividendRules.DRPActive && holdingSettings.ParticipateInDrp && (DRPPrice != 0.00m))
            { 
                incomeReceived.CreateCashTransaction = false;

                int drpUnits;
                decimal costBase;

                if (dividendRules.DRPMethod == DRPMethod.RoundUp)
                {
                    drpUnits = (int)Math.Ceiling(amountPaid / DRPPrice);
                    costBase = amountPaid;
                }
                else if (dividendRules.DRPMethod == DRPMethod.RoundDown)
                {
                    drpUnits = (int)Math.Floor(amountPaid / DRPPrice);
                    costBase = amountPaid;
                }
                else if (dividendRules.DRPMethod == DRPMethod.RetainCashBalance)
                {
                    var drpCashBalance = holding.DrpAccount.Balance(Date);

                    var availableAmount = amountPaid + drpCashBalance;
                    drpUnits = (int)Math.Floor(availableAmount / DRPPrice);
                    costBase = (drpUnits * DRPPrice).ToCurrency(dividendRules.DividendRoundingRule);
                    incomeReceived.DRPCashBalance = availableAmount - costBase;
                }
                else
                {
                    drpUnits = (int)Math.Round(amountPaid / DRPPrice);
                    costBase = amountPaid;
                }

                if (drpUnits > 0)
                {
                    transactions.Add(new OpeningBalance()
                    {
                        Id = Guid.NewGuid(),
                        Date = PaymentDate,
                        Stock = Stock,
                        Units = drpUnits,
                        CostBase = costBase,
                        AquisitionDate = PaymentDate,
                        Comment = "DRP " + MathUtils.FormatCurrency(DRPPrice, false, true)
                    });
                }
            } 

            return transactions;
        }

        public override bool HasBeenApplied(ITransactionList<Transaction> transactions)
        {
            return transactions.InDateRange(new DateRange(PaymentDate, PaymentDate)).Any(x => x is IncomeReceived);
        }
    }
}
