using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;

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

        public async Task<bool> Authenticate(string userName, SecureString password)
        {
            var userResource = new UserResource(Session);

            var result = await userResource.Authenticate(userName, password);

            _HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session.JwtToken);

            return result;
        }

        public void SetPortfolio(Guid portfolio)
        {
            Session.Portfolio = portfolio;
        }
    }

    public class ClientSession
    {
        public string JwtToken { get; set; }
        public Guid Portfolio { get; set; }
        public HttpClient HttpClient { get; }

        public ClientSession(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }
    }
}
