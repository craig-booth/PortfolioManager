using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service.Interface;


namespace PortfolioManager.Service.Local
{
    class IncomeService : IIncomeService
    {
        private readonly Obsolete.IncomeService _IncomeService;

        public IncomeService(Obsolete.IncomeService incomeService)
        {
            _IncomeService = incomeService;
        }

        public Task<IncomeResponce> GetIncome(DateTime fromDate, DateTime toDate)
        {
            var responce = new IncomeResponce();

            var incomes = _IncomeService.GetIncome(fromDate, toDate);
            foreach (var income in incomes)
            {
                var item = new IncomeItem()
                {
                    Stock = new StockItem(income.Stock),
                    UnfrankedAmount = income.UnfrankedAmount,
                    FrankedAmount = income.FrankedAmount,
                    FrankingCredits = income.FrankingCredits,
                    TotalAmount = income.TotalIncome
                };

                responce.Income.Add(item);
            }

            responce.SetStatusToSuccessfull();

            return Task.FromResult<IncomeResponce>(responce);
        }

    }
}
