using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;

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

            CreateMap<Domain.Stocks.Stock, RestApi.Portfolios.Stock>().ConvertUsing<StockTypeConverter>();
            CreateMap<Domain.Transactions.Transaction, RestApi.Portfolios.TransactionsResponse.TransactionItem>();
                
        

            CreateMap<Domain.Portfolios.Holding, RestApi.Portfolios.Holding>();
        }

    }

    public class StockTypeConverter : ITypeConverter<Domain.Stocks.Stock, RestApi.Portfolios.Stock>
    {
        public RestApi.Portfolios.Stock Convert(Domain.Stocks.Stock source, RestApi.Portfolios.Stock destination, ResolutionContext context)
        {
            if (source == null)
            {
                return new RestApi.Portfolios.Stock()
                {
                    Id = Guid.Empty,
                    ASXCode = "",
                    Name = "",
                    Category = AssetCategory.Cash
                };
            }

            DateTime date;
            if (context.Items.ContainsKey("date"))
                date = (DateTime)context.Items["date"];
            else
                date = DateTime.Today;

            return source.Convert(date);
        }
    }

    public static class DomainToRestApiConverter
    {
        public static RestApi.Portfolios.Stock Convert(this Domain.Stocks.Stock source, DateTime date)
        {
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
