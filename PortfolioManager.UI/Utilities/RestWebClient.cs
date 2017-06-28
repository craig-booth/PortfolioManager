using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;

using PortfolioManager.Service.Interface;

namespace PortfolioManager.UI.Utilities
{
    class RestWebClient
    {
        private string _BaseURL;

        public RestWebClient(string baseURL)
        {
            _BaseURL = baseURL;
        }
     
        public async Task<PortfolioPropertiesResponce> GetPortfolioPropertiesAsync()
        {
            return await GetAsync<PortfolioPropertiesResponce>("/api/portfolio/properties");
        }

        public async Task<PortfolioSummaryResponce> GetPortfolioSummaryAsync(DateTime date)
        {
            return await GetAsync<PortfolioSummaryResponce>("/api/portfolio/summary?date=" + date.ToString("yyyy-MM-dd"));
        }

        public async Task<PortfolioPerformanceResponce> GetPortfolioPerformanceAsync(DateTime fromDate, DateTime toDate)
        {
            return await GetAsync<PortfolioPerformanceResponce>("api/portfolio/performance?fromdate=" + fromDate.ToString("yyyy-MM-dd") + "&todate" + toDate.ToString("yyyy-MM-dd"));
        }

        public async Task<HoldingResponce> GetPortfolioHoldingsAsync(Guid stock, DateTime date)
        {
            return await GetAsync<HoldingResponce>("/api/portfolio/holding?stock=" + stock.ToString() + "&date=" + date.ToString("yyyy-MM-dd"));
        }

        public async Task<HoldingsResponce> GetPortfolioHoldingsAsync(DateTime date)
        {
            return await GetAsync<HoldingsResponce>("/api/portfolio/holdings?date=" + date.ToString("yyyy-MM-dd"));
        }

        public async Task<HoldingsResponce> GetPortfolioTradeableHoldingsAsync(DateTime date)
        {
            return await GetAsync<HoldingsResponce>("/api/portfolio/holdings?date=" + date.ToString("yyyy-MM-dd") + "&tradeable=Y");
        }

        public async Task<SimpleUnrealisedGainsResponce> GetSimpleUnrealisedGainsAsync(DateTime date)
        {
            return await GetAsync<SimpleUnrealisedGainsResponce>("api/portfolio/capitalgains?date=" + date.ToString("yyyy-MM-dd"));
        }

        public async Task<SimpleUnrealisedGainsResponce> GetSimpleUnrealisedGainsAsync(Guid stock, DateTime date)
        {
            return await GetAsync<SimpleUnrealisedGainsResponce>("api/portfolio/capitalgains?stock=" + stock.ToString() + "&date=" + date.ToString("yyyy-MM-dd"));
        }

        public async Task<DetailedUnrealisedGainsResponce> GetDetailedUnrealisedGainsAsync(DateTime date)
        {
            return await GetAsync<DetailedUnrealisedGainsResponce>("api/portfolio/detailedcapitalgains?date=" + date.ToString("yyyy-MM-dd"));
        }

        public async Task<DetailedUnrealisedGainsResponce> GetDetailedUnrealisedGainsAsync(Guid stock, DateTime date)
        {
            return await GetAsync<DetailedUnrealisedGainsResponce>("api/portfolio/detailedcapitalgains?stock=" + stock.ToString() + "&date=" + date.ToString("yyyy-MM-dd"));
        }

        public async Task<CGTLiabilityResponce> GetCGTLiabilityAsync(DateTime fromDate, DateTime toDate)
        {
            return await GetAsync<CGTLiabilityResponce>("api/portfolio/cgtliability?fromdate=" + fromDate.ToString("yyyy-MM-dd") + "&todate=" + toDate.ToString("yyyy-MM-dd"));
        }

        public async Task<GetTransactionsResponce> GetTransactionsAsync(DateTime fromDate, DateTime toDate)
        {
            return await GetAsync<GetTransactionsResponce>("api/transactions?fromdate=" + fromDate.ToString("yyyy-MM-dd") + "&todate=" + toDate.ToString("yyyy-MM-dd"));
        }

