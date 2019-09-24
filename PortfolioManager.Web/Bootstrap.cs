using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using AutoMapper;

using PortfolioManager.Common.Scheduler;
using PortfolioManager.DataServices;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.TradingCalanders;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Stocks.Events;
using PortfolioManager.Domain.Users;
using PortfolioManager.EventStore;
using PortfolioManager.EventStore.Mongodb;
using PortfolioManager.RestApi.Converters;

using PortfolioManager.Web.Utilities;
using PortfolioManager.Web.Mappers;
using PortfolioManager.Web.Services;
using PortfolioManager.Web.DataImporters;


namespace PortfolioManager.Web
{

    public static class PortfolioManagerServiceCollectionExtensions
    {

        public static IServiceCollection AddPortfolioManagerService(this IServiceCollection services, PortfolioManagerSettings settings)
        {
            // Ensure that PortfolioManager.Domain assembly is loaded
            var xx = new StockListedEvent(Guid.Empty, 0, "", "", DateTime.Today, Common.AssetCategory.AustralianStocks, false);


            services.AddSingleton<IStockResolver, StockResolver>();
            services.Add(ServiceDescriptor.Singleton(typeof(IEntityCache<>), typeof(EntityCache<>)));

            services.AddSingleton<PortfolioManagerSettings>(settings);
            services.AddSingleton<IEventStore>(CreateEventStore);

            services.Add(ServiceDescriptor.Singleton(typeof(IRepository<>), typeof(Repository<>)));
            services.Add(ServiceDescriptor.Singleton(typeof(IEntityFactory<>), typeof(DefaultEntityFactory<>)));

            services.AddSingleton<IEventStream<User>>(x => x.GetRequiredService<IEventStore>().GetEventStream<User>("Users"));

            services.AddSingleton<IEventStream<Stock>>(x => x.GetRequiredService<IEventStore>().GetEventStream<Stock>("Stocks"));
            services.AddSingleton<IStockQuery, StockQuery>();
            services.AddSingleton<IEntityFactory<Stock>, StockEntityFactory>();

            services.AddSingleton<IEventStream<StockPriceHistory>>(x => x.GetRequiredService<IEventStore>().GetEventStream<StockPriceHistory>("StockPriceHistory"));

            services.AddSingleton<IEventStream<TradingCalander>>(x => x.GetRequiredService<IEventStore>().GetEventStream<TradingCalander>("TradingCalander"));         

            services.AddSingleton<IEventStream<Portfolio>>(x => x.GetRequiredService<IEventStore>().GetEventStream<Portfolio>("Portfolios"));
            services.AddSingleton<IEntityFactory<Portfolio>, PortfolioEntityFactory>();
            services.AddSingleton<IPortfolioCache, PortfolioCache>();


            // Add services
            services.AddSingleton<ICashAccountService, CashAccountService>();
            services.AddSingleton<IPortfolioCapitalGainsService, PortfolioCapitalGainsService>();
            services.AddSingleton<IPortfolioCgtLiabilityService, PortfolioCgtLiabilityService>();
            services.AddSingleton<IPortfolioCorporateActionsService, PortfolioCorporateActionsService>();
            services.AddSingleton<IPortfolioIncomeService, PortfolioIncomeService>();
            services.AddSingleton<IPortfolioPerformanceService, PortfolioPerformanceService>();
            services.AddSingleton<IPortfolioPropertiesService, PortfolioPropertiesService>();
            services.AddSingleton<IPortfolioService, PortfolioService>();
            services.AddSingleton<IPortfolioTransactionService, PortfolioTransactionService>();
            services.AddSingleton<IPortfolioValueService, PortfolioValueService>();
            services.AddSingleton<IStockService, StockService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ICorporateActionService, CorporateActionService>();
            services.AddSingleton<ITradingCalanderService, TradingCalanderService>();

            services.AddSingleton<MapperStockResolver>();
            services.AddSingleton<Profile, RestApiToDomainMappingProfile>();
            services.AddSingleton<Profile, DomainToRestApiMappingProfile>();
            services.AddSingleton<IMapper>(CreateMapper);

            services.AddSingleton<IAuthorizationHandler, PortfolioOwnerAuthorizationHandler>();
            services.AddSingleton<TransactionJsonConverter, TransactionJsonConverter>();
            services.AddSingleton<IConfigureOptions<MvcJsonOptions>, JsonMvcConfiguration>();

            // Add default trading calander for ASX 
            services.AddSingleton<ITradingCalander>(x => x.GetRequiredService<IEntityCache<TradingCalander>>().Get(TradingCalanderIds.ASX));

            return services;
        }

