using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Booth.Common;

using PortfolioManager.RestApi.Stocks;

namespace PortfolioManager.RestApi.Client
{
    public class StockResource : RestResource
    {
        public StockResource(ClientSession session)
            : base(session)
        {

        }

        public async Task<IEnumerable<StockResponse>> Get()
        {
            return await GetAsync<IEnumerable<StockResponse>>("/api/v2/stocks");
        }

        public async Task<IEnumerable<StockResponse>> Get(DateTime date)
        {
            return await GetAsync<IEnumerable<StockResponse>>("/api/v2/stocks?date=" + date.ToIsoDateString());
        }

        public async Task<IEnumerable<StockResponse>> Get(DateRange dateRange)
        {
            return await GetAsync<IEnumerable<StockResponse>>("/api/v2/stocks?fromdate=" + dateRange.FromDate.ToIsoDateString() + "&todate=" + dateRange.ToDate.ToIsoDateString());
        }

        public async Task<StockResponse> Get(Guid id)
        {
            return await GetAsync<StockResponse>("/api/v2/stocks/" + id.ToString());
        }

        public async Task<StockResponse> Get(Guid id, DateTime date)
        {
            return await GetAsync<StockResponse>("/api/v2/stocks/" + id.ToString() + "?date=" + date.ToIsoDateString());
        }

        public async Task<IEnumerable<StockResponse>> Find(string query)
        {
            return await GetAsync<IEnumerable<StockResponse>>("/api/v2/stocks?query=" + query);
        }

        public async Task<IEnumerable<StockResponse>> Find(string query, DateTime date)
        {
            return await GetAsync<IEnumerable<StockResponse>>("/api/v2/stocks/?query=" + query + "&date=" + date.ToIsoDateString());
        }

        public async Task<IEnumerable<StockResponse>> Find(string query, DateRange dateRange)
        {
            return await GetAsync<IEnumerable<StockResponse>>("/api/v2/stocks/?query=" + query + "&fromdate=" + dateRange.FromDate.ToIsoDateString() + "&todate=" + dateRange.ToDate.ToIsoDateString());
        }

        public async Task<StockHistoryResponse> GetHistory(Guid id)
        {
            return await GetAsync<StockHistoryResponse>("/api/v2/stocks/" + id.ToString() + "/history");
        }

        public async Task<StockHistoryResponse> GetPrices(Guid id, DateRange dateRange)
        {
            return await GetAsync<StockHistoryResponse>("/api/v2/stocks/" + id.ToString() + "/closingprices?fromdate=" + dateRange.FromDate.ToIsoDateString() + "&todate=" + dateRange.ToDate.ToIsoDateString());
        }

        public async Task CreateStock(CreateStockCommand command)
        {
            await PostAsync<CreateStockCommand>("/api/v2/stocks", command);
        }

        public async Task ChangeStock(ChangeStockCommand command)
        {
            await PostAsync<ChangeStockCommand>("/api/v2/stocks/" + command.Id.ToString() + "/change", command);
        }

        public async Task DelistStock(DelistStockCommand command)
        {
            await PostAsync<DelistStockCommand>("/api/v2/stocks/" + command.Id.ToString() + "/delist", command);
        }

        public async Task UpdateClosingPrices(UpdateClosingPricesCommand command)
        {
            await PostAsync<UpdateClosingPricesCommand>("/api/v2/stocks/" + command.Id.ToString() + "/closingprices", command);
        }

        public async Task ChangeDividendRules(ChangeDividendRulesCommand command)
        {
            await PostAsync<ChangeDividendRulesCommand>("/api/v2/stocks/" + command.Id.ToString() + "/changedividendrules", command);
        }

        public async Task ChangeReleativeNTAs(ChangeRelativeNTAsCommand command)
        {
            await PostAsync<ChangeRelativeNTAsCommand>("/api/v2/stocks/" + command.Id.ToString() + "/relativenta", command);
        }
    }
}
