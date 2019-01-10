using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PortfolioManager.RestApi.Client
{
    public class RestClient
    {
        private readonly HttpClient _HttpClient;

        public StockResource Stocks { get; }
        public TradingCalanderResource TradingCalander { get; }
        public CorporateActionResource CorporateActions { get; }
        public PortfolioResource Portfolio { get; }
        public HoldingResource Holdings { get; }

        public RestClient(string baseURL, Guid apiKey)
        {
            _HttpClient = new HttpClient();
            _HttpClient.BaseAddress = new Uri(baseURL);
            _HttpClient.DefaultRequestHeaders.Add("Api-Key", apiKey.ToString());
            _HttpClient.DefaultRequestHeaders.Accept.Clear();
            _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Stocks = new StockResource(_HttpClient);
            TradingCalander = new TradingCalanderResource(_HttpClient);
            CorporateActions = new CorporateActionResource(_HttpClient);
            Portfolio = new PortfolioResource(Guid.Empty, _HttpClient);
            Holdings = new HoldingResource(Guid.Empty, _HttpClient);
    }
    }
}
