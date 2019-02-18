using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.RestApi.Transactions;

namespace PortfolioManager.Web.Controllers.v2
{
    [Route("api/v2/portfolio/{portfolioId:guid}/transactions")]
    public class TransactionController : BasePortfolioController
    {
        private IMapper _Mapper;

        public TransactionController(IPortfolioCache portfolioCache, IMapper mapper)
            : base(portfolioCache)
        {
            _Mapper = mapper;
        }

        // GET:  transactions/id
        [HttpGet("{id:guid}")]
        public ActionResult<Transaction> Get(Guid id)
        {
            var transaction = _Portfolio.Transactions[id];
            if (transaction == null)
                return NotFound();

            return _Mapper.Map<Transaction>(transaction);
        }

        // POST: transactions
        [HttpPost]
        public ActionResult AddTransaction([FromBody]Transaction transaction)
        {
            if (transaction == null)
                return BadRequest("Unknown Transaction type");

            try
            {
                var domainTransaction = _Mapper.Map<Domain.Transactions.Transaction>(transaction);

                _Portfolio.Transactions.Apply(domainTransaction);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // POST: transactions
     //   [HttpPost]
    //    public ActionResult AddTransactions([FromBody]List<Transaction> transactions)
        public ActionResult AddTransactions(List<Transaction> transactions)
        {
            if (transactions == null)
                return BadRequest("Unknown Transaction type");

            try
            {
                var domainTransactions = _Mapper.Map<List<Domain.Transactions.Transaction>>(transactions);

                _Portfolio.Transactions.Apply(domainTransactions);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        } 

        // GET:  transactions/corporateaction/id
        [HttpGet("corporateaction/{id:guid}")]
        public ActionResult<List<Transaction>> GetTransactionsForCorporateAction(Guid id)
        {
            var transactions = new List<Transaction>();

            return transactions;
        }
    }
}
