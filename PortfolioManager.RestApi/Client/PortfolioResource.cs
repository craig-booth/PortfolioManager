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

        public async Task<PortfolioValueResponse> GetValue(DateRange dateRange, ValueFrequency frequency)
        {
            var url = "/api/v2/portfolio/" + PortfolioId + "/value?fromdate=" + dateRange.FromDate.ToIsoDateString() + "?todate=" + dateRange.ToDate.ToIsoDateString();

            if (frequency == ValueFrequency.Weekly)
                url += "&frequency=weekly";
            else if (frequency == ValueFrequency.Monthly)
                url += "&frequency=monthly";
            else
                url += "&frequency=daily";

            return await GetAsync<PortfolioValueResponse>(url);
        }

        public async Task<PortfolioValueResponse> GetValue(Guid stock, DateRange dateRange, ValueFrequency frequency)
        {
            var url = "/api/v2/portfolio/" + PortfolioId + "/value?stock=" + stock.ToString() + "&fromdate=" + dateRange.FromDate.ToIsoDateString() + "?todate=" + dateRange.ToDate.ToIsoDateString();

            if (frequency == ValueFrequency.Weekly)
                url += "&frequency=week";
            else if (frequency == ValueFrequency.Monthly)
                url += "&frequency=month";
            else
                url += "&frequency=day";

            return await GetAsync<PortfolioValueResponse>(url);
        }

        public async Task<SimpleUnrealisedGainsResponse> GetCapitalGains(DateTime date)
        {
            return await GetAsync<SimpleUnrealisedGainsResponse>("/api/v2/portfolio/" + PortfolioId + "/capitalgains?date=" + date.ToIsoDateString());
        }

        public async Task<SimpleUnrealisedGainsResponse> GetCapitalGains(Guid stock, DateTime date)
        {
            return await GetAsync<SimpleUnrealisedGainsResponse>("/api/v2/portfolio/" + PortfolioId + "/capitalgains?stock=" + stock.ToString() + "&date=" + date.ToIsoDateString());
        }

        public async Task<DetailedUnrealisedGainsResponse> GetDetailedCapitalGains(DateTime date)
        {
            return await GetAsync<DetailedUnrealisedGainsResponse>("/api/v2/portfolio/" + PortfolioId + "/detailedcapitalgains?date=" + date.ToIsoDateString());
        }

        public async Task<DetailedUnrealisedGainsResponse> GetDetailedCapitalGains(Guid stock, DateTime date)
        {
            return await GetAsync<DetailedUnrealisedGainsResponse>("/api/v2/portfolio/" + PortfolioId + "/capitalgains?stock=" + stock.ToString() + "&date=" + date.ToIsoDateString());
        }

        public async Task<CgtLiabilityResponse> GetCGTLiability(DateRange dateRange)
        {
            return await GetAsync<CgtLiabilityResponse>("/api/v2/portfolio/" + PortfolioId + "/cgtliability?fromdate=" + dateRange.FromDate.ToIsoDateString() + "?todate=" + dateRange.ToDate.ToIsoDateString());
        }

        public async Task<CashAccountTransactionsResponse> GetCashAccount(DateRange dateRange)
        {
            return await GetAsync<CashAccountTransactionsResponse>("/api/v2/portfolio/" + PortfolioId + "/cashaccount?fromdate=" + dateRange.FromDate.ToIsoDateString() + "?todate=" + dateRange.ToDate.ToIsoDateString());
        }
    }
}
