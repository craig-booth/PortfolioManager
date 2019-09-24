using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Http;


using PortfolioManager.RestApi.Transactions;
using PortfolioManager.Web.Services;


namespace PortfolioManager.Web.Controllers.v2
{
    [Route("api/v2/portfolio/{portfolioId:guid}/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        // GET:  transactions/id
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Transaction> Get([FromServices] IPortfolioTransactionService service, [FromRoute] Guid portfolioId, Guid id)
        {
            return service.GetTransaction(portfolioId, id);
        }

        // POST: transactions
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public ActionResult AddTransaction([FromServices] IPortfolioTransactionService service, [FromRoute] Guid portfolioId, [FromBody]Transaction transaction)
        {
            if (transaction == null)
                return BadRequest("Unknown Transaction type");

            service.ApplyTransaction(portfolioId, (dynamic)transaction);

            return Ok();
        }

        // POST: transactions
        [NonAction]
     //   [HttpPost]
    //    public ActionResult AddTransactions([FromBody]List<Transaction> transactions)
        public ActionResult AddTransactions([FromServices] IPortfolioTransactionService service, [FromRoute] Guid portfolioId, List<Transaction> transactions)
        {
            if (transactions == null)
                return BadRequest("Unknown Transaction type");

            foreach (var transaction in transactions)
                service.ApplyTransaction(portfolioId, (dynamic)transaction);

            return Ok();
        }  

        // GET:  transactions/id/corporateaction/id
        [HttpGet("{stockId:guid}/corporateaction/{actionId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Transaction>> GetTransactionsForCorporateAction([FromServices] IPortfolioCorporateActionsService service, [FromRoute] Guid portfolioId, Guid stockId, Guid actionId)
        {
            var transactions = service.GetTransactionsForCorporateAction(portfolioId, stockId, actionId);

            return transactions.ToList();
        } 
    }
}
