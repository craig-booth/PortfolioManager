using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

namespace PortfolioManager.Web.Mapping
{
    public class DomainToRestApiMappingProfile : Profile
    {
        public DomainToRestApiMappingProfile(TransactionConfiguration configuration)
        {
            var baseMap = CreateMap<Domain.Transactions.Transaction, RestApi.Transactions.Transaction>()
                                .ForMember(x => x.Stock, x => x.MapFrom(y => y.Stock.Id));
            foreach (var item in configuration.Items)
            {
                baseMap.Include(item.DomainTransactionType, item.RestApiTransactionType);

                CreateMap(item.DomainTransactionType, item.RestApiTransactionType);
            }
        }
    }
}
