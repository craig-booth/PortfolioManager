using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using AutoMapper;

using PortfolioManager.Domain;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.RestApi.Transactions;
using PortfolioManager.Web.Services;
using PortfolioManager.Web.Utilities;


namespace PortfolioManager.Web.Controllers.v2
{
    [Route("api/v2/portfolio/{portfolioId:guid}/transactions")]
    [ApiController]
    public class TransactionController : PortfolioControllerBase
    {
        private readonly IMapper _Mapper;
        private readonly IStockQuery _StockQuery;

        public TransactionController(IRepository<Portfolio> portfolioRepository, IStockQuery stockQuery, IMapper mapper, IAuthorizationService authorizationService)
            : base(portfolioRepository, authorizationService)
        {
            _StockQuery = stockQuery;
            _Mapper = mapper;
        }

        // GET:  transactions/id
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Transaction> Get(Guid id)
        {
            var transaction = _Portfolio.Transactions[id];
            if (transaction == null)
                return NotFound();

            return _Mapper.Map<Transaction>(transaction);
        }

        // POST: transactions
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public ActionResult AddTransaction([FromBody]Transaction transaction)
        {
            if (transaction == null)
                return BadRequest("Unknown Transaction type");

            try
            {
                var transactionService = new PortfolioTransactionService(_Portfolio, _PortfolioRepository, _StockQuery, _Mapper);
                transactionService.ApplyTransaction((dynamic)transaction);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // POST: transactions
        [NonAction]
     //   [HttpPost]
    //    public ActionResult AddTransactions([FromBody]List<Transaction> transactions)
        public ActionResult AddTransactions(List<Transaction> transactions)
        {
            if (transactions == null)
                return BadRequest("Unknown Transaction type");

            try
            {
                var transactionService = new PortfolioTransactionService(_Portfolio, _PortfolioRepository, _StockQuery, _Mapper);

                foreach (var transaction in transactions)
                    transactionService.ApplyTransaction((dynamic)transaction);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }  

        // GET:  transactions/id/corporateaction/id
        [HttpGet("{stockId:guid}/corporateaction/{actionId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Transaction>> GetTransactionsForCorporateAction(Guid stockId, Guid actionId)
        {
            var holding = _Portfolio.Holdings.Get(stockId);
            if (holding == null)
                return NotFound();

            var corporateAction = holding.Stock.CorporateActions[actionId];
            if (corporateAction == null)
                return NotFound();

            var corporateActionsService = new PortfolioCorporateActionsService(_Portfolio, _Mapper);
           
            var transactions = corporateActionsService.GetTransactionsForCorporateAction(holding, corporateAction);

            return transactions.ToList();
        } 
    }
}
