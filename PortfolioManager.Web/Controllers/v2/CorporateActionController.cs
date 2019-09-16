using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using PortfolioManager.Common;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.RestApi.CorporateActions;
using PortfolioManager.Web.Mappers;

namespace PortfolioManager.Web.Controllers.v2
{
    [Authorize]
    [Route("api/v2/stocks/{stockId:guid}/corporateactions")]
    public class CorporateActionController : ControllerBase
    {
        private IRepository<Stock> _StockRepository;

        public CorporateActionController(IRepository<Stock> stockRepository)
        {
            _StockRepository = stockRepository;
        }

        // GET : /api/stocks/{stockId}/corporateactions
        [Route("")]
        [HttpGet]
        public ActionResult<List<CorporateAction>> GetCorporateActions([FromRoute]Guid stockId, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            var stock = _StockRepository.Get(stockId);
            if (stock == null)
                return NotFound();

            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return stock.CorporateActions.InDateRange(dateRange).Select(x => CorporateActionResponse(x)).ToList();
        }

        // GET : /api/stocks/{stockId}/corporateactions/{id}
        [Route("{id:guid}")]
        [HttpGet]
        public ActionResult<CorporateAction> GetCorporateAction([FromRoute]Guid stockId, [FromRoute]Guid id)
        {
            var stock = _StockRepository.Get(stockId);
            if (stock == null)
                return NotFound();

            var corporateAction = stock.CorporateActions[id];
            if (corporateAction == null)
                return NotFound();

            var response = CorporateActionResponse(corporateAction);
            if (response == null)
                return BadRequest("Unknown corporate action type");

            return response;       
        }

        private CorporateAction CorporateActionResponse(Domain.CorporateActions.CorporateAction corporateAction)
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

        // POST : /api/stocks/{stockId}/corporateactions
        [Authorize(Policy.CanMantainStocks)]
        [Route("")]
        [HttpPost]
        public ActionResult AddCorporateAction([FromRoute]Guid stockId, [FromBody] RestApi.CorporateActions.CorporateAction corporateAction)
        {
            if (corporateAction == null)
                return BadRequest("Unknown Corporate Action type");

            // Check id in URL and id in command match
            if (stockId != corporateAction.Stock)
                return BadRequest("Id in command doesn't match id on URL");

            var stock = _StockRepository.Get(stockId);
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

                _StockRepository.Update(stock);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            } 

            return Ok();
        }

        private void AddDividend(Stock stock, RestApi.CorporateActions.Dividend dividend)
        {
            stock.CorporateActions.AddDividend(dividend.Id, dividend.ActionDate, dividend.Description, dividend.PaymentDate, dividend.DividendAmount, dividend.PercentFranked, dividend.DRPPrice);
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

