﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Utils;


namespace PortfolioManager.Service.Local
{
    class IncomeService : IIncomeService
    {
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly StockUtils _StockUtils;

        public IncomeService(IPortfolioQuery portfolioQuery, IStockDatabase stockDatabase, IStockQuery stockQuery)
        {
            _PortfolioQuery = portfolioQuery;
            _StockUtils = new StockUtils(stockQuery, stockDatabase);
        }

        public Task<IncomeResponce> GetIncome(DateTime fromDate, DateTime toDate)
        {
            var responce = new IncomeResponce();

            var incomeTransactions = _PortfolioQuery.GetTransactions(TransactionType.Income, fromDate, toDate).Cast<IncomeReceived>();

            var incomeSummary = from income in incomeTransactions
                                group income by income.ASXCode into incomeGroup
                                select new IncomeItem()
                                {
                                    Stock = _StockUtils.Get(incomeGroup.Key, toDate),
                                    UnfrankedAmount = incomeGroup.Sum(x => x.UnfrankedAmount),
                                    FrankedAmount = incomeGroup.Sum(x => x.FrankedAmount),
                                    FrankingCredits = incomeGroup.Sum(x => x.FrankingCredits),
                                    TotalAmount = incomeGroup.Sum(x => x.TotalIncome)
                                };

            responce.Income.AddRange(incomeSummary);
           
            responce.SetStatusToSuccessfull();

            return Task.FromResult<IncomeResponce>(responce);
        }

    }
}