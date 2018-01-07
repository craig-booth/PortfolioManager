﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using PortfolioManager.Common;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.Web.Controllers
{
    [Route("api/portfolio")]
    public class PortfolioController : Controller
    {
        private IServiceProvider _ServiceProvider;

        public PortfolioController(IServiceProvider serviceProvider)
        {
            _ServiceProvider = serviceProvider;
        }

        // GET: api/portfolio/summary?date
        [Route("summary")]
        [HttpGet]
        public async Task<PortfolioSummaryResponce> GetSummary(DateTime? date)
        {
            var service = _ServiceProvider.GetRequiredService<IPortfolioSummaryService>();
            if (date == null)
                date = DateTime.Today;

            return await service.GetSummary((DateTime)date);
        }

        // GET: /api/portfolio/properties
        [Route("properties")]
        [HttpGet]
        public async Task<PortfolioPropertiesResponce> GetProperties()
        {
            var service = _ServiceProvider.GetRequiredService<IPortfolioSummaryService>();

            return await service.GetProperties();
        }

        // GET: /api/portfolio/performance?fromDate&toDate
        [Route("performance")]
        [HttpGet]
        public async Task<PortfolioPerformanceResponce> GetPerformance(DateTime? fromDate, DateTime? toDate)
        {
            var service = _ServiceProvider.GetRequiredService<IPortfolioPerformanceService>();

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
            var service = _ServiceProvider.GetRequiredService<ICapitalGainService>();

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
            var service = _ServiceProvider.GetRequiredService<ICapitalGainService>();

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
            var service = _ServiceProvider.GetRequiredService<ICapitalGainService>();

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
            var service = _ServiceProvider.GetRequiredService<IPortfolioValueService>();

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
        [Route("corporateactions/unapplied")]
        [HttpGet]
        public async Task<UnappliedCorporateActionsResponce> GetUnappliedCorporateActions()
        {
            var service = _ServiceProvider.GetRequiredService<ICorporateActionService>();

            return await service.GetUnappliedCorporateActions(); 
        }

        // GET: /api/portfolio/corporateactions/transactions?id
        [Route("corporateactions/transactions")]
        [HttpGet]
        public async Task<TransactionsForCorparateActionsResponce> GetCorporateActionTransactions(Guid id)
        {
            var service = _ServiceProvider.GetRequiredService<ICorporateActionService>();

            return await service.TransactionsForCorporateAction(id); 
        }

        // GET: /api/portfolio/holding?stock&date
        [Route("holding")]
        [HttpGet]
        public async Task<HoldingResponce> GetHolding(Guid? stock, DateTime? date)
        {
            var service = _ServiceProvider.GetRequiredService<IHoldingService>();

            if (date == null)
                date = DateTime.Now;
            
            return await service.GetHolding((Guid)stock, (DateTime)date); 
        }

        // GET: /api/portfolio/holdings?date
        // GET: /api/portfolio/holdings?date&tradeable=true
        [Route("holdings")]
        [HttpGet]
        public async Task<HoldingsResponce> GetHoldings(DateTime? date, bool? tradeable)
        {
            var service = _ServiceProvider.GetRequiredService<IHoldingService>();

            if (date == null)
                date = DateTime.Now;

            if ((tradeable != null) && (bool)tradeable)
                return await service.GetTradeableHoldings((DateTime)date);
            else
                return await service.GetHoldings((DateTime)date);
        }

        // GET: /api/portfolio/cashaccount/transactions?fromDate&toDate
        [Route("cashaccount/transactions")]
        [HttpGet]
        public async Task<CashAccountTransactionsResponce> GetCashAccountTransactions(DateTime? fromDate, DateTime? toDate)
        {
            var service = _ServiceProvider.GetRequiredService<ICashAccountService>();

            if (fromDate == null)
                fromDate = DateUtils.NoStartDate;
            if (toDate == null)
                toDate = DateTime.Today;

            return await service.GetTranasctions((DateTime)fromDate, (DateTime)toDate);
        }

        // GET: /api/portfolio/income?fromDate&toDate
        [Route("income")]
        [HttpGet]
        public async Task<IncomeResponce> GetIncome(DateTime? fromDate, DateTime? toDate)
        {
            var service = _ServiceProvider.GetRequiredService<IIncomeService>();

            if (fromDate == null)
                fromDate = DateUtils.NoStartDate;
            if (toDate == null)
                toDate = DateTime.Today;

            return await service.GetIncome((DateTime)fromDate, (DateTime)toDate);
        }
    }
}