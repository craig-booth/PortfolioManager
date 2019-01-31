using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

using PortfolioManager.RestApi.TradingCalanders;

namespace PortfolioManager.RestApi.Client
{
    public class TradingCalanderResource : RestResource
    {
        public TradingCalanderResource(HttpClient httpClient)
            : base(httpClient)
        {

        }

        public async Task<TradingCalanderResponse> Get(int year)
        {
            return await GetAsync<TradingCalanderResponse>("/api/v2/tradingcalander/" + year.ToString());
        }

        public async Task Update(UpdateTradingCalanderCommand command)
        {
            await PostAsync<UpdateTradingCalanderCommand>("/api/v2/tradingcalander/" + command.Year.ToString(), command);
        }
    }
}
