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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

using AutoMapper;

using PortfolioManager.Common.Scheduler;
using PortfolioManager.ImportData;
using PortfolioManager.ImportData.DataServices;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.TradingCalanders;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Stocks.Events;
using PortfolioManager.EventStore;
using PortfolioManager.EventStore.Mongodb;
using PortfolioManager.RestApi.Converters;

using PortfolioManager.Web.Mappers;


namespace PortfolioManager.Web
{
    public static class PortfolioManagerServiceCollectionExtensions
    {

        public static IServiceCollection AddPortfolioManagerService(this IServiceCollection services, PortfolioManagerSettings settings)
        {
            // Ensure that PortfolioManager.Domain assembly is loaded
            var xx = new StockListedEvent(Guid.Empty, 0, "", "", DateTime.Today, Common.AssetCategory.AustralianStocks, false);

            services.AddSingleton<PortfolioManagerSettings>(settings);
            services.AddSingleton<IEventStore>(CreateEventStore);

            services.Add(ServiceDescriptor.Singleton(typeof(IEntityCache<>), typeof(EntityCache<>)));
            services.Add(ServiceDescriptor.Singleton(typeof(IRepository<>), typeof(Repository<>)));
            services.Add(ServiceDescriptor.Singleton(typeof(IEntityFactory<>), typeof(DefaultEntityFactory<>)));

            services.AddSingleton<IEventStream<Stock>>(x => x.GetRequiredService<IEventStore>().GetEventStream<Stock>("StockRepository"));
            services.AddSingleton<IStockQuery, StockQuery>();
            services.AddSingleton<IEntityFactory<Stock>, StockEntityFactory>();
        
            services.AddSingleton<IEventStream<TradingCalander>>(x => x.GetRequiredService<IEventStore>().GetEventStream<TradingCalander>("TradingCalander"));
            services.AddSingleton<ITradingCalander>(x => x.GetRequiredService<IRepository<TradingCalander>>().Get(TradingCalanderIds.ASX));

            services.AddSingleton<IEventStream<Portfolio>>(x => x.GetRequiredService<IEventStore>().GetEventStream<Portfolio>("Portfolios"));
            services.AddSingleton<IEntityFactory<Portfolio>, PortfolioEntityFactory>();

            services.AddSingleton<StockResolver, StockResolver>();
            services.AddSingleton<Profile, RestApiToDomainMappingProfile>();
            services.AddSingleton<Profile, DomainToRestApiMappingProfile>();
            services.AddSingleton<IMapper>(CreateMapper);

            services.AddSingleton<TransactionJsonConverter, TransactionJsonConverter>();
            services.AddSingleton<IConfigureOptions<MvcJsonOptions>, JsonMvcConfiguration>();

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

            return new MongodbEventStore3(settings.EventStore, logger);
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
        public static IApplicationBuilder InitializeStockExchange(this IApplicationBuilder app)
        {
            app.ApplicationServices.InitializeStockExchange();

            return app;
        }

        public static IServiceProvider InitializeStockExchange(this IServiceProvider serviceProvider)
        {
            var stockRepository = serviceProvider.GetRequiredService<IRepository<Stock>>();
            stockRepository.PopulateCache();

            var tradingCalanderRepository = serviceProvider.GetRequiredService<IRepository<TradingCalander>>();
            tradingCalanderRepository.PopulateCache();

            return serviceProvider;
        }
    }

    public class PortfolioManagerSettings
    {
        public Guid ApiKey { get; set; }
        public string EventStore { get; set; }
        public int Port { get; set; }
    }

    public class JsonMvcConfiguration : IConfigureOptions<MvcJsonOptions>
    {
        private TransactionJsonConverter _TransactionConverter;

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
