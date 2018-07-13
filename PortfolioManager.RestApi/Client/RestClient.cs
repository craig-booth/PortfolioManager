using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;

using PortfolioManager.Common;

namespace PortfolioManager.RestApi.Client
{
    public class RestClient
    {
        private readonly HttpClient _HttpClient;

        public StockResouce Stocks { get; }
        public TradingCalanderResource TradingCalander { get; }

        public RestClient(string baseURL, Guid apiKey)
        {
            _HttpClient = new HttpClient();
            _HttpClient.BaseAddress = new Uri(baseURL);
            _HttpClient.DefaultRequestHeaders.Add("Api-Key", apiKey.ToString());
            _HttpClient.DefaultRequestHeaders.Accept.Clear();
            _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Stocks = new StockResouce(_HttpClient);
            TradingCalander = new TradingCalanderResource(_HttpClient);
        }
    }
}
