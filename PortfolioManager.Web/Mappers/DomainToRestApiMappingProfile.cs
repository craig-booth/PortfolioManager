using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Transactions;

namespace PortfolioManager.Web.Mappers
{
    public class DomainToRestApiMappingProfile : Profile
    {
        public DomainToRestApiMappingProfile()
        {
            CreateMap<Domain.Transactions.Transaction, RestApi.Transactions.Transaction>()
                .ForMember(x => x.Stock, x => x.MapFrom(y => y.Stock.Id))
                .ForMember(x => x.TransactionDate, x => x.MapFrom(y => y.Date))
                .Include<Domain.Transactions.Aquisition, RestApi.Transactions.Aquisition>()
                .Include<Domain.Transactions.Disposal, RestApi.Transactions.Disposal>()
                .Include<Domain.Transactions.CashTransaction, RestApi.Transactions.CashTransaction>()
                .Include<Domain.Transactions.OpeningBalance, RestApi.Transactions.OpeningBalance>()
                .Include<Domain.Transactions.IncomeReceived, RestApi.Transactions.IncomeReceived>();

            CreateMap<Domain.Transactions.Aquisition, RestApi.Transactions.Aquisition>();
            CreateMap<Domain.Transactions.Disposal, RestApi.Transactions.Disposal>()
                .ForMember(x => x.CGTMethod, x => x.MapFrom(y => y.CGTMethod.ToRestName()));
            CreateMap<Domain.Transactions.CashTransaction, RestApi.Transactions.CashTransaction>()
                .ForMember(x => x.TransactionDate, x => x.MapFrom(y => y.Date));
            CreateMap<Domain.Transactions.OpeningBalance, RestApi.Transactions.OpeningBalance>();
            CreateMap<Domain.Transactions.IncomeReceived, RestApi.Transactions.IncomeReceived>();

            CreateMap<Domain.Stocks.Stock, RestApi.Portfolios.Stock>().ConvertUsing<StockTypeConverter>();
            CreateMap<Domain.Transactions.Transaction, RestApi.Portfolios.TransactionsResponse.TransactionItem>()
                .ForMember(x => x.TransactionDate, x => x.MapFrom(y => y.Date));

            CreateMap<Domain.Portfolios.Holding, RestApi.Portfolios.Holding>().ConvertUsing<HoldingConverter>();

            CreateMap<Domain.Portfolios.CashAccount.Transaction, RestApi.Portfolios.CashAccountTransactionsResponse.CashTransactionItem>();
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
                    AsxCode = "",
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

    public class HoldingConverter : ITypeConverter<Domain.Portfolios.Holding, RestApi.Portfolios.Holding>
    {
        public RestApi.Portfolios.Holding Convert(Domain.Portfolios.Holding source, RestApi.Portfolios.Holding destination, ResolutionContext context)
        {
            if (source == null)
                return new RestApi.Portfolios.Holding();

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
                AsxCode = properties.ASXCode,
                Name = properties.Name,
                Category = properties.Category
            };
        }

        public static RestApi.Portfolios.Holding Convert(this Domain.Portfolios.Holding source, DateTime date)
        {
            var properties = source.Properties.ClosestTo(date);
            return new RestApi.Portfolios.Holding()
            {
                Stock = source.Stock.Convert(date),
                Units = properties.Units,
                Value = properties.Units * source.Stock.GetPrice(date),
                Cost = properties.Amount,
                CostBase = properties.CostBase
            };
        }
    }

}
