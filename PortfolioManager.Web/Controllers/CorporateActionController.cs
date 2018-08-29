using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.CorporateActions;
using PortfolioManager.RestApi.Stocks;
using PortfolioManager.RestApi.CorporateActions;

namespace PortfolioManager.Web.Controllers
{

    [Route("api/stocks/{id:guid}/corporateactions")]
    public class CorporateActionController : Controller
    {
        private IStockRepository _StockRepository;

        public CorporateActionController(IStockRepository stockRepository)
        {
            _StockRepository = stockRepository;
        }

        // GET : /api/stocks/{id}/corporateactions
        [Route("")]
        [HttpGet]
        public ActionResult GetCorporateActions([FromRoute]Guid id, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return Ok(stock.CorporateActions.Get(dateRange).Select(x => CorporateActionResponse(x)));
        }

        // GET : /api/stocks/{id}/corporateactions/{id}
        [Route("{actionid:guid}")]
        [HttpGet]
        public ActionResult GetCorporateAction([FromRoute]Guid id, [FromRoute]Guid actionId)
        {
            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            var corporateAction = stock.CorporateActions[actionId];
            if (corporateAction == null)
                return NotFound();

            var response = CorporateActionResponse(corporateAction);
            if (response == null)
                return BadRequest("Unknown corporate action type");

            return Ok(response);       
        }

        private RestApi.CorporateActions.CorporateAction CorporateActionResponse(Domain.CorporateActions.CorporateAction corporateAction)
        {
            if (corporateAction.Type == CorporateActionType.Dividend)
                return (corporateAction as Domain.CorporateActions.Dividend).ToResponse();
            else if (corporateAction.Type == CorporateActionType.CapitalReturn)
                return (corporateAction as Domain.CorporateActions.CapitalReturn).ToResponse();
            else if (corporateAction.Type == CorporateActionType.Transformation)
                return (corporateAction as Domain.CorporateActions.Transformation).ToResponse();
            else
                return null;
        }

        // POST : /api/stocks/{id}/corporateactions
        [Route("")]
        [HttpPost]
        public ActionResult AddCorporateAction([FromRoute]Guid id, [FromBody] RestApi.CorporateActions.CorporateAction corporateAction)
        {
            if (corporateAction == null)
                return BadRequest("Unknown Corporate Action type");

            // Check id in URL and id in command match
            if (id != corporateAction.Stock)
                return BadRequest("Id in command doesn't match id on URL");

            var stock = _StockRepository.Get(id);
            if (stock == null)
                return NotFound();

            try
            {
                if (corporateAction is RestApi.CorporateActions.Dividend)
                    AddDividend(stock, corporateAction as RestApi.CorporateActions.Dividend);
                else if (corporateAction is RestApi.CorporateActions.CapitalReturn)
                    AddCapitalReturn(stock, corporateAction as RestApi.CorporateActions.CapitalReturn);
                else if (corporateAction is RestApi.CorporateActions.Transformation)
                    AddTransformation(stock, corporateAction as RestApi.CorporateActions.Transformation);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            } 

            return Ok();
        }

        private void AddDividend(Stock stock, RestApi.CorporateActions.Dividend dividend)
        {
            stock.CorporateActions.AddDividend(dividend.Id, dividend.ActionDate, dividend.Description, dividend.PaymentDate, dividend.DividendAmount, dividend.CompanyTaxRate, dividend.PercentFranked, dividend.DRPPrice);
        }

        private void AddCapitalReturn(Stock stock, RestApi.CorporateActions.CapitalReturn capitalReturn)
        {
            stock.CorporateActions.AddCapitalReturn(capitalReturn.Id, capitalReturn.ActionDate, capitalReturn.Description, capitalReturn.PaymentDate, capitalReturn.Amount);
        }

        private void AddTransformation(Stock stock, RestApi.CorporateActions.Transformation transformation)
        {
            var resultingStocks = transformation.ResultingStocks.Select(x => new Domain.CorporateActions.Transformation.ResultingStock(x.Stock, x.OriginalUnits, x.NewUnits, x.CostBase, x.AquisitionDate));
            stock.CorporateActions.AddTransformation(transformation.Id, transformation.ActionDate, transformation.Description, transformation.ImplementationDate, transformation.CashComponent, transformation.RolloverRefliefApplies, resultingStocks);
        }
    }

}

