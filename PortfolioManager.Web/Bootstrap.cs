using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using AutoMapper;

using PortfolioManager.Data.Portfolios;
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
            var portfolioDatabase = new SQLitePortfolioDatabase(settings.PortfolioDatabase);
            services.AddSingleton<IPortfolioDatabase>(portfolioDatabase);

            var eventStore = new SqliteEventStore(settings.EventDatabase);
            var stockExchange = new StockExchange(eventStore);
            stockExchange.LoadFromEventStream();
            services.AddSingleton<StockExchange>(stockExchange);

            var config = new MapperConfiguration(cfg =>
                    cfg.AddProfile(new ModelToServiceMapping(stockExchange))
            );
            services.AddSingleton<IMapper>(config.CreateMapper());

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

            services.AddSingleton<IHostedService, DataImportBackgroundService>();
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
