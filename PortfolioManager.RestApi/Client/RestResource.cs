using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json.Converters;

namespace PortfolioManager.RestApi.Client
{
    public abstract class RestResource
    {
        private readonly HttpClient _HttpClient;

        public RestResource(HttpClient httpClient)
        {
            _HttpClient = httpClient;
        }

        protected async Task<T> GetAsync<T>(string url)
        {
            T response = default(T);

            HttpResponseMessage httpResponse = await _HttpClient.GetAsync(url);
            if (httpResponse.IsSuccessStatusCode)
            {
                var formatter = new JsonMediaTypeFormatter
                {
                    SerializerSettings = {TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto }
                };
                formatter.SerializerSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });

                response = await httpResponse.Content.ReadAsAsync<T>(new List<MediaTypeFormatter> { formatter });
            }

            return response;
        }

        protected async Task PostAsync<D>(string url, D data)
        {
 
            var formatter = new JsonMediaTypeFormatter
            {
                SerializerSettings = { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects }
            };
            formatter.SerializerSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });

            HttpResponseMessage response = await _HttpClient.PostAsync<D>(url, data, formatter);
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            return;
        }
    }
}
