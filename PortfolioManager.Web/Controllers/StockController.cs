using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.RestApi.Commands;
using PortfolioManager.RestApi.Responses;

namespace PortfolioManager.Web.Controllers
{
    [Route("api/stocks")]
    public class StockController : Controller
    {
        private IStockRepository _StockRepository;

        public StockController(IStockRepository stockRepository)
        {
            _StockRepository = stockRepository;
        }

        // GET: api/stocks
        [HttpGet]
        public ActionResult<IEnumerable<StockResponse>> Get([FromQuery]string query, [FromQuery]DateTime? date, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            IEnumerable<Stock> stocks;
            DateTime resultDate;
        
            if (date != null)
            {
                if (query == null)
                    stocks = _StockRepository.All((DateTime)date);
                else
                    stocks = _StockRepository.Find((DateTime)date, x => MatchesQuery(x, query));

                resultDate = (DateTime)date;
            }
            else
            {
                var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateUtils.NoEndDate);

                if (query == null)
                    stocks = _StockRepository.All(dateRange);
                else
                    stocks = stocks = _StockRepository.Find(dateRange, x => MatchesQuery(x, query));

                resultDate = dateRange.ToDate;
            }

            return Ok(stocks.Select(x => x.ToResponse(resultDate)));
        }

        // GET: api/stocks/{id}
        [HttpGet]
        [Route("{id}")]
        public ActionResult<StockResponse> Get([FromRoute]Guid id, [FromQuery]DateTime? date)
        {
            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            if (date != null)
                return Ok(stock.ToResponse((DateTime)date));
            else
                return Ok(stock.ToResponse(DateTime.Today));
        }

        // GET : /api/stocks/{id}/history
        [HttpGet]
        [Route("{id}/history")]
        public ActionResult<StockHistoryResponse> GetHistory([FromRoute]Guid id)
        {
            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            return Ok(stock.ToHistoryResponse());
        }

        // GET : /api/stocks/{id}/closingprices
        [HttpGet]
        [Route("{id}/closingprices")]
        public ActionResult<StockPriceResponse> GetClosingPrices([FromRoute]Guid id, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateTime.Today.AddYears(-1), (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return Ok(stock.ToPriceResponse(dateRange));
        }

        // POST : /api/stocks
        [HttpPost]
        public ActionResult CreateStock([FromBody] CreateStockCommand command)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (command.ChildSecurities.Count == 0)
                {
                    _StockRepository.ListStock(command.Id, command.AsxCode, command.Name, command.ListingDate, command.Trust, command.Category);
                }
                else
                {
                    if (command.Trust)
                        return BadRequest("A Stapled security cannot be a trust");

                    _StockRepository.ListStapledSecurity(command.Id, command.AsxCode, command.Name, command.ListingDate, command.Category, command.ChildSecurities.Select(x => new StapledSecurityChild(x.ASXCode, x.Name, x.Trust)));
                } 
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // POST : /api/stocks/{id}/change
        [Route("{id}/change")]
        [HttpPost]
        public ActionResult ChangeStock([FromRoute]Guid id, [FromBody] ChangeStockCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Id)
                return BadRequest("Id in command doesn't match id on URL");

            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            try
            {
                stock.ChangeProperties(command.ChangeDate, command.AsxCode, command.Name, command.Category);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // POST : /api/stocks/{id}/delist
        [Route("{id}/delist")]
        [HttpPost]
        public ActionResult DelistStock([FromRoute]Guid id, [FromBody] DelistStockCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Id)
                return BadRequest("Id in command doesn't match id on URL");

            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            try
            {
                stock.DeList(command.DelistingDate);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // POST : /api/stocks/{id}/closingprices
        [Route("{id}/closingprices")]
        [HttpPost]
        public ActionResult UpdateClosingPrices([FromRoute]Guid id, [FromBody] UpdateClosingPricesCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Id)
                return BadRequest("Id in command doesn't match id on URL");

            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            try
            {
                var closingPrices = new Dictionary<DateTime, decimal>();
                foreach (var closingPrice in command.Prices)
                    closingPrices.Add(closingPrice.Date, closingPrice.Price);

                stock.UpdateClosingPrices(command.Prices.Select(x => new Tuple<DateTime, decimal>(x.Date, x.Price)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // POST : /api/stocks/{id}/changedrp
        [Route("{id}/changedrp")]
        [HttpPost]
        public ActionResult ChangeDRP([FromRoute]Guid id, [FromBody] ChangeDividendReinvestmentPlanCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Id)
                return BadRequest("Id in command doesn't match id on URL");

            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            try
            {
                stock.ChangeDRPRules(command.ChangeDate, command.DRPActive, command.DividendRoundingRule, command.DRPMethod);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // GET : /api/stocks/{id}/relativenta
        [Route("{id}/relativenta")]
        [HttpGet]
        public ActionResult GetRelativeNTA([FromRoute]Guid id, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            var stapledSecurity = stock as StapledSecurity;
            if (stapledSecurity == null)
            {
                return BadRequest("Relative NTAs only apply stapled securities");
            }

            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return Ok(stapledSecurity.ToRelativeNTAResponse(dateRange));
        }

        // POST : /api/stocks/{id}/relativenta
        [Route("{id}/relativenta")]
        [HttpPost]
        public ActionResult ChangeRelativeNTA([FromRoute]Guid id, [FromBody] ChangeRelativeNTAsCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Id)
                return BadRequest("Id in command doesn't match id on URL");

            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            var stapledSecurity = stock as StapledSecurity;
            if (stapledSecurity == null)
            {
                return BadRequest("Relative NTAs only apply stapled securities");
            }

            if (command.RelativeNTAs.Count != stapledSecurity.ChildSecurities.Count)
            {
                return BadRequest(String.Format("The number of relative ntas provided ({0}) did not match the number of child securities ({1})", command.RelativeNTAs.Count, stapledSecurity.ChildSecurities.Count));
            }

            var ntas = new decimal[stapledSecurity.ChildSecurities.Count];
            for (var i = 0; i < stapledSecurity.ChildSecurities.Count; i++)
            {
                var nta = command.RelativeNTAs.Find(x => x.ChildSecurity == stapledSecurity.ChildSecurities[i].ASXCode);
                if (nta == null)
                    return BadRequest(String.Format("Relative nta not provided for {0}", stapledSecurity.ChildSecurities[i].ASXCode));

                ntas[i] = nta.Percentage;
            }
         
            try
            {
                stapledSecurity.SetRelativeNTAs(command.ChangeDate, ntas);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
        

        // GET : /api/stocks/{id}/corporateactions

        // GET : /api/stocks/{id}/corporateactions/{id}

        // POST : /api/stocks/{id}/corporateactions/{id}




        private bool MatchesQuery(StockProperties stock, string query)
        {
            return ((stock.ASXCode.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) || (stock.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0));
        }
    }

}
