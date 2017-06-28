using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using PortfolioManager.Common;
using PortfolioManager.Service.Local;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.Web.Controllers
{
    [Route("api/transactions")]
    public class TransactionController : Controller
    {
        private LocalPortfolioManagerService _PortfolioManagerService = new LocalPortfolioManagerService();

        public TransactionController()
        {
            var portfolioDatabase = @"C:\Users\Craig\Documents\GitHubVisualStudio\PortfolioManager\PortfolioManager.Web\bin\Debug\netcoreapp1.0\Natalies Portfolio.db";
            var stockDatabase = @"C:\Users\Craig\Documents\GitHubVisualStudio\PortfolioManager\PortfolioManager.Web\bin\Debug\netcoreapp1.0\stocks.db";

            _PortfolioManagerService.Connect(portfolioDatabase, stockDatabase);
        }

        // GET: /api/transaction/id
        /*   [HttpGet("{id}")]
           public TransactionItem Get(Guid id)
           {
               return "value";
           } */

        // GET: /api/transactions?stock&fromDate&toDate
        [HttpGet]
        public async Task<GetTransactionsResponce> Get(Guid? stock, DateTime? fromDate, DateTime? toDate)
        {
            var service = _PortfolioManagerService.GetService<ITransactionService>();

            if (fromDate == null)
                fromDate = DateUtils.NoStartDate;
            if (toDate == null)
                toDate = DateUtils.NoEndDate;

            if (stock == null)
                return await service.GetTransactions((DateTime)fromDate, (DateTime)toDate);
            else
                return await service.GetTransactions((Guid)stock, (DateTime)fromDate, (DateTime)toDate);

        }


        // POST: /api/transactions
        [HttpPost]
        public void Post([FromBody]TransactionItem value)
        {
            var service = _PortfolioManagerService.GetService<ITransactionService>();

            service.AddTransaction(value);
        }

        // PUT: /api/transactions/id
        [HttpPut("{id}")]
        public void Put(int Guid, [FromBody]TransactionItem value)
        {
            var service = _PortfolioManagerService.GetService<ITransactionService>();

            service.UpdateTransaction(value);
        }

        // DELETE: /api/transactions/id
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            var service = _PortfolioManagerService.GetService<ITransactionService>();

            service.DeleteTransaction(id);
        }
    }
}