        public static IServiceCollection AddDataImportService(this IServiceCollection services)
        {
            services.AddScoped<IHistoricalStockPriceService, ASXDataService>();
            services.AddScoped<ILiveStockPriceService, ASXDataService>();
            services.AddScoped<ITradingDayService, ASXDataService>();

            services.AddScoped<HistoricalPriceImporter>();
            services.AddScoped<LivePriceImporter>();
            services.AddScoped<TradingDayImporter>();

            services.AddSingleton<Scheduler>();

            services.AddSingleton<IHostedService, DataImportBackgroundService>();

            return services;
        }

        private static IEventStore CreateEventStore(IServiceProvider serviceProvider)
        {
            var settings = serviceProvider.GetRequiredService<PortfolioManagerSettings>();
            var logger = serviceProvider.GetRequiredService<ILogger<IEventStore>>();

            return new MongodbEventStore(settings.EventStore, logger);
        }

        private static IMapper CreateMapper(IServiceProvider serviceProvider)
        {
            var profiles = serviceProvider.GetServices<Profile>();
            var config = new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                    cfg.AddProfile(profile);
            });

            return config.CreateMapper();
        }
    }

    public static class PortfolioManagerAppBuilderExtensions
    {
        public static IApplicationBuilder InitializeStockCache(this IApplicationBuilder app)
        {
            app.ApplicationServices.InitializeStockCache();

            return app;
        }

        public static IServiceProvider InitializeStockCache(this IServiceProvider serviceProvider)
        {
            var tradingCalanderRepository = serviceProvider.GetRequiredService<IRepository<TradingCalander>>();
            var tradingCalanderCache = serviceProvider.GetRequiredService<IEntityCache<TradingCalander>>();
            tradingCalanderCache.PopulateCache(tradingCalanderRepository);

            var stockRepository = serviceProvider.GetRequiredService<IRepository<Stock>>();
            var stockCache = serviceProvider.GetRequiredService<IEntityCache<Stock>>();
            stockCache.PopulateCache(stockRepository);

            var stockPriceRepository = serviceProvider.GetRequiredService<IRepository<StockPriceHistory>>();
            var stockPriceHistoryCache = serviceProvider.GetRequiredService<IEntityCache<StockPriceHistory>>();
            stockPriceHistoryCache.PopulateCache(stockPriceRepository);

            // Hook up stock prices to stocks
            foreach (var stock in stockCache.All())
            {
                var stockPriceHistory = stockPriceHistoryCache.Get(stock.Id);
                if (stockPriceHistory != null)
                    stock.SetPriceHistory(stockPriceHistory);
            }

            return serviceProvider;
        }
    }

    public class JsonMvcConfiguration : IConfigureOptions<MvcJsonOptions>
    {
        private readonly TransactionJsonConverter _TransactionConverter;

        public JsonMvcConfiguration(TransactionJsonConverter transactionConverter)
        {
            _TransactionConverter = transactionConverter;
        }

        public void Configure(MvcJsonOptions options)
        {
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });

            options.SerializerSettings.Converters.Add(new CorporateActionJsonConverter());
            options.SerializerSettings.Converters.Add(_TransactionConverter);
        }
    } 
}
