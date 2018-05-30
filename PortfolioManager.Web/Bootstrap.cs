using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Configuration;

using AutoMapper;

using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Local;
using PortfolioManager.Service.Utils;

namespace PortfolioManager.Web
{
    public static class PortfolioManagerServiceCollectionExtensions
    {

        public static void AddPortfolioManagerService(this IServiceCollection services, Settings settings)
        {
            var portfolioDatabase = new SQLitePortfolioDatabase(settings.PortfolioDatabase);
            services.AddSingleton<IPortfolioDatabase>(portfolioDatabase);

            var stockDatabase = new SQLiteStockDatabase(settings.StockDatabase);
            services.AddSingleton<IStockDatabase>(stockDatabase);

            var config = new MapperConfiguration(cfg =>
                    cfg.AddProfile(new ModelToServiceMapping(stockDatabase))
                );
            var mapper = config.CreateMapper();
            services.AddSingleton<IMapper>(mapper);

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
        }
    }

    public class Settings
    {
        public string PortfolioDatabase { get; set; }
        public string StockDatabase { get; set; }
    }


}
