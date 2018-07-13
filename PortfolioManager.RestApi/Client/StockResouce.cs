using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Stocks;

namespace PortfolioManager.RestApi.Client
{
    public class StockResouce : RestResource
    {
        public StockResouce(HttpClient httpClient)
            : base(httpClient)
        {

        }

        public async Task<IEnumerable<StockResponse>> Get()
        {
            return await GetAsync<IEnumerable<StockResponse>>("/api/stocks");
        }

        public async Task<IEnumerable<StockResponse>> Get(DateTime date)
        {
            return await GetAsync<IEnumerable<StockResponse>>("/api/stocks?date=" + date.ToString("yyyy-MM-dd"));
        }

        public async Task<IEnumerable<StockResponse>> Get(DateRange dateRange)
        {
            return await GetAsync<IEnumerable<StockResponse>>("/api/stocks?fromdate=" + dateRange.FromDate.ToString("yyyy-MM-dd") + "?todate=" + dateRange.ToDate.ToString("yyyy-MM-dd"));
        }

        public async Task<StockResponse> Get(Guid id)
        {
            return await GetAsync<StockResponse>("/api/stocks/" + id.ToString());
        }

        public async Task<StockResponse> Get(Guid id, DateTime date)
        {
            return await GetAsync<StockResponse>("/api/stocks/" + id.ToString() + "?date=" + date.ToString("yyyy-MM-dd"));
        }

        public async Task<StockHistoryResponse> GetHistory(Guid id)
        {
            return await GetAsync<StockHistoryResponse>("/api/stocks/" + id.ToString() + "/history");
        }

        public async Task<StockHistoryResponse> GetPrices(Guid id, DateRange dateRange)
        {
            return await GetAsync<StockHistoryResponse>("/api/stocks/" + id.ToString() + "/closingprices?fromdate=" + dateRange.FromDate.ToString("yyyy-MM-dd") + "?todate=" + dateRange.ToDate.ToString("yyyy-MM-dd"));
        }

        public async Task CreateStock(CreateStockCommand command)
        {
            await PostAsync<CreateStockCommand>("/api/stocks", command);
        }

        public async Task ChangeStock(ChangeStockCommand command)
        {
            await PostAsync<ChangeStockCommand>("/api/stocks/" + command.Id.ToString() + "/change", command);
        }

        public async Task DelistStock(DelistStockCommand command)
        {
            await PostAsync<DelistStockCommand>("/api/stocks/" + command.Id.ToString() + "/delist", command);
        }

        public async Task UpdateClosingPrices(UpdateClosingPricesCommand command)
        {
            await PostAsync<UpdateClosingPricesCommand>("/api/stocks/" + command.Id.ToString() + "/closingprices", command);
        }

        public async Task ChangeDRP(ChangeDividendReinvestmentPlanCommand command)
        {
            await PostAsync<ChangeDividendReinvestmentPlanCommand>("/api/stocks/" + command.Id.ToString() + "/changedrp", command);
        }

        public async Task ChangeReleativeNTAs(ChangeRelativeNTAsCommand command)
        {
            await PostAsync<ChangeRelativeNTAsCommand>("/api/stocks/" + command.Id.ToString() + "/relativenta", command);
        }
    }
}
