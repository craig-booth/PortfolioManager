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

namespace PortfolioManager.Web
{
    public static class PortfolioManagerServiceCollectionExtensions
    {

        public static IServiceCollection AddPortfolioManagerService(this IServiceCollection services, PortfolioManagerSettings settings)
        {
            // Ensure that PortfolioManager.Domain assembly is loaded
            var xx = new StockListedEvent(Guid.Empty, 0, "", "", DateTime.Today, Common.AssetCategory.AustralianStocks, false);

            services.AddSingleton<PortfolioManagerSettings>(settings);
            services.AddSingleton<IPortfolioDatabase>(CreatePortfolioDatabase);
            services.AddSingleton<IEventStore>(CreateEventStore);
            services.AddSingleton<StockExchange>(CreateStockExchange);

            services.AddSingleton<IStockRepository>(x => x.GetRequiredService<StockExchange>().Stocks);
            services.AddSingleton<ITradingCalander>(x => x.GetRequiredService<StockExchange>().TradingCalander);

            services.AddSingleton<IPortfolioCache>(new PortfolioCache());
            services.AddSingleton<FunkyTransactionService>(CreateTransactionService);
            services.AddSingleton<StockResolver, StockResolver>();
            services.AddSingleton<IMapper>(CreateMapper);
            services.AddSingleton<IConfigureOptions<MvcJsonOptions>, JsonMvcConfiguration>();

            services.AddScoped<IPortfolioSummaryService, PortfolioSummaryService>();
            services.AddScoped<IPortfolioPerformanceService, PortfolioPerformanceService>();
            services.AddScoped<ICapitalGainService, CapitalGainService>();
            services.AddScoped<IPortfolioValueService, PortfolioValueService>();
            services.AddScoped<ICorporateActionService, CorporateActionService>();
            services.AddScoped<ITransactionService, TransactionService>();
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

        private static StockExchange CreateStockExchange(IServiceProvider serviceProvider)
        {
            var eventStore = serviceProvider.GetRequiredService<IEventStore>();

            return new StockExchange(eventStore);
        }

        private static IPortfolioDatabase CreatePortfolioDatabase(IServiceProvider serviceProvider)
        {
            var settings = serviceProvider.GetRequiredService<PortfolioManagerSettings>();

            return new SQLitePortfolioDatabase(settings.PortfolioDatabase);
        }

        private static FunkyTransactionService CreateTransactionService(IServiceProvider serviceProvider)
        {
            var stockResolver = serviceProvider.GetRequiredService<StockResolver>();
            var service = new FunkyTransactionService(stockResolver);

            service.RegisterTransaction<Domain.Transactions.Aquisition, RestApi.Transactions.Aquisition>("aquisition", new AquisitionHandler());
            service.RegisterTransaction<Domain.Transactions.CashTransaction, RestApi.Transactions.CashTransaction>("cashtransaction", new CashTransactionHandler());

            return service;
        }

        private static IMapper CreateMapper(IServiceProvider serviceProvider)
        {
            var stockExchange = serviceProvider.GetRequiredService<StockExchange>();
            var stockResolver = serviceProvider.GetRequiredService<StockResolver>();

            var transactionService = serviceProvider.GetRequiredService<FunkyTransactionService>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new Service.Utils.ModelToServiceMapping(stockExchange));
                cfg.AddProfile(transactionService.RestApiToDomainMappingProfile());
                cfg.AddProfile(transactionService.DomainToRestApiMappingProfile());
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
        private FunkyTransactionService _TransactionService;

        public JsonMvcConfiguration(FunkyTransactionService transactionService)
        {
            _TransactionService = transactionService;
        }

        public void Configure(MvcJsonOptions options)
        {
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });

