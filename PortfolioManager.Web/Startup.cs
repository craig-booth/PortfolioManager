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
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

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
            if (PortfolioManagerSettings.EnableAuthentication)
            {
                var tokenConfiguration = PortfolioManagerSettings.JwtTokenConfiguration;

                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = tokenConfiguration.GetKey(),
                            ValidateIssuer = true,
                            ValidIssuer = tokenConfiguration.Issuer,
                            ValidateAudience = true,
                            ValidAudience = tokenConfiguration.Audience,
                            ValidateLifetime = true
                        };
                    });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.IsPortfolioOwner, policy => policy.Requirements.Add(new PortfolioOwnerRequirement()));
                    options.AddPolicy(Policy.CanMantainStocks, policy => policy.RequireRole(Role.Administrator));
                });
            }
            else
            {
                services.AddAuthentication("AnonymousAuthentication")
                    .AddScheme<AuthenticationSchemeOptions, AnonymousAuthenticationHandler>("AnonymousAuthentication", null);

                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.IsPortfolioOwner, policy => policy.RequireUserName("DebugUser"));
                    options.AddPolicy(Policy.CanMantainStocks, policy => policy.RequireUserName("DebugUser"));
                });
            }

            services.AddMemoryCache();

            services.AddLogging(config =>
            {
                config.AddConfiguration(Configuration.GetSection("Logging"));
                config.AddConsole();
                config.AddDebug();              
            });

            // Add framework services.
            services.AddMvc(options =>
                {
                    options.Filters.Add<PortfolioExceptionFilter>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            // OpenAPI support
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Portfolio Manager API", Version = "v2" });
            });


            services.Configure<KestrelServerOptions>(x => x.Listen(IPAddress.Any, PortfolioManagerSettings.Port))
                .AddPortfolioManagerService(PortfolioManagerSettings)
                .AddDataImportService();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Portfolio Manager API v2");
            });


            app.UseAuthentication();
            app.UseMvc();

            app.InitializeStockCache();
        }

        private SymmetricSecurityKey LoadKey(string fileName)
        {
            var base64Key = System.IO.File.ReadAllText(fileName);
            var key = Convert.FromBase64String(base64Key);
            return new SymmetricSecurityKey(key);
        }
    }

}
