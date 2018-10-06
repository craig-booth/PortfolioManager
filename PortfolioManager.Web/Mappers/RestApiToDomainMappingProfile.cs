using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Web.Mapping
{
    public class RestApiToDomainMappingProfile: Profile
    {
        public RestApiToDomainMappingProfile(TransactionConfiguration configuration, StockResolver stockResolver)
        {
            var baseMap = CreateMap<RestApi.Transactions.Transaction, Domain.Transactions.Transaction>()
                                .ForMember(dest => dest.Stock, opts => opts.ResolveUsing(stockResolver));
            foreach (var item in configuration.Items)
            {
                baseMap.Include(item.RestApiTransactionType, item.DomainTransactionType);

                CreateMap(item.RestApiTransactionType, item.DomainTransactionType);
            }
        }
    }
}
