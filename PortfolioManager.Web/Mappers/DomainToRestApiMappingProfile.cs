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

            CreateMap<Domain.Portfolios.Holding, RestApi.Portfolios.Holding>();
        }

    }

    public class StockTypeConverter : ITypeConverter<Domain.Stocks.Stock, RestApi.Portfolios.Stock>
    {
        public RestApi.Portfolios.Stock Convert(Domain.Stocks.Stock source, RestApi.Portfolios.Stock destination, ResolutionContext context)
        {
            DateTime date;
            if (context.Items.ContainsKey("date"))
                date = (DateTime)context.Items["date"];
            else
                date = DateTime.Today;

            var properties = source.Properties.ClosestTo(date);
            return new RestApi.Portfolios.Stock()
            {
                Id = source.Id,
                ASXCode = properties.ASXCode,
                Name = properties.Name,
                Category = properties.Category
            };
        }
    }
}
