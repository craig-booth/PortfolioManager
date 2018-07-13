using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

using PortfolioManager.RestApi.TradingCalander;

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
            return await GetAsync<TradingCalanderResponse>("/api/tradingcalander/" + year.ToString());
        }

        public async Task Update(UpdateTradingCalanderCommand command)
        {
            await PostAsync<UpdateTradingCalanderCommand>("/api/tradingcalander/" + command.Year.ToString(), command);
        }
    }
}
