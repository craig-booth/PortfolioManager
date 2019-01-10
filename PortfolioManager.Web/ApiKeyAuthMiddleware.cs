using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace PortfolioManager.Web
{
    public class ApiKeyAuthMiddleware
    {

        private readonly RequestDelegate _Next;
        private readonly Guid _ApiKey;

        public ApiKeyAuthMiddleware(RequestDelegate next, Guid apiKey)
        {
            _Next = next;
            _ApiKey = apiKey;
        }

        public async Task Invoke(HttpContext httpContext)
        {
#if !DEBUG
            var apiKeyHeader = httpContext.Request.Headers["Api-Key"];
            if (!Guid.TryParse(apiKeyHeader, out Guid apiKey) || !apiKey.Equals(_ApiKey))
            {
                httpContext.Response.StatusCode = 401;
                await httpContext.Response.WriteAsync("Invalid API Key");
                return;
            }
#endif
            // Call the next middleware delegate in the pipeline 
            await _Next.Invoke(httpContext);
        }
    }

    public static class ApiKeyAuthMiddlewareAppBuilderExtensions
    {
        public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder app, Guid apiKey)
        {
            return app.UseMiddleware<ApiKeyAuthMiddleware>(apiKey);

        }
    }
}
