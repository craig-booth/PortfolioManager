using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Portfolios;

namespace PortfolioManager.RestApi.Client
{
    public class PortfolioResource : RestResource
    {
        public Guid PortfolioId { get; }

        public PortfolioResource(Guid portfolioId, HttpClient httpClient)
            : base(httpClient)
        {
            PortfolioId = portfolioId;
        }

        public async Task<PortfolioSummaryResponse> GetSummary(DateTime date)
        {
            return await GetAsync<PortfolioSummaryResponse>("/api/v2/portfolio/" + PortfolioId + "/summary?date=" + date.ToIsoDateString());
        }

        public async Task<PortfolioPerformanceResponse> GetPerformance(DateRange dateRange)
        {
            return await GetAsync<PortfolioPerformanceResponse>("/api/v2/portfolio/" + PortfolioId + "/performance?fromdate=" + dateRange.FromDate.ToIsoDateString() + "?todate=" + dateRange.ToDate.ToIsoDateString());
        }

        public async Task<PortfolioValueResponse> GetValue(DateRange dateRange)
        {
            return await GetAsync<PortfolioValueResponse>("/api/v2/portfolio/" + PortfolioId + "/value?fromdate=" + dateRange.FromDate.ToIsoDateString() + "?todate=" + dateRange.ToDate.ToIsoDateString());
        }

        public async Task<SimpleUnrealisedGainsResponse> GetCapitalGains(DateTime date)
        {
            return await GetAsync<SimpleUnrealisedGainsResponse>("/api/v2/portfolio/" + PortfolioId + "/capitalgains?date=" + date.ToIsoDateString());
        }

        public async Task<SimpleUnrealisedGainsResponse> GetCapitalGains(Guid stock, DateTime date)
        {
            return await GetAsync<SimpleUnrealisedGainsResponse>("/api/v2/portfolio/" + PortfolioId + "/capitalgains?stock=" + stock.ToString() +  "&date=" + date.ToIsoDateString());
        }

        public async Task<DetailedUnrealisedGainsResponse> GetDetailedCapitalGains(DateTime date)
        {
            return await GetAsync<DetailedUnrealisedGainsResponse>("/api/v2/portfolio/" + PortfolioId + "/detailedcapitalgains?date=" + date.ToIsoDateString());
        }

        public async Task<DetailedUnrealisedGainsResponse> GetDetailedCapitalGains(Guid stock, DateTime date)
        {
            return await GetAsync<DetailedUnrealisedGainsResponse>("/api/v2/portfolio/" + PortfolioId + "/capitalgains?stock=" + stock.ToString() + "&date=" + date.ToIsoDateString());
        }

        public async Task<CGTLiabilityResponse> GetCGTLiability(DateRange dateRange)
        { 
            return await GetAsync<CGTLiabilityResponse>("/api/v2/portfolio/" + PortfolioId + "/cgtliability?fromdate=" + dateRange.FromDate.ToIsoDateString() + "?todate=" + dateRange.ToDate.ToIsoDateString());
        }
    }
}
