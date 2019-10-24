using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PortfolioManager.RestApi.Transactions;

namespace PortfolioManager.RestApi.Client
{
    public class TransactionResource : RestResource
    {

        public TransactionResource(ClientSession session)
            : base(session)
        {
        }

        public async Task<Transaction> Get(Guid id)
        {
            var url = "/api/v2/portfolio/" + _Session.Portfolio + "/transactions/" + id;

            return await GetAsync<Transaction>(url);
        }

        public async Task<bool> Add(Transaction transaction)
        {
            var url = "/api/v2/portfolio/" + _Session.Portfolio + "/transactions";

           return await PostAsync<Transaction>(url, transaction);       
        }

        public async Task<bool> Add(IEnumerable<Transaction> transactions)
        {
            var url = "/api/v2/portfolio/" + _Session.Portfolio + "/transactions";

            return await PostAsync<IEnumerable<Transaction>>(url, transactions);
        }

        public async Task<List<Transaction>> GetTransactionsForCorporateAction(Guid stock, Guid action)
        {
            return await GetAsync<List<Transaction>>("/api/v2/portfolio/" + _Session.Portfolio + "/transactions/" + stock.ToString() + "/corporateaction/" + action.ToString());
        }

    }
}
