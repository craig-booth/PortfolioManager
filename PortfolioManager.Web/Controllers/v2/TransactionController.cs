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
    public class TransactionController : Controller
    {
        private IPortfolioCache _PortfolioCache;
        private IStockRepository _StockRepository;
        private IMapper _Mapper;
        private Portfolio _Portfolio;
        private FunkyHandler _TransactionHandler;

        public TransactionController(IPortfolioCache portfolioCache, IStockRepository stockRepository, IMapper mapper, FunkyTransactionService transactionService)
        {
            _PortfolioCache = portfolioCache;
            _StockRepository = stockRepository;
            _Mapper = mapper;
            _TransactionHandler = transactionService.TransactionHandler();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var portfolioId = (string)filterContext.RouteData.Values["portfolioId"];

            _Portfolio = _PortfolioCache.Get(Guid.Parse(portfolioId));
        }

        // GET:  transactions/id
        [HttpGet("{id:guid}")]
        public ActionResult<Transaction> Get(Guid id)
        {
            var transaction = _Portfolio.Transactions.FirstOrDefault(x => x.Id == id);

            if (transaction == null)
                return NotFound();

            return _Mapper.Map<Transaction>(transaction);
        }

        // GET: transactions?stock&fromDate&toDate
        [HttpGet]
        public ActionResult<List<Transaction>> Get(Guid? stock, DateTime? fromDate, DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            var transactions = _Portfolio.Transactions.Where(x => dateRange.Contains(x.TransactionDate));
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

                _TransactionHandler.Handle(domainTransaction, _Portfolio);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }
}
