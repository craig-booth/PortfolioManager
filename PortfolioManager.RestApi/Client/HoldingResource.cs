using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

using PortfolioManager.RestApi.Portfolio;

namespace PortfolioManager.RestApi.Client
{
    public class HoldingResource : RestResource
    {
        public HoldingResource(HttpClient httpClient)
            : base(httpClient)
        {
        }

        public async Task<HoldingResponse> Get(Guid stock, DateTime date)
        {
            return new HoldingResponse();
        }

        public async Task<IEnumerable<HoldingResponse>> Get(DateTime date)
        {
            var holdings = new List<HoldingResponse>();

            return holdings;
        }
    }
}
