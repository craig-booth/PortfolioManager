﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace PortfolioManager.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
          
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        public PortfolioManagerSettings PortfolioManagerSettings
        {
            get {  return Configuration.GetSection("Settings").Get<PortfolioManagerSettings>(); }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(x => x.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto);

            services.Configure<KestrelServerOptions>(x => x.Listen(IPAddress.Any, PortfolioManagerSettings.Port))
                .AddPortfolioManagerService(PortfolioManagerSettings);          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApiKeyAuthentication(PortfolioManagerSettings.ApiKey);
            app.UseMvc();
        }
    }
}
