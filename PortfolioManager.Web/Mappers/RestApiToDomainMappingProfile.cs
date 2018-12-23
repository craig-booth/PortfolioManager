using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Transactions;

namespace PortfolioManager.Web.Mapping
{
    public class RestApiToDomainMappingProfile: Profile
    {
        public RestApiToDomainMappingProfile(StockResolver stockResolver)
        {
            CreateMap<RestApi.Transactions.Transaction, Domain.Transactions.Transaction>()
                .ForMember(dest => dest.Stock, opts => opts.ResolveUsing(stockResolver))
                .Include<RestApi.Transactions.Aquisition, Domain.Transactions.Aquisition>()
                .Include<RestApi.Transactions.Disposal, Domain.Transactions.Disposal>()
                .Include<RestApi.Transactions.CashTransaction, Domain.Transactions.CashTransaction>()
                .Include<RestApi.Transactions.OpeningBalance, Domain.Transactions.OpeningBalance>()
                .Include<RestApi.Transactions.IncomeReceived, Domain.Transactions.IncomeReceived>();

            CreateMap<RestApi.Transactions.Aquisition, Domain.Transactions.Aquisition>();
            CreateMap<RestApi.Transactions.Disposal, Domain.Transactions.Disposal>()
                .ForMember(x => x.CGTMethod, x => x.MapFrom(y => CGTMethodMapping.ToDomain(y.CGTMethod)));
            CreateMap<RestApi.Transactions.CashTransaction, Domain.Transactions.CashTransaction>();
            CreateMap<RestApi.Transactions.OpeningBalance, Domain.Transactions.OpeningBalance>();
            CreateMap<RestApi.Transactions.IncomeReceived, Domain.Transactions.IncomeReceived>();
        }
    }
}