            options.SerializerSettings.Converters.Add(new CorporateActionJsonConverter());
            options.SerializerSettings.Converters.Add(_TransactionService.JsonConverter());          
        }
    }

    public class FunkyMappingProfile : Profile
    {

    }

    public class FunkyHandler
    {
        private Dictionary<Type, ITransactionHandler> _Handlers = new Dictionary<Type, ITransactionHandler>();
               
        public void Register(Type type, ITransactionHandler handler)
        {
            _Handlers.Add(type, handler);
        }

        public void Handle(Domain.Transactions.Transaction transaction, Portfolio portfolio)
        {
            if (_Handlers.TryGetValue(transaction.GetType(), out var handler))
                handler.ApplyTransaction(transaction, portfolio);
        }
    }

    public class FunkyJsonConverter : JsonConverter
    {
        private Dictionary<string, Type> _TransactionTypes = new Dictionary<string, Type>();

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(RestApi.Transactions.Transaction));
        }


        public void Register(string typeName, Type type)
        {
            _TransactionTypes.Add(typeName, type);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            if (jsonObject.TryGetValue("type", out var jsonToken))
            {
                var transactionType = jsonToken.Value<string>();

                if (_TransactionTypes.TryGetValue(transactionType, out var type))
                    return jsonObject.ToObject(type, serializer);
                else
                    return null;
            }

            return null;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class FunkyTransactionService
    {
        private IValueResolver<RestApi.Transactions.Transaction, Domain.Transactions.Transaction, Stock> _StockResolver;

        private class TransactionMapping
        {
            public string Name;
            public Type DomainTransactionType;
            public Type RestApiTransactionType;
            public ITransactionHandler Handler;

            public TransactionMapping(string name, Type domainType, Type restApiType, ITransactionHandler handler)
            {
                Name = name;
                DomainTransactionType = domainType;
                RestApiTransactionType = restApiType;
                Handler = handler;
            }
        }

        private List<TransactionMapping> _Mappings = new List<TransactionMapping>();

        public FunkyTransactionService(IValueResolver<RestApi.Transactions.Transaction, Domain.Transactions.Transaction, Stock> stockResolver)
        {
            _StockResolver = stockResolver;
        }

        public void RegisterTransaction<D, R>(string name, ITransactionHandler handler)
            where D : Domain.Transactions.Transaction
            where R : RestApi.Transactions.Transaction      
        {
            _Mappings.Add(new TransactionMapping(name, typeof(D), typeof(R), handler));
        }

        public FunkyMappingProfile DomainToRestApiMappingProfile()
        {
            var profile = new FunkyMappingProfile();

            var baseMap = profile.CreateMap<Domain.Transactions.Transaction, RestApi.Transactions.Transaction>()
                                .ForMember(x => x.Stock, x => x.MapFrom(y => y.Stock.Id));
            foreach (var mapping in _Mappings)
            {
                baseMap.Include(mapping.DomainTransactionType, mapping.RestApiTransactionType);

                profile.CreateMap(mapping.DomainTransactionType, mapping.RestApiTransactionType);
            }

            return profile;
        }

        public FunkyMappingProfile RestApiToDomainMappingProfile()
        {
            var profile = new FunkyMappingProfile();

            var baseMap = profile.CreateMap<RestApi.Transactions.Transaction, Domain.Transactions.Transaction>()
                                .ForMember(dest => dest.Stock, opts => opts.ResolveUsing(_StockResolver));
            foreach (var mapping in _Mappings)
            {
                baseMap.Include(mapping.RestApiTransactionType, mapping.DomainTransactionType);

                profile.CreateMap(mapping.RestApiTransactionType, mapping.DomainTransactionType);
            }

            return profile;
        }

        public FunkyJsonConverter JsonConverter()
        {
            var converter = new FunkyJsonConverter();

            foreach (var mapping in _Mappings)
                converter.Register(mapping.Name, mapping.RestApiTransactionType);

            return converter;
        }

        public FunkyHandler TransactionHandler()
        {
            var handler = new FunkyHandler();

            foreach (var mapping in _Mappings)
                handler.Register(mapping.DomainTransactionType, mapping.Handler);

            return handler;
        }
    }
}
