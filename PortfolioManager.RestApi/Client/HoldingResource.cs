using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Portfolios;

namespace PortfolioManager.RestApi.Client
{
    public class HoldingResource : RestResource
    {

        public HoldingResource(ClientSession session)
            : base(session)
        {
        }

        public async Task<List<Holding>> Get(DateTime date)
        {
            var url = "/api/v2/portfolio/" + _Session.Portfolio + "/holdings?date=" +date.ToIsoDateString();

            return await GetAsync<List<Holding>>(url);

        }
        public async Task<List<Holding>> Get(DateRange dateRange)
        {
            var url = "/api/v2/portfolio/" + _Session.Portfolio + "/holdings?fromdate=" + dateRange.FromDate.ToIsoDateString() + "&todate=" + dateRange.ToDate.ToIsoDateString();

            return await GetAsync<List<Holding>>(url);
        }

        public async Task<Holding> Get(Guid stockId, DateTime date)
        {
            var url = "/api/v2/portfolio/" + _Session.Portfolio + "/holdings/" + stockId + "?date=" + date.ToIsoDateString();

            return await GetAsync<Holding>(url);
        }

        public async Task<PortfolioValueResponse> GetValue(Guid stockId, DateRange dateRange, ValueFrequency frequency)
        {
            var url = "/api/v2/portfolio/" + _Session.Portfolio + "/holdings/" + stockId + "/value?fromdate=" + dateRange.FromDate.ToIsoDateString() + "&todate=" + dateRange.ToDate.ToIsoDateString();

            if (frequency == ValueFrequency.Weekly)
                url += "&frequency=weekly";
            else if (frequency == ValueFrequency.Monthly)
                url += "&frequency=monthly";
            else
                url += "&frequency=daily";

            return await GetAsync<PortfolioValueResponse>(url);
        }

        public async Task<TransactionsResponse> GetTransactions(Guid stockId, DateRange dateRange)
        {
            return await GetAsync<TransactionsResponse>("/api/v2/portfolio/" + _Session.Portfolio + "/holdings/" + stockId + "/transactions?fromdate=" + dateRange.FromDate.ToIsoDateString() + "&todate=" + dateRange.ToDate.ToIsoDateString());
        }

        public async Task<SimpleUnrealisedGainsResponse> GetCapitalGains(Guid stockId, DateTime date)
        {
            return await GetAsync<SimpleUnrealisedGainsResponse>("/api/v2/portfolio/" + _Session.Portfolio + "/holdings/" + stockId + "/capitalgains?date=" + date.ToIsoDateString());
        }

        public async Task<DetailedUnrealisedGainsResponse> GetDetailedCapitalGains(Guid stockId, DateTime date)
        {
            return await GetAsync<DetailedUnrealisedGainsResponse>("/api/v2/portfolio/" + _Session.Portfolio + "/holdings/" + stockId + "/detailedcapitalgains?date=" + date.ToIsoDateString());
        }

        public async Task<CorporateActionsResponse> GetCorporateActions(Guid stockId)
        {
            return await GetAsync<CorporateActionsResponse>("/api/v2/portfolio/" + _Session.Portfolio + "/holdings/" + stockId + "/corporateactions");
        }

        public async Task<bool> ChangeDrpParticipation(Guid stockId, bool participate)
        {
            return await PostAsync("/api/v2/portfolio/" + _Session.Portfolio + "/holdings/" + stockId + "/changedrpparticipation?participate=" + participate.ToString());
        }
    }
}
