using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using PortfolioManager.Common;
using PortfolioManager.Service.Local;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.Web.Controllers
{
    [Route("api/portfolio")]
    public class PortfolioController : Controller
    {
        private LocalPortfolioManagerService _PortfolioManagerService = new LocalPortfolioManagerService();

        public PortfolioController()
        {
            var portfolioDatabase = @"C:\Users\Craig\Documents\GitHubVisualStudio\PortfolioManager\PortfolioManager.Web\bin\Debug\netcoreapp1.0\Natalies Portfolio.db";
            var stockDatabase = @"C:\Users\Craig\Documents\GitHubVisualStudio\PortfolioManager\PortfolioManager.Web\bin\Debug\netcoreapp1.0\stocks.db";

            _PortfolioManagerService.Connect(portfolioDatabase, stockDatabase);
        }

        // GET: api/portfolio/summary?date
        [Route("summary")]
        [HttpGet]
        public async Task<PortfolioSummaryResponce> GetSummary(DateTime? date)
        {
            var service = _PortfolioManagerService.GetService<IPortfolioSummaryService>();

            if (date == null)
                date = DateTime.Today;

            return await service.GetSummary((DateTime)date);
        }

        // GET: /api/portfolio/properties
        [Route("properties")]
        [HttpGet]
        public async Task<PortfolioPropertiesResponce> GetProperties()
        {
            var service = _PortfolioManagerService.GetService<IPortfolioSummaryService>();

            return await service.GetProperties();
        }

        // GET: /api/portfolio/performance?fromDate&toDate
        [Route("performance")]
        [HttpGet]
        public async Task<PortfolioPerformanceResponce> GetPerformance(DateTime? fromDate, DateTime? toDate)
        {
            var service = _PortfolioManagerService.GetService<IPortfolioPerformanceService>();

            if (fromDate == null)
                fromDate = DateUtils.NoStartDate;
            if (toDate == null)
                toDate = DateTime.Today;

            return await service.GetPerformance((DateTime)fromDate, (DateTime)toDate);
        }

        // GET: /api/portfolio/capitalgains?date
        // GET: /api/portfolio/capitalgains?stock&date

        [Route("capitalgains")]
        [HttpGet]
        public async Task<SimpleUnrealisedGainsResponce> GetCapitalGains(Guid? stock, DateTime? date)
        {
            var service = _PortfolioManagerService.GetService<ICapitalGainService>();

            if (date == null)
                date = DateTime.Today;

            if (stock == null)
                return await service.GetSimpleUnrealisedGains((DateTime)date);
            else
                return await service.GetSimpleUnrealisedGains((Guid)stock, (DateTime)date);
        }

        // GET: /api/portfolio/detailedcapitalgains?date
        // GET: /api/portfolio/detailedcapitalgains?stock&date
        [Route("detailedcapitalgains")]
        [HttpGet]
        public async Task<DetailedUnrealisedGainsResponce> GetDetailedCapitalGains(Guid? stock, DateTime? date)
        {
            var service = _PortfolioManagerService.GetService<ICapitalGainService>();

            if (date == null)
                date = DateTime.Today;

            if (stock == null)
                return await service.GetDetailedUnrealisedGains((DateTime)date);
            else
                return await service.GetDetailedUnrealisedGains((Guid)stock, (DateTime)date);
        }

        // GET: /api/portfolio/cgtliability?fromDate&toDate
        [Route("cgtliability")]
        [HttpGet]
        public async Task<CGTLiabilityResponce> GetCGTLiability(DateTime? fromDate, DateTime? toDate)
        {
            var service = _PortfolioManagerService.GetService<ICapitalGainService>();

            if (fromDate == null)
                fromDate = DateUtils.NoStartDate;
            if (toDate == null)
                toDate = DateTime.Today;

           return await service.GetCGTLiability((DateTime)fromDate, (DateTime)toDate);
        }

        // GET: /api/portfolio/value?fromDate&toDate&frequency
        // GET: /api/portfolio/value?stock&fromDate&toDate&frequency
        [Route("value")]
        [HttpGet]
        public async Task<PortfolioValueResponce> GetPortfolioValue(Guid? stock, DateTime? fromDate, DateTime? toDate, ValueFrequency? freqency)
        {
            var service = _PortfolioManagerService.GetService<IPortfolioValueService>();

            if (fromDate == null)
                fromDate = DateUtils.NoStartDate;
            if (toDate == null)
                toDate = DateTime.Today;
            if (freqency == null)
                freqency = ValueFrequency.Daily;

            if (stock == null)
                return await service.GetPortfolioValue((DateTime)fromDate, (DateTime)toDate, (ValueFrequency)freqency);
            else
                return await service.GetPortfolioValue((Guid) stock, (DateTime)fromDate, (DateTime)toDate, (ValueFrequency)freqency);
        }


        // GET: /api/portfolio/corporateactions/unapplied
        // GET: /api/portfolio/corporateactions/transactions?id
        // GET: /api/portfolio/holdings?stock&date
        // GET: /api/portfolio/holdings?date
        // GET: /api/portfolio/holdings?date&tradeable=true
        // GET: /api/portfolio/cashaccount/transactions?fromDate&toDate
        // GET: /api/portfolio/cashaccount/transactions/id
        // GET: /api/portfolio/income?fromDate&toDate
    }
}
