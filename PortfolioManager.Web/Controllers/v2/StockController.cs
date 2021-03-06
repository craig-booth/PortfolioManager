﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

using PortfolioManager.Common;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.RestApi.Stocks;

using PortfolioManager.Web.Mappers;
using PortfolioManager.Web.Services;
using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.Controllers.v2
{
    [Authorize]
    [Route("api/v2/stocks")]
    public class StockController : Controller
    {
        private IStockQuery _StockQuery;
        private IStockService _StockService;

        public StockController(IStockQuery stockQuery, IStockService stockService)
        {
            _StockQuery = stockQuery;
            _StockService = stockService;
        }

        // GET: api/stocks
        [HttpGet]
        public ActionResult<List<StockResponse>> Get([FromQuery]string query, [FromQuery]DateTime? date, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            IEnumerable<Stock> stocks;
            DateTime resultDate;
        
            if (date != null)
            {
                if (query == null)
                    stocks = _StockQuery.All((DateTime)date);
                else
                    stocks = _StockQuery.Find((DateTime)date, x => MatchesQuery(x, query));

                resultDate = (DateTime)date;
            }
            else
            {
                var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateUtils.NoEndDate);

                if (query == null)
                    stocks = _StockQuery.All(dateRange);
                else
                    stocks = stocks = _StockQuery.Find(dateRange, x => MatchesQuery(x, query));

                resultDate = dateRange.ToDate;
            }

            return Ok(stocks.Select(x => x.ToResponse(resultDate)).ToList());
        }

        // GET: api/stocks/{id}
        [HttpGet]
        [Route("{id:guid}")]
        public ActionResult<StockResponse> Get([FromRoute]Guid id, [FromQuery]DateTime? date)
        {
            var stock = _StockQuery.Get(id);
            if (stock == null)
                return NotFound();

            if (date != null)
                return Ok(stock.ToResponse((DateTime)date));
            else
                return Ok(stock.ToResponse(DateTime.Today));
        }

        // GET : /api/stocks/{id}/history
        [HttpGet]
        [Route("{id:guid}/history")]
        public ActionResult<StockHistoryResponse> GetHistory([FromRoute]Guid id)
        {
            var stock = _StockQuery.Get(id);
            if (stock == null)
                return NotFound();

            return Ok(stock.ToHistoryResponse());
        }

        // GET : /api/stocks/{id}/closingprices
        [HttpGet]
        [Route("{id:guid}/closingprices")]
        public ActionResult<StockPriceResponse> GetClosingPrices([FromRoute]Guid id, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            var stock = _StockQuery.Get(id);
            if (stock == null)
                return NotFound();

            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateTime.Today.AddYears(-1), (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return Ok(stock.ToPriceResponse(dateRange));
        }

        // POST : /api/stocks
        [Authorize(Policy.CanMantainStocks)]
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
                    _StockService.ListStock(command.Id, command.AsxCode, command.Name, command.ListingDate, command.Trust, command.Category);
                }
                else
                {
                    if (command.Trust)
                        return BadRequest("A Stapled security cannot be a trust");

                    _StockService.ListStapledSecurity(command.Id, command.AsxCode, command.Name, command.ListingDate, command.Category, command.ChildSecurities.Select(x => new StapledSecurityChild(x.ASXCode, x.Name, x.Trust)));
                } 
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // POST : /api/stocks/{id}/change
        [Authorize(Policy.CanMantainStocks)]
        [Route("{id:guid}/change")]
        [HttpPost]
        public ActionResult ChangeStock([FromRoute]Guid id, [FromBody] ChangeStockCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Id)
                return BadRequest("Id in command doesn't match id on URL");

            var stock = _StockQuery.Get(id);
            if (stock == null)
                return NotFound();

            try
            {
                _StockService.ChangeStock(id, command.ChangeDate, command.AsxCode, command.Name, command.Category);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // POST : /api/stocks/{id}/delist
        [Authorize(Policy.CanMantainStocks)]
        [Route("{id:guid}/delist")]
        [HttpPost]
        public ActionResult DelistStock([FromRoute]Guid id, [FromBody] DelistStockCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Id)
                return BadRequest("Id in command doesn't match id on URL");

            var stock = _StockQuery.Get(id);
            if (stock == null)
                return NotFound();

            try
            {
                _StockService.DelistStock(id, command.DelistingDate);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // POST : /api/stocks/{id}/closingprices
        [Authorize(Policy.CanMantainStocks)]
        [Route("{id:guid}/closingprices")]
        [HttpPost]
        public ActionResult UpdateClosingPrices([FromRoute]Guid id, [FromBody] UpdateClosingPricesCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Id)
                return BadRequest("Id in command doesn't match id on URL");

            var stock = _StockQuery.Get(id);
            if (stock == null)
                return NotFound();

            var closingPrices = command.Prices.Select(x => new Tuple<DateTime, decimal>(x.Date, x.Price));

            // Check that the date is within the effective period
            foreach (var closingPrice in closingPrices)
            {
                if (stock.IsEffectiveAt(closingPrice.Item1))
                    throw new Exception(String.Format("Stock not active on {0}", closingPrice.Item1));
            }

            try
            {
                _StockService.UpdateClosingPrices(id, closingPrices);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // POST : /api/stocks/{id}/changedividendrules
        [Authorize(Policy.CanMantainStocks)]
        [Route("{id}/changedividendrules")]
        [HttpPost]
        public ActionResult ChangeDividendRules([FromRoute]Guid id, [FromBody] ChangeDividendRulesCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Id)
                return BadRequest("Id in command doesn't match id on URL");

            var stock = _StockQuery.Get(id);
            if (stock == null)
                return NotFound();

            try
            {
                _StockService.ChangeDividendRules(id, command.ChangeDate, command.CompanyTaxRate, command.DividendRoundingRule, command.DRPActive, command.DRPMethod);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        // GET : /api/stocks/{id}/relativenta
        [Route("{id:guid}/relativenta")]
        [HttpGet]
        public ActionResult GetRelativeNTA([FromRoute]Guid id, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            var stock = _StockQuery.Get(id);
            if (stock == null)
                return NotFound();

            if (stock is StapledSecurity stapledSecurity)
            {
                var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

                return Ok(stapledSecurity.ToRelativeNTAResponse(dateRange));
            }
            else
            {
                return BadRequest("Relative NTAs only apply stapled securities");
            }


        }

        // POST : /api/stocks/{id}/relativenta
        [Authorize(Policy.CanMantainStocks)]
        [Route("{id:guid}/relativenta")]
        [HttpPost]
        public ActionResult ChangeRelativeNTA([FromRoute]Guid id, [FromBody] ChangeRelativeNTAsCommand command)
        {
            // Check id in URL and id in command match
            if (id != command.Id)
                return BadRequest("Id in command doesn't match id on URL");

            var stock = _StockQuery.Get(id);
            if (stock == null)
                return NotFound();

            if (stock is StapledSecurity stapledSecurity)
            {
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
                    _StockService.ChangeRelativeNTAs(id, command.ChangeDate, ntas);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }

                return Ok();
            }
            else
            {
                return BadRequest("Relative NTAs only apply stapled securities");
            }

 
        }

        private bool MatchesQuery(StockProperties stock, string query)
        {
            return ((stock.ASXCode.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) || (stock.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0));
        }
    }

}
