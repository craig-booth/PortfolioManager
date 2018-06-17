using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;

using AutoMapper;

using PortfolioManager.Common.Scheduler;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.ImportData;
using PortfolioManager.ImportData.DataServices;
using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Services;
using PortfolioManager.Service.Utils;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.EventStore;


namespace PortfolioManager.Web
{
    public static class PortfolioManagerServiceCollectionExtensions
    {

        public static void AddPortfolioManagerService(this IServiceCollection services, PortfolioManagerSettings settings)
        {
            services.AddSingleton<PortfolioManagerSettings>(settings);
            services.AddSingleton<IPortfolioDatabase>(CreatePortfolioDatabase);
            services.AddSingleton<IEventStore>(CreateEventStore);
            services.AddSingleton<StockExchange>(CreateStockExchange);
            services.AddSingleton<IMapper>(CreateMapper);

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

            services.AddScoped<IHistoricalStockPriceService, ASXDataService>();
            services.AddScoped<ILiveStockPriceService, ASXDataService>();
            services.AddScoped<ITradingDayService, ASXDataService>();

            services.AddScoped<HistoricalPriceImporter>();
            services.AddScoped<LivePriceImporter>();
            services.AddScoped<TradingDayImporter>();

            services.AddSingleton<Scheduler>();

            services.AddSingleton<IHostedService, DataImportBackgroundService>();

           // stockExchange.LoadFromEventStream();
        }

        private static IEventStore CreateEventStore(IServiceProvider serviceProvider)
        {
            var settings = serviceProvider.GetRequiredService<PortfolioManagerSettings>();

            return new SqliteEventStore(settings.EventDatabase);
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

        private static IMapper CreateMapper(IServiceProvider serviceProvider)
        {
            var stockExchange = serviceProvider.GetRequiredService<StockExchange>();

            var config = new MapperConfiguration(cfg =>
                    cfg.AddProfile(new ModelToServiceMapping(stockExchange))
            );
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
        public string EventDatabase { get; set; }
        public string PortfolioDatabase { get; set; }
        public int Port { get; set; }   
    }


}
