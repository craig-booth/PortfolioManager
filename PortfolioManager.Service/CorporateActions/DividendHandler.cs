﻿using System;
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

        public DividendHandler(StockService stockService, ParcelService parcelService)
        {
            _StockService = stockService;
            _ParcelService = parcelService;
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
            if (dividend.DRPPrice != 0.00M)
                applyDRP = true;

            var unitsHeld = parcels.Sum(x => x.Units);
            var amountPaid = unitsHeld * dividend.DividendAmount;
            var franked = (amountPaid * dividend.PercentFranked).ToCurrency(stock.DividendRoundingRule);
            var unFranked = (amountPaid * (1 - dividend.PercentFranked)).ToCurrency(stock.DividendRoundingRule);
            var frankingCredits = (((amountPaid / (1 - dividend.CompanyTaxRate)) - amountPaid) * dividend.PercentFranked).ToCurrency(stock.DividendRoundingRule);

            transactions.Add(new IncomeReceived()
            {
                TransactionDate = dividend.PaymentDate,
                ASXCode = stock.ASXCode,
                RecordDate = dividend.ActionDate,
                FrankedAmount = franked,
                UnfrankedAmount = unFranked,
                FrankingCredits = frankingCredits,
                CreateCashTransaction = !applyDRP,
                Comment = dividend.Description
            });

            /* add drp shares */
            if (applyDRP)
            {
                int drpUnits = (int)Math.Round(amountPaid / dividend.DRPPrice);

                transactions.Add(new OpeningBalance()
                {
                    TransactionDate = dividend.PaymentDate,
                    ASXCode = stock.ASXCode,
                    Units = drpUnits,
                    CostBase = amountPaid,
                    AquisitionDate = dividend.PaymentDate,
                    RecordDate = dividend.PaymentDate,
                    Comment = "DRP " + MathUtils.FormatCurrency(dividend.DRPPrice, false, true)
                }
                );
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