        public async Task<GetTransactionsResponce> GetTransactionsAsync(Guid stock, DateTime fromDate, DateTime toDate)
        {
            return await GetAsync<GetTransactionsResponce>("api/transactions?stock=" + stock.ToString() + "&fromdate=" + fromDate.ToString("yyyy-MM-dd") + "&todate=" + toDate.ToString("yyyy-MM-dd"));
        }

        public async Task<IncomeResponce> GetIncomeAsync(DateTime fromDate, DateTime toDate)
        {
            return await GetAsync<IncomeResponce>("api/portfolio/income?fromdate=" + fromDate.ToString("yyyy-MM-dd") + "&todate=" + toDate.ToString("yyyy-MM-dd"));
        }

        public async Task<PortfolioValueResponce> GetPortfolioValueAsync(DateTime fromDate, DateTime toDate, ValueFrequency frequency)
        {
            var url = "api/portfolio/value?fromdate=" + fromDate.ToString("yyyy-MM-dd") + "&todate=" + toDate.ToString("yyyy-MM-dd");
            if (frequency == ValueFrequency.Weekly)
                url += "&frequency=week";
            else if (frequency == ValueFrequency.Monthly)
                url += "&frequency=month";
            else
                url += "&frequency=day";

            return await GetAsync<PortfolioValueResponce>(url);
        }

        public async Task<PortfolioValueResponce> GetPortfolioValueAsync(Guid stock, DateTime fromDate, DateTime toDate, ValueFrequency frequency)
        {
            var url = "api/portfolio/value?stock=" + stock.ToString() + "&fromdate=" + fromDate.ToString("yyyy-MM-dd") + "&todate=" + toDate.ToString("yyyy-MM-dd");
            if (frequency == ValueFrequency.Weekly)
                url += "&frequency=week";
            else if (frequency == ValueFrequency.Monthly)
                url += "&frequency=month";
            else
                url += "&frequency=day";

            return await GetAsync<PortfolioValueResponce>(url);
        }

        public async Task<GetStockResponce> GetStocksAsync(DateTime date, bool includeStapled, bool includeChildren)
        {
            var url = "/api/stock/stocks?date=" + date.ToString("yyyy-MM-dd");

            if (includeStapled)
                url += "&includestapled=Y";
            else
                url += "&includestapled=N";

            if (includeChildren)
                url += "&includechildren=Y";
            else
                url += "&includechildren=N";

            return await GetAsync<GetStockResponce>(url);
        }

        public async Task<UnappliedCorporateActionsResponce> GetUnappliedCorporateActionsAsync()
        {
            return await GetAsync<UnappliedCorporateActionsResponce>("api/portfolio/corporateactions/unapplied");
        }

        public async Task<TransactionsForCorparateActionsResponce> GetTransactionsForCorporateActionAsync(Guid action)
        {
            return await GetAsync<TransactionsForCorparateActionsResponce>("api/portfolio/corporateactions/transactions?id=" + action.ToString());
        }

        public async Task<CashAccountTransactionsResponce> GetCashAccountTransactionsAsync(DateTime fromDate, DateTime toDate)
        {
            return await GetAsync<CashAccountTransactionsResponce>("api/portfolio/cashaccount/transactions?fromdate=" + fromDate.ToString("yyyy-MM-dd") + "&todate=" + toDate.ToString("yyyy-MM-dd"));
        }

        private async Task<T> GetAsync<T>(string url)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_BaseURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            T responce = default(T);

            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var formatter = new JsonMediaTypeFormatter
                {
                    SerializerSettings = { TypeNameHandling =  Newtonsoft.Json.TypeNameHandling.Auto }
                };
                responce = await response.Content.ReadAsAsync<T>(new List<MediaTypeFormatter> { formatter });
            }

            return responce;
        }
    }
}
