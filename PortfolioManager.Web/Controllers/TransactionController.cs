using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using PortfolioManager.Common;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.Web.Controllers
{
    [Route("api/transactions")]
    public class TransactionController : Controller
    {
        private IServiceProvider _ServiceProvider;

        public TransactionController(IServiceProvider serviceProvider)
        {
            _ServiceProvider = serviceProvider;
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
            var service = _ServiceProvider.GetRequiredService<ITransactionService>();

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
            var service = _ServiceProvider.GetRequiredService<ITransactionService>();

            service.AddTransaction(value);
        }

        // PUT: /api/transactions/id
        [HttpPut("{id}")]
        public void Put(int Guid, [FromBody]TransactionItem value)
        {
            var service = _ServiceProvider.GetRequiredService<ITransactionService>();

            service.UpdateTransaction(value);
        }

        // DELETE: /api/transactions/id
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            var service = _ServiceProvider.GetRequiredService<ITransactionService>();

            service.DeleteTransaction(id);
        }
    }
}
