using System;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Utils;


namespace PortfolioManager.Service.Local
{
    public class IncomeService : IIncomeService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly IStockDatabase _StockDatabase;

        public IncomeService(IPortfolioDatabase portfolioDatabase, IStockDatabase stockDatabase)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockDatabase = stockDatabase;
        }

        public Task<IncomeResponce> GetIncome(DateTime fromDate, DateTime toDate)
        {
            var responce = new IncomeResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                using (var stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
                {
                    var incomeTransactions = portfolioUnitOfWork.PortfolioQuery.GetTransactions(TransactionType.Income, fromDate, toDate).Cast<IncomeReceived>();

                    var incomeSummary = from income in incomeTransactions
                                        group income by income.ASXCode into incomeGroup
                                        select new IncomeItem()
                                        {
                                            Stock = StockUtils.Get(incomeGroup.Key, toDate, stockUnitOfWork.StockQuery),
                                            UnfrankedAmount = incomeGroup.Sum(x => x.UnfrankedAmount),
                                            FrankedAmount = incomeGroup.Sum(x => x.FrankedAmount),
                                            FrankingCredits = incomeGroup.Sum(x => x.FrankingCredits),
                                            TotalAmount = incomeGroup.Sum(x => x.TotalIncome)
                                        };

                    responce.Income.AddRange(incomeSummary);

                    responce.SetStatusToSuccessfull();
                }
            }

            return Task.FromResult<IncomeResponce>(responce);
        }

    }
}
