﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Portfolios;

namespace PortfolioManager.RestApi.Client
{

    public class PortfolioResource : RestResource
    {
        public PortfolioResource(ClientSession session)
            : base(session)
        {
        }

        public async Task<PortfolioPropertiesResponse> GetProperties()
        {
            return await GetAsync<PortfolioPropertiesResponse>("/api/v2/portfolio/" + _Session.Portfolio + "/properties");
        }

        public async Task<PortfolioSummaryResponse> GetSummary(DateTime date)
        {
            return await GetAsync<PortfolioSummaryResponse>("/api/v2/portfolio/" + _Session.Portfolio + "/summary?date=" + date.ToIsoDateString());
        }

        public async Task<PortfolioPerformanceResponse> GetPerformance(DateRange dateRange)
        {
            return await GetAsync<PortfolioPerformanceResponse>("/api/v2/portfolio/" + _Session.Portfolio + "/performance?fromdate=" + dateRange.FromDate.ToIsoDateString() + "&todate=" + dateRange.ToDate.ToIsoDateString());
        }

        public async Task<PortfolioValueResponse> GetValue(DateRange dateRange, ValueFrequency frequency)
        {
            var url = "/api/v2/portfolio/" + _Session.Portfolio + "/value?fromdate=" + dateRange.FromDate.ToIsoDateString() + "&todate=" + dateRange.ToDate.ToIsoDateString();

            if (frequency == ValueFrequency.Weekly)
                url += "&frequency=weekly";
            else if (frequency == ValueFrequency.Monthly)
                url += "&frequency=monthly";
            else
                url += "&frequency=daily";

            return await GetAsync<PortfolioValueResponse>(url);
        }

        public async Task<TransactionsResponse> GetTransactions(DateRange dateRange)
        {
            return await GetAsync<TransactionsResponse>("/api/v2/portfolio/" + _Session.Portfolio + "/transactions?fromdate=" + dateRange.FromDate.ToIsoDateString() + "&todate=" + dateRange.ToDate.ToIsoDateString());
        }

        public async Task<SimpleUnrealisedGainsResponse> GetCapitalGains(DateTime date)
        {
            return await GetAsync<SimpleUnrealisedGainsResponse>("/api/v2/portfolio/" + _Session.Portfolio + "/capitalgains?date=" + date.ToIsoDateString());
        }

        public async Task<DetailedUnrealisedGainsResponse> GetDetailedCapitalGains(DateTime date)
        {
            return await GetAsync<DetailedUnrealisedGainsResponse>("/api/v2/portfolio/" + _Session.Portfolio + "/detailedcapitalgains?date=" + date.ToIsoDateString());
        }

        public async Task<CgtLiabilityResponse> GetCGTLiability(DateRange dateRange)
        {
            return await GetAsync<CgtLiabilityResponse>("/api/v2/portfolio/" + _Session.Portfolio + "/cgtliability?fromdate=" + dateRange.FromDate.ToIsoDateString() + "&todate=" + dateRange.ToDate.ToIsoDateString());
        }

        public async Task<CashAccountTransactionsResponse> GetCashAccount(DateRange dateRange)
        {
            return await GetAsync<CashAccountTransactionsResponse>("/api/v2/portfolio/" + _Session.Portfolio + "/cashaccount?fromdate=" + dateRange.FromDate.ToIsoDateString() + "&todate=" + dateRange.ToDate.ToIsoDateString());
        }

        public async Task<IncomeResponse> GetIncome(DateRange dateRange)
        {
            return await GetAsync<IncomeResponse>("/api/v2/portfolio/" + _Session.Portfolio + "/income?fromdate=" + dateRange.FromDate.ToIsoDateString() + "&todate=" + dateRange.ToDate.ToIsoDateString());
        }

        public async Task<CorporateActionsResponse> GetCorporateActions()
        {
            return await GetAsync<CorporateActionsResponse>("/api/v2/portfolio/" + _Session.Portfolio + "/corporateactions");
        }
    }
}
