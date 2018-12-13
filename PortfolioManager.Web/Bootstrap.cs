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
using PortfolioManager.Data.Portfolios;
using PortfolioManager.ImportData;
using PortfolioManager.ImportData.DataServices;
using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Services;
using PortfolioManager.Service;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Stocks.Events;
using PortfolioManager.Domain.Transactions;
using PortfolioManager.EventStore;
using PortfolioManager.EventStore.Mongodb;

using PortfolioManager.Web.Mapping;
using PortfolioManager.Web.Converters;

namespace PortfolioManager.Web
{
    public static class PortfolioManagerServiceCollectionExtensions
    {

        public static IServiceCollection AddPortfolioManagerService(this IServiceCollection services, PortfolioManagerSettings settings)
        {
            // Ensure that PortfolioManager.Domain assembly is loaded
            var xx = new StockListedEvent(Guid.Empty, 0, "", "", DateTime.Today, Common.AssetCategory.AustralianStocks, false);

            var config = new TransactionConfiguration();
            config.RegisterTransaction<Domain.Transactions.Aquisition, RestApi.Transactions.Aquisition>("aquisition");
            config.RegisterTransaction<Domain.Transactions.CashTransaction, RestApi.Transactions.CashTransaction>("cashtransaction");
            config.RegisterTransaction<Domain.Transactions.OpeningBalance, RestApi.Transactions.OpeningBalance>("openingbalance");
            config.RegisterTransaction<Domain.Transactions.IncomeReceived, RestApi.Transactions.IncomeReceived>("incomereceived");
            services.AddSingleton<TransactionConfiguration>(config);

            services.AddSingleton<PortfolioManagerSettings>(settings);
            services.AddSingleton<IPortfolioDatabase>(x => new SQLitePortfolioDatabase(settings.PortfolioDatabase));
            services.AddSingleton<IEventStore>(CreateEventStore);
            services.AddSingleton<StockExchange>();
            services.AddSingleton<IStockRepository>(x => x.GetRequiredService<StockExchange>().Stocks);
            services.AddSingleton<ITradingCalander>(x => x.GetRequiredService<StockExchange>().TradingCalander);
            services.AddSingleton<IPortfolioCache>(new PortfolioCache());

            services.AddSingleton<StockResolver, StockResolver>();
            services.AddSingleton<Profile, RestApiToDomainMappingProfile>();
            services.AddSingleton<Profile, DomainToRestApiMappingProfile>();
            services.AddSingleton<IMapper>(CreateMapper);

            services.AddSingleton<TransactionJsonConverter, TransactionJsonConverter>();
            services.AddSingleton<IConfigureOptions<MvcJsonOptions>, JsonMvcConfiguration>();

            services.AddScoped<IPortfolioSummaryService, PortfolioSummaryService>();
            services.AddScoped<IPortfolioPerformanceService, PortfolioPerformanceService>();
            services.AddScoped<ICapitalGainService, CapitalGainService>();
            services.AddScoped<IPortfolioValueService, PortfolioValueService>();
            services.AddScoped<ICorporateActionService, CorporateActionService>();
            services.AddScoped<PortfolioManager.Service.Interface.ITransactionService, PortfolioManager.Service.Services.TransactionService>();
            services.AddScoped<IHoldingService, HoldingService>();
            services.AddScoped<ICashAccountService, CashAccountService>();
            services.AddScoped<IIncomeService, IncomeService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IAttachmentService, AttachmentService>();

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
            var stockExchange = serviceProvider.GetRequiredService<StockExchange>();

            var profiles = serviceProvider.GetServices<Profile>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new Service.Utils.ModelToServiceMapping(stockExchange));
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
            var stockExchange = app.ApplicationServices.GetRequiredService<StockExchange>();
            stockExchange.LoadFromEventStream();

            return app;
        }
    }

    public class PortfolioManagerSettings
    {
        public Guid ApiKey { get; set; }
        public string PortfolioDatabase { get; set; }
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
