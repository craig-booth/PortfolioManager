using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Transactions;

namespace PortfolioManager.RestApi.Client
{
    public class TransactionResource : RestResource
    {
        public Guid PortfolioId { get; }

        public TransactionResource(Guid portfolioId, HttpClient httpClient)
            : base(httpClient)
        {
            PortfolioId = portfolioId;
        }

        public async Task<Transaction> Get(Guid id)
        {
            var url = "/api/v2/portfolio/" + PortfolioId + "/transactions/" + id;

            return await GetAsync<Transaction>(url);
        }

        public async Task<bool> Add(Transaction transaction)
        {
            var url = "/api/v2/portfolio/" + PortfolioId + "/transactions";

           return await PostAsync<Transaction>(url, transaction);       
        }

        public async Task<bool> Add(IEnumerable<Transaction> transactions)
        {
            var url = "/api/v2/portfolio/" + PortfolioId + "/transactions";

            return await PostAsync<IEnumerable<Transaction>>(url, transactions);
        }

        public async Task<List<Transaction>> GetTransactionsForCorporateAction(Guid id)
        {
            return await GetAsync<List<Transaction>>("/api/v2/portfolio/" + PortfolioId + "/transactions/corporateaction/" + id.ToString());
        }

    }
}
