using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


using PortfolioManager.Common;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Services;

namespace PortfolioManager.Web.Controllers.v2
{

    [Route("api/v2/portfolio/{portfolioId:guid}")]
    [PortfolioExceptionFilter]
    [PortfolioOwnerAuthorize]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        // GET: properties
        [Route("properties")]
        [HttpGet]
        public ActionResult<PortfolioPropertiesResponse> GetProperties([FromServices] IPortfolioPropertiesService service, [FromRoute] Guid portfolioId)
        {
            return service.GetProperties(portfolioId);        
        } 

        // GET: summary?date
        [Route("summary")]
        [HttpGet]
        public ActionResult<PortfolioSummaryResponse> GetSummary([FromServices] IPortfolioSummaryService service, [FromRoute] Guid portfolioId, [FromQuery] DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            return service.GetSummary(portfolioId, requestedDate);        
        } 


        // GET: performance?fromDate&toDate
        [Route("performance")]
        [HttpGet]
        public ActionResult<PortfolioPerformanceResponse> GetPerformance([FromServices] IPortfolioPerformanceService service, [FromRoute] Guid portfolioId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return service.GetPerformance(portfolioId, dateRange);
        } 

        // GET: value?fromDate&toDate
        [Route("value")]
        [HttpGet]
        public ActionResult<PortfolioValueResponse> GetValue([FromServices] IPortfolioValueService service, [FromRoute] Guid portfolioId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] ValueFrequency? frequency)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : (toDate != null) ? ((DateTime)toDate).AddYears(-1) : DateTime.Today.AddYears(-1), (toDate != null) ? (DateTime)toDate : DateTime.Today);
            var requestedFrequency = (frequency != null) ? (ValueFrequency)frequency : ValueFrequency.Daily;

            return service.GetValue(portfolioId, dateRange, requestedFrequency);
        } 

        // GET: transactions?fromDate&toDate
        [Route("transactions")]
        [HttpGet]
        public ActionResult<TransactionsResponse> GetTransactions([FromServices] IPortfolioTransactionService service, [FromRoute] Guid portfolioId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return service.GetTransactions(portfolioId, dateRange);
        }  

        // GET: capitalgains?date
        [Route("capitalgains")]
        [HttpGet]
        public ActionResult<SimpleUnrealisedGainsResponse> GetCapitalGains([FromServices] IPortfolioCapitalGainsService service, [FromRoute] Guid portfolioId, [FromQuery] DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            return service.GetCapitalGains(portfolioId, requestedDate);
        } 

        // GET: detailedcapitalgains?date
        [Route("detailedcapitalgains")]
        [HttpGet]
        public ActionResult<DetailedUnrealisedGainsResponse> GetDetailedCapitalGains([FromServices] IPortfolioCapitalGainsService service, [FromRoute] Guid portfolioId, [FromQuery] DateTime? date)
        {
            var requestedDate = (date != null) ? (DateTime)date : DateTime.Today;

            return service.GetDetailedCapitalGains(portfolioId, requestedDate);         
        } 

        // GET: cgtliability?fromDate&toDate
        [Route("cgtliability")]
        [HttpGet]
        public ActionResult<CgtLiabilityResponse> GetCGTLiability([FromServices] IPortfolioCgtLiabilityService service, [FromRoute] Guid portfolioId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return service.GetCGTLiability(portfolioId, dateRange);
        } 

        // GET: cashaccount?fromDate&toDate
        [Route("cashaccount")]
        [HttpGet]
        public ActionResult<CashAccountTransactionsResponse> GetCashAccountTransactions([FromServices] ICashAccountService service, [FromRoute] Guid portfolioId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return service.GetCashAccountTransactions(portfolioId, dateRange);
        }  

        // GET: income?fromDate&toDate
        [Route("income")]
        [HttpGet]
        public ActionResult<IncomeResponse> GetIncome([FromServices] IPortfolioIncomeService service, [FromRoute] Guid portfolioId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var dateRange = new DateRange((fromDate != null) ? (DateTime)fromDate : DateUtils.NoStartDate, (toDate != null) ? (DateTime)toDate : DateTime.Today);

            return service.GetIncome(portfolioId, dateRange);
        } 

        // GET: corporateactions
        [Route("corporateactions")]
        [HttpGet]
        public ActionResult<CorporateActionsResponse> GetCorporateActions([FromServices] IPortfolioCorporateActionsService service, [FromRoute] Guid portfolioId)
        {
            return service.GetCorporateActions(portfolioId);
        } 
    }

}
