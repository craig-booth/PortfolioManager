using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace PortfolioManager.Web
{
    public class Startup
    {

        private readonly bool _InDevelopment;

        public Startup(IHostingEnvironment env)
        {
            _InDevelopment = env.IsDevelopment();

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
            var key = System.Text.Encoding.ASCII.GetBytes("this is a secret");

            if (_InDevelopment)
            {
                services.AddAuthentication("AnonymousAuthentication")
                    .AddScheme<AuthenticationSchemeOptions, AnonymousAuthenticationHandler>("AnonymousAuthentication", null);

                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.IsPortfolioOwner, policy => policy.RequireUserName("DebugUser"));
                    options.AddPolicy(Policy.CanMantainStocks, policy => policy.RequireUserName("DebugUser"));
                });
            } 
            else
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateIssuer = true,
                            ValidIssuer = "http://portfolio.boothfamily.id.au",
                            ValidateAudience = true,
                            ValidAudience = "http://portfolio.boothfamily.id.au",
                            ValidateLifetime = true
                        };
                    });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.IsPortfolioOwner, policy => policy.Requirements.Add(new PortfolioOwnerRequirement()));
                    options.AddPolicy(Policy.CanMantainStocks, policy => policy.RequireRole(Role.Administrator));
                });
            }

            services.AddLogging(config =>
            {
                config.AddConfiguration(Configuration.GetSection("Logging"));
                config.AddConsole();
                config.AddDebug();              
            });

            // Add framework services.
            services.AddMvc();

            services.Configure<KestrelServerOptions>(x => x.Listen(IPAddress.Any, PortfolioManagerSettings.Port))
                .AddPortfolioManagerService(PortfolioManagerSettings)
                .AddDataImportService();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAuthentication();
            app.UseMvc();

            app.InitializeStockCache();
        }
    }

}
