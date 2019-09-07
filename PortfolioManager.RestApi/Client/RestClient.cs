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
        public ClientSession Session { get; private set; }
        
        public StockResource Stocks { get; }
        public TradingCalanderResource TradingCalander { get; }
        public CorporateActionResource CorporateActions { get; }
        public PortfolioResource Portfolio { get; }
        public HoldingResource Holdings { get; }
        public TransactionResource Transactions { get; }

        public RestClient(string server)
        {
            _HttpClient = new HttpClient();
            _HttpClient.BaseAddress = new Uri(server);
            var apiKey = new Guid("B34A4C8B-6B17-4E25-A3CC-2E512D5F1B3D");
            _HttpClient.DefaultRequestHeaders.Add("Api-Key", apiKey.ToString());
            _HttpClient.DefaultRequestHeaders.Accept.Clear();
            _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Session = new ClientSession(_HttpClient);

            Stocks = new StockResource(Session);
            TradingCalander = new TradingCalanderResource(Session);
            CorporateActions = new CorporateActionResource(Session);
            Portfolio = new PortfolioResource(Session);
            Holdings = new HoldingResource(Session);
            Transactions = new TransactionResource(Session);
        }

        public bool Authenticate(string userName, string password)
        {
            Session.UserName = userName;
            Session.Password = password;

            return true;
        }

        public void SetPortfolio(Guid portfolio)
        {
            Session.Portfolio = portfolio;
        }
    }

    public class ClientSession
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid Portfolio { get; set; }
        public HttpClient HttpClient { get; }

        public ClientSession(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }
    }
}
