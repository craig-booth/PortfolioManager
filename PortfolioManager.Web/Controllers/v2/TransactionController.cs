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

using PortfolioManager.Web.Mapping;

namespace PortfolioManager.Web.Controllers.v2
{
    [Route("api/v2/portfolio/{portfolioId:guid}/transactions")]
    public class TransactionController : BasePortfolioController
    {
        private IStockRepository _StockRepository;
        private IMapper _Mapper;

        public TransactionController(IPortfolioCache portfolioCache, IStockRepository stockRepository, IMapper mapper)
            : base(portfolioCache)
        {
            _StockRepository = stockRepository;
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

        // GET: transactions?stock&fromDate&toDate
        [HttpGet]
        public ActionResult<List<Transaction>> Get(Guid? stock, DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var transactions = _Portfolio.Transactions.InDateRange(dateRange);
            if (stock != null)
                transactions = transactions.Where(x => x.Stock.Id == stock);

            return _Mapper.Map<List<Transaction>>(transactions);
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
        [HttpPost]
        public ActionResult AddTransactions([FromBody]List<Transaction> transactions)
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
    }
}
